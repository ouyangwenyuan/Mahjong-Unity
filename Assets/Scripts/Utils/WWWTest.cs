using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;


//图片下载测试
public class WWWTest : MonoBehaviour
{

    public Transform m_sprite;      //场景中的一个Sprite

    WWW www;                        //请求
    Texture2D texture2D;            //下载的图片

    string file_path;               //保存的文件路径
    string url;


    void Start()
    {
        //保存路径，手机上不能放在Resource的路径上
        //file_path = Application.dataPath + "/Resources/picture.jpg";
    }

    public void SetFithPath(string file) {
        file_path = file;
    }

    public void SetURL(string u)
    {
        url = u;
    }

    public void OnBtnClick() {
        Debug.Log("Downloading.......");
        StartCoroutine(LoadImg());
    }

    IEnumerator LoadImg()
    {
        //开始下载图片
        www = new WWW(url);
        yield return www;

        //下载完成，保存图片到路径filePath
        texture2D = www.texture;
        byte[] bytes = texture2D.EncodeToPNG();
        File.WriteAllBytes(file_path, bytes);

        //将图片赋给场景上的Sprite
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
        m_sprite.GetComponent<Image>().sprite = sprite;
        
        Debug.Log("Done");
    }
}