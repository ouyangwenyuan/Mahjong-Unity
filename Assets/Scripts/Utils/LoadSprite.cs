using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class LoadSprite 
    {
        private static LoadSprite loadSprite;
        static Dictionary<string, Dictionary<string, Sprite>> maps;

        public static LoadSprite getInstance()
        {
            if (loadSprite == null)
            {

                maps = new Dictionary<string, Dictionary<string, Sprite>>();
                loadSprite = new LoadSprite();
                Dictionary<string, Sprite> spDir = loadSprite.loadSprites(CommonData.MAHJONG_TILE_SET);
                maps.Add(CommonData.MAHJONG_TILE_SET, spDir);
                
            }
            return loadSprite;
        } 

        public Dictionary<string, Sprite> loadSprites(string filename)
        {
            Dictionary<string, Sprite> spDir;
            Sprite[] sp = Resources.LoadAll<Sprite>(filename);
            spDir = new Dictionary<string, Sprite>();
            for (int i = 0; i < sp.Length; i++)
            {
                spDir.Add(sp[i].name, sp[i]);
            }
            return spDir;
        }

        public Sprite getSpriteByName(string set_name, string sprite_name)
        {
            Dictionary<string, Sprite> spDir;
            maps.TryGetValue(set_name, out spDir);
            Sprite sp = new Sprite(); 
            spDir.TryGetValue(sprite_name, out sp);

            return sp;
        }

        public List<string> getSpriteNames(string set_name)
        {
            Dictionary<string, Sprite> spDir;
            maps.TryGetValue(set_name, out spDir);
            List<string> tileName = new List<string>();
            foreach (KeyValuePair<string, Sprite> kvp in spDir)
            {
                tileName.Add(kvp.Key);
            }
            return tileName;
        }
    }
}