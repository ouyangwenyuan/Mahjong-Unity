using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSourceMonoHandler : MonoBehaviour {

    public AUDIO_TYPE type = AUDIO_TYPE.MUSIC;
    public AudioSource m_audio;

    int id;
	void Awake () {
        m_audio = GetComponent<AudioSource>();

        if (m_audio != null)
        {
            bool on = LocalDynamicData.GetInstance().GetMusicOn();

            m_audio.mute = !on;

            id = AudioSourcesManager.GetInstance().AddAudioSource(this);
        }

	}

    void OnDestroy() {
        if (m_audio != null)
        {
            AudioSourcesManager.GetInstance().RemoveAudioSource(id);
        }
    }
}