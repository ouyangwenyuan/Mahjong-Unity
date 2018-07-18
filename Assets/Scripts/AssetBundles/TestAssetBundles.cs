using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TestAssetBundles : MonoBehaviour
{
    public bool show_prefab_or_scene = true;

    private string url;
    private string assetname = "TanksExample";

	// Use this for initialization
	void Start () {
        if (show_prefab_or_scene)
        {
            Instantiate(AssetBundleLoad.LoadGameObject("sampleassets/mycube"), Vector3.zero, Quaternion.identity);
        }else{
            //AssetBundleLoad.LoadGameObject("sampleassets/tanks/scenes/tanksexample");

            url = "file://" + Application.streamingAssetsPath + "/AssetBundles/Win64" + "/sampleassets/tanks/scenes/tanksexample.unity3d";
            StartCoroutine(Download());
        }
	}

    IEnumerator Download()
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("加载失败");
        }
        else
        {
            AssetBundle bundle = www.assetBundle;

            SceneManager.LoadScene(assetname);
            
            print("跳转场景");

            // AssetBundle.Unload(false)，释放AssetBundle文件内存镜像，不销毁Load创建的Assets对象
            // AssetBundle.Unload(true)，释放AssetBundle文件内存镜像同时销毁所有已经Load的Assets内存镜像
            bundle.Unload(false);
        }

        // 中断正在加载过程中的WWW
        www.Dispose();
    }
}