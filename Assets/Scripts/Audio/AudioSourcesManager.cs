using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcesManager
{

    static int id = 0;
    static AudioSourcesManager instance;

    public static AudioSourcesManager GetInstance() {
        if (instance == null) {
            instance = new AudioSourcesManager();
        }

        return instance;
    }


    public Dictionary<int, AudioSourceMonoHandler> listener_list = new Dictionary<int, AudioSourceMonoHandler>();

    public int AddAudioSource(AudioSourceMonoHandler a)
    {
        id++;

        listener_list.Add(id, a);

        return id;
    }

    public void RemoveAudioSource(int id)
    {
        listener_list.Remove(id);
    }

    public void ChangeStatus(bool b , AUDIO_TYPE type) {
        foreach (KeyValuePair<int, AudioSourceMonoHandler> kv in listener_list)
        {
            if (kv.Value != null && type == kv.Value.type) {
                kv.Value.m_audio.mute = !b;
            }
        }
    }

    public void Play(AudioSource audio , AudioClip clip , bool is_loop = false) {
        if (audio != null && clip != null) {
            if (audio.isPlaying) {
                audio.Stop();
            }

            audio.clip = clip;
            audio.loop = is_loop;
            audio.Play();
        }
    }

    public void Stop(AudioSource audio)
    {
        if (audio != null)
        {
            if (audio.isPlaying)
            {
                audio.Stop();
            }
        }
    }
}
