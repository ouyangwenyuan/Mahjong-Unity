using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDesignTools{
    [MenuItem("LevelDesignTools/SetCardsID")]
    public static void SetCardsID()
    {
        GameObject[] gos = Selection.gameObjects;

        foreach(GameObject go in gos){
            //Debug.Log(go.name);

            CardMonoHandler[] cards = go.GetComponentsInChildren<CardMonoHandler>();

            foreach(CardMonoHandler card in cards){
                EditorUtility.SetDirty(card);
                card.text_id.text = card.transform.parent.gameObject.name;

                //Undo.RecordObject(card,"");
                //EditorUtility.SetDirty(card);
            }
        }

        //AssetDatabase.SaveAssets();

        //EditorApplication.MarkSceneDirty();

        //UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo(); ;

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    [MenuItem("LevelDesignTools/ShowCardsNum")]
    public static void ShowCardsNum()
    {
        GameObject[] gos = Selection.gameObjects;

		Debug.Log(gos.Length);
    }

    [MenuItem("LevelDesignTools/FindAudioSourcePlayAwake")]
    public static void FindAudioSourcePlayAwake()
    {
        GameObject[] gos = Selection.gameObjects;

        AudioSource[] audios = gos[0].GetComponentsInChildren<AudioSource>();

        for (int i = 0; i < audios.Length; i++ )
        {
            if (audios[i].playOnAwake) {
                Debug.Log(audios[i].gameObject.name);

               
            }
        }
    }
}
