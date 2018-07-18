using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using Assets.Scripts.Data.BeanMap;
using Assets.Scripts.FireBaseManager;
using UnityEngine;

namespace Assets.ATest
{
    public class FirebaseTest:MonoBehaviour
    {
        void Start()
        {
            
        }

        public void OnSignIn()
        {
            Debug.Log("OnSignIn");
            LoginManager.Instance.FireBaseSignIn("jansen@gmail.com", "123456");
        }

        public void OnRegister()
        {
            Debug.Log("OnRegister");
            LoginManager.Instance.RegisterUser("jansen@gmail.com", "123456");
        }

        public void UploadTask()
        {
//            List<DailyStatistic> dailyStatistics = DynamicDataBaseService.GetInstance().GetDailyStatistics().ToList();
//            List<DailyStatisticMap> dailyMaps = dailyStatistics.ConvertAll(x => new DailyStatisticMap(x));
//            string dailyStr = JsonUtility.ToJson(dailyMaps);
//            Dictionary<string, object> entryValues = new Dictionary<string, object>();
//            entryValues["data"] = dailyStr;
//            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
//            childUpdates[FireBaseConfig.TaskPath + LoginManager.UserId] = entryValues;
//            RemoteDbManager.Instance.Rdb.UpdateChildrenAsync(childUpdates);
        }

        public void Download()
        {
            
        }
    }
}
