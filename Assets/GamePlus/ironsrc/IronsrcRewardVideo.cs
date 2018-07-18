using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.advertise;
using UnityEngine;

namespace Assets.GamePlus.ironsrc
{

    public class IronsrcRewardVideo :AbstractGpVideo
    {

        public Action<ShowResult> loadFailCallback { get; set; }
        void Start()
        {
            AutoLoad = true;
            _typeTizer=VideoTizer.Ironsrc;
            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent; 
        }

        public void ShowRewardedVideoButtonClicked()
        {
            Debug.Log("ShowRewardedVideoButtonClicked");
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("IronSource.Agent.isRewardedVideoAvailable - False");
                if (loadFailCallback != null)
                {
                    loadFailCallback(ShowResult.Failed);
                }
            }
        }

        void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
        {
            Debug.Log("I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
            isLoad = canShowAd;
        }

        void RewardedVideoAdOpenedEvent()
        {
        }

        void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
        {
            Debug.Log("I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
        }

        void RewardedVideoAdClosedEvent()
        {
            if (ResultCallback != null)
            {
                ResultCallback(ShowResult.Finished);
            }
        }

        void RewardedVideoAdStartedEvent()
        {
        }

        void RewardedVideoAdEndedEvent()
        {
        }

        void RewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            Debug.Log("I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
            if (ResultCallback != null)
            {
                ResultCallback(ShowResult.Failed);
            }
        }

        public override void ShowVideoAd()
        {
            ShowRewardedVideoButtonClicked();
        }

        public override void LoadVideoAd()
        {
            ShowVideoAd();
        }
    }
}
