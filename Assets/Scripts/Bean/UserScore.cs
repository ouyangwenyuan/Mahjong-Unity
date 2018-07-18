using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bean
{
    [Serializable]
    public class UserScore:BeanDic
    {
        public string Uid;
        public int Level;
        public long Score;
        public string LevelUid;
        public override Dictionary<string, object> ToDictionary()
        {
            LevelUid = Uid + "_" + Level+"_scr";
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["Uid"] = Uid;
            result["Level"] = Level;
            result["Score"] = Score;
            return result;
        }
    }
}
