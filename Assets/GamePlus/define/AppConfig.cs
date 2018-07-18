using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.gameplus.define
{
    class AppConfig
    {
        /**
         * play and apple appid
         * */
        public const string APPLE_STORE_ID = "1238653104";
        public const string PLAY_STORE_ID = "com.gameplus.mahjong";

        /**
         * facebook share config
         **/
        public const string shareTitle = "Mahjong Master";
        public static string shareDescription() {
            int score = 0;
            string shareDesc = "Want to be a Mahjong Master? Challenge this matching game Now!";
            return shareDesc;
        }
        public static string shareImageUrl()
        {
            string imageUrl = "https://s20.postimg.org/s5xtla4u5/fb_Mah_Jongshare.jpg";
            return imageUrl;
        }
        /**
         *  facebook 分享链接
         *  使用facebook applink host-api 生成
         *  https://developers.facebook.com/docs/applinks/hosting-api
         **/
        public const string APP_LINK = "https://fb.me/324612724624026";
        /**
         * 排行榜 id
         **/
        //TODO: error, modify by ouyang
        // #if UNITY_ANDROID
        //     public const string BEST_SCORE_BOARD = GPGSIds.leaderboard_score;
        // #elif UNITY_IPHONE
        public const string BEST_SCORE_BOARD = "mj.score";
        // #endif
        /**
         * 成就 id
         **/
            //#if UNITY_ANDROID
            //    public const string ROOKIE = GPGSIds.achievement_rookie;
            //    public const string AMATEUR = GPGSIds.achievement_amateur;
            //    public const string SKILLED = GPGSIds.achievement_skilled;
            //    public const string EXPERT = GPGSIds.achievement_expert;
            //    public const string MASTER = GPGSIds.achievement_master;
            //#elif UNITY_IPHONE
            //    public const string ROOKIE = "grp.dropit.rookie";
            //    public const string AMATEUR = "grp.dropit.amateur";
            //    public const string SKILLED = "grp.dropit.skilled";
            //    public const string EXPERT = "grp.dropit.expert";
            //    public const string MASTER = "grp.dropit.master";
            //#endif
        /**
         *  内购id,ios和android后台中尽量保证内购id的一致性
         **/
        public const string PURCHASE_COIN_200 = "mj.coin.200";
        public const string PURCHASE_COIN_20 = "mj.coin.20";
        public const string PURCHASE_COIN_60= "mj.coin.60";
        public const string PURCHASE_COIN_130= "mj.coin.130";
        public const string PURCHASE_COIN_280= "mj.coin.280";
        public const string PURCHASE_COIN_750= "mj.coin.750";
        public const string PURCHASE_COIN_1600= "mj.coin.1600";
        public const string PURCHASE_BAG_SMALL = "mj.bag.small";
        public const string PURCHASE_BAG_MIDDLE= "mj.bag.middle";
        public const string PURCHASE_BAG_BIG= "mj.bag.big";

        //TODO: error, modify by ouyang
        //ironsource 配置
        // #if UNITY_ANDROID
        //     public const string IRONSRC_APIKEY = "6408d28d";
        // #elif UNITY_IPHONE
        public const string IRONSRC_APIKEY = "64090c15";
        // #endif
        public const string FB_REWARD_SHOP_ID = "319476231804342_357135018038463";
        public const string FB_REWARD_RETRY_ID = "319476231804342_358473164571315";
        //错误日志保存间隔时间
        public const float ERROR_LOG_BACKUP = 60*3;
    }
}
