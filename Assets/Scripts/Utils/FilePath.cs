using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilePath {

        //得到保存路径
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

    public static string GetPersistentDataPath() {
        return Application.persistentDataPath + "/";
    } 
}
