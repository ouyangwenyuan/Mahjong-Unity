using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.manager.bean;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.Bean;
using Assets.Scripts.Data.BeanMap;
using Assets.Scripts.FireBaseManager;
using Firebase.Database;
using UnityEngine;

namespace Assets.Scripts.FirebaseController
{
    public class SdkController:MonoBehaviour
    {
        public static SdkController Instance;
        private List<BoardData> ranksDatas = new List<BoardData>();
        private int _friendLenth;
        private int _curFriendIndex;
        private bool _waitForFinish;
        private int _curLevel;
        void Start()
        {
            Instance = this;
        }

        void Update()
        {
            if (_waitForFinish)
            {
                if (_friendLenth == _curFriendIndex && ranksDatas.Any())
                {
                    Debug.Log("cache score");
                    LeaderBoard dataBoard = new LeaderBoard();
                    dataBoard.id = _curLevel;
                    dataBoard.boardInfo = JsonUtility.ToJson(new Serialization<BoardData>(ranksDatas));
                    DynamicDataBaseService.GetInstance().InsertOrReplace(dataBoard);
                    ranksDatas.Clear();
                    _waitForFinish = false;
                }
            }
        }

        public void GetFriendUid()
        {
            Debug.Log("GetFriendUid");
            //取得好友uuid
            List<FriendData> friendsInfos = DynamicDataBaseService
                .GetInstance().GetFriendData()
                .Where(x => x.id > 1)
                .ToList();
            //好友为空
            if (friendsInfos.Count < 1)
            {
                Debug.Log("no friends");
                return;
            }
            foreach (var friendsInfo in friendsInfos)
            {
                Debug.Log("get friends uuid,fbid=" + friendsInfo.fb_id);
                RemoteDbManager.Instance.Rdb.Child(FireBaseConfig.UserInfoPath)
                    .OrderByChild("Fbid")
                    .EqualTo(friendsInfo.fb_id)
                    .GetValueAsync().ContinueWith(UidCallBack);
            }
        }

        private void UidCallBack(Task<DataSnapshot> task)
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UidCallBack was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UidCallBack encountered an error: " + task.Exception);
                return;
            }
            Debug.Log(task.Result.GetRawJsonValue());
            if (string.IsNullOrEmpty(task.Result.GetRawJsonValue()))
            {
                Debug.Log("UidCallBack is null");
                return;
            }
            foreach (var dataSnapshot in task.Result.Children)
            {
                Debug.Log(dataSnapshot.GetRawJsonValue());
                UserInfo userInfo = JsonUtility.FromJson<UserInfo>(dataSnapshot.GetRawJsonValue());
                FriendData curFriendData =
                    DynamicDataBaseService.GetInstance().GetFriendData().First(x => (x.fb_id).Equals(userInfo.Fbid));
                curFriendData.gs_id = userInfo.Uid;
                curFriendData.stage = userInfo.Stage;
                DynamicDataBaseService.GetInstance().UpdateData(curFriendData);
            }
        }

        /// <summary>
        /// 上传分数      
        /// </summary>
        /// <param name="level">关卡</param>
        /// <param name="score">分数</param>
        public void ReportScore(int level)
        {
            if (!FireBaseAvailable()) return;
            List<Highscore> scioreList = DynamicDataBaseService.GetInstance().GetHighscores().ToList();
            if (scioreList.All(x => x.id != level))return;
            Highscore score = scioreList.First(x => x.id == level);
            //本地控制，防止低分覆盖高分
            UserScore userScore = new UserScore();
            userScore.Uid = LoginManager.UserId;
            userScore.Level = level;
            userScore.Score = score.highscore;
            Dictionary<string, object> entryValues = userScore.ToDictionary();
            entryValues.Add("time", ServerValue.Timestamp);
            //低分数会覆盖高分数
            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates[FireBaseConfig.ScorePath + userScore.LevelUid] = entryValues;
            RemoteDbManager.Instance.Rdb.UpdateChildrenAsync(childUpdates);
        }

        /// <summary>
        /// 获取排行榜分数
        /// </summary>
        /// <param name="level"></param>
        public void GetLevelSocres(int level)
        {
            if (!FireBaseAvailable()) return;
            List<FriendData> friends = DynamicDataBaseService
                .GetInstance().GetFriendData()
                .Where(x => x.id > 1)
                .Where(x => x.id <= Constance.BOARD_NUM)
                .Where(x => !string.IsNullOrEmpty(x.gs_id))
                .ToList();
            //好友为空
            if (friends.Count < 1)
            {
                Debug.Log("no friends");
                setNullRecord(level);
                return;
            }
            _curFriendIndex = 0;
            _friendLenth = friends.Count;
            _curLevel = level;
            _waitForFinish = true;
            foreach (var friendData in friends)
            {
                RemoteDbManager.Instance.Rdb.Child(FireBaseConfig.ScorePath)
                    .OrderByKey()
                    .EqualTo(friendData.gs_id+"_"+level+"_scr")
                    .GetValueAsync().ContinueWith(FriendCallBack);
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        public void UpdateUserInfo()
        {
            if (!FireBaseAvailable()) return;
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First();
            UserInfo newUserInfo = new UserInfo(playerInfo);
            string fbid = PlayerPrefs.GetString(Constance.FACEBOOK_ID, "");
            newUserInfo.Fbid = fbid;
            newUserInfo.Uid = LoginManager.UserId;
            newUserInfo.OsType = SystemInfo.operatingSystem;
            newUserInfo.DeviceId = DeviceUtils.GetUuid();

            UploadData(newUserInfo, FireBaseConfig.UserInfoPath);
        }

        /// <summary>
        /// 更新单个字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vlaue"></param>
        public void UpdateUserInfo(string key,string vlaue)
        {
            if (!FireBaseAvailable()) return;
            RemoteDbManager.Instance.Rdb
                .Child(FireBaseConfig.UserInfoPath + LoginManager.UserId)
                .Child(key)
                .SetValueAsync(vlaue);
        }

        private void FriendCallBack(Task<DataSnapshot> task)
        {
            if (task.IsCanceled)
            {
                Debug.LogError("FriendCallBack was canceled.");
                _curFriendIndex++;
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("FriendCallBack encountered an error: " + task.Exception);
                _curFriendIndex++;
                return;
            }
            if (string.IsNullOrEmpty(task.Result.GetRawJsonValue()))
            {
                _curFriendIndex++;
                return;
            }
            foreach (var child in task.Result.Children)
            {
                UserScore userScore = JsonUtility.FromJson<UserScore>(child.GetRawJsonValue());
                BoardData boardData = new BoardData();
                boardData.gsid = userScore.Uid;
                boardData.socre = userScore.Score;
                ranksDatas.Add(boardData);
                _curFriendIndex++;
//                Debug.Log("_curFriendIndex:" + _curFriendIndex + "\n" + "gsid:" + userScore.Score);
            }
        }

        private  void setNullRecord(int level)
        {
            LeaderBoard dataBoard = new LeaderBoard();
            dataBoard.id = level;
            dataBoard.boardInfo = "";
            DynamicDataBaseService.GetInstance().InsertOrReplace(dataBoard);
        }

        private bool FireBaseAvailable()
        {
            if (LoginManager.Instance.CurFirebaseUser != null
                && RemoteDbManager.Instance.Rdb != null)
            {
                return true;
            }
            Debug.Log("firebae not avalaible");
            return false;
        }

        private void UploadData(BeanDic bean, string path)
        {
            Dictionary<string, object> entryValues = bean.ToDictionary();
            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates[path + LoginManager.UserId] = entryValues;
            RemoteDbManager.Instance.Rdb.UpdateChildrenAsync(childUpdates);
        }

        public void UploadLogFile(string path,string remotePath)
        {
            if (!FireBaseAvailable()) return;
            CloudStorateManager.Instance.UploadFileFirebaseStorage(path, remotePath);
        }
    }
}
