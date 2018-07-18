using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.manager.bean
{
    [Serializable]
    public class BoardData
    {
        public string fbid;
        public long socre;
        public long rank;
        public string name;
        public string gsid;
        public BoardData()
        {
        }

        public BoardData(string fbid, long socre, long rank, string name, string gsid)
        {
            this.fbid = fbid;
            this.socre = socre;
            this.rank = rank;
            this.name = name;
            this.gsid = gsid;
        }

        public string Gsid
        {
            get { return gsid; }
            set { gsid = value; }
        }

        public string Fbid
        {
            get { return fbid; }
            set { fbid = value; }
        }

        public long Socre
        {
            get { return socre; }
            set { socre = value; }
        }

        public long Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
