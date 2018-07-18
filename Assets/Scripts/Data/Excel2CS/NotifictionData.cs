using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite4Unity3d;

namespace Assets.Scripts.Data.Excel2CS
{
    public enum MsgType
    {
        SYSTEM = 1,
        FRIEND = 2,
        NOTIFY = 3
    }
    public class NotifictionData
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string time { get; set; }
        public string desc { get; set; }
        public int type { get; set; }
        public int status { get; set; }
        public int awardType { get; set; }
        public int awardNum { get; set; }

        public NotifictionData(string image, string title, string time, string desc, int type, int status, int awardType, int awardNum)
        {
            this.image = image;
            this.title = title;
            this.time = time;
            this.desc = desc;
            this.type = type;
            this.status = status;
            this.awardType = awardType;
            this.awardNum = awardNum;
        }
    }
}
