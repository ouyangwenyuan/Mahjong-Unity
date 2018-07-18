using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using Assets.Script.gameplus.define;
using Assets.Scripts.Bean.log;
using Assets.Scripts.Data.BeanMap;
using Assets.Scripts.Data.Excel2CS.dynamic;
using Assets.Scripts.FireBaseManager;
using Assets.Scripts.Utils;
using Fabric.Internal.ThirdParty.MiniJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.GamePlus.utils
{
    public class GamePlusLog:MonoBehaviour
    {
        public static GamePlusLog Instance;
        //日志正式服
        public const string LogRelease = "http://ani.gameplus.io/api/";
        //日志测试服
        public const string LogDebug = "http://10.0.12.189:9000/api/";
        //是否开启正式服
        public bool ReleaseOn=true;
        //应用id
        public const int GAME_ID = 1001;
        //日志保留天数
        public int LogCacheTime = 3;
        private long DaySeconds = 24*60*60;
        private string _purchaseFunction = "https://us-central1-balls-d0b54.cloudfunctions.net/get_charge?Uid=";
        public IEnumerator UploadLog(BaseLog log,string action)
        {
            UnityWebRequest www = UnityWebRequest.Post(ReleaseOn ? LogRelease : LogDebug + ParseAction(action), log.GenerateForm());
            yield return www.Send();

            if (www.isNetworkError)
            {
                AddErrorLog(log, action);
                Debug.LogError(www.error);
            }
            else
            {
                DelCacheLog(action);
                Debug.Log(action + "log upload complete!   " + www.responseCode);
            }
        }

        public IEnumerator UploadLog(string url)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("purchase log upload complete!");
            }
        }

        //日志重发成功后删除
        private static void DelCacheLog(string action)
        {

            if (DynamicDataBaseService.GetInstance().GetLogCache().Any(x => x.Action.Equals(action)))
            {
                LogCache map = DynamicDataBaseService.GetInstance().GetLogCache().First(x => x.Action.Equals(action));
                DynamicDataBaseService.GetInstance().Connection.Delete(map);
            }
        }

        //防止重复日志
        private static void AddErrorLog(BaseLog log, string action)
        {
            if (!DynamicDataBaseService.GetInstance().GetLogCache().Any(x => x.Action.Equals(action)))
            {
                LogCache cache = new LogCache();
                cache.Action = action + "_" + PlayerInfoUtil.GetTimeStamp();
                cache.JsonData = Json.Serialize(log.Args);
                cache.UpdateTime = PlayerInfoUtil.GetTimeStamp();
                DynamicDataBaseService.GetInstance().InsertData(cache);
            }
            else
            {
                Debug.Log("duplicate log");
            }
        }

        void Update()
        {
            
        }

        void Start()
        {
            Instance = this;
            //删除过期日志
            DeleteOldLog();
        }

        void DeleteOldLog()
        {
            List<LogCache> oldCaches = DynamicDataBaseService.GetInstance().GetLogCache().Where(FilterOldCache).ToList();
            foreach (var oldCach in oldCaches)
            {
                DynamicDataBaseService.GetInstance().Connection.Delete(oldCach);
            }
            //重复检查失败日志
            InvokeRepeating("CheckFailLig", 10f, 20f);
        }

        bool FilterOldCache(LogCache cache)
        {
            long timeDelta = PlayerInfoUtil.StampSub(DateTime.Now, PlayerInfoUtil.GetTime(cache.UpdateTime));
            if (timeDelta >= LogCacheTime*DaySeconds)
            {
                return true;
            }
            return false;
        }

        void CheckFailLig()
        {
            if (DynamicDataBaseService.GetInstance().GetLogCache().Any())
            {
                LogCache map = DynamicDataBaseService.GetInstance().GetLogCache().FirstOrDefault();
                Dictionary<string, object> args = (Dictionary<string, object>) Json.Deserialize(map.JsonData);
                BaseLog baseLog = new BaseLog(args);
                StartCoroutine(UploadLog(baseLog, map.Action));
            }
        }

        static string ParseAction(string action)
        {
            return action.Split('_')[0];
        }

        /// <summary>
        /// 绑定日志
        /// </summary>
        public void BindLog()
        {
            string fbid = PlayerPrefs.GetString(Constance.FACEBOOK_ID, "");
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("playerId", LoginManager.UserId);
            args.Add("game_id", GAME_ID+"");
            args.Add("deviceId", DeviceUtils.GetUuid());
            args.Add("plat_id", "facebook");
            args.Add("plat_account", fbid);
            BaseLog baseLog = new BaseLog(args);
            StartCoroutine(UploadLog(baseLog, ActionLog.Bind.ToString()));
        }

        /// <summary>
        /// 绑定日志
        /// </summary>
        public void LoginLog()
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("playerId", LoginManager.UserId);
            args.Add("game_id", GAME_ID + "");
            args.Add("deviceId", DeviceUtils.GetUuid());
            args.Add("version", Application.version);
            BaseLog baseLog = new BaseLog(args);
            StartCoroutine(UploadLog(baseLog, ActionLog.Login.ToString()));
        }

        /// <summary>
        /// 消耗金币
        /// </summary>
        public void SpentLog(float gold, string itemId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("playerId", LoginManager.UserId);
            args.Add("game_id", GAME_ID + "");
            args.Add("spent", gold+"");
            args.Add("itemID", itemId);
            BaseLog baseLog = new BaseLog(args);
            StartCoroutine(UploadLog(baseLog, ActionLog.Spent.ToString()));
        }

        /// <summary>
        /// 游戏时间
        /// </summary>
        public void PlayTimeLog(float time,int level)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("playerId", LoginManager.UserId);
            args.Add("game_id", GAME_ID + "");
            args.Add("time", time+"");
            args.Add("gate", level+"");
            BaseLog baseLog = new BaseLog(args);
            StartCoroutine(UploadLog(baseLog, ActionLog.Playtime.ToString()));
        }

        /// <summary>
        /// 购买
        /// </summary>
        public void ChargeLog(float usd)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("playerId", LoginManager.UserId);
            args.Add("game_id", GAME_ID + "");
            args.Add("amount", usd + "");
            args.Add("TransactionID", InAppPurchaseSup.Transactionid);
            BaseLog baseLog = new BaseLog(args);
            StartCoroutine(UploadLog(_purchaseFunction + LoginManager.UserId));
            StartCoroutine(UploadLog(baseLog, ActionLog.Charge.ToString()));
        }
    }
}
