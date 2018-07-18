using System.Collections.Generic;
using System.Linq;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.FireBaseManager;
using Assets.Scripts.UIController;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.FirebaseController
{
    public enum Fboperate
    {
        //通过fb绑定firebase
        Bind=1,
        //通过fb登录firebase
        Signin=2,
        //正常fb登录
        ConmmonLogin=3
    }
    public class LoginController:MonoBehaviour
    {
        public static Fboperate FbOp;
        private bool _isLogin;
        public static LoginController Instance;
        public  static bool AccountChange =false;
        void Start()
        {
            Instance = this;
        }

        void Update()
        {
            if (!_isLogin && LoginManager.Instance.CurFirebaseUser!=null)
            {
                //登录成功
                GamePlusLog.Instance.LoginLog();
                _isLogin = true;
            }
        }

        /// <summary>
        /// 开始登录
        /// </summary>
        public void StartLogin()
        {
            //firebase初始化失败
            if (LoginManager.Instance.auth==null)
            {
                Init.LoginCompleted = true;
                return;
            }
            //检查登录记录
            if (LoginManager.Instance.GetLoginStatus())
            {
                CheckFbUser();
            }
            else
            {
                //注册
                LoginManager.Instance.RegisterUser(DeviceUtils.GetUuid() + "@gmail.com", FireBaseConfig.DEFAULT_PWD);
            }
        }

        /// <summary>
        /// 检查是否登录过fb
        /// </summary>
        public void CheckFbUser()
        {
            string fb_token = PlayerPrefs.GetString(Constance.FB_ACCESSTOKEN, "");
            if (string.IsNullOrEmpty(fb_token))
            {
                //firebase登录
                LoginManager.Instance.FireBaseSignIn(DeviceUtils.GetUuid() + "@gmail.com", FireBaseConfig.DEFAULT_PWD);
            }
            else
            {
                string expire = PlayerPrefs.GetString(Constance.FB_EXPIRATION, "");
                string curTime = PlayerInfoUtil.GetTimeStamp();
                double expireNum;
                double curTimeNum;
                double.TryParse(expire, out expireNum);
                double.TryParse(curTime, out curTimeNum);
                //检查token是否过期
                if (curTimeNum < expireNum)
                {
                    LoginManager.Instance.FacebookSignin(fb_token);
                }
                else
                {
                    FbOp = Fboperate.Signin;
                    FacebookSup.Instance.CallFBLogin();
                }
            }
        }

        /// <summary>
        /// 绑定faceook
        /// </summary>
        public void BindFacebook()
        {
            Debug.Log("绑定fb");
            FbOp = Fboperate.Bind;
            FacebookSup.Instance.CallFBLogin();
        }

        public void FacebookSup_fbLoginHandler(string flag)
        {
            string token = PlayerPrefs.GetString(Constance.FB_ACCESSTOKEN, "");
            if (string.IsNullOrEmpty(token) || token.Length<=4)
            {
                Debug.Log("FacebookSup_fbLoginHandler => token is null");
                return;
            }
            if (FbOp==Fboperate.Signin)
            {
                LoginManager.Instance.FacebookSignin(token);
            }
            else if (FbOp == Fboperate.Bind)
            {
                AdloadUtils.Instance.Show();
                LoginManager.Instance.LinkWithCredential(token);
            }
            else
            {
                Debug.Log("ConmmonLogin");
            }
        }

        /// <summary>
        /// 由于冲突导致的登录失败
        /// </summary>
        /// <param name="error"></param>
        private void FacebookSigninCallback(bool result, string ex)
        {
            if (result)
            {
                //强制等待db初始化完成
                InvokeRepeating("WaitRdbInit",0.2f,0.5f);
            }
            LoginTimeout(ex);
        }

        void WaitRdbInit()
        {
            if (RemoteDbManager.Instance.Rdb!=null)
            {
                SdkController.Instance.GetFriendUid();
                IEnumerable<FriendData> friendDatas=DynamicDataBaseService.GetInstance().GetFriendData().ToList();
                if (friendDatas.Any())
                {
                    FriendData friendData = friendDatas.First();
                    friendData.gs_id = LoginManager.UserId;
                    DynamicDataBaseService.GetInstance().Connection.Update(friendData);
                    CancelInvoke("WaitRdbInit");
                }
            }
        }

        /// <summary>
        /// 解绑回调
        /// </summary>
        /// <param name="result"></param>
        private void UnlinkFacebookCallback(bool result)
        {
            if (result)
            {
                Debug.Log("解绑定成功");
                SdkController.Instance.UpdateUserInfo("Fbid","");
                string token = PlayerPrefs.GetString(Constance.FB_ACCESSTOKEN, "");
                LoginManager.Instance.LinkWithCredential(token);
            }
            else
            {
                Debug.Log("解绑定失败");
            }
        }

        /// <summary>
        /// 绑定回调
        /// </summary>
        /// <param name="result"></param>
        private void LinkFacebookCallback(bool result, string ex)
        {
            if (result)
            {
                Debug.Log("绑定成功");
                string fbid = PlayerPrefs.GetString(Constance.FACEBOOK_ID, "");
                SdkController.Instance.UpdateUserInfo("Fbid", fbid);
                GamePlusLog.Instance.BindLog();
                WaitRdbInit();
                AdloadUtils.Instance.hide();
            }
            else
            {
                Debug.Log("绑定失败");
                if (ex.Contains(LoginManager.FB_DUPLICATE_LINKED))
                {
                    string fbid = PlayerPrefs.GetString(Constance.FACEBOOK_ID, "");
                    SdkController.Instance.UpdateUserInfo("Fbid", fbid);
                    WaitRdbInit();
                    AdloadUtils.Instance.hide();
                    Debug.Log("重复绑定,更新fb信息");
                }

                if (ex.Contains(LoginManager.FB_ALREADY_LINKED))
                {
                    //当前游戏帐号退出登录，使用手机当前登录的facebook帐号登录
                    Debug.Log("此fb已被绑定,使用当前FB登录");
//                    LoginManager.Instance.auth.SignOut();
                    AccountChange = true;
                    ResetData();
                }

                if (ex.Contains(LoginManager.ACCOUNT_ALREADY_LINKED))
                {
                    //todo 错误的精确检测
                    //解绑当前游戏帐号绑定的facebook帐号，绑定手机当前登录的facebook帐号
                    Debug.Log("此帐号已绑定fb,先解绑，在绑定当前fb");
                    FriendData friendData = DynamicDataBaseService.GetInstance().GetFriendData().First();
                    if (string.IsNullOrEmpty(friendData.fb_id))
                    {
                        Debug.Log("fb id is null");
                        return;
                    }
                    LoginManager.Instance.UnlinkCredential("facebook");
                }
            }
        }

        /// <summary>
        /// 注销登录回调
        /// </summary>
        private void LogOutCallback()
        {
            Debug.Log("注销成功");
            DynamicDataBaseService.GetInstance().ResetDatabases();
            //清表
            string fbToken = PlayerPrefs.GetString(Constance.FB_ACCESSTOKEN, "");
            if (string.IsNullOrEmpty(fbToken))
            {
                Debug.Log("fbToken is null");
            }
            AdloadUtils.Instance.hide();
            ReloadInit();
        }

        void ResetData()
        {
            //清表
            DynamicDataBaseService.GetInstance().ResetDatabases();
            string fbToken = PlayerPrefs.GetString(Constance.FB_ACCESSTOKEN, "");
            if (string.IsNullOrEmpty(fbToken))
            {
                Debug.Log("fbToken is null");
            }
            AdloadUtils.Instance.hide();
            ReloadInit();
        }

        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="suc"></param>
        /// <param name="ex"></param>
        private void FirebaseRegisterCallback(bool suc,string ex)
        {
            if (!suc)
            {
                LoginTimeout(ex);
            }
        }

        /// <summary>
        /// firebase登录回调
        /// </summary>
        /// <param name="suc"></param>
        /// <param name="ex"></param>
        private void FirebaseSigninCallback(bool suc, string ex)
        {
            if (!suc)
            {
                LoginTimeout(ex);
            }
        }

        private static void LoginTimeout(string ex)
        {
            if (ex.Contains(LoginManager.NETWORK_ERROR))
            {
                Debug.Log("firebase服务不可用，立即进入游戏");
                Init.LoginCompleted = true;
            }
        }

        void ReloadInit()
        {
            Init.LoginCompleted = false;
            SceneManager.LoadScene("Init");
            DynamicData.GetInstance().Clear();
        }

        void OnEnable()
        {
            FacebookSup.fbLoginHandler += FacebookSup_fbLoginHandler;
            LoginManager.FacebookSigninCallback += FacebookSigninCallback;
            LoginManager.UnlinkFacebookCallback += UnlinkFacebookCallback;
            LoginManager.LinkFacebookCallback += LinkFacebookCallback;
            LoginManager.LogOutCallback += LogOutCallback;
            LoginManager.FirebaseRegisterCallback += FirebaseRegisterCallback;
            LoginManager.FirebaseSigninCallback += FirebaseSigninCallback;
        }

        void OnDisable()
        {
            FacebookSup.fbLoginHandler -= FacebookSup_fbLoginHandler;
            LoginManager.FacebookSigninCallback -= FacebookSigninCallback;
            LoginManager.UnlinkFacebookCallback -= UnlinkFacebookCallback;
            LoginManager.LinkFacebookCallback -= LinkFacebookCallback;
            LoginManager.LogOutCallback -= LogOutCallback;
            LoginManager.FirebaseRegisterCallback -= FirebaseRegisterCallback;
            LoginManager.FirebaseSigninCallback -= FirebaseSigninCallback;
        }
    }
}
