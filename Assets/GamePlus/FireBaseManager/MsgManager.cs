using System;
using Assets.Scripts.FireBaseManager;
using Firebase.Database;
using UnityEngine;

namespace Assets.GamePlus.FireBaseManager
{
    public class MsgManager : MonoBehaviour
    {
        public static Action<string> MessageAction;
        public static MsgManager Instance;
        //监听服务器时间变化
        public static event Action<string> OnServerTime; 
        void Start()
        {
            Instance = this;
        }

        //todo 用户切换后能否继续监听新用户的消息盒子
        void Update()
        {

        }

        void HandleChildAdded(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            LogContent("HandleChildAdded", sender, args);
            if (MessageAction!=null)
            {
                MessageAction(args.Snapshot.GetRawJsonValue());
            }
            RemoteDbManager.Instance.Rdb.Child(FireBaseConfig.MessagePath + LoginManager.UserId)
                .Child(args.Snapshot.Key).RemoveValueAsync();
        }

        void HandleChildChanged(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            LogContent("HandleChildChanged", sender, args);
        }

        void HandleChildRemoved(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            LogContent("HandleChildRemoved", sender, args);
        }

        void HandleChildMoved(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            LogContent("HandleChildMoved", sender, args);
        }

        private static void LogContent(string operate,object sender, ChildChangedEventArgs args)
        {
            Debug.Log(operate + "=>" + args.Snapshot.Key + args.Snapshot.GetRawJsonValue());
        }

        void DBInitComplete()
        {
            var msgBox = RemoteDbManager.Instance.Rdb.Child(FireBaseConfig.MessagePath + LoginManager.UserId);
            msgBox.ChildAdded += HandleChildAdded;
            msgBox.ChildChanged += HandleChildChanged;
            msgBox.ChildRemoved += HandleChildRemoved;
            msgBox.ChildMoved += HandleChildMoved;
            var time = RemoteDbManager.Instance.Rdb.Child("Time");
            time.ChildChanged += HandleTimeChanged;
            Debug.Log("MsgManager 已初始化");
        }

        /// <summary>
        /// 监听服务器时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void HandleTimeChanged(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            LogContent("HandleTimeChanged", sender, args);
            OnServerTime((string) args.Snapshot.Value);
        }

        void OnEnable()
        {
            RemoteDbManager.DBInitComplete += DBInitComplete;
        }

        void OnDisable()
        {
            RemoteDbManager.DBInitComplete -= DBInitComplete;
        }
    }
}
