using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    public class TextAttachUtils:MonoBehaviour
    {
        private IEnumerable<GameStr> gameStrs;
        void Start()
        {
            gameStrs = StaticDataBaseService.GetInstance().GetGameStr();
            Invoke("init",3f);
        }

        void init()
        {
            Text[] texts = GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                GameStr txt = gameStrs.FirstOrDefault(x => (x.id + "").Equals(text.text));
                if (txt!=null)
                {
                    text.text = txt.guid_text;
                }
            }
        }
    }
}
