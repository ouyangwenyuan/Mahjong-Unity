using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Fabric.Internal.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Assets.Editor
{
    
    public class FileCopyProcess
    {
        private static readonly string topLevelManifestRelativePath = "Plugins/Android/AndroidManifest.xml";
        private static readonly string topLevelManifestPath = Fabric.Internal.Editor.Update.FileUtils.NormalizePathForPlatform(Path.Combine(
            Application.dataPath,
            topLevelManifestRelativePath
        ));

        [PostProcessBuild(2)]
        public static void CopyClass(BuildTarget target, string pathToBuiltProject)
        {
            if (!IsAndroidBuild() || Application.isPlaying)
            {
                return;
            }
            string pakagePath = "com.test.test"; //TODO: error, modify by ouyang  Application.bundleIdentifier.Replace('.','/');
            string class_path = pathToBuiltProject + "/" + PlayerSettings.productName + "/" + "src/main/java/"+pakagePath;
            string exportManefist = pathToBuiltProject + "/" + PlayerSettings.productName + "/" + "src/main/";
            File.Copy(Application.dataPath + "/Editor/class/InitApplication.java", class_path + "/InitApplication.java", true);
            File.Copy(Application.dataPath + "/Editor/class/GamePlusActivity.java", class_path + "/GamePlusActivity.java", true);
            File.Copy(Application.dataPath + "/Editor/class/UnityNotificationManager.java", class_path + "/UnityNotificationManager.java", true);

        }

        [PostProcessScene(99)]
        public static void InjectMinefist()
        {
            if (!IsAndroidBuild() || Application.isPlaying)
            {
                return;
            }
            string exportManefist = topLevelManifestPath;
            XmlDocument doc = new XmlDocument();
            doc.Load(exportManefist);
            //修改application节点
            var applicationNodes = doc.GetElementsByTagName("application");
            var applicationNode = applicationNodes[0];
            if (applicationNode.Attributes != null)
            {
                if (applicationNode.Attributes["android:name"]!=null)
                {
                    applicationNode.Attributes["android:name"].Value = PlayerSettings.applicationIdentifier + ".InitApplication";
                }
                else
                {
                    XmlAttribute attribute = doc.CreateAttribute("android","name",":");
                    attribute.Value=PlayerSettings.applicationIdentifier + ".InitApplication";
                    applicationNode.Attributes.Append(attribute);
                }
            }
                
            //修改lauchActivity节点
            var activitys = doc.GetElementsByTagName("activity");
            var lauchActivity = activitys[0];
            if (lauchActivity.Attributes != null) lauchActivity.Attributes["android:name"].Value = PlayerSettings.applicationIdentifier + ".GamePlusActivity";

            //插入本地推送
//            <receiver android:name=".UnityNotificationManager"/>
            XmlNode node1 = doc.CreateNode(XmlNodeType.Element, "pagecount", null);
            node1.InnerText = "222";

            doc.Save(exportManefist);
        }

        private static bool IsAndroidBuild()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
        }
    }
}
