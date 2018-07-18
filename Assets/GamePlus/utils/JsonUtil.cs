using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fabric.Internal.ThirdParty.MiniJSON;
using UnityEngine;

namespace Assets.GamePlus.utils
{
    public class JsonUtil
    {
        public static Dictionary<string, object> ReadLocalJson(string name)
        {
            string userinfo = File.ReadAllText(Application.dataPath + "/JsonData/" + name);
            return (Dictionary<string, object>)Json.Deserialize(userinfo);
        }
    }
}
