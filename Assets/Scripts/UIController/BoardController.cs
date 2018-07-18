
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.manager.bean;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.Data.BeanMap;
using Assets.Scripts.FirebaseController;
using Fabric.Internal.ThirdParty.MiniJSON;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UIController
{
    public class BoardController : MonoBehaviour
    {
        public Transform PanelTransform;
        public Transform ParenTransform;
        public Text FbText;
        private int DivideWidth = 2;
        private int ItemWidth = 228;
        private ImageUtil imageUtil;
        public const string BOT_IMG_PATH = "Texture/bot/";
        private FacebookSup fbSup;
        private string BotData = "{\"data\":[{\"name\":\"Jim\",\"score\":500},{\"name\":\"Marry\",\"score\":600},{\"name\":\"Jerry\",\"score\":700},{\"name\":\"Sandy\",\"score\":800},{\"name\":\"Monica\",\"score\":900},{\"name\":\"Jack\",\"score\":1000}]}";
        private string BotTimeData = "{\"data\":[{\"name\":\"Jim\",\"score\":70},{\"name\":\"Marry\",\"score\":189},{\"name\":\"Jerry\",\"score\":370},{\"name\":\"Sandy\",\"score\":535},{\"name\":\"Monica\",\"score\":731},{\"name\":\"Jack\",\"score\":911}]}";
        void Awake()
        {
            imageUtil = GameObject.Find("GamePlus").GetComponent<ImageUtil>();
            fbSup = GameObject.Find("GamePlus").GetComponent<FacebookSup>();
        }

        void Start()
        {
            RectTransform transform =  PanelTransform as RectTransform;
            transform.sizeDelta=new Vector2(1368,200);
            PanelTransform.localPosition=new Vector3(394,-15,0);
        }

        void Update()
        {
            if (FB.IsLoggedIn)
            {
                FbText.text = "INVITE";
            }
            else
            {
                FbText.text = "LOGIN";
            }
        }

        public void Visible(int NewScore,int lv)
        {
            string level_str = Regex.Replace(SceneManager.GetActiveScene().name, @"[^0-9]+", "");
            int level;
            int.TryParse(level_str,out level);
            if (lv!=0)
            {
                level = lv;
            }
            if (FB.IsLoggedIn && NewScore==0)
            {
                SdkController.Instance.GetLevelSocres(level);
            }
           
            var obj = getCurScore(NewScore, level);

            if (FB.IsLoggedIn)
            {
                FbText.text = "INVITE";
                FilterScore(level, obj);
            }
            else
            {
                FbText.text = "LOGIN";
            }
            GameSparksManager_BoardCallback(obj, level);
        }

        private static void FilterScore(int level, List<BoardData> obj)
        {
            IEnumerable<LeaderBoard> cacheDatas = DynamicDataBaseService.GetInstance().GetBoardInfo().ToList();
            if (cacheDatas.Any())
            {
                var firstOrDefault = cacheDatas.FirstOrDefault(x => x.id == level);
                if (firstOrDefault == null || "".Equals(firstOrDefault.boardInfo))
                {
                    return;
                }
                List<BoardData> cache =
                    JsonUtility.FromJson<Serialization<BoardData>>(firstOrDefault.boardInfo).ToList();
                obj.AddRange(cache);
            }
        }

        private  List<BoardData> getCurScore(int NewScore, int level)
        {
            List<BoardData> obj = new List<BoardData>();
            Highscore score = DynamicDataBaseService.GetInstance().GetHighscores().FirstOrDefault(x => x.id == level);
            if (score != null && score.highscore!=0)
            {
                obj.Add(new BoardData("", score.highscore, 1, "You", LoginManager.UserId));
            }
            return obj;
        }

        private void GameSparksManager_BoardCallback(List<BoardData> obj,int level)
        {
            if (obj == null)
            {
                Debug.Log("no data");
            }
            else
            {
                Stage stage = StaticData.GetInstance().GetStageByID(level);
                STAGE_TYPE stage_type = (STAGE_TYPE)stage.type;
                Debug.Log(stage_type.ToString() + " " + level);
                //机器人填充
                var botNum = Constance.BOARD_NUM - obj.Count;
                Dictionary<string, object> botsData = null;
                if (IsTimer(stage_type))
                {
                    botsData = (Dictionary<string, object>)Json.Deserialize(BotTimeData);
                }
                else
                {
                    botsData = (Dictionary<string, object>)Json.Deserialize(BotData);
                }
                var bots= (List<object>)botsData["data"];
                for (var i = 0; i < botNum; i++)
                {
                    var bot = (Dictionary<string, object>)bots[i];
                    var botBoardData = new BoardData
                    {
                        Fbid = "",
                        Name = (string) bot["name"],
                        Socre = (long) bot["score"],
                        gsid =""
                    };
                    obj.Add(botBoardData);
                }
                SetUI(obj, stage_type);
            }

        }

        private void SetUI(List<BoardData> obj, STAGE_TYPE stage_type)
        {
            var prefab = Resources.Load("prefabs/UserBorder0") as GameObject;
            var j = 0;

            RerangeScore(obj, stage_type);
            foreach (var boardData in obj)
            {
                var BoardItem = Instantiate(prefab);
                BoardItem.name = "BoardItem" + j;
                BoardItem.transform.SetParent(PanelTransform);
                BoardItem.transform.localPosition = new Vector3(-573 + (ItemWidth + DivideWidth)*j, 0, 0);
                BoardItem.transform.localScale = new Vector3(1, 1, 1);
                Text rankText = BoardItem.transform.Find("Text_Rank").GetComponent<Text>();
                SetScore(BoardItem, stage_type, boardData);
                var headImage = BoardItem.transform.Find("Image_Pic").GetComponent<Image>();
                //名次
                rankText.text = j + 1 + "";
                SetHeader(boardData, BoardItem, headImage);
                j++;
            }
        }

        //重新排序
        private static void RerangeScore(List<BoardData> obj, STAGE_TYPE stage_type)
        {
            if (IsTimer(stage_type))
            {
                obj.Sort((x, y) => x.Socre.CompareTo(y.Socre));
            }
            else
            {
                obj.Sort((x, y) => y.Socre.CompareTo(x.Socre));
            }
        }

        private static bool IsTimer(STAGE_TYPE stage_type)
        {
            return stage_type == STAGE_TYPE.KILL_CHOCOLATE_TIMER ||
                   stage_type == STAGE_TYPE.COLLECT_KILL_ALL_TIMER ||
                   stage_type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE_TIMER ||
                   stage_type == STAGE_TYPE.KILL_ALL_TIMER;
        }

        private void SetScore(GameObject BoardItem, STAGE_TYPE stage_type, BoardData boardData)
        {
            Text textScore = BoardItem.transform.Find("Text_Score").GetComponent<Text>();
            if (IsTimer(stage_type))
            {
                int minus = (int) (boardData.socre/60);
                int second = (int) (boardData.Socre%60);
                textScore.text = PadZero(minus) + ":" + PadZero(second);
            }
            else
            {
                textScore.text = boardData.Socre + "";
            }
        }

        //头像
        private void SetHeader(BoardData boardData, GameObject BoardItem, Image headImage)
        {
            if (!string.IsNullOrEmpty(boardData.gsid) && FB.IsLoggedIn)
            {
                FriendData data =
                    DynamicDataBaseService.GetInstance()
                        .GetFriendData()
                        .FirstOrDefault(x => x.gs_id == boardData.gsid);
                if (data != null)
                {
                    BoardItem.transform.Find("Text_Name").GetComponent<Text>().text = data.name + "";
                    imageUtil.SetHeadImage(data.fb_id, headImage);
                }
            }
            else
            {
                BoardItem.transform.Find("Text_Name").GetComponent<Text>().text = boardData.Name + "";
                headImage.sprite = Resources.Load<Sprite>(BOT_IMG_PATH + boardData.Name);
            }
        }

        public void OnFBClick()
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("board", "OnFBClick");
            AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

            if (FB.IsLoggedIn)
            {
                fbSup.invite();
                
            }
            else
            {
                LoginController.Instance.BindFacebook();
            }
        }

        void OnEnable()
        {
            PlayBoardController.SucAction += Visible;
            FrontController.boardAction += Visible;
        }

        void OnDisable()
        {
            PlayBoardController.SucAction -= Visible;
            FrontController.boardAction -= Visible;
        }

        string PadZero(int value)
        {
            return value.ToString().PadLeft(2, '0');
        }
    }
}

