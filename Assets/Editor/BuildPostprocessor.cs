/**
 * 文件名:BuildPostprocessor.cs
 * Des:在导出Android工程之后对assets/bin/Data/Managed/Assembly-CSharp.dll进行加密
 * **/

using System;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Text;
using Debug = UnityEngine.Debug;
using Process = System.Diagnostics.Process;

public class BuildPostprocessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.Android && (!pathToBuiltProject.EndsWith(".apk")))
        {
            Debug.Log("target: " + target.ToString());
            Debug.Log("pathToBuiltProject: " + pathToBuiltProject);
            Debug.Log("productName: " + PlayerSettings.productName);

            string dllPath = pathToBuiltProject + "/" + PlayerSettings.productName + "/" + "src/main/assets/bin/Data/Managed/Assembly-CSharp.dll";

            if (File.Exists(dllPath))
            {
                //加密 Assembly-CSharp.dll;
                Debug.Log("Encrypt src/main/assets/bin/Data/Managed/Assembly-CSharp.dll Start");

                byte[] bytes = File.ReadAllBytes(dllPath);
                
                //bytes[0] += 1;

                int length = bytes.Length;

                if (length > 10)
                    length = 10;

                for (int i = 0; i < length; i++ )
                {
                    bytes[i] ^= 7;
                }

                File.WriteAllBytes(dllPath, bytes);

                Debug.Log("Encrypt src/main/assets/bin/Data/Managed/Assembly-CSharp.dll Success");

                Debug.Log("Encrypt libmono.so Start !!");

                Debug.Log("Current is : " + EditorUserBuildSettings.development.ToString());

                //替换 libmono.so;
                if (EditorUserBuildSettings.development)
                {
                    string armv7a_so_path = pathToBuiltProject + "/" + PlayerSettings.productName + "/" + "src/main/jniLibs/armeabi-v7a/libmono.so";
                    File.Copy(Application.dataPath + "/Editor/libs/development/armeabi-v7a/libmono.so", armv7a_so_path, true);
                }
                else
                {
                    string armv7a_so_path = pathToBuiltProject + "/" + PlayerSettings.productName + "/" + "src/main/jniLibs/armeabi-v7a/libmono.so";
                    File.Copy(Application.dataPath + "/Editor/libs/release/armeabi-v7a/libmono.so", armv7a_so_path, true);
                }

                Debug.Log("Encrypt libmono.so Success !!");
                copyBuildFiles(pathToBuiltProject);
            }
            else
            {
                Debug.LogError(dllPath+ "  Not Found!!");
            }
        }
    }

    static void copyBuildFiles(string pathToBuiltProject)
    {
        string batch_file = "build.bat";
        string cur_dir = Directory.GetCurrentDirectory() + "/";
        string export_dir = pathToBuiltProject + "/" + PlayerSettings.productName + "/";
        File.Copy(cur_dir + batch_file, export_dir + batch_file, true);
        File.Copy(cur_dir + "gradlew.bat", export_dir + "gradlew.bat", true);
//        File.Delete(export_dir + "libs/unity-ads.aar");
        //res
        foreach (string dirPath in Directory.GetDirectories(cur_dir + "Assets/Plugins/Android/res", "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(cur_dir + "Assets/Plugins/Android/res", export_dir + "src/main/res"));

        foreach (string newPath in Directory.GetFiles(cur_dir + "Assets/Plugins/Android/res", "*.png",
            SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(cur_dir + "Assets/Plugins/Android/res", export_dir + "src/main/res"), true);
        //复制gradle文件夹
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(cur_dir + "gradle", "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(cur_dir + "gradle", export_dir + "gradle"));

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(cur_dir + "gradle", "*.*",
            SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(cur_dir + "gradle", export_dir + "gradle"), true);

        ExcuteBatchFile(pathToBuiltProject + "/" + PlayerSettings.productName, cur_dir + batch_file);
    }

    static void ExcuteBatchFile(string WorkingDirectory, string batchpath)
    {
        Process p = new Process();
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.WorkingDirectory = WorkingDirectory;
        p.StartInfo.RedirectStandardOutput = false;
        p.StartInfo.CreateNoWindow = false;
        DateTime dt = DateTime.Now;
        string cur_time = "" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute;
        StringBuilder ars = new StringBuilder();
        string apkName = PlayerSettings.productName.Replace(" ","") + "_v"
                         + Application.version + "_" + PlayerSettings.Android.bundleVersionCode + "_" + cur_time;
        Debug.Log(apkName);
        ars.Append(PlayerSettings.Android.bundleVersionCode + " ")
            .Append(Application.version + " ")
            .Append(15 + " ")
            .Append(PlayerSettings.Android.keystoreName + " ")
            .Append(PlayerSettings.Android.keystorePass + " ")
            .Append(PlayerSettings.Android.keyaliasName + " ")
            .Append(PlayerSettings.Android.keyaliasPass + " ")
            /*//TODO: error, modify by ouyang .Append(Application.bundleIdentifier + " ")  */
            .Append(apkName + " ")
            .Append(PlayerSettings.productName + " ");
        if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass))
        {
            Debug.Log("keystore密码未填写");
            return;
        }

        p.StartInfo.Arguments = ars.ToString();
        p.StartInfo.FileName = batchpath;
        p.Start();
//        string output = p.StandardOutput.ReadToEnd();
//        //输出脚本打印
//        Debug.Log("output: " + output);
        p.WaitForExit();
        
    }
}