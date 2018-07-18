using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.bean
{
    public class GPFriendInfo : FBFriendInfo
    {
        private string localPicPath;

        public string LocalPicPath
        {
            get { return localPicPath; }
            set { localPicPath = value; }
        }
        private string sparkID;

        public string SparkID
        {
            get { return sparkID; }
            set { sparkID = value; }
        }
    }
}
