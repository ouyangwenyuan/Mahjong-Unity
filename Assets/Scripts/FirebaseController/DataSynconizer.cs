using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GamePlus.advertise;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.Bean;
using Assets.Scripts.Data.BeanMap;
using Assets.Scripts.FireBaseManager;
using Assets.Scripts.Utils;
using Firebase.Database;
using UnityEngine;

namespace Assets.Scripts.FirebaseController
{
    public enum SynType
    {
        Upload=1,
        Download=2
    }
    public class DataSynconizer:MonoBehaviour
    {
        public static DataSynconizer Instance;
        //是否成功得到数据
        private bool _loadConfigDone;
        private bool _loadUserInfoDone;
        private bool _loadScoresDone;
        void Start()
        {
            Instance = this;
        }

        void Update()
        {

        }

        void DBInitComplete()
        {
            //新用户数据初始化
            if (LoginManager.UserType == UserType.UserNew)
            {
                InitNewUserData();
                PlayerPrefs.SetInt(FireBaseConfig.NEW_USER, 0);
            }
            else
            {   //
                UpdateDeviceId();
                SyncUserData();
            }
            SyncGlobalConfig();
        }

        void OnEnable()
        {
            RemoteDbManager.DBInitComplete += DBInitComplete;
        }

        void OnDisable()
        {
            RemoteDbManager.DBInitComplete -= DBInitComplete;
        }

        void CheckLoadAll()
        {
            if (_loadConfigDone&&
                _loadUserInfoDone&&
                _loadScoresDone)
            {
                Init.LoginCompleted = true;
                CancelInvoke("CheckLoadAll");
            }
        }

        void UpdateDeviceId()
        {
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First();
            playerInfo.device_id = DeviceUtils.GetUuid();
            //更新deviceid,但不刷新update字段的值
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo,false);
        }

        void InitNewUserData()
        {
            InitUserInfo();
            InitUserConfig();
            Init.LoginCompleted = true;
        }

        void SyncUserData()
        {
            SyncUserInfo();
        }

        void SyncGlobalConfig()
        {
            //商店顺序
            UpdateShopOrder();
            //广告
            UpdateAdConfig();
        }

        //初始化数据
        void InitUserInfo()
        {
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First();
            playerInfo.device_id = DeviceUtils.GetUuid();
            UserInfo userInfo = new UserInfo(playerInfo)
            {
                Fbid = "",
                Uid = LoginManager.UserId,
                OsType = SystemInfo.operatingSystem
            };
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            UploadData(userInfo, FireBaseConfig.UserInfoPath);
        }

        void InitUserConfig()
        {
            UserConfig config = new UserConfig();
            config.GameAdCountDown = "0";
            config.GameAdPlayCount = 0;
            config.MusicOn = 0;
            config.NotifictionOn = 0;
            config.ShopAdCountDown = "0";
            config.SoundOn = 0;
            config.TutorialOn = 1;
            config.GuidSteps = "0|0|0|0|0|0|0|0|0";
            config.MaxlifeBuyTime = 5;
            config.Time = PlayerInfoUtil.GetTimeStamp();
            UploadData(config, FireBaseConfig.UserConfigPath);
        }
        //同步数据
        public void SyncUserInfo()
        {
            RemoteDbManager.Instance.Rdb
                .Child(FireBaseConfig.UserInfoPath + LoginManager.UserId)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SyncUserInfo was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SyncUserInfo encountered an error: " + task.Exception);
                        return;
                    }
                    if (string.IsNullOrEmpty(task.Result.GetRawJsonValue()))
                    {
                        Init.LoginCompleted = true;
                    }
                    Debug.Log("SyncUserInfo " + task.Result.GetRawJsonValue());
                    UserInfoRefresh(task);
                });
        }

        private void DownloadConfig()
        {
            RemoteDbManager.Instance.Rdb
                .Child(FireBaseConfig.UserConfigPath + LoginManager.UserId)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SyncUserConfig was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SyncUserConfig encountered an error: " + task.Exception);
                        return;
                    }
                    Debug.Log("DownloadConfig " + task.Result.GetRawJsonValue());
                    UserConfig userConfig = JsonUtility.FromJson<UserConfig>(task.Result.GetRawJsonValue());
                    PlayerPrefs.SetString("DAY_FIRST_PLAY", userConfig.GameAdCountDown);
                    PlayerPrefs.SetString("shop_ads_down", userConfig.ShopAdCountDown);
                    PlayerPrefs.SetInt("AD_PLAY_COUNT", userConfig.GameAdPlayCount);
                    PlayerPrefs.SetInt("MUSIC_ON", userConfig.MusicOn);
                    PlayerPrefs.SetInt("HINT_ON", userConfig.NotifictionOn);
                    PlayerPrefs.SetInt("SFX_ON", userConfig.SoundOn);
                    PlayerPrefs.SetInt("NEWBIE_DONE", userConfig.TutorialOn);
                    PlayerPrefs.SetInt("MAXLIFE_BUY_NUM", userConfig.MaxlifeBuyTime);
                    string[] steps = userConfig.GuidSteps.Split('|');
                    for (int i = 1; i < steps.Length; i++)
                    {
                        int guid;
                        int.TryParse(steps[i-1], out guid);
                        Debug.Log("GUIDE_STEP_" + i + "     " + guid);
                        PlayerPrefs.SetInt("GUIDE_STEP_" + i, guid);
                    }
                    _loadConfigDone = true;
                });
        }

        /// <summary>
        /// 上传用户配置
        /// </summary>
        public void UploadConfig()
        {
            if (!FireBaseAvailable()) return;
            UserConfig config = new UserConfig();
            config.GameAdCountDown = PlayerPrefs.GetString("DAY_FIRST_PLAY", "");
            config.GameAdPlayCount = PlayerPrefs.GetInt("AD_PLAY_COUNT", 0);
            config.MusicOn = PlayerPrefs.GetInt("MUSIC_ON", 0);
            config.NotifictionOn = PlayerPrefs.GetInt("HINT_ON", 0);
            config.ShopAdCountDown = PlayerPrefs.GetString("shop_ads_down", "");
            config.SoundOn = PlayerPrefs.GetInt("SFX_ON", 0);
            config.TutorialOn = PlayerPrefs.GetInt("NEWBIE_DONE", 0);
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < 10; i++)
            {
                builder.Append(PlayerPrefs.GetInt("GUIDE_STEP_" + i) + "|");
            }
            config.GuidSteps = builder.ToString();
            config.MaxlifeBuyTime = PlayerPrefs.GetInt("MAXLIFE_BUY_NUM", 0);
            config.Time = PlayerInfoUtil.GetTimeStamp();
            UploadData(config, FireBaseConfig.UserConfigPath);
            Init.LoginCompleted = true;
        }

        //同步用户分数
        void SynUserScores()
        {
            RemoteDbManager.Instance.Rdb.Child(FireBaseConfig.ScorePath)
                .OrderByChild("Uid")
                .EqualTo(LoginManager.UserId)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SyncUserInfo was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SyncUserInfo encountered an error: " + task.Exception);
                        return;
                    }
                    Debug.Log("SynUserScores " + task.Result.GetRawJsonValue());
                    if (string.IsNullOrEmpty(task.Result.GetRawJsonValue()))
                    {
                        Init.LoginCompleted = true;
                        return;
                    }
                    List<Highscore> highscores=new List<Highscore>();
                    foreach (var dataSnapshot in task.Result.Children)
                    {
                        UserScore socre = JsonUtility.FromJson<UserScore>(dataSnapshot.GetRawJsonValue());
                        Highscore highscore = new Highscore(socre);
                        highscores.Add(highscore);
                    }
                    DynamicDataBaseService.GetInstance().Connection.DeleteAll<Highscore>();
                    DynamicDataBaseService.GetInstance().Connection.InsertAll(highscores);
                    _loadScoresDone = true;
                });
        }

        private void UserInfoRefresh(Task<DataSnapshot> task)
        {
            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(task.Result.GetRawJsonValue());
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First();
            FriendData friendData = DynamicDataBaseService.GetInstance().GetFriendData().First();
            //devieid比较
            if (CompareDeviceid(userInfo.DeviceId,playerInfo.device_id))
            {
                if (LoginManager.UserType==UserType.UserReinstall)
                {
                    Debug.Log("老用户重装游戏");
                    SynDownload(userInfo, friendData);
                }
                else
                {
                    if (LoginController.AccountChange)
                    {
                        Debug.Log("帐号切换登录");
                        SynDownload(userInfo, friendData);
                        LoginController.AccountChange = false;
                    }
                    else
                    {
                        Debug.Log("老用户相同设备登录");
                        CheckDataRefresh(task, playerInfo, userInfo, friendData);
                    }
                }
            }
            else
            {
                Debug.Log("老用户不同设备登录");
                LoginManager.UserType = UserType.UserChangeDevice;
                SynDownload(userInfo, friendData);
            }
        }

        private void SynDownload(UserInfo userInfo, FriendData friendData)
        {
            InvokeRepeating("CheckLoadAll", 0f, 1f);
            DownloadConfig();
            SynUserScores();
            DownLoadUserIno(userInfo, friendData);
        }

        private void CheckDataRefresh(Task<DataSnapshot> task, PlayerInfo playerInfo, UserInfo userInfo, FriendData friendData)
        {
            
            //本地与远程的时间比较
            if (TimeCompare(playerInfo.player_update, userInfo.Update) &&
                task.Result.Key.Equals(LoginManager.UserId))
            {
                UserInfo newUserInfo = new UserInfo(playerInfo);
                string fbid = PlayerPrefs.GetString(Constance.FACEBOOK_ID, "");
                newUserInfo.Fbid = fbid;
                newUserInfo.Uid = LoginManager.UserId;
                newUserInfo.OsType = SystemInfo.operatingSystem;
                UploadData(newUserInfo, FireBaseConfig.UserInfoPath);
                UploadConfig();
                Debug.Log("上传存档");
            }
            else
            {
                Debug.Log("下载存档");
                DownLoadUserIno(userInfo, friendData);
            }
        }

        private void DownLoadUserIno(UserInfo userInfo, FriendData friendData)
        {
            PlayerInfo newInfo = new PlayerInfo(userInfo);
            DynamicDataBaseService.GetInstance().UpdateData(newInfo);
            friendData.fb_id = userInfo.Fbid;
            friendData.gs_id = userInfo.Uid;
            DynamicDataBaseService.GetInstance().UpdateData(friendData);
            _loadUserInfoDone = true;
            //老用户直接结束登录
            if (LoginManager.UserType==UserType.UserOld)
            {
                Init.LoginCompleted = true;
            }
        }


        private void UploadData(BeanDic bean, string path)
        {
            if (!FireBaseAvailable()) return;
            Dictionary<string, object> entryValues = bean.ToDictionary();
            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates[path + LoginManager.UserId] = entryValues;
            RemoteDbManager.Instance.Rdb.UpdateChildrenAsync(childUpdates);
        }

        private void UpdateAdConfig()
        {
            if (!FireBaseAvailable()) return;
            RemoteDbManager.Instance.Rdb.Child(FireBaseConfig.GlobalConfigPath + "AdConfig/content")
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("UpdateAdConfig was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("UpdateAdConfig encountered an error: " + task.Exception);
                        return;
                    }
                    Debug.Log("AdConfig loaded " + (string)task.Result.Value);
                    PlayerPrefs.SetString(GPVideoAd.AD_CONFIG, (string)task.Result.Value);
                });
        }

        private void UpdateShopOrder()
        {
            if (!FireBaseAvailable()) return;
            RemoteDbManager.Instance.Rdb.Child(FireBaseConfig.GlobalConfigPath + "ShopOrder/content")
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("UpdateShopOrder was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("UpdateShopOrder encountered an error: " + task.Exception);
                        return;
                    }
                    Debug.Log("ShopOrder loaded " + (string)task.Result.Value);
                    List<ShopOrderMap> shopOrders =
                        JsonUtility.FromJson<Serialization<ShopOrderMap>>(task.Result.GetRawJsonValue()).ToList();
                    List<ShopOrder> shopOrderNew = shopOrders.ConvertAll(x => new ShopOrder(x));
                    DynamicDataBaseService.GetInstance().Connection.UpdateAll(shopOrderNew);
                });
        }

        private bool CompareDeviceid(string remoteId,string localId)
        {
            if (string.IsNullOrEmpty(remoteId))
            {
                Debug.Log("remoteId is null");
            }

            if (string.IsNullOrEmpty(localId))
            {
                Debug.Log("localId is null");
            }
            return remoteId.Equals(localId);
        }

        private bool TimeCompare(string st1, string st2)
        {
            double t1, t2;
            double.TryParse(st1, out t1);
            double.TryParse(st2, out t2);
            return t1 - t2 >= 0;
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

        void ResetTimeout()
        {
            GameObject initGameObject = GameObject.Find("CanvasLoading");
            if (initGameObject!=null)
            {
                Init init = initGameObject.GetComponent<Init>();
                init.ReSetTimeout();
            }
        }
    }
}
