using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

namespace Assets.Scripts.FireBaseManager
{
    public class RemoteDbManager:MonoBehaviour
    {
        public static RemoteDbManager Instance;
        public DatabaseReference Rdb;
        DependencyStatus _dependencyStatus = DependencyStatus.UnavailableOther;
        public static event Action DBInitComplete; 
        void Start()
        {
            Instance = this;
//            InitializeRemoteDbManager();
        }

        void Update()
        {
        }
        
        // Initialize the Firebase database:notasecret
        void InitializeRemoteDbManager()
        {
//            FirebaseApp app = FirebaseApp.DefaultInstance;
//            app.SetEditorDatabaseUrl(FireBaseConfig.DatabaseUrl);
//            FirebaseApp.DefaultInstance.SetEditorP12FileName("gameplus-27ce2f3701ff.p12");
//            FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("jansen@balls-d0b54.iam.gserviceaccount.com");
//            FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");

            Rdb = FirebaseDatabase.DefaultInstance.RootReference;
            DBInitComplete();
            Debug.Log("RemoteDbManager 已初始化");
        }

        void LoginManager_LoginCompleted()
        {
            AppsFlyer.setCustomerUserID(LoginManager.UserId);
            InitializeRemoteDbManager();
        }

        void OnEnable()
        {
            LoginManager.LoginCompleted += LoginManager_LoginCompleted;
        }

        void OnDisable()
        {
            LoginManager.LoginCompleted -= LoginManager_LoginCompleted;
        }
    }
}
