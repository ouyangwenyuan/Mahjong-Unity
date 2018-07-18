using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasManager{
    static AtlasManager instance;

    public static AtlasManager GetInstance()
    {
        if (instance == null)
        {
            instance = new AtlasManager();
        }

        return instance;
    }

    AtlasManager() {
        LoadSprites();
    }

    Sprite[] sprites;

    Dictionary<string, Sprite> cards_dic = new Dictionary<string, Sprite>();

    void LoadSprites() {
        sprites = Resources.LoadAll<Sprite>("Atlas/Cards/Atlas");

        for (int i = 0; i < sprites.Length; i++ )
        {
            cards_dic.Add(sprites[i].name, sprites[i]);
        }
    }

    public Sprite GetCardSprite(string name) {

        if (name == null) {
            Debug.Log("Sprite No Name !!!");

            return null;
        }

        if(cards_dic.ContainsKey(name)){
            return cards_dic[name];         
        }

        return null;
    }
}
