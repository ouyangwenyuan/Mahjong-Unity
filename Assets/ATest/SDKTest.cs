using Assets.GamePlus.listner;
using Assets.Script.gameplus.define;
using Assets.Script.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabric.Internal.ThirdParty.MiniJSON;
using Assets.GamePlus.advertise;
using Assets.GamePlus.bean;
using Assets.GamePlus.ironsrc;
using Random = System.Random;

public class SDKTest : MonoBehaviour, PurchaseListner, GPVideoAdsLitener
{
    private FacebookSup fbSup;
    private SocialServiceSup socialSup;
    private RateSup rateSup;
    private InAppPurchaseSup inappSup;
    private IronsrcRewardVideo ironsrcReward;
    private Hashtable coinMaps;
    private const string REF_REATE = "refRate";
    bool flag = true;
    private int i = 1;
	void Start () {
//        inappSup = GameObject.Find("GamePlus").GetComponent<InAppPurchaseSup>();
//        inappSup.SetPurchaseListner(this);
        fbSup = GameObject.Find("GamePlus").GetComponent<FacebookSup>();
//        socialSup = GameObject.Find("GamePlus").GetComponent<SocialServiceSup>();
//        ironsrcReward = GameObject.Find("GamePlus").GetComponent<IronsrcRewardVideo>();
//	    ironsrcReward.resultCallback = AdCallbackhanler;

        coinMaps = new Hashtable();
        coinMaps.Add(AppConfig.PURCHASE_COIN_200, 200);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void onAdClick()
    {
        ironsrcReward.ShowRewardedVideoButtonClicked();
    }


    void AdCallbackhanler(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Ad Finished. Rewarding player...");
                break;
            case ShowResult.Skipped:
                Debug.Log("Ad Skipped");
                break;
            case ShowResult.Failed:
                Debug.Log("Ad failed");
                break;
        }
    }

    public void onRankClick(string location)
    {
        AdsUtils.setInterStutas(false);
        socialSup.pressLeaderBord(location);
    }

    public void reportScore() {
        socialSup.reportScore(10, AppConfig.BEST_SCORE_BOARD);
    }

    public void showAchivementUI()
    {
        socialSup.showAchivementUI();
    }

    public void unlockAchivement()
    {
        socialSup.unlockAchive("AppConfigID");
        Debug.Log("path " + Application.persistentDataPath);
    }

    public void onFBShareClick(string location)
    {
        AdsUtils.setInterStutas(false);
        fbSup.share();
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("flavor", "facebook");
        desc.Add("status", "clicked");
        desc.Add("location", location);
        AnalysisSup.fabricLog(EventName.SHARE, desc);
    }

    public void onFBLogin()
    {
        fbSup.CallFBLogin();
    }

    public void onInvite()
    {
        fbSup.invite();
    }

    public void onRateClick()
    {
        AdsUtils.setInterStutas(false);
        rateSup.rate();
    }

    public void onPurchaseCLick(string arguments)
    {
        string[] args = arguments.Split(' ');
        Debug.Log(args[0]);
        Debug.Log(args[1]);
        inappSup.PurchaseProduct(args[0], args[1]);
    }

    void addCoin(int coin)
    {
        Debug.Log("purchase coin " + coin);
    }

    public void PurchaseResult(bool success, string productId)
    {
        Debug.Log("PurchaseResult ======" + success + " " + productId);
        if (success)
        {
            if (coinMaps.ContainsKey(productId)) {
                int coin = (int)coinMaps[productId];
                Debug.Log("purchase:" + coin);
                PlayerData data = DataBaseService.GetInstance().Connection.Table<PlayerData>().Where(x => x.id == 1).ElementAt(0);
                data.gold += coin;
                DataBaseService.GetInstance().Connection.Update(data);
            }
            else
            {
                Debug.Log("product ======"+ productId + " initial fail");
            }
        }
    }

    public void OnAdRewarded()
    {
        Debug.Log("OnAdRewarded");
    }

    public void OnAdFailedToLoad()
    {
        Debug.Log("OnAdFailedToLoad");
    }

    public void OnAdLoaded()
    {
        Debug.Log("OnAdLoaded");
    }

    public void getCurPic()
    {
//        fbSup.getUserCover();
    }

    public void getsinglePic()
    {
        fbSup.getFriendCover("108917223017760");
    }

    public void getFriendsPic()
    {
        fbSup.getFriendsCover();
    }

    void FacebookSup_singleCoverCallback(string obj)
    {
        Debug.Log("FacebookSup_singleCoverCallback " + obj);
    }

    void FacebookSup_CoversCallback(List<FBFriendInfo> obj)
    {
        foreach (FBFriendInfo info in obj)
        {
            info.ToString();
        }
    }

    void OnEnable()
    {
        FacebookSup.singleCoverCallback += FacebookSup_singleCoverCallback;
        FacebookSup.CoversCallback += FacebookSup_CoversCallback;
    }

    void OnDisable()
    {
        FacebookSup.singleCoverCallback -= FacebookSup_singleCoverCallback;
        FacebookSup.CoversCallback -= FacebookSup_CoversCallback;
    }

    public void addObj()
    {
        PlayerData data =new PlayerData();
        data.level = i++;
        data.gold = new Random().Next(0, 100);
        data.life = new Random().Next(0, 100);
        DataBaseService.GetInstance().Connection.Insert(data);
    }

    public void delObj()
    {
        PlayerData data = DataBaseService.GetInstance().Connection.Table<PlayerData>().Where(x => x.id == 3).ElementAt(0);
        DataBaseService.GetInstance().Connection.Delete(data);
    }

    public void queryObj()
    {
        PlayerData data = DataBaseService.GetInstance().Connection.Table<PlayerData>().Where(x => x.id == 1).ElementAt(0);
        Debug.Log("queryObj:" + JsonUtility.ToJson(data));
    }

    public void updateObj()
    {
        PlayerData data = DataBaseService.GetInstance().Connection.Table<PlayerData>().Where(x => x.id == 1).ElementAt(0);
        data.gold = 1000000;
        DataBaseService.GetInstance().Connection.Update(data);
    }

    public void createCo()
    {
        DynamicDataBaseService.GetInstance().Connection.CreateIndex("Highscore", "aaaaa", true);
    }

    public void jsonTest()
    {
        string datastr = "{\"id\":1,\"gold\":200,\"life\":5,\"level\":0,\"maxLife\":5,\"uploadId\":\"aaa\",\"uploadTime\":\"bbb\",\"deviceId\":\"ccc\"}";
        //在创建一个对象对应于此对象
        PlayerData data = JsonUtility.FromJson<PlayerData>(datastr);
        Debug.Log("reverse" + data.gold);
//        DataBaseService.GetInstance().Connection.Update(data);
//        string data = "{\"age\":4,\"name\":\"jansen\"}";
//        Person person = JsonUtility.FromJson<Person>(data);
//        Debug.Log(person.name);
    }

    public class Person
    {
        public int age;
        public string name;

    }
}
