using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.gameplus.define
{
    class Constance
    {
        public const string PLAER_DIE_NUM = "playerDieNum";
        public const string PLAER_ENTER_GAME_NUM = "playerEnterGameNum";
        public const string SOUND = "sound";
        public const string MUSIC = "music";
        public const string BEST_SCORE = "BestScore";
        public const string RESUME_GAME = "resume_game";
        //the time of application pause offset to show ads
        public const int RESUME_TIME_OFFSET = 20;
        //should show banner ads、interstitial ads
        public const string SHOW_ASDS = "show_ads";
        //should show interstitial ads when application resume
        public const string INTERS_BAN_STATUS = "video_ad_show";
        public const string FIRST_ENTERGAME = "first_entergame";
        public const string FB_EXPIRATION= "expiration_timestamp";
        public const string FB_ACCESSTOKEN= "fb_accesstoken";
        //好友头像存储目录
        public const string COVER_PATH = "Covers";
        public const string FACEBOOK_ID = "facebook_id";
        //本地文件名
        public const string ERROR_LOG = "error.log";
        //排行榜显示数目
        public static int BOARD_NUM = 6;
        public static int BOARD_TOP_NUM = 3;
    }
}
