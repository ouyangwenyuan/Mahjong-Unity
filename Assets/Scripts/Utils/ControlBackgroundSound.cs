using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlBackgroundSound : MonoBehaviour {
    AudioSource audio_background;
    string scene_name = null;
    AudioClipSet audioclip_set;
    bool is_random = true;

    bool is_begin = false;

    System.Random random = new System.Random();

    AudioClip clip;

	// Use this for initialization
	void Start () {
        audio_background = GetComponent<AudioSource>();

        GameObject go_audioclip_set = GameObject.Find("AudioClipSet");
        if (go_audioclip_set != null) { audioclip_set = go_audioclip_set.GetComponent<AudioClipSet>(); }

        scene_name = SceneManager.GetActiveScene().name;

        if (scene_name.Equals("Main"))
        {

            AudioSourcesManager.GetInstance().Play(audio_background, (audioclip_set == null) ? null : GetBS());
        }
        else if(scene_name.StartsWith("Level")) {
            is_random = false;

            AudioSourcesManager.GetInstance().Play(audio_background, CommonData.level_clip, true);
        }

        is_begin = true;
	}

	// Update is called once per frame
	void Update () {
        if (is_random && is_begin){
            if (!audio_background.isPlaying)
            {
                AudioSourcesManager.GetInstance().Play(audio_background, (audioclip_set == null) ? null : GetBS());

                //Debug.Log("Play");
            }
        }
	}

    AudioClip GetBS()
    {
        int pos = random.Next() % (3);

        if (pos == 0)
        {
            return CommonData.level_clip = audioclip_set.background1;
        }
        else if (pos == 1) 
        {
            return CommonData.level_clip = audioclip_set.background2;
        }

        return CommonData.level_clip = audioclip_set.background3;
    }

    public void Stop() {
        audio_background.Stop();
    }
}
