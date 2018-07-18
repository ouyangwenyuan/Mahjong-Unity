using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.SceneManagement;

public class RewardedVideoAdTest : MonoBehaviour
{

    private RewardedVideoAd rewardedVideoAd;
    private bool isLoaded;

    // UI elements in scene
    public Text statusLabel;

    // Load button
    public void LoadRewardedVideo ()
    {
        AdSettings.AddTestDevice("8444a1cfd9df5265afe21602195cdaa4");
        AdSettings.AddTestDevice("466012b616a0089be74a91d269f91617");
        this.statusLabel.text = "Loading rewardedVideo ad...";

        // Create the rewarded video unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        RewardedVideoAd rewardedVideoAd = new RewardedVideoAd("319476231804342_358473164571315");
        this.rewardedVideoAd = rewardedVideoAd;
        this.rewardedVideoAd.Register (this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate() {
            Debug.Log ("RewardedVideo ad loaded.");
            this.isLoaded = true;
            this.statusLabel.text = "Ad loaded. Click show to present!";
        });
        rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate(string error) {
            Debug.Log ("RewardedVideo ad failed to load with error: " + error);
            this.statusLabel.text = "RewardedVideo ad failed to load. Check console for details.";
        });
        rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate() {
            Debug.Log ("RewardedVideo ad logged impression.");
        });
        rewardedVideoAd.RewardedVideoAdDidClick = (delegate() {
            Debug.Log ("RewardedVideo ad clicked.");
        });

        // Initiate the request to load the ad.
        this.rewardedVideoAd.LoadAd ();
    }

    // Show button
    public void ShowRewardedVideo ()
    {
        if (this.isLoaded) {
            this.rewardedVideoAd.Show ();
            this.isLoaded = false;
            this.statusLabel.text = "";
        } else {
            this.statusLabel.text = "Ad not loaded. Click load to request an ad.";
        }
    }

    void OnDestroy ()
    {
        // Dispose of rewardedVideo ad when the scene is destroyed
        if (this.rewardedVideoAd != null) {
            this.rewardedVideoAd.Dispose ();
        }
        Debug.Log ("RewardedVideoAdTest was destroyed!");
    }

    // Next button
    public void NextScene ()
    {
        SceneManager.LoadScene ("InterstitialAdScene");
    }
}
