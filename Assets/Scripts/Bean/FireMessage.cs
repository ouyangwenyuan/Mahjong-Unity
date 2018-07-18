using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bean
{
    public enum MsgType
    {
        AdminPrivate=1,
        UserPrivate=2,
        AdminPublic=3
    }
    [Serializable]
    public class FireMessage:BeanDic
    {
        public string Content;
        public string Time;
        public string From;
        public string Type;
        public override Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["Content"] = Content;
            result["Time"] = Time;
            result["From"] = From;
            result["Type"] = Type;
            return result;
        }
    }
}
