using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GamePlus.utils
{
    public class AssetsUtils : MonoBehaviour
    {
        void Start()
        {
        }

        public void readData(string dirName)
        {
            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, dirName);

        }

        public static string GetStreamingAssetsPath()
        {
            string path;
            #if UNITY_EDITOR
                    path = "Assets/StreamingAssets/";
            #elif UNITY_ANDROID 
                    path = "jar:file://" + Application.dataPath + "!/assets/";  // this is the path to your StreamingAssets in android
            #elif UNITY_IOS
                    path = Application.dataPath + "/Raw/";  // this is the path to your StreamingAssets in iOS
            #endif
            return path;
        }
    }
}
