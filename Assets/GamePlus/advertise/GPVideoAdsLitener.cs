using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.advertise
{
    public interface GPVideoAdsLitener
    {
        void OnAdRewarded();
        void OnAdFailedToLoad();
        void OnAdLoaded();
    }
}
