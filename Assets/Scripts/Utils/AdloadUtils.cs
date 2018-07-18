using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIController
{
    public class AdloadUtils:MonoBehaviour
    {
        public Transform ParenTransform;
        public Text DoText;
        public GameObject MaskTransform;
        public static AdloadUtils Instance;
        private string[] _dotStrings = {"    "," .  ", " . . ", " . . ."};
        private int _index;
        public static bool AdMaskShow=true;
        public const float AdLoadTimeout = 7f;
        void Start()
        {
            Instance = this;
        }
        void Update()
        {

        }

        public void Show()
        {
            AdMaskShow = true;
            setMask(true);
            InvokeRepeating("ChangeDots",0f,0.5f);
            Invoke("hide", AdLoadTimeout);
        }

        public void hide()
        {
            AdMaskShow = false;
            CancelInvoke("ChangeDots");
            setMask(false);
        }

        void ChangeDots()
        {
            DoText.text = _dotStrings[_index++%_dotStrings.Length];
        }

        public void setMask(bool status)
        {
            Image image = MaskTransform.GetComponent<Image>();
            if (status)
            {
                ParenTransform.localPosition=new Vector3(0,0,0);
                MaskTransform.SetActive(true);
                image.DOFade(0.8f, 0.5f).SetAutoKill(true);
            }
            else
            {
                ParenTransform.localPosition = new Vector3(6000, 0, 0);
                image.DOFade(0, 0.5f)
                    .OnComplete(Complete)
                    .SetAutoKill(true);
            }
        }

        void Complete()
        {
            MaskTransform.SetActive(false);
        }
    }
}
