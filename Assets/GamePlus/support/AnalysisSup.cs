using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Script.gameplus.define;
using Fabric.Answers;
using Assets.GamePlus.bean;
using Assets.GamePlus.FireBaseManager;
using Assets.Scripts.GameDefine;
using Assets.Scripts.Utils;
using Facebook.Unity;
using Firebase.Analytics;

public class AnalysisSup : MonoBehaviour
{

    public const string validUser = "validUser";
    public const int VALID_DAYS = 2;
    public const string pre = "day";
    public Dictionary<string, int> LevelProgress = new Dictionary<string, int>
    {
        {"day1",1},{"day2",2},{"day7",7},{"day30",30}
    };
    //登录时间
    public float onlineTime = 60*10;
	// Use this for initialization
	void Start () {

        int startCount = PlayerPrefs.GetInt(EventName.APP_START_COUNT, 0) + 1;
        PlayerPrefs.SetInt(EventName.APP_START_COUNT, startCount);
	    long subtime = PlayerInfoUtil.getSubTime(EventName.APP_INSTALL_TIME);
	    int days = (int) (subtime/(24*60*60));
        //启动次数
        logStartCount(startCount, days);
        //有效用户数
        logValidUser(days);
        Invoke("LogOnlineTime", onlineTime);
	    WeeklyReport();
	}

    private void logValidUser(int day)
    {
        //有效玩家
        if (day == VALID_DAYS && PlayerPrefs.GetInt(EventName.VALID_USER, 1) == 1)
        {
            int count = DynamicDataBaseService.GetInstance().GetHighscores().Count();
            if (count>=10)
            {
                Dictionary<string, object> validUser = new Dictionary<string, object>();
                validUser.Add("valid user", true + "");
                fabricLog(EventName.VALID_USER, validUser);
            }
            PlayerPrefs.SetInt(EventName.VALID_USER, 0);
        }
        //完成进度
        int a;
        if (LevelProgress.TryGetValue(pre + day, out a) && PlayerPrefs.GetInt(pre + day, 1) == 1)
        {
            int count = DynamicDataBaseService.GetInstance().GetHighscores().Count();
            int gold = PlayerPrefs.GetInt(MJDefine.GOLD_SPEND, 0);
            Dictionary<string, object> validUser = new Dictionary<string, object>();
            validUser.Add("LevelProgress_" + pre + day, count + "");
            validUser.Add("GoldProgress_s" + pre + day, gold + "");
            fabricLog(EventName.GAME_PROCESS, validUser);
            PlayerPrefs.SetInt(pre + day, 0);
        }
    }

    private void logStartCount(int startCount,int days)
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("app start count", startCount);
        fabricLog(EventName.APP_START_COUNT, desc);
    }

    string GetCountSection(int score)
    {
        string score_str = "";
        string score_pre = "startCount_";
        if (score <= 10)
        {
            return score_pre + score;
        }
        int remainder = score / 10;
        score_str = score_pre + (remainder + 1) * 10;
        return score_str;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void init() { 
    }
    /// <summary>
    /// fabric 购买日志
    /// </summary>
    public static void logPurchase(PurchaseItem item, string reason, bool success, string location)
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("status", reason);
        desc.Add("location", location);
        Answers.LogPurchase(
            new decimal(item.Price),
            "USD",
            success,
            item.DisplayName,
            item.ItemType,
            item.ProductId,
            desc
            );

        Dictionary<string, object> descGood = new Dictionary<string, object>();
        desc.Add("uid", LoginManager.UserId);
        desc.Add("transanctionid",InAppPurchaseSup.Transactionid);
        desc.Add("ProductId", item.ProductId);
        desc.Add("Price", item.Price+"");
        fabricLog("PurchaseLog", descGood);
        // firebase loger
        FirebaseAnalytics
            .LogEvent("purchase", new[]
            {
                new Parameter("price",item.Price),
                new Parameter("currency","USD"),
                new Parameter("name",item.DisplayName),
                new Parameter("type",item.ItemType),
            });
        //fb logger
        FB.LogPurchase(item.Price, "USD", descGood);
        //appflyer logger
        Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();
        purchaseEvent.Add("af_currency", "USD");
        purchaseEvent.Add("af_revenue", item.Price+"");
        purchaseEvent.Add("af_quantity", "1");
        AppsFlyer.trackRichEvent("af_purchase", purchaseEvent);
    }
    /// <summary>
    /// fabric 自定义日志
    /// </summary>
    public static void fabricLog(string eventName, Dictionary<string, object> desc)
    {
        //Dictionary<string, object> desc = new Dictionary<string, object>();
        Answers.LogCustom(eventName, desc);
    }

    public static void LogAd(bool suc,string reson="")
    {
        Dictionary<string, object> desc=new Dictionary<string, object>();
        desc.Add("result", suc?"fail":"success");
        desc.Add("reson", reson);
        Answers.LogCustom(EventName.PLAY_VIDEO, desc);
        if (suc)
        {
            int startCount = PlayerPrefs.GetInt(EventName.VIDEO_THREE, 0) + 1;
            PlayerPrefs.SetInt(EventName.VIDEO_THREE, startCount);
            if (startCount==3)
            {
                FacebookSup.Log(EventName.VIDEO_THREE, desc);
            }
        }

        desc.Add("EventName","watch video");
        FB.LogAppEvent(
        AppEventName.ViewedContent,
        null,
        desc);
        Debug.Log("LogAd");
    }

    void LogOnlineTime()
    {
        int startCount = PlayerPrefs.GetInt(EventName.ONLINE_TIME, 0) + 1;
        PlayerPrefs.SetInt(EventName.ONLINE_TIME, startCount);
    }

    void WeeklyReport()
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
        {
            if (PlayerPrefs.GetInt(EventName.ONLINE_TIME, 0)==6)
            {
                FacebookSup.Log(EventName.PERSIST10_LOGIN6, null);
            }
            if (PlayerPrefs.GetInt(EventName.ONLINE_TIME, 0) == 2)
            {
                FacebookSup.Log(EventName.PERSIST10_LOGIN2, null);
            }
        }
    }
}
