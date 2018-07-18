using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.support;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.FireBaseManager;
using Assets.Scripts.FirebaseController;
using Assets.Scripts.Utils;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIController
{
    public enum SetItems
    {
        Music=1,
        Sound=2,
        Hint=3
    }
    class SettingController : MonoBehaviour
    {
        //常量
        private float pauseTime = 0;
        private const string InappLink = "fb://page/1299846306791430";
        private const string WebLink = "https://www.facebook.com/MahJong-Community-1299846306791430";
        private const string EmailAdress = "blueflames.lau@gmail.com";
        //支持类
        private FacebookSup fbSup;
        private SocialServiceSup socialSup;
        //UI
        public Text FbText;
        public Text FBdescText;
        public Transform ParenTransform;
        //是否点击了fb登录按钮
        public static bool FBClicked = false;
        //public Transform ParenTransform;
        public Transform userSet;
        //public GameObject[] Objects;
        public Dictionary<SetItems,bool> status=new Dictionary<SetItems, bool>(); 
        //

        public AudioSource music;
        public AudioSource sfx;
        public AudioSource audio_hint;

        AudioClipSet audioclip_set;
        void Awake()
        {
            FBdescText = transform.Find("TextFB").GetComponent<Text>();
            FbText = transform.Find("Button_Facebook").Find("Text").GetComponent<Text>();
            fbSup = GameObject.Find("GamePlus").GetComponent<FacebookSup>();
            socialSup = GameObject.Find("GamePlus").GetComponent<SocialServiceSup>();
            GameObject go_audioclip_set = GameObject.Find("AudioClipSet");
            if (go_audioclip_set != null) { audioclip_set = go_audioclip_set.GetComponent<AudioClipSet>(); }
        }

        void Start()
        {
            UpdateFB();
            string[] items = Enum.GetNames(typeof(SetItems));
            foreach (string item in items)
            {
                bool b = false;

                if(item.Equals("Music")){
                    b = LocalDynamicData.GetInstance().GetMusicOn();
                }else if(item.Equals("Sound")){
                    b = LocalDynamicData.GetInstance().GetSFXOn();
                }
                else if (item.Equals("Hint"))
                {
                    b = LocalDynamicData.GetInstance().GetHintOn();
                }

                //int setType = PlayerPrefs.GetInt(item, 1);
                SetItems curItem = (SetItems)Enum.Parse(typeof(SetItems), item);
                status.Add(curItem, b/*setType==1*/);
                ChangeState(curItem);
            }
        }

        public void UpdateFB()
        {
            if (FB.IsLoggedIn)
            {
                FbText.text = "DISCONNECT";
                FBdescText.text = "disconnect fb change account";
            }
            else
            {
                FbText.text = "CONNECT";
                FBdescText.text = "log in facebook to play with friends";
            }
        }

        public bool ChangeState(SetItems type)
        {
            GameObject on = userSet.Find(type.ToString()).Find("Button").Find("toggelOn").gameObject;
            GameObject off = userSet.Find(type.ToString()).Find("Button").Find("toggelOff").gameObject;
            if (status[type])
            {
                status[type] = false;
                //PlayerPrefs.SetInt(type.ToString(), 1);
                on.SetActive(true);
                off.SetActive(false);
                return true;
            }
            else
            {
                status[type] = true;
                //PlayerPrefs.SetInt(type.ToString(), 0);
                on.SetActive(false);
                off.SetActive(true);
                return false;
            }
        }

        void Update()
        {
            UpdateFB();
        }

        public void OnHelp()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("setting", "OnHelp");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            Application.OpenURL(InappLink);
            Invoke("OpenInWeb", 0.5f);
            Debug.Log("OnHelp");
        }

        public void OnContact()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("setting", "OnContact");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            OnEmail();
            Debug.Log("OnContact");
        }


        public void OnCopy()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("setting", "OnCopy");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

//            UploadErrorLog();

            Debug.Log("OnCopy");
            NativeUtils.CopyText(LoginManager.UserId);
        }

        private static void UploadErrorLog()
        {
            string path = Application.persistentDataPath + "//" + Constance.ERROR_LOG;

            StreamWriter writer = new StreamWriter(path, true);
            writer.AutoFlush = true;
            writer.WriteLine("uuid=>" + LoginManager.UserId);
            writer.WriteLine("deviceid=>" + DeviceUtils.GetUuid());
            writer.WriteLine("ostype=>" + SystemInfo.operatingSystem);
            writer.Close();

            string remotePath = FireBaseConfig.EROR_LOG_PATH + "/" + DeviceUtils.GetUuid() + "_" +
                                PlayerInfoUtil.GetTimeStamp() + ".log";
            SdkController.Instance.UploadLogFile(path, remotePath);
        }

        public void OnShare()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("setting", "OnShare");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);
//            Application.OpenURL("http://imei.mdc.akamaized.net/app18_Privacy.html");
            Debug.Log("OnShare");
            fbSup.share();
//            if (FB.IsLoggedIn)
//            {
//                fbSup.share();
//            }
//            else
//            {
//                controller.FBConnect();
//            }
//            GameObject.Find("GamePlus").GetComponent<NativeShare>().ShareScreenshotWithText("Mahjong");
        }

        public void OnFBlogin()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("setting", "OnFBlogin");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

             Debug.Log("OnFBlogin");
            if (FB.IsLoggedIn)
            {
                FB.LogOut();
                FacebookSup.Instance.SetHeader();
            }
            else
            {
                LoginController.Instance.BindFacebook();
            }
        }

        public void OnEmail()
        {
            //email Id to send the mail to
            string email = EmailAdress;
            //subject of the mail
            string subject = MyEscapeURL("FEEDBACK/SUGGESTION");
            //body of the mail which consists of Device Model and its Operating System
            string body = MyEscapeURL("Please Enter your message here\n\n\n\n" +
             "________" +
             "\n\nPlease Do Not Modify This\n\n" +
             "Model: " + SystemInfo.deviceModel + "\n\n" +
                "OS: " + SystemInfo.operatingSystem + "\n\n" +
                "id: " + LoginManager.UserId + "\n\n" +
             "________");
            //Open the Default Mail App
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        void OpenInWeb()
        {
            if (SupportListener.pauseTime < 1f)
            {
                //fail. Open safari.
                Application.OpenURL(WebLink);
            }
        }

        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public void ToogleMusic()
        {
            bool b = ChangeState(SetItems.Music);
            if (b)
            {
                AudioSourcesManager.GetInstance().Play(music, (audioclip_set == null) ? null : audioclip_set.card_toggle);
            }
            LocalDynamicData.GetInstance().ChangeMusicOn();
            Debug.Log("ToogleMusic");
        }

        public void ToogleSound()
        {
            bool b = ChangeState(SetItems.Sound);
            if (b)
            {
                AudioSourcesManager.GetInstance().Play(music, (audioclip_set == null) ? null : audioclip_set.card_toggle);
            }
            LocalDynamicData.GetInstance().ChangeSFXOn();
            Debug.Log("ToogleSound");
        }

        public void ToogleHint()
        {
            bool b = ChangeState(SetItems.Hint);
            if (b)
            {
                AudioSourcesManager.GetInstance().Play(music, (audioclip_set == null) ? null : audioclip_set.card_toggle);
            }
            LocalDynamicData.GetInstance().ChangeHintOn();
            Debug.Log("ToogleHint");
        }

    }
}
