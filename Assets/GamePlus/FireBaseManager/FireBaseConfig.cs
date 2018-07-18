using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.FireBaseManager
{
    public class FireBaseConfig
    {
        //realtime db
        public const string DatabaseUrl = "https://balls-d0b54.firebaseio.com/";
        public const string CloudStorage = "gs://balls-d0b54.appspot.com";
        public const string DbPath = "Mahjong";
        //dbpath
        public const string MessagePath =DbPath+ "/MessageBox/";
        public const string UserConfigPath = DbPath + "/UserConfig/";
        public const string UserInfoPath = DbPath + "/UserInfo/";
        public const string GlobalConfigPath = DbPath + "/GlobalConfig/";
        public const string ScorePath = DbPath + "/Score/";
        public const string TimePath = DbPath + "/Time/";
        //const
        public const string FIREBASE_LOGIN = "FIREBASE_LOGIN";
        public const string DEFAULT_PWD = "Eey=E_2LPv";
        public const string NEW_USER = "REGISTER_USER";
        public const string USER_REINSTALL = "USER_REINSTALL";
        //storage
        public const string EROR_LOG_PATH = "ErrorLog";
    }
}
