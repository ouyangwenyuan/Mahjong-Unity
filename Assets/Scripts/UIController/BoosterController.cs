using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.FireBaseManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIController
{
    public class BoosterItem
    {
        public int propNum;
        public int goldNum;
        
        public BoosterItem(int propNum, int goldNum)
        {
            this.propNum = propNum;
            this.goldNum = goldNum;
        }
    }
    public class BoosterController : MonoBehaviour
    {
        public static BoosterController instance;
        public Image titleImage;
        private Text descText;
        public Transform ItemsTransform;
        public Transform Boooster;
        private Sprite icon;
        private long openTime;
        private IEnumerable<BuyItem> payments;
        private int _spendNum;
        private string _propName;
        private bool _hideByPropClick;
        void Awake()
        {
            // Limit the number of instances to one
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            descText = Boooster.Find("desc").GetComponent<Text>();
            payments = StaticDataBaseService.GetInstance().GetBuyItem();
			Boooster.gameObject.SetActive (false);
        }

        void Update()
        {

        }

        public void ShowBooster(string propName,int spendNum)
        {
            openTime = PlayerInfoUtil.GetTStamp();
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("booster", "open");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            //设置desc
			Boooster.gameObject.SetActive (true);

            _spendNum = spendNum;
            _propName = propName;
			Boooster.gameObject.SetActive (true);

            GameObject.Find("CanvasFront").GetComponent<FrontController>().setMask(true);
            ShopItemCode code = (ShopItemCode) Enum.Parse(typeof (ShopItemCode), propName);
            BuyItem itemList = payments.First(x => x.item == (int)code);
            descText.text = StaticDataBaseService
                        .GetInstance()
                        .GetGameStr()
                        .First(x => x.id == itemList.descTxt)
                        .guid_text;
            //设置图片
            icon = Resources.Load<Sprite>(ShopController.IMG_PATH + "det_" + propName.ToLower());
            titleImage.sprite = icon;

            Transform child = ItemsTransform.Find("ButtonBuy");
            int goldSpend = itemList.base_gold + itemList.step_gold * _spendNum;
            child.Find("num").GetComponent<Text>().text = goldSpend + "";
            Button buy = child.GetComponent<Button>();
            buy.onClick.RemoveAllListeners();
            buy.onClick.AddListener(delegate() { OnBuyClick(propName, itemList); });

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(Boooster.transform.DOLocalMoveX(-30, 0.5f));
            mySequence.Append(Boooster.transform.DOLocalMoveX(0, 0.2f));
        }

        public void HideBooster()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("booster", "hide");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            Dictionary<string, object> desc2 = new Dictionary<string, object>();
            desc2.Add("shop", SecitonUtils.CountSection(3, (int)(PlayerInfoUtil.GetTStamp() - openTime)));
            AnalysisSup.fabricLog(EventName.UI_SHOW_TIME, desc2);

            GameObject front = GameObject.Find("CanvasFront");
            if (front!=null)
            {
                front.GetComponent<FrontController>().setMask(false);
            }
            Resources.UnloadAsset(icon);
            Boooster.transform.DOLocalMoveX(-CommonData.BASE_WIDTH, 0.5f)
                .SetEase(Ease.InQuint)
                .OnComplete(InitPos)
                .SetAutoKill(true)
                .SetUpdate(true);
        }

        void InitPos()
        {
            Boooster.localPosition = new Vector3(7000, 0, 0);
			Boooster.gameObject.SetActive (false);
            if (_hideByPropClick)
            {
                GameObject.Find("CanvasFront").GetComponent<FrontController>().OnBoosterClick(_propName);
                _hideByPropClick = false;
            }
        }

        void OnBuyClick(string propName, BuyItem BoostItem)
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("booster", propName);
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            if (ClickUtils.IsDoubleClick()) return;
            Debug.Log("OnBuyClick:" + propName);
            _hideByPropClick = true;
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            int goldSpend = BoostItem.base_gold + BoostItem.step_gold * _spendNum;
            if (playerInfo.Gold < goldSpend)
            {
                Debug.Log("金币不足");
                HideBooster();
                ShopController.instance.show(ShopItem.Gold);
                return;
            }
            SetReflectValue(playerInfo, propName, BoostItem.count);
            SetReflectValue(playerInfo, "Gold", -goldSpend);
            PlayerInfoUtil.SetConsumeGold(goldSpend);
            GamePlusLog.Instance.SpentLog(goldSpend, propName);
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            GameObject.Find("CanvasFront").GetComponent<FrontController>().InitSkillsUI();
			HideBooster();
        }

        private static object GetReflectValue(object obj, string itemKey)
        {
            PropertyInfo prop = obj.GetType().GetProperty(itemKey, BindingFlags.Public | BindingFlags.Instance);
            return prop.GetValue(obj, null);
        }

        private static void SetReflectValue(object obj, string itemKey, int num)
        {
            PropertyInfo prop = obj.GetType().GetProperty(itemKey, BindingFlags.Public | BindingFlags.Instance);
            int prop_num = (int)prop.GetValue(obj, null);
            prop.SetValue(obj, prop_num + num, null);
        }
    }
}
