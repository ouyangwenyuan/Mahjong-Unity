using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Assets.GamePlus.utils
{
    public class NativeUtils
    {
        #if (UNITY_IPHONE)

        [DllImport("__Internal")]
        private static extern void CopyTextIOS(string text);

        #endif

        public static void CopyText(string text)
        {
            #if (UNITY_ANDROID)
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call("CopyText", text);
                }
            }
            #endif
            #if (UNITY_IPHONE)
                CopyTextIOS(text);
            #endif
        }

        public static void Toast(string text,int time)
        {
            #if (UNITY_ANDROID)
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call("MakeToast", text, time);
                }
            }
            #endif
            #if (UNITY_IPHONE)
                //CopyTextIOS(text);
            #endif   
        }
    }
}
