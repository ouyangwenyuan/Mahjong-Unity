using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.listner
{
    public abstract class AdAction
    {
        private AdLitener mLitener;

        public AdLitener MLitener
        {
            get { return mLitener; }
            set { mLitener = value; }
        }
        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public abstract void loadAd(string loc);
        public abstract void showAd(AdLitener mLitener);
    }
}
