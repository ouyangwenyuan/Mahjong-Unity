using UnityEngine;
using UnityEditor;
using System.IO;
using System;
public class AssetBundleEditor : Editor {
    [MenuItem("ABTool/BuildAssetBundlesWin64")]
    public static void BuildAllAssetBundleWin64() {
        BuildAllAssetBundles("Win64" , BuildTarget.StandaloneWindows64);

        string src_path = AssetBundleConfig.PROJECT_PATH + AssetBundleConfig.ASSETBUNDLE_FILENAM + "/" + "Win64";
        string dest_path = Application.streamingAssetsPath + "/" + AssetBundleConfig.ASSETBUNDLE_FILENAM + "/" + "Win64";

        DelectDir(dest_path , true);

        CopyDir(src_path, dest_path);

        AssetDatabase.Refresh();
    }

    [MenuItem("ABTool/BuildAssetBundlesAndroid")]
    public static void BuildAllAssetBundleAndroid()
    {
        BuildAllAssetBundles("Android" , BuildTarget.Android);

        AssetDatabase.Refresh();
    }

    [MenuItem("ABTool/BuildAssetBundlesIOS")]
    public static void BuildAllAssetBundleIOS()
    {
        BuildAllAssetBundles("IOS" , BuildTarget.iOS);

        AssetDatabase.Refresh();
    }

	public static void BuildAllAssetBundles (string outpath,BuildTarget target)
	{
        string path = AssetBundleConfig.PROJECT_PATH + AssetBundleConfig.ASSETBUNDLE_FILENAM + "/" + outpath;

        if (Directory.Exists(path))
        {
            DelectDir(path);
        }
        else {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle
                                          //| BuildAssetBundleOptions.CollectDependencies
                                          | BuildAssetBundleOptions.DeterministicAssetBundle,
                                          target);
	}

    public static void DelectDir(string src_path , bool del_all = false)
    {
        try
        {
            if (!Directory.Exists(src_path))
            {
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(src_path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();

            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(i.FullName);
                }
            }

            if(del_all){
                dir.Delete(true);
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            //throw;
        }
    }

    public static void CopyDir(string source_path, string destination_path)
    {
        try
        {
            if (!Directory.Exists(destination_path))
            {
                Directory.CreateDirectory(destination_path);
                File.SetAttributes(destination_path, File.GetAttributes(source_path));
            }

            if (destination_path[destination_path.Length - 1] != Path.DirectorySeparatorChar)
                destination_path = destination_path + Path.DirectorySeparatorChar;

            string[] files = Directory.GetFiles(source_path);
            foreach (string file in files)
            {
                if (File.Exists(destination_path + Path.GetFileName(file)))
                    continue;

                File.Copy(file, destination_path + Path.GetFileName(file), true);
                File.SetAttributes(destination_path + Path.GetFileName(file), FileAttributes.Normal);
                
                //total++;
            }

            string[] dirs = Directory.GetDirectories(source_path);
            foreach (string dir in dirs)
            {
                CopyDir(dir, destination_path + Path.GetFileName(dir));
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            //throw;
        }
    }

    [MenuItem("ABTool/SetAssetBundleName")]
    static void SetResourcesAssetBundleName()
    {
        UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets | SelectionMode.ExcludePrefab );

        string[] Filtersuffix = new string[] { ".prefab", ".unity" };//{ ".prefab",".mat",".dds" };

        if (!(SelectedAsset.Length == 1)) return;

        string fullPath = AssetBundleConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);

        if (Directory.Exists(fullPath))
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);

            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            
            for (var i = 0; i < files.Length; ++i)
            {
                var fileInfo = files[i];

                EditorUtility.DisplayProgressBar("Set Name", "......", 1f * i / files.Length);
                foreach (string suffix in Filtersuffix)
                {
                    if (fileInfo.Name.EndsWith(suffix))
                    {
                        string path = fileInfo.FullName.Replace('\\', '/').Substring(AssetBundleConfig.PROJECT_PATH.Length);
                        var importer = AssetImporter.GetAtPath(path);
                        if (importer)
                        {
                            string name = path.Substring(fullPath.Substring(AssetBundleConfig.PROJECT_PATH.Length).Length + 1);
                            importer.assetBundleName = name.Substring(0,name.LastIndexOf('.')) + AssetBundleConfig.SUFFIX;
                        }
                    }
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        EditorUtility.ClearProgressBar();
    }


    [MenuItem("ABTool/GetAllAssetBundleName")]

    static void GetAllAssetBundleName()
    {

        string[] names = AssetDatabase.GetAllAssetBundleNames();

        foreach (var name in names)
        {
            Debug.Log(name);
        }

    }


    [MenuItem("ABTool/ClearAssetBundleName")]

    static void ClearResourcesAssetBundleName()
    {
        UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets | SelectionMode.ExcludePrefab);

        string[] Filtersuffix = new string[] { ".prefab", ".unity"};//{ ".prefab", ".mat", ".dds" }; 

        if (!(SelectedAsset.Length == 1)) return;

        string fullPath = AssetBundleConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);

        if (Directory.Exists(fullPath))
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);

            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            
            for (var i = 0; i < files.Length; ++i)
            {
                var fileInfo = files[i];
                EditorUtility.DisplayProgressBar("Clear Name", "......", 1f * i / files.Length);
                foreach (string suffix in Filtersuffix)
                {
                    if (fileInfo.Name.EndsWith(suffix))
                    {
                        string path = fileInfo.FullName.Replace('\\', '/').Substring(AssetBundleConfig.PROJECT_PATH.Length);
                        var importer = AssetImporter.GetAtPath(path);
                        if (importer)
                        {
                            importer.assetBundleName = null;

                        }
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

}
