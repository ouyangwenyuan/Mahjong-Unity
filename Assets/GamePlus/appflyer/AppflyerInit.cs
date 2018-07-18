using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using UnityEngine;

namespace Assets.GamePlus.appflyer
{
    public class AppflyerInit:MonoBehaviour
    {
        void Start()
        {
            // For detailed logging
            AppsFlyer.setIsDebug (true);
#if UNITY_IOS
            AppsFlyer.setAppsFlyerKey ("aenSuFdSXYGEa8HyMfspZA");
           AppsFlyer.setAppID (AppConfig.APPLE_STORE_ID);
           AppsFlyer.trackAppLaunch ();
             AppsFlyer.getConversionData ();
#elif UNITY_ANDROID
            //Mandatory - set your Android package name
            AppsFlyer.setAppID(AppConfig.PLAY_STORE_ID);
            //Mandatory - set your AppsFlyer’s Developer key.
            AppsFlyer.init("aenSuFdSXYGEa8HyMfspZA");
            AppsFlyer.loadConversionData("AppsFlyerTrackerCallbacks");
#endif
        }
    }
}
