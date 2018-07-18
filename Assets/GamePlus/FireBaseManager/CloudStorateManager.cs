using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assets.GamePlus.FireBaseManager;
using Firebase;
using Firebase.Storage;
using UnityEngine;

namespace Assets.Scripts.FireBaseManager
{
    public class CloudStorateManager:MonoBehaviour
    {
        public static CloudStorateManager Instance;
        private StorageReference reference;
        private FirebaseStorage storage;
        void Start()
        {
            Instance = this;
        }

        void Update()
        {

        }
        void LoginManager_LoginCompleted()
        {
            Debug.LogError("LoginManager_LoginCompleted");
            InitializeCloudStorateManager();
        }

        void OnEnable()
        {
            LoginManager.LoginCompleted += LoginManager_LoginCompleted;
        }

        void OnDisable()
        {
            LoginManager.LoginCompleted -= LoginManager_LoginCompleted;
        }

        void InitializeCloudStorateManager()
        {
            // Get a reference to the storage service, using the default Firebase App
            storage = FirebaseStorage.DefaultInstance;
            // Create a storage reference from our storage service
            reference = storage.GetReferenceFromUrl(FireBaseConfig.CloudStorage);
            Debug.Log("InitializeCloudStorateManager 已初始化");
        }

        public void DownloadFromFirebaseStorage(string remotePath, string pathToSave)
        {
            // Start downloading a file
            Task task = storage.GetReference(remotePath).GetFileAsync(pathToSave);

            task.ContinueWith(resultTask =>
            {
                if (!resultTask.IsFaulted && !resultTask.IsCanceled)
                {
                    Debug.Log("Download finished.");
                }
            });
        }
        public void UploadFileFirebaseStorage(string filePath,string remotePath)
        {
            // Create a reference to the file you want to upload
            StorageReference rivers_ref = reference.Child(remotePath);

            // Upload the file to the path "images/rivers.jpg"
            rivers_ref.PutFileAsync(filePath)
              .ContinueWith (task => 
              {
                if (task.IsFaulted || task.IsCanceled) {
                  Debug.Log(task.Exception.ToString());
                  // Uh-oh, an error occurred!
                } else {
                  // Metadata contains file metadata such as size, content-type, and download URL.
                  StorageMetadata metadata = task.Result;
                  string download_url = metadata.DownloadUrl.ToString();
                  Debug.Log("Finished uploading...");
                  Debug.Log("download url = " + download_url);
                }
              });
        }

        public void DebugLog(string s)
        {
            Debug.Log(s);
        }
    }
}
