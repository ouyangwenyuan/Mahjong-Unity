using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Assets.Script.gameplus.define;
using Assets.Scripts.Utils;
using Facebook.Unity;

#if UNITY_IPHONE
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

public class NotificationUtils : MonoBehaviour {

    public static readonly string[] NotificationDesc = 
        { 
        "Boring day? Take worlds challenge in Mahjong Master! \uD83C\uDF89",
        "New levels \uD83C\uDC04 are waiting for you to unlock!", 
        "Wonderful moment in Mahjong Master, don’t miss it!",
        "Spend time for a little amusement with Mahjong Master.",
        "Share the joy of challenging with your friends!",
        "Trust Mahjong Master, it will bring you more happy time!",
        "Wonderful moment in Mahjong Master, don’t miss it!",
        "What amazing mahjong tiles! Come to try!"
        };

    public int NotifyTime = 20;
    public bool debug = false;

    public bool SendHeartNty = false;
    //本地推送
    public static void NotificationMessage(string message, int hour, bool isRepeatDay)
    {
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        DateTime newDate = new DateTime(year, month, day, hour, 0, 0);
        NotificationMessage(message, newDate, isRepeatDay);
    }

    public static void NotificationMessage2(string message, DateTime date, bool isRepeatDay)
    {
        NotificationMessage(message, date, isRepeatDay);
    }
    //本地推送 你可以传入一个固定的推送时间
    public static void NotificationMessage(string message, DateTime newDate, bool isRepeatDay)
    {
#if UNITY_IPHONE
		//推送时间需要大于当前时间
		if(newDate > System.DateTime.Now)
		{
			UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
			localNotification.fireDate =newDate;	
			localNotification.alertBody = message;
			localNotification.applicationIconBadgeNumber = -1;
			localNotification.hasAction = true;
			localNotification.alertAction = Application.productName;
			if(isRepeatDay)
			{
				//是否每天定期循环
				localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
				localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
			}
			localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
		}
#endif
#if UNITY_ANDROID
        if (newDate > DateTime.Now)
        {
            long delay = PlayerInfoUtil.StampSub(newDate, DateTime.Now);
            Debug.Log("delay " + delay);
            if (isRepeatDay)
            {
                LocalNotification.SendRepeatingNotification(2, delay, 24 * 60 * 60, Application.productName, message, new Color32(0xff, 0x44, 0x44, 255));
            }
            else
            {
                LocalNotification.SendNotification(1, delay, Application.productName, message, new Color32(0xff, 0x44, 0x44, 255));
            }
        }
#endif
    }

    void Awake()
    {
#if UNITY_IPHONE
		UnityEngine.iOS.NotificationServices.RegisterForNotifications (
			NotificationType.Alert |
			NotificationType.Badge |
			NotificationType.Sound);
#endif
        //第一次进入游戏的时候清空，有可能用户自己把游戏冲后台杀死，这里强制清空
        CleanNotification();
    }

    void OnApplicationPause(bool paused)
    {
        if (PlayerPrefs.GetInt(EventName.NOTIFICTION, 1) != 1) return;
        //程序进入后台时
        if (paused)
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("notifiction", "send notifiction");
            AnalysisSup.fabricLog(EventName.GAME, desc);
            LifeNotify();
            FixedNotify();
        }
        else
        {
            //程序从后台进入前台时
            CleanNotification();
        }
    }

    private static void LifeNotify()
    {
        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
        Debug.Log("playerInfo.Life " + playerInfo.Life);
        Debug.Log("playerInfo.MaxLife " + playerInfo.MaxLife);
        //无限体力不推送
        if ("0".Equals(playerInfo.Infinite_time) && 
            playerInfo.Life < playerInfo.MaxLife && 
            MainFrontController.CoolTimes!=0)
        {
                long time = PlayerInfoUtil.GetTStamp() + MainFrontController.CoolTimes;
                DateTime newTime = PlayerInfoUtil.GetTime(time + "");
                Debug.Log("体力推送开启newTime " + newTime);
            NotificationMessage("Lives are recovered. Play again Now!", newTime, false);
        }
    }

    private void FixedNotify()
    {
        //每天准点点推送
        int index = PlayerPrefs.GetInt("NOTIFICATION_INDEX", 0);
        Debug.Log("每天" + NotifyTime + "点发送通知");
        NotificationMessage(NotificationDesc[index], NotifyTime, true);
        index++;
        if (index >= NotificationDesc.Length)
            index = 0;
        PlayerPrefs.SetInt("NOTIFICATION_INDEX", index);
    }

    //
    bool isStart()
    {
        return Time.timeSinceLevelLoad > 30;
    }

    void Update()
    {
    }

    //清空所有本地消息 
    void CleanNotification()
    {
#if UNITY_IPHONE
    StartCoroutine(waitClear());
    UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications (); 
    UnityEngine.iOS.NotificationServices.ClearLocalNotifications (); 
#endif
#if UNITY_ANDROID
        LocalNotification.CancelNotification(1);
        LocalNotification.CancelNotification(2);
#endif
    }
    //[UIApplication sharedApplication].applicationIconBadgeNumber=0;
#if UNITY_IPHONE
  IEnumerator waitClear()
  {
    yield return new WaitForSeconds(10f);
    UnityEngine.iOS.LocalNotification l = new UnityEngine.iOS.LocalNotification (); 
    l.applicationIconBadgeNumber = -1; 
    l.hasAction = false;
    UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow (l); 
    UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications (); 
    UnityEngine.iOS.NotificationServices.ClearLocalNotifications (); 
  }
#endif
}
