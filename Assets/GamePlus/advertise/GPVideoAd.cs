using System;
using System.Collections.Generic;
using System.Linq;
using Assets.GamePlus.ironsrc;
using Fabric.Internal.ThirdParty.MiniJSON;
using UnityEngine;
using Random = System.Random;

namespace Assets.GamePlus.advertise
{
    public class GPVideoAd : MonoBehaviour
    {
        private const float CONFIG_WAIT = 10f;
        private const float CONFIG_REPEAT_WAIT = 5f;
        private float refresh = 60 * 7;
        private Random mRandom;
        private List<AbstractGpVideo> videoSrc;
        private IEnumerator<AbstractGpVideo> videoEnumerator; 
        //配置key
        public const string AD_CONFIG = "ad_config";
        public string defaultConfig = "{\"Ironsrc\":50,\"Facebook\":50}";
        public static bool AdLoading;
        void Awake()
        {
        }

        void Start()
        {
            IronsrcRewardVideo ironsrcReward = GameObject.Find("GamePlus").GetComponent<IronsrcRewardVideo>();
            FacebookReward facebookReward = GameObject.Find("GamePlus").GetComponent<FacebookReward>();
            mRandom = new Random();
            videoSrc = new List<AbstractGpVideo>();
            videoSrc.Add(ironsrcReward);
            videoSrc.Add(facebookReward);
        }

        //设置回调
        public void SetCallback(Action<ShowResult> result)
        {
            videoSrc.ForEach(x => x.ResultCallback = result);
        }

        public void ShowAd()
        {
            string config = PlayerPrefs.GetString(AD_CONFIG, defaultConfig);
            int num_random = 1;
            string randomKey = VideoTizer.Ironsrc.ToString();
            //解析json配置
            Dictionary<string, object> adConfig = Json.Deserialize(config) as Dictionary<string, object>;
            adConfig.Reverse();
            int sum = adConfig.Sum(valuePair => int.Parse(valuePair.Value.ToString()));
            //产生随机数
            num_random = mRandom.Next(0, sum);
            //根据随机数取值广告源
            for (int i = 0; i < adConfig.Count; i++)
            {
                int num = int.Parse(adConfig.ElementAt(i).Value.ToString());
                num_random =num_random- num;
                if (num_random<0)
                {
                    randomKey = adConfig.ElementAt(i).Key;
                    break;
                }
            }
            Debug.Log(num_random + " " + randomKey + " " + sum);
            VideoTizer tizer = (VideoTizer)Enum.Parse(typeof(VideoTizer), randomKey);
            PlayerAd(tizer);
        }

        private void PlayerAd(VideoTizer tizer)
        {
            AbstractGpVideo showVideo = videoSrc.FirstOrDefault(x => x._typeTizer == tizer);
            if (showVideo != null)
            {
                if (tizer == VideoTizer.Ironsrc && showVideo.isLoad)
                {
                    showVideo.ShowVideoAd();
                }
                else
                {
                    showVideo.LoadVideoAd();
                }
            }
            else
            {
                Debug.Log("没有实例播放广告");
            }
        }
    }
}
