using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GamePlus.advertise
{
    // 摘要: 
    //     ShowResult is passed to ShowOptions.resultCallback after the advertisement
    //     has completed.
    public enum ShowResult
    {
        // 摘要: 
        //     Indicates that the advertisement failed to complete.
        Failed = 0,
        //
        // 摘要: 
        //     Indicates that the advertisement was skipped.
        Skipped = 1,
        //
        // 摘要: 
        //     Indicates that the advertisement completed successfully.
        Finished = 2,

        Loaded=3,

        Timeout=4
    }
    public enum VideoTizer
    {
        Ironsrc = 1,
        Example = 2,
        Facebook=3
    }
    public abstract class AbstractGpVideo : MonoBehaviour
    {
        public bool isLoad = false;
        public bool AutoLoad = false;
        public VideoTizer _typeTizer;
        public Action<ShowResult> ResultCallback { get; set; }

        public abstract void ShowVideoAd();

        public abstract void LoadVideoAd();
    }
}
