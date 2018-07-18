using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.gameplus.define;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System;
using Assets.GamePlus.Bean;
using Assets.Script.utils;

public class SocialServiceSup : MonoBehaviour {
    //login call back event
    public static event Action<bool> socialLoginHandler;
    /// <summary>
    /// 请求排行榜用户信息回调
    /// </summary>
    public delegate void QueryCallback(LeaderBoardData data);
    public QueryCallback mUserInfoCallback;
    private Hashtable achiveNameMap;
	// Use this for initialization
	void Start () {
        //Invoke("init", 3);
        achiveNameMap = new Hashtable();
        achiveNameMap.Add("AppConfigID","displayName");
        init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void init() {
        initSocialService();
    }

    public QueryCallback UserInfoCallback
    {
        internal get
        {
            return this.mUserInfoCallback;
        }
        set
        {
            this.mUserInfoCallback = value;
        }
    }

    void initSocialService()
    {
        AdsUtils.setInterStutas(false);
		#if UNITY_ANDROID
             //云存储需要单独配置并且请求权限
            //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //.EnableSavedGames()
            //.Build();
            //PlayGamesPlatform.InitializeInstance(config);

		    PlayGamesPlatform.DebugLogEnabled = false;
		    PlayGamesPlatform.Activate();
            Debug.Log("initPlayService");
		#elif UNITY_IPHONE

		#endif
        Social.localUser.Authenticate((bool success) =>
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("status", success);
            //socialLoginHandler(success);
		    #if UNITY_ANDROID
                //getPlayerStatus();
                desc.Add("flavor", "google play game service");
		    #elif UNITY_IPHONE
                desc.Add("flavor", "apple game center");
		    #endif
            AnalysisSup.fabricLog(EventName.LOGIN, desc);
        });
    }

    /// <summary>
    /// 解锁成就
    /// </summary>
     public void unlockAchive(string achId)
    {
        bool isGCAuthenticated = Social.localUser.authenticated;
        Debug.Log("unlockAchivement:" + isGCAuthenticated);
        if (isGCAuthenticated)
        {
            // unlock achievement (achievement ID "Cfjewijawiu_QA")
            Social.ReportProgress(achId, 100.0f, (bool success) =>
            {
                // handle success or failure
                Dictionary<string, object> desc = new Dictionary<string, object>();
                desc.Add("status", success);
                desc.Add("type", achiveNameMap[achId]);
                AnalysisSup.fabricLog(EventName.UNLOCK_ACHIVEMENT, desc);
                Debug.Log("unlockAchivement:" + success);
            });
        }
    }
     /// <summary>
     /// 查看成就
     /// </summary>
     public void showAchivementUI()
     {
         bool isGCAuthenticated = Social.localUser.authenticated;
         Debug.Log("showAchivementUI:" + isGCAuthenticated);
         if (isGCAuthenticated)
         {
             // show achievements UI
             Dictionary<string, object> desc = new Dictionary<string, object>();
             desc.Add("status", true);
             AnalysisSup.fabricLog(EventName.SHOW_CHIVEMENTUI, desc);
             Social.ShowAchievementsUI();
         }
         else 
         {
             Social.localUser.Authenticate((bool success) =>
             {
                 // handle success or failure
                 Debug.Log("Authenticate:" + success);
                 if (success) {
                     Dictionary<string, object> desc = new Dictionary<string, object>();
                     desc.Add("status", success);
                     AnalysisSup.fabricLog(EventName.SHOW_CHIVEMENTUI, desc);
                     Social.ShowAchievementsUI();
                 }
             });
         }
     }
     /// <summary>
     /// 上传成就进度
     /// </summary>
     public void reportProgress(string achId,float progress)
     {
         bool isGCAuthenticated = Social.localUser.authenticated;
         Debug.Log("unlockAchivement:" + isGCAuthenticated);
         if (isGCAuthenticated)
         {
             // unlock achievement (achievement ID "Cfjewijawiu_QA")
             Social.ReportProgress(achId, progress, (bool success) =>
             {
                 // handle success or failure
                 Debug.Log("reportProgress:" + success);
             });
         }
     }

     /// <summary>
     /// 查看排行榜
     /// </summary>
     public void pressLeaderBord(string location)
    {
        bool isGCAuthenticated = Social.localUser.authenticated;
        Debug.Log("Authenticate:" + isGCAuthenticated);
        if (isGCAuthenticated)
        {
            // show leaderboard UI
            #if UNITY_ANDROID
            // show leaderboard UI
                    PlayGamesPlatform.Instance.ShowLeaderboardUI();
            #elif UNITY_IPHONE
				    Social.ShowLeaderboardUI();
            #endif
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("status", true);
            desc.Add("location", location);
            AnalysisSup.fabricLog(EventName.SHOW_LEADERBOARDUI, desc);
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                // handle success or failure
                Debug.Log("Authenticate:" + success);
                if (success)
                {
                    // show leaderboard UI
                    #if UNITY_ANDROID
                    // show leaderboard UI
                            PlayGamesPlatform.Instance.ShowLeaderboardUI();
                    #elif UNITY_IPHONE
				            Social.ShowLeaderboardUI();
                    #endif
                    Dictionary<string, object> desc = new Dictionary<string, object>();
                    desc.Add("status", success);
                    desc.Add("location", location);
                    AnalysisSup.fabricLog(EventName.SHOW_LEADERBOARDUI, desc);
                }
            });
        }
    }

    /// <summary>
    /// 向排行榜上传游戏分数
    /// </summary>
   public void reportScore(int score,string boardId)
    {
        bool isGCAuthenticated = Social.localUser.authenticated;
        Debug.Log("Authenticate:" + isGCAuthenticated);
        if (isGCAuthenticated)
        {

            Social.ReportScore(score, boardId, (bool success2) =>
            {
                Debug.Log("ReportScore:" + success2);
                Dictionary<string, object> desc = new Dictionary<string, object>();
                desc.Add("status", success2);
                AnalysisSup.fabricLog(EventName.REPORT_SCORE, desc);
            });
        }
    }


    /// <summary>
    /// 获取玩家排行榜数据
    /// </summary>
    public void queryUserInfo(string BoardId,QueryCallback callback)
    {

		#if UNITY_IPHONE || UNITY_IOS

		    ILeaderboard lb = Social.CreateLeaderboard();

			lb.id = BoardId;
		    lb.LoadScores(ok => 
		    {
			    if (ok) 
                {
                    processUsersInfo(lb, callback);
			    }
			    else
                {
			        Debug.Log("Error retrieving leaderboardi");
			    }
		    });

        #elif UNITY_ANDROID
            PlayGamesPlatform.Instance.LoadScores(
            BoardId,
            LeaderboardStart.PlayerCentered,
            10,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) =>
            {
                if (data.Valid)
                {
                    processUsersInfo(data, callback);
                }
                else
                {
                    Debug.Log("Error retrieving leaderboardi");
                }

            });
		#endif
   }

        #if UNITY_ANDROID
            internal void processUsersInfo(LeaderboardScoreData data,QueryCallback callback)
            {
                IScore userScore = data.PlayerScore;
                double percent = (data.ApproximateCount - (ulong)userScore.rank) * 1.0 / data.ApproximateCount;
                LeaderBoardData boardData = new LeaderBoardData();
                boardData.userRank = userScore.rank;
                boardData.leaderBaoardCount = data.ApproximateCount;
                boardData.boardScore = userScore.value;
                boardData.userID = userScore.userID;
                if (callback != null) {
                    callback(boardData);
                }
            }
        #elif UNITY_IPHONE
            internal void processUsersInfo(ILeaderboard lb,QueryCallback callback)
            {
                IScore userScore = lb.localUserScore;
                double percent = (lb.scores.Length - userScore.rank) * 1.0 / lb.scores.Length;
                string percentTxt = "you beat " + Math.Round(percent, 2) * 100 + "% players woreldwide";
                LeaderBoardData boardData = new LeaderBoardData();
                boardData.userRank = userScore.rank;
                boardData.leaderBaoardCount = (ulong)lb.range.count;
                boardData.boardScore = userScore.value;
                boardData.userID = userScore.userID;
                if (callback != null) {
                    callback(boardData);
                }
            }
        #endif

            /// <summary>
            /// 获取玩家对游戏的参与状态
            /// https://developers.google.com/games/services/android/stats
            /// </summary>
        //public void getPlayerStatus() {
        //    ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
        //    {
        //        // -1 means cached stats, 0 is succeess
        //        // see  CommonStatusCodes for all values.
        //        if (rc <= 0 && stats.HasDaysSinceLastPlayed())
        //        {
        //            Debug.Log("It has been " + stats.DaysSinceLastPlayed + " days");
        //        }
        //    });
        //}
}
