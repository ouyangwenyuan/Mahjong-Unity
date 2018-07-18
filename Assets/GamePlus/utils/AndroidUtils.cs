using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.gameplus.define
{
    class AndroidUtils
    {
        public static bool isPlayInstalled() 
        {
            bool installed = true;
            string bundleId = "com.android.vending";
            AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = ajc.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject pkm = context.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = null;
            try
            {
                launchIntent = pkm.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
                installed = false;
            }
            ajc.Dispose();
            context.Dispose();
            pkm.Dispose();
            launchIntent.Dispose();
            return installed;
        }
    }
}
