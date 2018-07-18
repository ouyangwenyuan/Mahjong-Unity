using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using UnityEngine;

namespace Assets.Scripts.FireBaseManager
{
    public class CloundMsg:MonoBehaviour
    {
        private string topic = "example";
        Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
        void Start()
        {

        }

        void Update()
        {

        }

        // Setup message event handlers.
        void InitializeCloundMsg()
        {
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.Subscribe(topic);
            Debug.Log("CloundMsg 已初始化");
        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            Debug.Log("Received Registration Token: " + token.Token);
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            Debug.Log("Received a new message from: " + e.Message.MessageId);
            IDictionary<string, string> datas = e.Message.Data;
            foreach (var data in datas)
            {
                Debug.Log(data.Key+"=>"+data.Value);
            }
        }

        // End our messaging session when the program exits.
        public void OnDestroy()
        {
            Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
//            Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/example");
        }

        void LoginManager_LoginCompleted()
        {
            InitializeCloundMsg();
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
