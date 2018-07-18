using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIController
{
    public class TestDot:MonoBehaviour
    {
        public Image image;
        void Start()
        {
            
        }

        public void setAlpha()
        {
            image.DOFade(200, 3)
                .SetAutoKill(true);
        }

        public void DeAlpha()
        {
            image.DOFade(0, 3)
                .SetAutoKill(true);
        }
    }
}
