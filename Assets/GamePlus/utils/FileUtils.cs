using Assets.Script.gameplus.define;
using Fabric.Internal.ThirdParty.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GamePlus.utils
{
    public class FileUtils : MonoBehaviour 
    {
       public static IEnumerator DownloadFile(string url,string filename,string path)
        {
            WWW w = new WWW(url);
            yield return w;
            if (w.isDone)
            {
                byte[] fileBytes = w.bytes;
                int length = fileBytes.Length;
                //保存文件到本地
                createConfigFile(path, filename, fileBytes, length);
                Debug.Log("DownloadFile success:" + filename);
            }
            else
            {
                Debug.Log("DownloadFile fail");
            }
        }

        public static void createConfigFile(string path,string name,byte[] content,int length){
            Stream sm;
            FileInfo configInfo = new FileInfo(path + "//" + name);
            //删除旧配置文件
            if (configInfo.Exists)
            {
                configInfo.Delete();
            }
            sm = configInfo.Create();
            sm.Write(content, 0, length);
            sm.Close();
            sm.Dispose();
        }

        public static string loadTxtFile(string path,string name){
            StreamReader sr = null;
            StringBuilder content =new StringBuilder();
            try
            {
                sr = File.OpenText(path + "//" + name);
            }
            catch(Exception e)
            {
                return "";
            }
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                //逐行读取
                content.AppendLine(line);
            }
            sr.Close();
            sr.Dispose();
            return content.ToString();
        }

        public static byte[] loadBaniryFile(string path,string name){
            try
            {
                return File.ReadAllBytes(path + "//" + name);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static Dictionary<string, object> getConfig(string filename)
        {
            string configStr = FileUtils.loadTxtFile(Application.persistentDataPath, filename);
            var dict = Json.Deserialize(configStr) as Dictionary<string, object>;
            Debug.Log("configStr: " + configStr);
            if ("".Equals(configStr))
            {
                return null;
            }
            return dict;
        }

        public static bool shouldReadConf(string filename)
        {
            double cur_time = TimeUtils.getUnixTime(DateTime.Now.ToUniversalTime().Ticks);
            FileInfo configInfo = new FileInfo(Application.persistentDataPath + "//" + filename);
            if(configInfo.Exists)
            {
                DateTime d1 = configInfo.LastWriteTime;
                double lastModified = TimeUtils.getUnixTime(d1.ToUniversalTime().Ticks);
                Debug.Log("lastModified:" + lastModified);
                Debug.Log("cur_time:" + cur_time);
                if (cur_time - lastModified > 20)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static void serializeObj(object obj,string fname)
        {
            //使用二进制序列化对象
            string fileName = Application.persistentDataPath + "//" + fname;//文件名称与路径
            Stream fStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
            binFormat.Serialize(fStream, obj);
        }

        public static object dserializeObj(string fname){
            string fileName = Application.persistentDataPath + "//" + fname;//文件名称与路径
            Stream fStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
            return binFormat.Deserialize(fStream);
        }


    }
}
