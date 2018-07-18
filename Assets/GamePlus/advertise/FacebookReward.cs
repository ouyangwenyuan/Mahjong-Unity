using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.gameplus.define;
using Assets.Scripts.UIController;
using AudienceNetwork;
using UnityEngine;

namespace Assets.GamePlus.advertise
{
    public class FacebookReward : AbstractGpVideo
    {
        private RewardedVideoAd rewardedVideoAd;
        public bool debug = false;
        void Start()
        {
            if (debug)
            {
                    AdSettings.AddTestDevice("8444a1cfd9df5265afe21602195cdaa4");
                    AdSettings.AddTestDevice("466012b616a0089be74a91d269f91617");
            }
            _typeTizer = VideoTizer.Facebook;
        }

        void RewardedVideoAdDidFailWithError(string error)
        {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);
            ResultCallback(ShowResult.Failed);
            isLoad = false;
        }

        void RewardedVideoAdDidLoad()
        {
            Debug.Log("RewardedVideo ad loaded.");
            ResultCallback(ShowResult.Loaded);
            isLoad = true;
            ShowVideoAd();
        }

        void RewardedVideoAdWillLogImpression()
        {
            Debug.Log("RewardedVideo ad logged impression.");
        }

        void RewardedVideoAdDidClick()
        {
            Debug.Log("RewardedVideo ad clicked.");
        }

        void rewardedVideoAdComplete()
        {
            ResultCallback(ShowResult.Finished);
            //播放结束加载
            LoadVideoAd();
        }

        public override void ShowVideoAd()
        {
            if (isLoad&&AdloadUtils.AdMaskShow)
            {
                rewardedVideoAd.Show();
                isLoad = false;
            }
            else
            {
                Debug.Log("Ad  loaded but timeout");
            }
        }

        public override void LoadVideoAd()
        {
            if (isLoad)
            {
                ShowVideoAd();
                return;
            }
            Debug.Log("Loading rewardedVideo ad...");
            GPVideoAd.AdLoading = true;
            rewardedVideoAd = new RewardedVideoAd(AppConfig.FB_REWARD_SHOP_ID);
            rewardedVideoAd.Register(gameObject);

            rewardedVideoAd.RewardedVideoAdDidLoad = RewardedVideoAdDidLoad;
            rewardedVideoAd.RewardedVideoAdDidFailWithError = RewardedVideoAdDidFailWithError;
            rewardedVideoAd.RewardedVideoAdWillLogImpression = RewardedVideoAdWillLogImpression;
            rewardedVideoAd.RewardedVideoAdDidClick = RewardedVideoAdDidClick;
            rewardedVideoAd.rewardedVideoAdComplete = rewardedVideoAdComplete;
            rewardedVideoAd.LoadAd();
        }

        void Timeout()
        {
            ResultCallback(ShowResult.Failed);
        }

        void OnDestroy()
        {
            // Dispose of rewardedVideo ad when the scene is destroyed
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.Dispose();
            }
            Debug.Log("RewardedVideoAdTest was destroyed!");
        }
    }
}
