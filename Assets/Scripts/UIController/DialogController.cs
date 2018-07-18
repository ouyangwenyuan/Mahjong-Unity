using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UIController
{
    public enum DialogItem
    {
        Locked=1,
        Network=2,
        Purchase=3,
        Gift=4
    }
    
    public class DialogController:MonoBehaviour
    {
        public Transform DialogTransform;
        public Text TileText;
        public Text DescText;
        private DialogItem curDialogItem;
		public GameObject maskShop;
        public static event Action OnGiftConfirm;
        void Start()
        {

			DialogTransform.gameObject.SetActive (false);
        }

        public void show(DialogItem item,string title = "")
        {
            curDialogItem = item;
			DialogTransform.gameObject.SetActive (true);
            mask(true);
            UpdateUi(item, title);
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(DialogTransform.transform.DOLocalMoveX(-30, 0.5f));
            mySequence.Append(DialogTransform.transform.DOLocalMoveX(0, 0.2f));
        }

        public void hide()
        {
            mask(false);
            DialogTransform.transform.DOLocalMoveX(-CommonData.BASE_WIDTH, 0.5f)
                .SetEase(Ease.InQuint)
                .OnComplete(InitPos)
                .SetAutoKill(true)
                .SetUpdate(true);
        }

        private void InitPos()
        {
            if (curDialogItem == DialogItem.Gift && OnGiftConfirm!=null)
            {
                OnGiftConfirm();
            }
            DialogTransform.transform.localPosition = new Vector3(CommonData.BASE_WIDTH, 0, 0);
			DialogTransform.gameObject.SetActive (false);
        }

        void UpdateUi(DialogItem item,string title = "")
        {
            string[] items = Enum.GetNames(typeof (DialogItem));
            foreach (string s in items)
            {
                if (s.Equals(item.ToString()))
                {
                    DialogTransform.Find(s).gameObject.SetActive(true);
                    if (item==DialogItem.Gift)
                    {
                        DialogTransform.Find(s).gameObject.transform.Find("desc").GetComponent<Text>().text
                            = "You've got one " + title;
                        Sprite icon = Resources.Load<Sprite>(ShopController.IMG_PATH + "det_" + title.ToLower());
                        DialogTransform.Find(s)
                            .gameObject.transform
                            .Find("Booster")
                            .GetComponent<Image>()
                            .sprite = icon;
                    }
                }
                else
                {
                    DialogTransform.Find(s).gameObject.SetActive(false);
                }
            }
        }

        public void mask(bool flag)
        {
            if ("Main".Equals(SceneManager.GetActiveScene().name))
            {
                GameObject.Find("CanvasFront").GetComponent<MainFrontController>().setMask(flag);
            }
            else
            {
                GameObject.Find("CanvasFront").GetComponent<FrontController>().setMask(flag);
            }

            if (curDialogItem==DialogItem.Purchase)
            {
                setMask(flag);
            }
        }

        public void setMask(bool status)
        {
            Image image = maskShop.GetComponent<Image>();
            if (status)
            {
                maskShop.SetActive(true);
                image.DOFade(0.8f, 0.5f).SetAutoKill(true);
            }
            else
            {
                image.DOFade(0, 0.5f)
                    .OnComplete(Complete)
                    .SetAutoKill(true);
            }
        }

        void Complete()
        {
            maskShop.SetActive(false);
        }
    }
}
