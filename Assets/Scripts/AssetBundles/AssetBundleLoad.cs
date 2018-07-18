using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleLoad{

	private static AssetBundleManifest manifest = null;

    private static Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
	
	public static AssetBundle LoadAB(string abPath)
    {
        if (abDic.ContainsKey(abPath) == true)
            return abDic[abPath];
        if (manifest == null)
        {
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(AssetBundleRuntimeConfig.ASSETBUNDLE_PATH + "Win64/" + /*AssetBundleRuntimeConfig.ASSETBUNDLE_FILENAM*/"Win64");
            manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
        }
        if (manifest != null)
        {
            // 2.获取依赖文件列表  
            string[] cubedepends = manifest.GetAllDependencies(abPath);

            for (int index = 0; index < cubedepends.Length; index++)
            {
                //Debug.Log(cubedepends[index]);
                // 3.加载所有的依赖资源
                LoadAB(cubedepends[index]);
            }

            // 4.加载资源
            abDic[abPath] = AssetBundle.LoadFromFile(AssetBundleRuntimeConfig.ASSETBUNDLE_PATH + "Win64/" + abPath);

            return abDic[abPath];
        }
        return null;
    }

    public static Object LoadGameObject(string abName)
    {
        string abPath = abName + AssetBundleRuntimeConfig.SUFFIX;
        int index = abName.LastIndexOf('/');
        //if (index == -1) index = abName.Length;
        string realName = abName.Substring(index + 1, abName.Length - index - 1);

        LoadAB(abPath);

        if (abDic.ContainsKey(abPath) && abDic[abPath] != null)
        {
            return abDic[abPath].LoadAsset(realName);
        }
        return null;
    }

}