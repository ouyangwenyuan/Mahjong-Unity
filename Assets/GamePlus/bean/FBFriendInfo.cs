using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.bean
{
    public class FBFriendInfo
    {
        public string fbid;

        public string Name { get; set; }

        public string PicUrl { get; set; }

        public override string ToString()
        {
            return "fbid:" + fbid + " name:" + Name + " Pic_url:" + PicUrl;
        }
    }
}
