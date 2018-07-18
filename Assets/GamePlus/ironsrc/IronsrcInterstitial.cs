using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.utils;
using UnityEngine;

namespace Assets.GamePlus.ironsrc
{
    public class IronsrcInterstitial : MonoBehaviour
    {
        // Use this for initialization
        private static double start_time = 0;
        void Start()
        {
            Debug.Log("ShowInterstitialScript Start called");

            // Add Interstitial Events
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailEvent;
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
            // Add Rewarded Interstitial Events
            IronSourceEvents.onInterstitialAdRewardedEvent += InterstitialAdRewardedEvent;
            start_time = TimeUtils.getUnixTime(DateTime.Now.ToUniversalTime().Ticks);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void LoadInterstitialButtonClicked()
        {
            Debug.Log("LoadInterstitialButtonClicked");
            if (!IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.loadInterstitial();
            }
        }

        public static void ShowInterstitialButtonClicked()
        {
            Debug.Log("ShowInterstitialButtonClicked");
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial();
            }
            else
            {
                LoadInterstitialButtonClicked();
                Debug.Log("IronSource.Agent.isInterstitialReady - False");
            }
        }

        void InterstitialAdReadyEvent()
        {
            Debug.Log("I got InterstitialAdReadyEvent");
        }

        void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
            Debug.Log("I got InterstitialAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
        }

        void InterstitialAdShowSucceededEvent()
        {
            Debug.Log("I got InterstitialAdShowSucceededEvent");
        }

        void InterstitialAdShowFailEvent(IronSourceError error)
        {
            Debug.Log("I got InterstitialAdShowFailEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        void InterstitialAdClickedEvent()
        {
            Debug.Log("I got InterstitialAdClickedEvent");
        }

        void InterstitialAdOpenedEvent()
        {
            Debug.Log("I got InterstitialAdOpenedEvent");
        }

        void InterstitialAdClosedEvent()
        {
            Debug.Log("I got InterstitialAdClosedEvent");
            IronSource.Agent.loadInterstitial();
        }

        void InterstitialAdRewardedEvent()
        {
            Debug.Log("I got InterstitialAdRewardedEvent");
        }
    }
}
