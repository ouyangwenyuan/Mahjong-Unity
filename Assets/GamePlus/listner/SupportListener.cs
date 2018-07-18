using Assets.GamePlus.utils;
using Assets.Script;
using Assets.Script.gameplus.define;
using Assets.Script.utils;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace Assets.GamePlus.support
{
    /// <summary>
    /// 登录、云存档下载的回调监听，全部初始化完成才能进入游戏并正常的使用第三方服务
    /// FB分享，成就、排行榜、云存档
    /// </summary>
    class SupportListener : MonoBehaviour
    {
        //private bool isCloudArchiveInit = false;
        // Update is called once per frame
        public static SupportListener instance;
        private double exitTime = 0;
        private int count = 0;
        public static long pauseTime=0;

        void Awake()
        {
            // Limit the number of instances to one
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // duplicate
                Destroy(gameObject);
            }
        }

        void Start()
        {
            //PlayerPrefs.DeleteAll();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)){
                double time = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;
                if (time - exitTime > 2000)
                {
                    Debug.Log("在按一次退出程序");
                    exitTime = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;
                }
                else {
                    ExitGame();
                }
            }
                
        }

        void OnGUI()
        {
        }

        private static void ExitGame()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
            else
            {
                Application.Quit();
            }
        }

         void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                string old_time_str = PlayerPrefs.GetString(Constance.RESUME_GAME, "5");
                double old_time = Convert.ToDouble(old_time_str);
                double cur_time = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
                //Debug.Log("OnApplicationPause:" + (cur_time - old_time));
                pauseTime = (long) (cur_time - old_time);
                if (cur_time - old_time >= Constance.RESUME_TIME_OFFSET) {
                    if (AdsUtils.getIntersStutas())
                    {
                        PlayerPrefs.SetString(Constance.RESUME_GAME, "5");
                    }
                    AdsUtils.setInterStutas(true);
                }
            }
            else
            {
                double time = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
                PlayerPrefs.SetString(Constance.RESUME_GAME, time + "0");
            }
        }


        void OnEnable()
        {
            SocialServiceSup.socialLoginHandler += socialLoginHandler;
        }

        void OnDisable()
        {
            SocialServiceSup.socialLoginHandler -= socialLoginHandler;
        }

        void socialLoginHandler(bool success)
        {
            Debug.Log("socialLoginHandler:" + success);
            
            if (success)
            {
                //  检查用户是否有云存档，下载云存档
            }
            else {
                //    以游客身份进入
            }
        }
    }
}
