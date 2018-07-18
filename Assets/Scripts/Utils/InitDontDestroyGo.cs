using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitDontDestroyGo : MonoBehaviour {

    public GameObject Go_Music;
    public GameObject Go_AudioClipSet;

    public GameObject LoadingCamera;

    void Awake() {
        Application.targetFrameRate = 60;

        DontDestroyOnLoad(Go_Music);
        DontDestroyOnLoad(Go_AudioClipSet);
        //DontDestroyOnLoad(LoadingCamera);

        AtlasManager.GetInstance();

        CommonData.b_init = true;

        CommonData.ADJUST_WIDTH = (float)CommonData.BASE_WIDTH / (float)Screen.width;
        CommonData.ADJUST_HEIGHT = (float)CommonData.BASE_HEIGHT / (float)Screen.height;
    }
}
