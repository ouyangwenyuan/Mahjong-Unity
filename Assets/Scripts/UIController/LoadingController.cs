using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

    void Awake() {

        //Invoke("Load",1);

        Load();
    }

    void Load() {
        AsyncOperation async = SceneManager.LoadSceneAsync(CommonData.loading_target_scene);
    }

	// Use this for initialization
	//void Start () {
		
	//}
	
	// Update is called once per frame
	void Update () {
		
	}
}
