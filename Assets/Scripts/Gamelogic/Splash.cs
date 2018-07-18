using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{

    public Animator animator;
    public static bool IsIpad;
    void Start()
    {
        IsIpad = SystemInfo.deviceModel.Contains("iPad");
//        IsIpad = true;
        if (IsIpad)
        {
            CommonData.BASE_WIDTH = 768;
            CommonData.BASE_HEIGHT = 1024;
        }
//        adapterCanvas();
        Invoke("Load", 2f);
    }

    void Update()
    {
    }

    public void Load()
    {
        SceneManager.LoadScene("Init");
    }

    void adapterCanvas()
    {
        float scale = 1;

        if (1280f / 720f > Screen.height * 1.0f / Screen.width)
        {
            scale = Screen.height / 1280f;
        }
        else
        {
            scale = Screen.width / 720f;
        }
        transform.GetComponent<CanvasScaler>().scaleFactor = scale;
    }
}
