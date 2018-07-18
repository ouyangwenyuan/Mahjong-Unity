using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Assets.GamePlus.advertise;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.ironsrc;
using Assets.GamePlus.listner;
using Assets.GamePlus.support;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.FireBaseManager;
using Assets.Scripts.GameDefine;
using Assets.Scripts.FirebaseController;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils;
using DG.Tweening;
using Facebook.Unity;
using DG.Tweening.Plugins.Core;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UIController
{
    public enum ShopItem
    {
        Gold = 1,
        Heart = 2,
        Packet = 3
    }
    public enum ShopItemCode
    {
        Life = 101,
        MaxLife = 102,
        Gold = 201,
        Redraw = 301,
        Bomb = 302,
        Clock = 303,
        Jewel = 304,
        Unchain = 305,
    }
    public enum PropsItem
    {
        Redraw = 1,
        Bomb = 2,
        Clock = 3,
        Jewel = 4,
        Unchain = 5
    }
    public enum PromotionType
    {
        None = 0,
        Hot = 1,
        Best = 2
    }
    public class ShopController : MonoBehaviour, PurchaseListner

    {
        public static ShopController instance;
		public Transform Parent;
        public Transform Panel;
        public Transform Items;
        public Transform Shop;
        public Transform ImageGold;
        //顶部UI
        public Transform topGold;
        public Transform topProps;
        //列表
        public Transform GoldList;
        public Transform GoldCon;
        public Transform HeartList;
        public Transform PacketList;
		//广告按钮遮罩
		public GameObject maskAdBtn;
        public GameObject Outside_UI_coins;
        public GameObject MaxlifeGameObject;
		//
        public Color itemActiveColor;
        public Color itemDefaultColor;
        public ShopItem CurItem = ShopItem.Gold;
        private float GoldItemHight = 168;
        private float DivideHeight = 2;
        //private string[] props = new[] { "Redraw", "Bomb", "Clock", "Jewel", "Unchain" };
        private IEnumerable<Shop> shops; 
        private IEnumerable<Item> propItems;
        private IEnumerable<BuyItem> payments;
        //促销UI
        //private bool promote = false; 
        public Transform PromoteUi;
        //item图片
        private Sprite ItemActive;
        private Sprite ItemDeActive;
        //资源路径
        public const string IMG_PATH = "Texture/UI/";
        //内购
        private InAppPurchaseSup inappSup;
        //广告
        //sdk
        private GPVideoAd gpVideoAd;
        private FacebookSup fbSup;
        //当前点击的item
        private Shop curShop;
        //fb
        public Text FbText;
        //广告按钮
        public const string ShopAdsDown = "shop_ads_down";
        public long AdCowntDown = 0;
        public const int AdDownTime = 5;
        public bool AdCori = false;
        private bool shopFBClicked = false;
        //打开时间
        private long openTime;
		//ads
		public Text adText;
		public AudioSource audio;
		public AudioClip CoinClip;
        //gold content Y轴变化
        private float _goldConY;
        private Text _goldText;
        private int _preGold;
        private int _endGold;
        private Dictionary<string,float> ImgPos;
        void Awake()
        {
            // Limit the number of instances to one
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // duplicate
                Destroy(gameObject);
            }
        }

        void Start()
        {   
            fbSup = GameObject.Find("GamePlus").GetComponent<FacebookSup>();
            FacebookSup.fbLoginHandler += fbloged;
            inappSup = GameObject.Find("GamePlus").GetComponent<InAppPurchaseSup>();
            gpVideoAd = GameObject.Find("GamePlus").GetComponent<GPVideoAd>();
            inappSup.SetPurchaseListner(this);
            ItemActive = Resources.Load<Sprite>(IMG_PATH + "det_chosen");
            ItemDeActive = Resources.Load<Sprite>(IMG_PATH + "det_unchosen");
            shops = StaticDataBaseService.GetInstance().GetPurchase();
            propItems = StaticDataBaseService.GetInstance().GetItem();

            ImgPos=new Dictionary<string, float>();
            payments = StaticDataBaseService.GetInstance().GetBuyItem();

            //菜单初始化
            InitGoldMenu();
            InitHeartMenu();
            InitPacketMenu();
            UpdateTopUi();
			Parent.gameObject.SetActive (false);
        }

        void Update()
        {
            UpdateFBStatus();
            _goldConY = GoldList.transform.localPosition.y;
        }

        void InitGoldMenu()
        {
            GameObject prefab = Resources.Load("prefabs/shop/GoldItem") as GameObject;
            List<Shop> itemShops = shops.Where(x => x.product_code.Contains("coin")).ToList();
            List<ShopOrder> shopOrders = DynamicDataBaseService.GetInstance().GetShopOrders().ToList();
            int j = 0;
            foreach (Shop itemShop in itemShops)
            {
                j++;
                ShopOrder order = shopOrders.First(x => x.id == itemShop.id);
                int i = order.id;
                GameObject GoldItem = Instantiate(prefab);
                GoldItem.name = "GoldItem"+i;
                GoldItem.transform.SetParent(GoldList);
                float Ypos = 474 - (GoldItemHight + DivideHeight)*(order.ItemOrder - 1);
                GoldItem.transform.localPosition = new Vector3(0, Ypos, 0);
                GoldItem.transform.localScale = new Vector3(1, 1, 1);
                Sprite imageSprite = Resources.Load<Sprite>(IMG_PATH + "det_gold_" + i);
                Image goldImage = GoldItem.transform.Find("ImageGold").GetComponent<Image>();
                goldImage.sprite = imageSprite;
                goldImage.SetNativeSize();
//                itemShop.benefit * 100 + "%";
                if (itemShop.benefit-0<0.01)
                {
                    GoldItem.transform.Find("GoldCon").Find("price").gameObject.SetActive(false);
                    GoldItem.transform.Find("GoldCon").Find("Image").gameObject.SetActive(false);
                    GoldItem.transform.Find("GoldCon").localPosition=new Vector3(0,-20,0);
                }
                else
                {
                    GoldItem.transform.Find("GoldCon").Find("price").GetComponent<Text>().text = "" + itemShop.count1;
                }
                GoldItem.transform.Find("ImageProfit").gameObject.SetActive(order.Tag==(int) PromotionType.Hot);
                GoldItem.transform.Find("ImageBest").gameObject.SetActive(order.Tag == (int)PromotionType.Best);
                GoldItem.transform.Find("BtnBuy").Find("Text").GetComponent<Text>().text ="$ "+itemShop.usd;
                float coinActualAdd = itemShop.count1*(1 + itemShop.benefit);
                GoldItem.transform.Find("GoldCon").Find("benifit").GetComponent<Text>().text = "" + coinActualAdd;
                
                //获得图片位置
                ImageGold = GoldItem.transform.Find("ImgGold");
                if (j == itemShops.Count)
                {
                    ImageGold = GoldItem.transform.Find("ImgGold");
                }
                else
                {
                    ImageGold.gameObject.SetActive(false);
                }
                ImgPos.Add(itemShop.product_code,Ypos);
                GoldItem.transform.Find("BtnBuy").GetComponent<Button>().onClick.AddListener(delegate() { OnGoldItemClick(itemShop); });
            }
        }

        void InitHeartMenu()
        {
            GameObject prefab = Resources.Load("prefabs/shop/HeartItem") as GameObject;
            List<BuyItem> itemShops = payments.Where(x => x.id<=5).ToList();
            int i = 0;
            foreach (BuyItem itemShop in itemShops)
            {
                GameObject HeartItem = Instantiate(prefab);
                HeartItem.name = "HeartItem" + i;
                HeartItem.transform.SetParent (HeartList);
                HeartItem.transform.localPosition = new Vector3(0, 367 - (GoldItemHight + DivideHeight) * i, 0);
                HeartItem.transform.localScale = new Vector3(1, 1, 1);
//                if (1==itemShop.discount)
//                {
//                    HeartItem.transform.Find("benifit").GetComponent<Text>().text = "";
//                }
//                else
//                {
//                    HeartItem.transform.Find("benifit").GetComponent<Text>().text = "+" + itemShop.discount * 100 + "%";
//                }
                Sprite imageSprite=null;
                string desc = "";
                HeartItem.transform.Find("BtnBuy").Find("Text").GetComponent<Text>().text = itemShop.base_gold + "";
                if (itemShop.infinite_life==1)
                {
                    imageSprite = Resources.Load<Sprite>(IMG_PATH + "det_life4");
                    if (itemShop.infinite_time == 1)
                    {
                        desc = itemShop.infinite_time + " Day \n unlimited lives";
                    }
                    else
                    {
                        desc = itemShop.infinite_time + " Days \n unlimited lives";
                    }
                }
                if (itemShop.item==(int) ShopItemCode.Life)
                {
                    imageSprite = Resources.Load<Sprite>(IMG_PATH + "det_life1");
                    desc = "Full set of lives";
                }
                if (itemShop.item == (int)ShopItemCode.MaxLife)
                { 
                    desc = "Add max lives";
                    imageSprite = Resources.Load<Sprite>(IMG_PATH + "det_life3");
                    MaxlifeGameObject = HeartItem;
                    UpdateMaxPrice(itemShop,true);
                }

                HeartItem.transform.Find("desc").GetComponent<Text>().text = desc;
                HeartItem.transform.Find("ImageHeart").GetComponent<Image>().sprite = imageSprite;
                var shop = itemShop;
                HeartItem.transform.Find("BtnBuy").GetComponent<Button>().onClick.AddListener(delegate() { OnHeartItemClick(shop); });
                i++;
            }
        }

        void UpdateMaxPrice(BuyItem itemShop,bool init)
        {
            if (!init)
            {
                LocalDynamicData.GetInstance().SetMaxLifeBuyNum(1);
            }
            int newPrice = itemShop.base_gold + itemShop.step_gold*LocalDynamicData.GetInstance().maxlifeBuyTime;
            MaxlifeGameObject.transform.Find("BtnBuy").Find("Text").GetComponent<Text>().text =newPrice  + "";
        }

        void InitPacketMenu()
        {
//            GameObject prefab = Resources.Load("prefabs/shop/PacketItem") as GameObject;
            List<Shop> itemShops = shops.Where(x => x.product_code.Contains("bag")).ToList();
            string[] strings = {"COMMON","RARE","EPIC"};
            int i = 0;
            foreach (Shop itemShop in itemShops)
            {
//                GameObject GoldItem = Instantiate(prefab);
                GameObject GoldItem = PacketList.GetChild(i).gameObject;
                GoldItem.transform.Find("ImageProfit").Find("Text").GetComponent<Text>().text = "+ " + itemShop.benefit * 100 + "%";
                GoldItem.transform.Find("PacketDesc").GetComponent<Text>().text =strings[i]+ "";
                GoldItem.transform.Find("BtnBuy").Find("Text").GetComponent<Text>().text = "$ " + itemShop.usd;
                GoldItem.transform.Find("Pack1").gameObject.SetActive(1 == i + 1);
                GoldItem.transform.Find("Pack2").gameObject.SetActive(2 == i + 1);
                GoldItem.transform.Find("Pack3").gameObject.SetActive(3 == i + 1);
                var shop = itemShop;
                GoldItem.transform.Find("BtnBuy").GetComponent<Button>().onClick.AddListener(delegate() { OnPacketItemClick(shop); });
                i++;
            }
        }

        void UpdateTopUi()
        {
            _goldText = topGold.Find("numGold").GetComponent<Text>();
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            _goldText.text = playerInfo.Gold + "";
            topProps.Find("Redraw").Find("Text").GetComponent<Text>().text = "" + playerInfo.Redraw; 
            topProps.Find("Bomb").Find("Text").GetComponent<Text>().text = "" + playerInfo.Bomb;
            topProps.Find("Clock").Find("Text").GetComponent<Text>().text = "" + playerInfo.Clock;
            topProps.Find("Jewel").Find("Text").GetComponent<Text>().text = "" + playerInfo.Jewel;
            topProps.Find("Unchain").Find("Text").GetComponent<Text>().text = "" + playerInfo.Unchain;
        }

        private void UpdatePromoteUI()
        {
//            if (promote && CurItem == ShopItem.Gold)
//            {
//                PromoteUi.localPosition = new Vector3(0, 378, 0);
//                GoldCon.localPosition = new Vector3(0, -148, 0);
//            }
//            else
//            {
//                PromoteUi.localPosition = new Vector3(0, -1000f, 0);
//                GoldCon.localPosition = new Vector3(0, 0, 0);
//            }
        }

        void UpdateUi(ShopItem item)
        {
            string[] items= Enum.GetNames(typeof(ShopItem));
            foreach (string s in items)
            {
                if (s.Equals(item.ToString()))
                {
                    Panel.Find(s).localPosition = new Vector3(0, -18.5f, 0);
                    Items.Find(s).gameObject.GetComponent<Image>().sprite = ItemActive;
                }
                else
                {
                    Panel.Find(s).localPosition = new Vector3(0, -1200, 0);
                    Items.Find(s).gameObject.GetComponent<Image>().sprite = ItemDeActive;
                }
            }

//            if (item==ShopItem.Packet)
//            {
//                topGold.localPosition=new Vector3(0,600,0);
//                topProps.localPosition = new Vector3(0, 415, 0);
//            }
//            else
//            {
//                topProps.localPosition = new Vector3(0, 600, 0);
//                topGold.localPosition = new Vector3(0, 415, 0);
//            }
            UpdatePromoteUI();
        }

        public void OnPacketItemClick(Shop itemShop)
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("shop", itemShop.product_code);
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            curShop = itemShop;
            inappSup.PurchaseProduct(itemShop.product_code, "shop");
        }

        public void OnGoldItemClick(Shop itemShop)
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("shop", itemShop.product_code);
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);
            curShop = itemShop;
            inappSup.PurchaseProduct(itemShop.product_code, "shop");
        }

        public void OnHeartItemClick(BuyItem itemShop)
        {

            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            if (playerInfo.Gold < itemShop.base_gold)
            {
                Debug.Log("金币不足");
                UpdateUi(ShopItem.Gold);
                return;
            }
            if (itemShop.infinite_life==1)
            {
                Dictionary<string, object> desc = new Dictionary<string, object>();
                desc.Add("shop", "infinite_life_" + itemShop.infinite_time);
                AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

                if ("0".Equals(playerInfo.Infinite_time))
                {
                    playerInfo.Infinite_time = itemShop.infinite_time * 24 * 60 * 60 + double.Parse(GetTimeStamp()) + "";
                }
                else
                {
                    double oldTime;
                    double.TryParse(playerInfo.Infinite_time, out oldTime);
                    playerInfo.Infinite_time = itemShop.infinite_time * 24 * 60 * 60 + oldTime + "";
                }
            }
            else
            {
                UpdateInfo(itemShop, playerInfo);
            }
            PlayerInfoUtil.SetConsumeGold((int) (itemShop.base_gold * itemShop.discount));
            SetReflectValue(playerInfo, "Gold", (int)(-itemShop.base_gold * itemShop.discount));
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            UpdateTopUi();
        }

        void PurchaseListner.PurchaseResult(bool success, string productId)
        {
            if (success)
            {
                if (productId.Contains("bag"))
                {
                    SucPacket();
                }

                if (productId.Contains("coin"))
                {
                    SucGold();
                }
            }
        }

        private void SucPacket()
        {
            GamePlusLog.Instance.ChargeLog(curShop.usd);

            PlayerInfoUtil.ShowDialog(DialogItem.Purchase);
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            Debug.Log("OnPurchaseClick:" + curShop.product_code);
            for (int i = 1; i < curShop.size + 1; i++)
            {
                string itemKey = "item" + i;
                string countKey = "count" + i;
                int itemCode = (int)GetReflectValue(curShop, itemKey);
                int count = (int)GetReflectValue(curShop, countKey);
                string itemName = propItems.First(x => x.id == itemCode).name;
                SetReflectValue(playerInfo, itemName, count);
            }
            if ("0".Equals(playerInfo.Infinite_time))
            {
                double time = curShop.infinite_time*24*60*60 + double.Parse(GetTimeStamp());
                playerInfo.Infinite_time = time + "";
            }
            else
            {
                double oldTime;
                double.TryParse(playerInfo.Infinite_time, out oldTime);
                double time = curShop.infinite_time*24*60*60 + oldTime;
                playerInfo.Infinite_time = time + "";
            }
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            UpdateTopUi();
        }

        private void SucGold()
        {
            GamePlusLog.Instance.ChargeLog(curShop.usd);
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            float coinActualAdd = curShop.count1 * (1 + curShop.benefit);
            _preGold = playerInfo.Gold;
            playerInfo.Gold += Mathf.RoundToInt(coinActualAdd);
            _endGold = playerInfo.Gold;
            Sprite imageSprite = Resources.Load<Sprite>(IMG_PATH + "det_gold_" + curShop.id);
            ImageGold.gameObject.GetComponent<Image>().sprite = imageSprite;
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            PlayAnim(ImgPos[curShop.product_code]);
        }

        private  void UpdateInfo(BuyItem itemShop, PlayerInfo playerInfo)
        {
            if (itemShop.item == (int)ShopItemCode.Life)
            {
                Dictionary<string, object> desc = new Dictionary<string, object>();
                desc.Add("shop", "full set of lives");
                AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

                if (playerInfo.Life == playerInfo.MaxLife)
                {
                    Debug.Log("已经是最大体力,不消耗金币");
                    return;
                }
                playerInfo.MaxLifeTime = "0";
                //静态变量手动重置倒计时
                PlayerInfoUtil.Countdown = 0;
                playerInfo.Life = playerInfo.MaxLife;
                SetReflectValue(playerInfo, "Gold", (int)(-itemShop.base_gold * itemShop.discount));
            }
            if (itemShop.item == (int)ShopItemCode.MaxLife)
            {
                Dictionary<string, object> desc = new Dictionary<string, object>();
                desc.Add("shop", "add max lives");
                AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

                playerInfo.Life += itemShop.count;
                playerInfo.MaxLife += itemShop.count;
                UpdateMaxPrice(itemShop,false);
                int goldSpend = itemShop.base_gold + itemShop.step_gold*LocalDynamicData.GetInstance().maxlifeBuyTime;
                SetReflectValue(playerInfo, "Gold", goldSpend);
            }

        }

        void PlayAnim(float imgPos)
        {
            ImageGold.localPosition = new Vector3(-175, imgPos + (GoldItemHight + DivideHeight)*2, 0);
            Sequence mySequence = DOTween.Sequence();
            float centerY = 544 - (560-Math.Abs(_goldConY));
            mySequence.Append(ImageGold.DOLocalMove(new Vector3(0, centerY, 0), 0.4f));
            mySequence.Insert(0,ImageGold.DOScale(new Vector3(2f, 2f, 2f), 0.4f));
            mySequence.OnComplete(EndGoldAnim);
        }

        void EndGoldAnim()
        {
            Outside_UI_coins.SetActive(true);
            Invoke("ResetGoldImage",1f);
            Invoke("ResetAnim", 2f);
            Invoke("AddGold", 1.35f);
        }

        void ResetGoldImage()
        {
            ImageGold.DOLocalMove(new Vector3(0, -1000, 0), 0f);
        }

        void ResetAnim()
        {
            Outside_UI_coins.SetActive(false);
            PlayerInfoUtil.ShowDialog(DialogItem.Purchase);
        }

        void AddGold()
        {
            AudioSourcesManager.GetInstance().Play(audio,CoinClip);
             DOTween.To(() => _preGold, x => _goldText.text=x+"", _endGold, 0.7f);
        }

        public void onItemClick(string item)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(Items.Find(item).DOScale(1.1f,0.1f));
            mySequence.Append(Items.Find(item).DOScale(1f,0.1f));
            CurItem = (ShopItem) Enum.Parse(typeof(ShopItem), item);
            UpdateUi(CurItem);
        }

        public void OnFBClick()
        {
            shopFBClicked = true;
            Debug.Log("OnFBClick");
            if (FB.IsLoggedIn)
            {
                fbSup.invite();
            }
            else
            {
                SettingController.FBClicked = true;
               LoginController.Instance.BindFacebook();
            }
        }

        void fbloged(string arg)
        {
            if (shopFBClicked&&"shop".Equals(arg))
            {
                hide();
                shopFBClicked = false;
            }
        }

        public void OnFreeLife()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("type", "OnFreeLife");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            if (playerInfo.Life==playerInfo.MaxLife)
            {
                Debug.Log("体力最大无法观看广告");
                 return;   
            }
            if (AdCori)
            {
                Debug.Log("已经在倒计时");
                return;
            }
//            updateAdsTime();
            Debug.Log("OnFreeLife");
            gpVideoAd.ShowAd();
            AdloadUtils.Instance.Show();
            gpVideoAd.SetCallback(AdCallbackhanler);
        }

        void AdCallbackhanler(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("Ad Finished. Rewarding player...");
                    PlayerInfoUtil.AddLife(1);
                    PlayerPrefs.SetString(ShopAdsDown, PlayerInfoUtil.GetTimeStamp());
                    updateAdsTime();
                    AnalysisSup.LogAd(true);
                    break;
                default:
                    Debug.Log(result.ToString());
                    AnalysisSup.LogAd(false, result.ToString());
                    break;
            }
            AdloadUtils.Instance.hide();
        }

        public void show(ShopItem CurItem)
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("shop", CurItem.ToString());
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);
//            CurItem = (ShopItem)Enum.Parse(typeof(ShopItem), item);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PlayerInfoUtil.ShowDialog(DialogItem.Network);
                Debug.Log("无网络连接");
                return;
            }
            
            openTime = PlayerInfoUtil.GetTStamp();
            UpdateFBStatus();
            updateAdsTime();
			Parent.gameObject.SetActive (true);
            UpdateUi(CurItem);
            UpdateTopUi();
            UpdateAdBtn();
            Shop.localPosition = new Vector3(0, 0, 0);
        }

        void UpdateAdBtn()
        {
            PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
            if (playerInfo.Life == playerInfo.MaxLife)
            {
                maskAdBtn.SetActive(true);
            }
            else
            {
                maskAdBtn.SetActive(false);
            }
        }

        private bool AdCountStart = false;
        public void updateAdsTime()
        {
            string oldTimeStr = PlayerPrefs.GetString(ShopAdsDown, "");
            string curTimeStr = PlayerInfoUtil.GetTimeStamp();
            long oldTime;
            long curTime;
            long.TryParse(oldTimeStr, out oldTime);
            long.TryParse(curTimeStr, out curTime);
            AdCowntDown = AdDownTime * 60 - (curTime - oldTime);
            long subTime = curTime - oldTime;
            if (subTime < AdDownTime * 60 && !AdCori)
            {
                StartCoroutine(UpdateAdsTime());
            }
            else
            {
                adText.text = "Free";
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) return;
            updateAdsTime();
        }

        public IEnumerator UpdateAdsTime()
        {
            while (AdCowntDown>=0)
            {
                AdCori = true;
                int minutes = (int)(AdCowntDown % 3600 / 60);
                int seconds = (int)(AdCowntDown % 3600 % 60);
                adText.text = PadZero(minutes) + ":" + PadZero(seconds);
                AdCowntDown--;
                yield return new WaitForSeconds(1);
            }
            adText.text = "Free";
            AdCori = false;
        }

        string PadZero(int value)
        {
            return value.ToString().PadLeft(2, '0');
        }

        void UpdateFBStatus()
        {
//            if (FB.IsLoggedIn)
//            {
//               
//                FbText.text = "Invite";
//            }
//            else
//            {
//                FbText.text = "Login";
//            }
        }

        public void hide()
        {
            Dictionary<string, object> desc2 = new Dictionary<string, object>();
            desc2.Add("shop", SecitonUtils.CountSection(3,(int) (PlayerInfoUtil.GetTStamp()-openTime)));
            AnalysisSup.fabricLog(EventName.UI_SHOW_TIME, desc2);

            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("shop", "shop_close");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            string name = SceneManager.GetActiveScene().name;
            if ("Main".Equals(name))
            {
                GameObject.Find("CanvasFront").GetComponent<MainFrontController>().StartTimeUpdate();

                Camera.main.GetComponent<DragCamera>().enabled = true;
            }
            Shop.localPosition = new Vector3(6000, 0, 0);
			Parent.gameObject.SetActive (false);
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private static object GetReflectValue(object obj, string itemKey)
        {
            PropertyInfo prop = obj.GetType().GetProperty(itemKey, BindingFlags.Public | BindingFlags.Instance);
            return prop.GetValue(obj, null);
        }

        private static void SetReflectValue(object obj, string itemKey,int num)
        {
            PropertyInfo prop = obj.GetType().GetProperty(itemKey, BindingFlags.Public | BindingFlags.Instance);
            int prop_num = (int)prop.GetValue(obj, null);
            prop.SetValue(obj, prop_num+num,null);
        }
    }
}
