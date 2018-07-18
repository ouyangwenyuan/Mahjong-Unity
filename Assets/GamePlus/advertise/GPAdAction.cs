using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.advertise
{
    public interface GPAdAction
    {
        void LoadAd(string unitID);
        void ShowAd();
        void Destroy();
    }
}
