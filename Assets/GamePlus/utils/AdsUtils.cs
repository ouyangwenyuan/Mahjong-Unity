using Assets.Script.gameplus.define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.utils
{
    class AdsUtils
    {
        // true 应用resume时显示广告
        public static void setInterStutas(bool isShow)
        {
            //Debug.Log("setInterStutas:" + isShow);
            if (isShow)
            {
                PlayerPrefs.SetInt(Constance.INTERS_BAN_STATUS, 1);
            }
            else
            {
                PlayerPrefs.SetInt(Constance.INTERS_BAN_STATUS, 0);
            }
        }

        public static bool getIntersStutas()
        {
            int status = PlayerPrefs.GetInt(Constance.INTERS_BAN_STATUS, 1);
            //Debug.Log("getIntersStutas:" + status);
            if (status == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
