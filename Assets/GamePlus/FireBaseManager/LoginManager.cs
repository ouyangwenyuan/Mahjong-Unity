using System;
using System.Collections.Generic;
using Assets.GamePlus.utils;
using Assets.Scripts.FireBaseManager;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Assets.GamePlus.FireBaseManager
{
    public enum UserType
    {
        UserNew=1,//新用户
        UserReinstall=2,
        UserOld=3,//老用户
        Null=4,
        UserChangeDevice=5
    }
    public enum LinkError
    {
        FacebookAreadyLinked=1,
        AccountAreadyLinked=2
    }

    //todo facbook绑定冲突
    //todo firebase的token刷新
    public class LoginManager:MonoBehaviour
    {
        public static LoginManager Instance;
        public FirebaseAuth auth;
        public static UserType UserType;
        public FirebaseUser CurFirebaseUser;
        public static string UserId="";
        protected Dictionary<string, FirebaseUser> userByAuth =
        new Dictionary<string, FirebaseUser>();
        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        private AppOptions otherAuthOptions;
        //error
        public const string EMAIL_AREADY_EXIST = "The email address is already in use by another account";
        public const string FB_ALREADY_LINKED = "This credential is already associated with a different user account";
        public const string ACCOUNT_ALREADY_LINKED = "unkown";
        public const string FB_DUPLICATE_LINKED = "User has already been linked to the given provider";
        public const string USER_BEEN_DELETED = "The user may have been deleted";
        public const string NETWORK_ERROR = "etwork error";
        public const string BAD_FORMATED = "badly";
        //fb登录失败回调
        public static event Action<bool, string> FacebookSigninCallback;
        //解绑fb回调
        public static event Action<bool> UnlinkFacebookCallback; 
        //绑定fb回调
        public static event Action<bool, string> LinkFacebookCallback; 
        //注销登录回调
        public static event Action LogOutCallback;
        //注册回调
        public static event Action<bool, string> FirebaseRegisterCallback;
        //firebase登录失败回调
        public static event Action<bool, string> FirebaseSigninCallback;
        //通用登录回调
        public static event Action LoginCompleted;
        private bool _setCallBack;
        static LoginManager()
        {
            UserType = UserType.Null;
        }

        public 
        void Start()
        {
            UserId = DeviceUtils.GetUuid();
            Instance = this;
            dependencyStatus = FirebaseApp.CheckDependencies();
            if (dependencyStatus != DependencyStatus.Available)
            {
                FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
                {
                    dependencyStatus = FirebaseApp.CheckDependencies();
                    if (dependencyStatus == DependencyStatus.Available)
                    {
                        InitializeFirebase();
                    }
                    else
                    {
                        Debug.LogError(
                            "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            }
            else
            {
                InitializeFirebase();
            }
        }

        /// <summary>
        /// 设置登录状态
        /// </summary>
        /// <param name="status">0未登录过，1登录过</param>
        public void SetLoginStatus(bool status)
        {
            PlayerPrefs.SetInt(FireBaseConfig.FIREBASE_LOGIN, status?1:0);
        }

        /// <summary>
        /// 获取登录状态
        /// </summary>
        /// <returns></returns>
        public bool GetLoginStatus()
        {
            return PlayerPrefs.GetInt(FireBaseConfig.FIREBASE_LOGIN, 0)==1;
        }

        void Update()
        {

        }



        /// <summary>
        /// 新用户注册
        /// </summary>
        /// <param name="email">伪造和deviceid相关的email</param>
        /// <param name="password">123456</param>
        public void RegisterUser(string email, string password)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    FirebaseRegisterCallback(false,"");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    string ex = string.Format("{0}", task.Exception);
                    if (ex.Contains(EMAIL_AREADY_EXIST))
                    {
                        Debug.Log("用户已存在");
                        UserType=UserType.UserReinstall;
                        FireBaseSignIn(email, password);
                    }
                    FirebaseRegisterCallback(false, ex);
                    return;
                }
                FirebaseRegisterCallback(true,"");
                UserType=UserType.UserNew;
                SetLoginStatus(true);
                // Firebase user has been created.
                FirebaseUser user = task.Result;
                UserId = user.UserId;
                CurFirebaseUser = user;
                LoginCompleted();
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    user.DisplayName, user.UserId);
            });
        }

        /// <summary>
        /// 游客登录
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void FireBaseSignIn(string email, string password)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    FirebaseSigninCallback(false, "");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    string ex = string.Format("{0}", task.Exception);
                    if (ex.Contains(USER_BEEN_DELETED))
                    {
                        Debug.Log("用户已被删除");
                        RegisterUser(email, password);
                    }
                    FirebaseSigninCallback(false, ex);
                    return;
                }
                if (UserType==UserType.Null)
                {
                    UserType = UserType.UserOld;
                }
                FirebaseSigninCallback(false, "");
                SetLoginStatus(true);
                FirebaseUser user = task.Result;
                CurFirebaseUser = user;
                UserId = user.UserId;
                LoginCompleted();
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    user.DisplayName, user.UserId);
            });
        }

        /// <summary>
        /// 如果绑定过fb,可以通过fb直接登录
        /// </summary>
        /// <param name="accessToken"></param>
        public void FacebookSignin(string accessToken)
        {
            Credential credential = FacebookAuthProvider.GetCredential(accessToken);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    FacebookSigninCallback(false, "");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    string ex = string.Format("{0}", task.Exception);
                    FacebookSigninCallback(false, ex);
                    return;
                }
                
                FacebookSigninCallback(true, "");
                UserType = UserType.UserOld;
                SetLoginStatus(true);
                FirebaseUser user = task.Result;
                CurFirebaseUser = user;
                UserId = user.UserId;
                LoginCompleted();
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    user.DisplayName, user.UserId);
            });
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public void Signout()
        {
            SetLoginStatus(false);
            auth.SignOut();
        }

        //todo 需要解决facebook账户绑定冲突
        /// <summary>
        /// 绑定facebook
        /// </summary>
        /// <param name="accessToken"></param>
        public void LinkWithCredential(string accessToken)
        {
            Credential credential = FacebookAuthProvider.GetCredential(accessToken);
            auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("LinkWithCredentialAsync was canceled.");
                    LinkFacebookCallback(false,"");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
                    string ex = string.Format("{0}", task.Exception);
                    LinkFacebookCallback(false,ex);
                    return;
                }
                LinkFacebookCallback(true,"");
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }

        /// <summary>
        /// 解绑facebook
        /// </summary>
        /// <param name="providerIdString"></param>
        public void UnlinkCredential(string providerIdString)
        {
            // Unlink the sign-in provider from the currently active user.
            // providerIdString is a string identifying a provider,
            // retrieved via FirebaseAuth.FetchProvidersForEmail().
            auth.CurrentUser.UnlinkAsync(providerIdString).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UnlinkAsync was canceled.");
                    UnlinkFacebookCallback(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UnlinkAsync encountered an error: " + task.Exception);
                    UnlinkFacebookCallback(false);
                    return;
                }
                UnlinkFacebookCallback(true);
                // The user has been unlinked from the provider.
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("Credentials successfully unlinked from user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }

        void InitializeFirebase()
        {
            DebugLog("Setting up Firebase Auth");
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            auth.IdTokenChanged += IdTokenChanged;
            AuthStateChanged(this, null);
        }

        // Track state changes of the auth object.
        void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            FirebaseAuth senderAuth = sender as FirebaseAuth;
            FirebaseUser user = null;
            if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
            if (senderAuth == auth && senderAuth.CurrentUser != user)
            {
                bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
                if (!signedIn && user != null)
                {
                    LogOutCallback();
                    DebugLog("Signed out " + user.UserId);
                }
                user = senderAuth.CurrentUser;
                userByAuth[senderAuth.App.Name] = user;
                if (signedIn)
                {
                    DebugLog("Signed in " + user.UserId);
                    DisplayUserInfo(user, 1);
                }
            }
        }

      // Display user information.
      public void DisplayUserInfo(IUserInfo userInfo, int indentLevel) {
            string indent = new String(' ', indentLevel * 2);
            var userProperties = new Dictionary<string, string> {
              {"Display Name", userInfo.DisplayName},
              {"Email", userInfo.Email},
              {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
              {"Provider ID", userInfo.ProviderId},
              {"User ID", userInfo.UserId}
            };
            foreach (var property in userProperties) {
              if (!String.IsNullOrEmpty(property.Value)) {
                DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
              }
            }
      }

        // Track ID token changes.
        void IdTokenChanged(object sender, EventArgs eventArgs)
        {
            FirebaseAuth senderAuth = sender as FirebaseAuth;
            if (senderAuth == auth && senderAuth.CurrentUser != null)
            {
                senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
                  task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
            }
        }


        void OnDestroy()
        {
            auth.StateChanged -= AuthStateChanged;
            auth.IdTokenChanged -= IdTokenChanged;
            auth = null;
        }

        void DebugLog(string arg)
        {
            Debug.Log(arg);
        }
    }
}
