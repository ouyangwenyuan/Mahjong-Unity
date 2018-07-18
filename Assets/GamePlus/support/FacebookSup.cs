using Assets.GamePlus.bean;
using Assets.Script.gameplus.define;
using Facebook.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using Fabric.Internal.ThirdParty.MiniJSON;
using UnityEngine;

public class FacebookSup : MonoBehaviour
{
    public static FacebookSup Instance;
    //login call back event
    public static event Action<string> fbLoginHandler;
    //single cover download callback
    public static event Action<string> singleCoverCallback;
    //friends cover download callback
    public static event Action<List<FBFriendInfo>> CoversCallback;
    private bool isShare = true;
    private int refreshTokenRate = 5*60;
    public static int IsFbLogining=0;
    private bool _samefbUser;
	// Use this for initialization
	void Start ()
	{
	    Instance = this;
        init();
	}
	
	void Update () {
        
	}
    void init() {
        FB.Init(OnInitComplete, OnHideUnity);
        string Status = "FB.Init() called with " + FB.AppId;
    }
    /**
     * 登录
     * */
    public void CallFBLogin()
    {
        IsFbLogining = 0;
        isShare = true;
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, HandleResult);
    }
    /**
     * 默认分享
     * */
    public void share()
    {
        isShare = false;
        FB.ShareLink(
				new Uri(AppConfig.APP_LINK),
                AppConfig.shareTitle,
                AppConfig.shareDescription(),
                new Uri(AppConfig.shareImageUrl()), HandleResult);
    }
    /**
     * 自定义分享
     * */
    public void share(string shareTitle, string shareDescription, string shareImageUrl)
    {
        isShare = false;
        FB.ShareLink(
                new Uri(AppConfig.APP_LINK),
                shareTitle,
                shareDescription,
                new Uri(shareImageUrl), HandleResult);
    }
    /**
     * facebook日志记录
     * */
    public static void Log(string evName, Dictionary<string,object> evDesc)
    {
        FB.LogAppEvent(
            evName,
            null,
            evDesc);
    }

    /**
     * facebook邀请好友
     * */
    public void invite()
    {
        FB.Mobile.AppInvite(new Uri(AppConfig.APP_LINK), callback: this.HandleResult);
    }

    //刷新token，延长登录时间
    public void refreshAccessToken()
    {
        FB.Mobile.RefreshCurrentAccessToken(this.HandleResult);
    }
    /**
     * 退出登录
     * */
    public void logout()
    {
        FB.LogOut();
    }

    //获取当前用户信息
    public void getUserInfo()
    {
        FB.API("/me?fields=id,name,picture", HttpMethod.GET, this.handleUserInfo);
    }

    //获取单个好友头像

    public void getFriendCover(string fbid)
    {
        FB.API("/" + fbid + "/picture", HttpMethod.GET, this.handleSingleCover);
    }

    //获取所有注册了应用的好友头像
    public void getFriendsCover()
    {
        FB.API("me/friends?fields=id,name", HttpMethod.GET, this.handleMultipleCover);
    }


    protected void HandleResult(IResult result)
    {
        IsFbLogining++;
        if (result == null)
        {
            string LastResponse = "Null Response\n";
            Debug.Log(LastResponse);
            return;
        }


        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            string LastResponse = "Error Response:\n" + result.Error;
            Debug.Log(LastResponse);
        }
        else if (result.Cancelled)
        {
            string LastResponse = "Cancelled Response:\n" + result.RawResult;
            Debug.Log(LastResponse);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            if (IsFbLogining==1)
            {
                fireEvent(result);
            }
            else
            {
                Debug.Log("重复的回调");
            }
        }
        else
        {
            string LastResponse = "Empty Response\n";
            Debug.Log(LastResponse);
        }
    }

    void fireEvent(IResult result)
    {
        if (result is ILoginResult)
        {
            handleLogin(result);
        }

//        if (result is IAccessTokenRefreshResult)
//        {
//            handleLogin(result);
//        }

        if (result is AppInviteResult)
        {
            handleInvite(result);
        }

    }

    void handleInvite(IResult result)
    {
        
    }

    void handleLogin(IResult result)
    {
        string ac_token = (string)result.ResultDictionary["access_token"];
        if (!string.IsNullOrEmpty(ac_token))
        {
            //存储过期时间
            string expiration_timestamp = (string)result.ResultDictionary["expiration_timestamp"];
            PlayerPrefs.SetString(Constance.FB_EXPIRATION, expiration_timestamp);
            PlayerPrefs.SetString(Constance.FB_ACCESSTOKEN, ac_token);
            SetHeader();
            getUserInfo();
        }
    }

    //好友表的第一条数据存取是当前登录用户信息
    void handleUserInfo(IResult result)
    {
        if (!string.IsNullOrEmpty(result.Error))
        {
            string LastResponse = "Error Response:\n" + result.Error;
            Debug.Log(LastResponse);
        }
        Dictionary<string, object> dataDictionary = (Dictionary<string, object>) Json.Deserialize(result.RawResult);
//        Dictionary<string, object> picture = (Dictionary<string, object>)dataDictionary["picture"];
//        Dictionary<string, object> data = (Dictionary<string, object>)picture["data"];
//        string url = (string)data["url"];
        string id = (string)dataDictionary["id"];
        string name = (string)dataDictionary["name"];
        FriendData friendData = DynamicDataBaseService.GetInstance().GetFriendData().First();
        friendData.fb_id = id;
        friendData.name = name;
        friendData.head_WebPath = GetHightQualityHead(id);
        PlayerPrefs.SetString(Constance.FACEBOOK_ID,id);
        DynamicDataBaseService.GetInstance().Connection.DeleteAll<FriendData>();
        DynamicDataBaseService.GetInstance().Connection.Insert(friendData);
//        GPLog(id);
        SetHeader();
        //获取好友信息
        getFriendsCover();
    }

    void handleSingleCover(IResult result)
    {
        Dictionary<string, object> singleData = (Dictionary<string, object>)result.ResultDictionary["data"];
        string pic_url = (string)singleData["url"];
        if (!string.IsNullOrEmpty(pic_url))
        {
            //存储图片
            Debug.Log("pic_url " + pic_url);
            if (singleCoverCallback != null)
            {
                singleCoverCallback(pic_url);
            }
        }
    }


    void handleMultipleCover(IResult result)
    {
        if (string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("fb好友为空");
            if (fbLoginHandler != null)
            {
                fbLoginHandler("");
                return;
            }
        }
        Debug.Log(result.RawResult);
        Dictionary<string, object> infoDictionary = (Dictionary<string, object>) Json.Deserialize(result.RawResult);
        List<object> friendsInfo = (List<object>)infoDictionary["data"];
        List<FriendData> friendsInfos = new List<FriendData>();
        for (int i = 0; i < friendsInfo.Count; i++)
        {
            FriendData fbinfo = new FriendData();
            object temp = friendsInfo[i];
            Dictionary<string, object> info = (Dictionary<string, object>)temp;
            string id = (string)info["id"];
            string name = (string)info["name"];
            fbinfo.fb_id = id;
            fbinfo.name = name;
            fbinfo.id = i + 1;
            fbinfo.head_WebPath = GetHightQualityHead(id);
            friendsInfos.Add(fbinfo);
        }
        DynamicDataBaseService.GetInstance().Connection.InsertAll(friendsInfos);
        if (fbLoginHandler != null)
        {
            fbLoginHandler("");
        }
        IsFbLogining = 0;
    }

    private void OnInitComplete()
    {
        string Status = "Success - Check log for details";
        Debug.Log(Status);
        string LastResponse = "Success Response: OnInitComplete Called\n";
        Debug.Log(LastResponse);
        string logMessage = string.Format(
            "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
            FB.IsLoggedIn,
            FB.IsInitialized);
        Debug.Log(logMessage);
        if (AccessToken.CurrentAccessToken != null)
        {
            Debug.Log(AccessToken.CurrentAccessToken.ToString());
        }
        FB.ActivateApp();
    }

    private void OnHideUnity(bool isGameShown)
    {
        string Status = "Success - Check log for details";
        Debug.Log(Status);
        string LastResponse = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
        Debug.Log(LastResponse);
        Debug.Log("Is game shown: " + isGameShown);
    }

    public static string storeShareUrl()
    {
        string uri = "";
        #if UNITY_ANDROID
                uri = "https://play.google.com/store/apps/details?id=" + AppConfig.PLAY_STORE_ID;
        #elif UNITY_IPHONE
			    uri = "https://itunes.apple.com/app/id" + AppConfig.APPLE_STORE_ID;
        #endif
        return uri;
    }

    // Unity will call OnApplicationPause(false) when an app is resumed
    // from the background
    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    Debug.Log("FB.ActivateApp");
                    FB.ActivateApp();
                });
            }
        }
    }

    public void SetHeader()
    {
        GameObject obj = GameObject.Find("CanvasMap");
        if (obj != null)
        {
            MapHeadcontroller headcontroller = obj.GetComponent<MapHeadcontroller>();
            headcontroller.SetHeade();
        }
    }

    string GetHightQualityHead(string uid)
    {
        string s = "https://graph.facebook.com/" + uid + "/picture?type=large&redirect=true&width=600&height=600";
        return s;
    }
}
