﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.GamePlus.utils
{
    public class DeviceUtils
    {
        public static string GetUuid()
        {
#if (UNITY_ANDROID||UNITY_EDITOR)
            return SystemInfo.deviceUniqueIdentifier;
#endif
#if (UNITY_IPHONE || UNITY_STANDALONE_OSX)
			string uuid = KeyChain.BindGetKeyChainUser();
			if(string.IsNullOrEmpty(uuid)){
				string CurUUID=SystemInfo.deviceUniqueIdentifier;
				KeyChain.BindSetKeyChainUser("0",CurUUID);
				return CurUUID;
			}
			return uuid;
#endif
        }
    }
}
