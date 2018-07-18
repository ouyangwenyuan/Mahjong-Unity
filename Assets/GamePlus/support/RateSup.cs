using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.gameplus.define;

public class RateSup : MonoBehaviour
{
    int out_rate = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void rate()
    {
        out_rate = 1;
        rateApp();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        //Debug.Log("OnApplicationFocus rate" + hasFocus);
        if (hasFocus && out_rate == 1)
        {
            Debug.Log("finish rate");
            //奖励
            PlayerPrefs.SetInt("isRate", 1);
            out_rate = 0;
        }
    }

    void rateApp()
    {
        //#if UNITY_ANDROID
        //    FacebookSup.log(EventName.RATE_ANDROID, "rate android app");
        //#elif UNITY_IPHONE
        //    FacebookSup.log(EventName.RATE_IOS, "rate ios app");
        //#endif
        Dictionary<string, object> desc = new Dictionary<string, object>();
        AnalysisSup.fabricLog(EventName.RATE, desc);
        Application.OpenURL(storeRateUri());
    }

    public static string storeRateUri()
    {
        string uri = "";
        #if UNITY_ANDROID
            uri = "market://details?id=" + AppConfig.PLAY_STORE_ID;
        #elif UNITY_IPHONE
			uri = "https://itunes.apple.com/app/id" + AppConfig.APPLE_STORE_ID;
        #endif
        return uri;
    }

}
