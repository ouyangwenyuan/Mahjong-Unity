using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Script.gameplus.define;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GamePlus.utils
{
    public class ImageUtil : MonoBehaviour 
    {
        //设置头像,需要判断facebook是否登录
        public void SetHeadImage(string fbid, Image headImage)
        {
            var data =
                DynamicDataBaseService.GetInstance()
                    .Connection.Table<FriendData>().FirstOrDefault(x => x.fb_id == fbid);
            if (data==null)
            {
                Debug.Log("设置头像出错");
                return;
            }
            var directoty = Application.persistentDataPath + "//" + Constance.COVER_PATH;
            var image = directoty + "//" + fbid + ".jpg";
            if (File.Exists(image))
            {
                LoadLocalImag(image, headImage);
            }
            else
            {
                StartCoroutine(LoadWebImg(data.head_WebPath, headImage, fbid, directoty));
            }
        }

        //从网络下载
        public static IEnumerator LoadWebImg(string url, Image headImage, string fbid, string directoty)
        {
            var www = new WWW(url);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
            }
            else
            {
                var texture2D = www.texture;
                var bytes = texture2D.EncodeToPNG();
                //保存到本地
                if (!Directory.Exists(directoty))
                {
                    Directory.CreateDirectory(directoty);
                }
                var filePath = directoty + "//" + fbid + ".jpg";
                File.WriteAllBytes(filePath, bytes);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
                headImage.sprite = sprite;
                Debug.Log("LoadWebImg Done " + filePath);
            }
        }

        //从本地加载
        public static void LoadLocalImag(string imgPath, Image headImage)
        {
            Texture2D tex = null;

            if (File.Exists(imgPath))
            {
                var fileData = File.ReadAllBytes(imgPath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            if (tex != null)
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                headImage.sprite = sprite;
            }
            Debug.Log("LoadLocalImag Done");
        }
    }
}
