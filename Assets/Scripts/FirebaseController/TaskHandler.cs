using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Bean;
using Firebase.Database;
using UnityEngine;

namespace Assets.Scripts.FirebaseController
{
    public class TaskHandler
    {
        public TaskHandler()
        {
            
        }

        public void UidCallBack(Task<DataSnapshot> task)
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
            if (string.IsNullOrEmpty(task.Result.GetRawJsonValue()))
            {
                return;
            }
            Debug.Log(task.Result.GetRawJsonValue());
            foreach (var dataSnapshot in task.Result.Children)
            {
                Debug.Log(dataSnapshot.GetRawJsonValue());
                UserInfo userInfo = JsonUtility.FromJson<UserInfo>(dataSnapshot.GetRawJsonValue());
                FriendData curFriendData =
                    DynamicDataBaseService.GetInstance().GetFriendData().First(x => x.fb_id.Equals(userInfo.Uid));
                Debug.Log(curFriendData.fb_id);
                Debug.Log(userInfo.Fbid);
                if (curFriendData != null)
                {
                    curFriendData.gs_id = userInfo.Uid;
                    curFriendData.stage = userInfo.Stage;
                    DynamicDataBaseService.GetInstance().UpdateData(curFriendData);
                }
            }
        }
    }
}
