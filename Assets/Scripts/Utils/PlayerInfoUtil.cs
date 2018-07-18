using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.utils;
using Assets.Scripts.GameDefine;
using Assets.Scripts.UIController;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class PlayerInfoUtil : MonoBehaviour
    {
        public static long Countdown = 0;
        public static long UnlimiteCountdown = 0;
        public static bool InfiniteHeart;
        public static PlayerInfo GetLifeInfo()
        {
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            long infiniteTime;
            long curTime;
            long.TryParse(playerInfo.Infinite_time, out infiniteTime);
            long.TryParse(GetTimeStamp(), out curTime);
            //无限体力倒计时
            if (infiniteTime > curTime)
            {
                InfiniteHeart = true;
                UnlimiteCountdown = infiniteTime - curTime;
                return playerInfo;
            }
            InfiniteHeart = false;
            playerInfo.Infinite_time = "0";
            //体力最大
            if ("0".Equals(playerInfo.MaxLifeTime))
            {
                Countdown = 0;
                return playerInfo;
            }
            //需要倒计时
            long oldTime;
            long.TryParse(GetTimeStamp(), out curTime);
            long.TryParse(playerInfo.MaxLifeTime, out oldTime);
            long subTime = curTime - oldTime;
            int multiple = (int) (subTime / (CommonData.HEART_COOLING_TIME * 60));
            Countdown = CommonData.HEART_COOLING_TIME*60 - (int) (subTime%(CommonData.HEART_COOLING_TIME*60));
            if (multiple+playerInfo.Life>=playerInfo.MaxLife)
            {
                playerInfo.MaxLifeTime = "0";
                playerInfo.Life = playerInfo.MaxLife;
                Countdown = 0;
            }
            else
            {
                playerInfo.Life += multiple;
            }
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            return playerInfo;
        }

        public static bool ConsumeLife()
        {
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            long infiniteTime;
            long curTime;
            long.TryParse(playerInfo.Infinite_time, out infiniteTime);
            long.TryParse(GetTimeStamp(), out curTime);
            //无限体力
            if (infiniteTime > curTime)
            {
                return true;
            }
            playerInfo.Infinite_time = "0";
            if (playerInfo.Life==0)
            {
                ShopController.instance.show(ShopItem.Heart);
                //体力为0
                return false;
            }
            if (playerInfo.Life==playerInfo.MaxLife)
            {
                playerInfo.MaxLifeTime = GetTimeStamp() + "";
            }
            playerInfo.Life -= 1;
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            return true;
        }

        public static void AddLife(int lifeNum)
        {
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            playerInfo.Life += lifeNum;
            if (playerInfo.Life >= playerInfo.MaxLife)
            {
                Countdown = 0;
                playerInfo.Life = playerInfo.MaxLife;
                playerInfo.MaxLifeTime = "0";
            }
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
        }

        public static void ShowDialog(DialogItem item,string title = "")
        {
            GameObject.Find("GobalUI").GetComponent<DialogController>().show(item, title);
        }

        public static void SetConsumeGold(int gold)
        {
 
            PlayerPrefs.SetInt(MJDefine.GOLD_SPEND, PlayerPrefs.GetInt(MJDefine.GOLD_SPEND, 0) + gold);
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static long GetTStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static long StampSub(DateTime dateTime1, DateTime dateTime2)
        {
            TimeSpan ts = dateTime1 - dateTime2;
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime;
            long.TryParse(timeStamp + "0000000",out lTime);
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }

        public static long getSubTime(string key)
        {
            string oldTimeStr = PlayerPrefs.GetString(key, "");
            string curTimeStr = GetTimeStamp();
            long oldTime;
            long curTime;
            long.TryParse(oldTimeStr, out oldTime);
            long.TryParse(curTimeStr, out curTime);
            return curTime - oldTime;
        }

        public static double GetTimeDouble()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalSeconds;
        }
    }

}
