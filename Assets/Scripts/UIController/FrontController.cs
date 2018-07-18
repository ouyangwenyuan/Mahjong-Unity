using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using System.Reflection;
using Assets.GamePlus.advertise;
using Assets.GamePlus.ironsrc;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.FirebaseController;
using Assets.Scripts.UIController;
using Assets.Scripts.Utils;
using Spine.Unity;
using Spine;
using Random = System.Random;

public class FrontController : MonoBehaviour
{
    //战斗UI各子界面
    public GameObject Pause;        //暂停界面         
    public GameObject Success;      //成功界面
    public GameObject Fail;         //失败界面

    //暂停界面内容
    public Text pause_text_id;
    public Text pause_text_card_num;
    public Text pause_text_timer;
    public Text pause_text_tips;

    //失败界面内容
    public GameObject Fail_Chance;
    //礼物赠送
    public GameObject DialogNotify;

    public GameObject Button_Fail_Refresh;
    public GameObject Button_Fail_Bomb;
    public GameObject Button_Fail_Add_Time;
    public GameObject Button_Fail_Collect_All_Jewels;
    public GameObject Button_Fail_TenGold;
    public Image Image_Fail_TenGold;
    public Text Text_Fail_TenGold;

    public GameObject Fail_Fail;
    public Text fail_chance_text_card_num;
    public Text fail_fail_text_card_num;
    public Text fail_level_id;
    //panael
    public GameObject suc_panel;
    public GameObject fail_panel;
    //free ad
    public GameObject Button_Free;
    //遮罩
    public GameObject mask;
    //退出界面
    public GameObject Quit;

    //成功界面内容
    public GameObject[] success_stars;

    public GameObject go_game_stage;    //游戏逻辑控制脚本
    private GameStage game_stage;
    //顶部信息显示UI
    public Text text_level;
    public GameObject Card_Num;
    public GameObject Key_Num;
    public Text text_card_num;
    public Text text_key_num;
    public Text text_target;
    public GameObject[] diamonds;
    public GameObject[] go_top_stars;
    public GameObject[] top_stars;
    public Image[] top_stars_up;
    public GameObject fill_effect;
    public Image image_fill;
    public GameObject Score;
    public GameObject Timer;
    public Text score_text_score;
    public Text text_hit;
    public Text text_timer;
    public Image image_hit;

    //底部技能UI
    public Text text_refresh_num;
    public Text text_bomb_num;
    public Text text_add_time_num;
    public Text text_unlock_num;
    public Text text_get_diamonds_num;
    //胜利界面
    public Text success_text_level;
    public Text success_level_type;
    public Text success_text_score;

    public Button button_pause;
    public Button button_refresh;
    public Button button_bomb;
    public Button button_add_timer;
    public Button button_jewels;
    public Button button_unlock;

    //sdk
    private GPVideoAd gpVideoAd;
    //广告
    public const int AD_PLAY_OFFSET = 10;
    public const string AD_PLAY_COUNT = "AD_PLAY_COUNT";
    public const string DAY_FIRST_PLAY = "DAY_FIRST_PLAY";
    private int TIME_AD_REFRESH = 60 * 60 * 24;
    //free ad offset
    public const string REMOTE_ADD_OFFSET = "REMOTE_ADD_OFFSET";
    public const string REMOTE_ADD_REFRESH = "REMOTE_ADD_REFRESH";

    public Image redraw_unopen;
    public Image bomb_unopen;
    public Image add_time_unopen;
    public Image add_time_lock;
    public Image jewels_unopen;
    public Image jewels_lock;

    public Image redraw;
    public Image bomb;
    public Image add_time;
    public Image jewels;
    public Image locks;

    public GameObject redraw_count;
    public GameObject bomb_count;
    public GameObject add_time_count;
    public GameObject jewels_count;
    public GameObject locks_count;

    public AudioSource level_audio;
    public AudioSource audio_card_click;
    public AudioSource audio_star;
    public AudioSource audio_combo;
    public AudioSource audio_jewel_fly;

    public AudioClipSet audioclip_set;

    public GameObject guide_root;

    public GameObject Success_Anim1;
    public GameObject Success_Anim2;

    public GameObject fx_win;

    public Transform fx_root;

    bool no_pause_ui = false;

    //新手奖励
    private Dictionary<int, int> bonusInts = new Dictionary<int, int>
	{
		{3, 301},
		{5, 302},
		{7, 303},
		{19, 304}
	};
    //道具使用次数
    private Dictionary<string, int> propUseNum = new Dictionary<string, int>();
    //道具是否打开
    private Dictionary<ShopItemCode, bool> propAvailable = new Dictionary<ShopItemCode, bool>();
    private IEnumerable<BuyItem> payments;
    private IEnumerable<Tips> _tipses;
    private int _chanceGoldSpend;

    public void SetButtonsStatus(bool b)
    {

        button_pause.enabled = b;

        if (redraw_open) button_refresh.enabled = b;
        if (bomb_open) button_bomb.enabled = b;
        if (add_timer_open) button_add_timer.enabled = b;
        if (jewels_open) button_jewels.enabled = b;
        //button_unlock.enabled = b;
    }

    Stage stage;
    ControlBackgroundSound cbs;

    List<GameObject> go_jewels = new List<GameObject>();
    //排行榜显示隐藏回调
    public static event Action<int,int> boardAction;

    public GameObject GetJewelPos(int i)
    {
        return go_jewels[i];
    }
    private float missDis;
    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        GameObject go_audioclip_set = GameObject.Find("AudioClipSet");
        if (go_audioclip_set != null) { audioclip_set = go_audioclip_set.GetComponent<AudioClipSet>(); }

        CommonData.is_success_level = false;

        if (go_game_stage == null)
        {
            go_game_stage = GameObject.Find("CanvasMid/GameStage");
        }

        game_stage = go_game_stage.GetComponent<GameStage>();

        GameObject music = Instantiate(Resources.Load("Prefabs/Music")) as GameObject;

        cbs = music.GetComponent<ControlBackgroundSound>();


    }

    void InitUI()
    {
        if (Splash.IsIpad)
        {
            RectTransform rect = GameObject.Find("CanvasBack/Background").GetComponent<RectTransform>();
            //rect.sizeDelta = new Vector2(rect.sizeDelta.x*1.3f, rect.sizeDelta.y*1.3f);
            rect.localScale = new Vector3(rect.localScale.x * 1.3f, rect.localScale.y * 1.3f, rect.localScale.z);
        }

        setMask(false);
        InitTopUI((STAGE_TYPE)stage.type);

        AdapterScreen();
        InitSkillsUI();
        InitPos();
        initGlobalCamara();
        InitBackground();

        if (!game_stage.timer_level) button_add_timer.enabled = false;
        if (!game_stage.jewels_level) button_jewels.enabled = false;
        if (!game_stage.locks_level) button_unlock.enabled = false;

        //pause_text_id.text = stage.id.ToString();
        //pause_text_card_num.text = stage.mahjong_cnt.ToString();
        //pause_text_timer.text = stage.count_down.ToString();
    }

    void AdapterScreen()
    {
        List<string> obList = new List<string>() { "CanvasBack", "CanvasMid", "CanvasFront"};
        foreach (var obj in obList)
        {
            CanvasScaler CanvasBack = GameObject.Find(obj).GetComponent<CanvasScaler>();
            CanvasBack.matchWidthOrHeight = 1f;
        }
        
    }

    void initGlobalCamara()
    {
        
        GameObject obj = GameObject.Find("GobalUI");
        _tipses = StaticDataBaseService.GetInstance().GetTips();
        if (LocalDynamicData.GetInstance().guide_step1 != 0 && obj != null)
        {
            Canvas canvas = obj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
        }
    }

    void InitBackground()
    {
        GameObject canvas = GameObject.Find("CanvasBack");

        canvas.GetComponent<Canvas>().sortingOrder = -1;

        GameObject background = GameObject.Find("CanvasBack/Background");

        Image b_image = background.GetComponent<Image>();

        if (b_image == null)
        {
            b_image = background.AddComponent<Image>();
        }

        b_image.sprite = Resources.Load<Sprite>(game_stage.stage.background);

        RectTransform rect = background.GetComponent<RectTransform>();

        rect.localPosition = new Vector3(0, 0, 0);
        rect.sizeDelta = new Vector2(CommonData.BASE_WIDTH, CommonData.BASE_HEIGHT);
    }

    void InitPos()
    {
        missDis = Screen.width*2;
        
        Success.transform.localPosition = new Vector3(missDis, -65, 0);
        Fail_Chance.transform.localPosition = new Vector3(missDis, -65, 0);
        Fail_Fail.transform.localPosition = new Vector3(missDis, -65, 0);
        Pause.transform.localPosition = new Vector3(missDis, 0, 0);
        DialogNotify.transform.localPosition = new Vector3(missDis, 0, 0);
    }

    void Start()
    {
        gpVideoAd = GameObject.Find("GamePlus").GetComponent<GPVideoAd>();
        stage = game_stage.stage;

        if (stage == null)
        {
            Debug.LogError("Not Level Data!!! ID : " + CommonData.current_level.ToString());

            return;
        }
        initAnim();
        InitUI();
        InitSkillItemUI();
        InitPropUseNum();
    }

    bool redraw_open = false;
    bool bomb_open = false;
    bool add_timer_open = false;
    bool jewels_open = false;

    void InitPropUseNum()
    {
        payments = StaticDataBaseService.GetInstance().GetBuyItem();
        string[] items = Enum.GetNames(typeof(PropsItem));
        foreach (string s in items)
        {
            propUseNum.Add(s, 0);
        }
    }

    void InitSkillItemUI()
    {
        int level_done_num = DynamicData.GetInstance().GetStagesDoneNum();
        if (level_done_num >= 3)
        {
            redraw_open = true;
            button_refresh.enabled = true;
            redraw_unopen.enabled = false;
            propAvailable.Add(ShopItemCode.Redraw, true);
        }
        else if (CommonData.current_level == 3 && LocalDynamicData.GetInstance().guide_step3 == 0)
        {
            redraw_count.SetActive(false);
            redraw.enabled = false;
            button_refresh.enabled = false;
            redraw_unopen.enabled = true;
            propAvailable.Add(ShopItemCode.Redraw, false);
        }
        else
        {
            redraw_count.SetActive(false);
            redraw.enabled = false;
            button_refresh.enabled = false;
            redraw_unopen.enabled = true;
            propAvailable.Add(ShopItemCode.Redraw, false);
        }
        if (level_done_num >= 5)
        {
            bomb_open = true;
            button_bomb.enabled = true;
            bomb_unopen.enabled = false;
            propAvailable.Add(ShopItemCode.Bomb, true);
        }
        else if (CommonData.current_level == 5 && LocalDynamicData.GetInstance().guide_step5 == 0)
        {
            bomb_count.SetActive(false);
            bomb.enabled = false;
            button_bomb.enabled = false;
            bomb_unopen.enabled = true;
            propAvailable.Add(ShopItemCode.Bomb, false);
        }
        else
        {
            bomb_count.SetActive(false);
            bomb.enabled = false;
            button_bomb.enabled = false;
            bomb_unopen.enabled = true;
            propAvailable.Add(ShopItemCode.Bomb, false);
        }
        if (level_done_num + 1 >= 7)
        {
            add_time_unopen.enabled = false;
            if (game_stage.timer_level)
            {
                button_add_timer.enabled = true;
                add_timer_open = true;
                add_time_lock.enabled = false;
                propAvailable.Add(ShopItemCode.Clock, true);
            }
            else
            {
                add_time_count.SetActive(false);
                button_add_timer.enabled = false;
                add_time.enabled = false;
                add_time_lock.enabled = true;
                propAvailable.Add(ShopItemCode.Clock, false);
            }
        }
        else
        {
            add_time_count.SetActive(false);
            add_time.enabled = false;
            button_add_timer.enabled = false;
            add_time_unopen.enabled = true;
            add_time_lock.enabled = false;
            propAvailable.Add(ShopItemCode.Clock, false);
        }
        if (level_done_num + 1 >= 19)
        {
            jewels_unopen.enabled = false;
            if (game_stage.jewels_level)
            {
                button_jewels.enabled = true;
                jewels_open = true;
                jewels_lock.enabled = false;
                propAvailable.Add(ShopItemCode.Jewel, true);
            }
            else
            {
                jewels_count.SetActive(false);
                jewels.enabled = false;
                button_jewels.enabled = false;
                jewels_lock.enabled = true;
                propAvailable.Add(ShopItemCode.Jewel, false);
            }
        }
        else
        {
            jewels.enabled = false;
            jewels_count.SetActive(false);
            button_jewels.enabled = false;
            jewels_unopen.enabled = true;
            jewels_lock.enabled = false;
            propAvailable.Add(ShopItemCode.Jewel, false);
        }
    }

    public void OpenRedrawButton()
    {
        redraw_open = true;

        button_refresh.enabled = true;
        redraw_unopen.enabled = false;
        propAvailable[ShopItemCode.Redraw] = true;

        redraw_count.SetActive(true);

        redraw.enabled = true;

        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        playerInfo.Redraw++;

        text_refresh_num.text = playerInfo.Redraw.ToString();
    }

    public void OpenBombButton()
    {
        bomb_open = true;


        button_bomb.enabled = true;
        bomb_unopen.enabled = false;
        propAvailable[ShopItemCode.Bomb] = true;

        bomb_count.SetActive(true);

        bomb.enabled = true;

        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        playerInfo.Bomb++;

        text_bomb_num.text = playerInfo.Bomb.ToString();
    }


    void initAnim()
    {
        /*
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            var button1 = button;
            button.onClick.AddListener(delegate
            {
                Vector3 preScale = button1.transform.localScale;
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(button1.transform.DOScale(new Vector3(preScale.x + 0.1f, preScale.y + 0.1f, preScale.z), 0.1f));
                mySequence.Append(button1.transform.DOScale(preScale, 0.1f));
            });
        }*/
    }

    void OnApplicationPause(bool isPause)
    {
        if (CommonData.b_init && !GuideManager.is_guide_now && !no_pause_ui)
        {
            if (isPause)
            {
                //缩到桌面的时候触发
            }
            else
            {
                //回到游戏的时候触发
                // OnPauseClick();
            }
        }
    }

    SkeletonAnimation anim;
    int star_num = 0;

    private int startNum;
    public void EndLevel(int _star_num)
    {
        no_pause_ui = true;

        startNum = _star_num;
        Debug.Log(CommonData.current_level + " CommonData.current_level");
        if (bonusInts.ContainsKey(CommonData.current_level))
        {
            //显示
            string itemName =
                StaticDataBaseService.GetInstance()
                    .GetItem()
                    .First(x => x.id == bonusInts[CommonData.current_level])
                    .name;
            if (PlayerPrefs.GetInt(itemName + "_tutoril", 0) == 0)
            {
                showDialog(itemName);
                PlayerPrefs.SetInt(itemName + "_tutoril", 1);
                //奖励
                PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
                SetReflectValue(playerInfo, itemName, 1);
                DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
                InitSkillsUI();
            }
            else
            {
                playAnim(_star_num);
            }

        }
        else
        {
            playAnim(_star_num);
        }
    }

    public void showDialog(string title)
    {
        DialogNotify.transform.Find("Gift").Find("desc").GetComponent<Text>().text = "You've got one " + title;
        Sprite icon = Resources.Load<Sprite>(ShopController.IMG_PATH + "det_" + title.ToLower());
        DialogNotify.transform
            .Find("Gift")
            .Find("Booster")
            .GetComponent<Image>()
            .sprite = icon;
        setMask(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(DialogNotify.transform.DOLocalMoveX(-30, 0.5f));
        mySequence.Append(DialogNotify.transform.DOLocalMoveX(0, 0.2f));
    }

    public void hideDialog()
    {
        setMask(false);
        DialogNotify.transform.DOLocalMoveX(-missDis, 0.5f)
            .SetEase(Ease.InQuint)
            .SetAutoKill(true)
            .OnComplete(OnDialogClose)
            .SetUpdate(true);
    }

    void OnDialogClose()
    {
        playAnim(startNum);
    }

    private static void SetReflectValue(object obj, string itemKey, int num)
    {
        PropertyInfo prop = obj.GetType().GetProperty(itemKey, BindingFlags.Public | BindingFlags.Instance);
        int prop_num = (int)prop.GetValue(obj, null);
        prop.SetValue(obj, prop_num + num, null);
    }

    private void playAnim(int _star_num)
    {
        star_num = _star_num;
        if (star_num == 3)
        {
            //Invoke("SuccessStar3" , 2.5f);

            SuccessStar3();
        }
        else
        {
            //Invoke("SuccessStar", 2.5f);

            SuccessStar();
        }

        cbs.Stop();
        AudioSourcesManager.GetInstance().Play(level_audio, (audioclip_set == null) ? null : audioclip_set.win);
    }

    void SuccessStar3()
    {
        Success_Anim2.SetActive(true);
        anim = Success_Anim2.GetComponent<SkeletonAnimation>();

        fx_win.SetActive(true);

        anim.state.Complete += ShowYouWin;
    }

    void SuccessStar()
    {

        Success_Anim1.SetActive(true);
        anim = Success_Anim1.GetComponent<SkeletonAnimation>();

        fx_win.SetActive(true);

        anim.state.Complete += ShowYouWin;
    }

    void ShowYouWin(TrackEntry trackEntry)
    {
        SdkController.Instance.UpdateUserInfo();
        DataSynconizer.Instance.UploadConfig();
        SdkController.Instance.ReportScore(CommonData.current_level);
        UpdateHighScore();
        LocalDynamicData.GetInstance().SetNewLevel(++CommonData.current_level);
        
        if (boardAction != null)
        {
            boardAction(game_stage.score, CommonData.current_level-1);
        }

        CommonData.is_success_level = true;

        int stars_count = 0;


        game_stage.hint_pause = true;
        anim.state.Complete -= ShowYouWin;

        //fx_win.SetActive(true);

        Success_Anim1.SetActive(false);
        Success_Anim2.SetActive(false);

        //Invoke("ShowSuccessUI", 0.3f);
        ShowSuccessUI();
    }

    void ShowSuccessUI()
    {
        top_stars[0].SetActive(false);
        top_stars[1].SetActive(false);
        top_stars[2].SetActive(false);

        fill_effect.SetActive(false);

        game_stage.hint_pause = true;

        if (star_num == 1)
        {
            Invoke("PlayStar1SFX", 0.5f);
        }
        else if (star_num == 2)
        {
            Invoke("PlayStar1SFX", 0.5f);
            Invoke("PlayStar2SFX", 1f);
        }
        else if (star_num == 3)
        {
            Invoke("PlayStar1SFX", 0.5f);
            Invoke("PlayStar2SFX", 1f);
            Invoke("PlayStar3SFX", 1.5f);
        }

        int curLevel = CommonData.current_level - 1;
        success_text_level.text = "Level " + curLevel;

        if (game_stage.timer_level)
        {
            success_level_type.text = "TIME";
            success_text_score.text = UIUtil.ShowTimeMinute(game_stage.stage.count_down - (int)game_stage.timer);
        }
        else
        {
            success_text_score.text = game_stage.score.ToString();
        }

        SetButtonsStatus(false);
        setMask(true);
        //弹出动画
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(Success.transform.DOLocalMoveX(-30, 0.5f));
        mySequence.Append(Success.transform.DOLocalMoveX(0, 0.2f));
        Transform rankInfo = Success.transform.Find("Rank_Info");
        mySequence.Append(rankInfo.DOLocalMoveY(-8, 0.2f));
        mySequence.Append(rankInfo.DOPunchPosition(new Vector3(0, -10, 0), 0.5f));
    }

    private static void logResult(int star_num, Highscore highscore)
    {
        //打点
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("star num", star_num + "");
        desc.Add("level retry num", highscore.try_num+"");
        desc.Add("level time spend", SecitonUtils.CountSection(5*60, (int) highscore.time_cost));
        desc.Add("prop refresh spend", highscore.skill_refresh_num + "");
        desc.Add("prop bomb spend", highscore.skill_bomb_num + "");
        desc.Add("prop add time spend", highscore.skill_add_timer_num + "");
        desc.Add("prop jewels spend", highscore.skill_jewels_num + "");
        desc.Add("prop unlock spend", highscore.skill_unlock_num + "");
        AnalysisSup.fabricLog(EventName.GAME_LEVEL + CommonData.current_level, desc);

    }

    void PlayStar1SFX() {
        AudioSourcesManager.GetInstance().Play(audio_star, (audioclip_set == null) ? null : audioclip_set.star1);
        success_stars[0].SetActive(true);
    }

    void PlayStar2SFX()
    {
        AudioSourcesManager.GetInstance().Play(audio_star, (audioclip_set == null) ? null : audioclip_set.star2);
        success_stars[1].SetActive(true);
    }

    void PlayStar3SFX()
    {
        AudioSourcesManager.GetInstance().Play(audio_star, (audioclip_set == null) ? null : audioclip_set.star3);
        success_stars[2].SetActive(true);
    }

    private void UpdateHighScore()
    {
        Highscore scoreOld = DynamicData.GetInstance().GetHighScoreByID(CommonData.current_level);
        PlayerInfo info = DynamicDataBaseService.GetInstance().GetPlayerInfo().FirstOrDefault(x=>x.id==1);
        info.stage = CommonData.current_level;
        DynamicDataBaseService.GetInstance().UpdateData(info);
        GamePlusLog.Instance.PlayTimeLog(scoreOld.time_cost, CommonData.current_level);
        //fabric日志
        logResult(star_num, scoreOld);
    }    

    public void ShowFailUI() {
        no_pause_ui = true;
        if (boardAction != null)
        {
            boardAction(-1, CommonData.current_level);
        }
        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);
        Sprite iconSprite = null;
        int propGoldSpend = 0;
        Debug.Log(game_stage.fail_chance_type.ToString());
        if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.REDRAW)
        {
            iconSprite = SetRedrawBtn(playerInfo, iconSprite, ref propGoldSpend);
        }

        if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.BOMB) 
        {
            if (propAvailable[ShopItemCode.Bomb])
            {
                iconSprite = SetBombBtn(playerInfo, iconSprite, ref propGoldSpend);
            }
            else
            {
                game_stage.fail_chance_type = FAIL_CHANCE_TYPE.REDRAW;
                iconSprite = SetRedrawBtn(playerInfo, iconSprite, ref propGoldSpend);
            }
        }

        if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.ADD_TIME)
        {
            if (playerInfo.Clock > 0)
            {
                Button_Fail_Add_Time.SetActive(true);
                Button_Fail_TenGold.SetActive(false);
            }
            else
            {
                Button_Fail_Refresh.SetActive(false);
                Button_Fail_TenGold.SetActive(true);
                AttachTxtImg(ref iconSprite, ref propGoldSpend, ShopItemCode.Clock);
            }
        }

        if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.COLLECT_JEWELS)
        {
            if (playerInfo.Jewel > 0)
            {
                Button_Fail_Collect_All_Jewels.SetActive(true);
                Button_Fail_TenGold.SetActive(false);
            }
            else
            {
                Button_Fail_Collect_All_Jewels.SetActive(false);
                Button_Fail_TenGold.SetActive(true);
                AttachTxtImg(ref iconSprite, ref propGoldSpend, ShopItemCode.Jewel);
            }
        }
        SetFailTxtImg(iconSprite, propGoldSpend);
        game_stage.hint_pause = true;

        setMask(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(Fail_Chance.transform.DOLocalMoveX(-30, 0.5f));
        mySequence.Append(Fail_Chance.transform.DOLocalMoveX(0, 0.2f));
    }

    private Sprite SetBombBtn(PlayerInfo playerInfo, Sprite iconSprite, ref int propGoldSpend)
    {
        if (playerInfo.Bomb > 0)
        {
            Button_Fail_Bomb.SetActive(true);
            Button_Fail_TenGold.SetActive(false);
        }
        else
        {
            Button_Fail_Bomb.SetActive(false);
            Button_Fail_TenGold.SetActive(true);
            AttachTxtImg(ref iconSprite, ref propGoldSpend, ShopItemCode.Bomb);
        }
        return iconSprite;
    }

    private Sprite SetRedrawBtn(PlayerInfo playerInfo, Sprite iconSprite, ref int propGoldSpend)
    {
        if (playerInfo.Redraw > 0)
        {
            Button_Fail_Refresh.SetActive(true);
            Button_Fail_TenGold.SetActive(false);
        }
        else
        {
            Button_Fail_Refresh.SetActive(false);
            Button_Fail_TenGold.SetActive(true);
            AttachTxtImg(ref iconSprite, ref propGoldSpend, ShopItemCode.Redraw);
        }
        return iconSprite;
    }

    private void SetFailTxtImg(Sprite iconSprite, int propGoldSpend)
    {
        if (iconSprite == null)
        {
            return;
        }
        if (propGoldSpend == 0)
        {
            return;
        }
        Image_Fail_TenGold.sprite = iconSprite;
        Text_Fail_TenGold.text = propGoldSpend + " COINS";
    }

    private void AttachTxtImg(ref Sprite iconSprite, ref int propGoldSpend, ShopItemCode itemCode)
    {
        iconSprite = Resources.Load<Sprite>(CommonData.IMG_PROP_PATH + "det_" + itemCode.ToString().ToLower());
        int id = (int)itemCode;
        BuyItem propBuyItem = payments.First(x => x.item == id);
        _chanceGoldSpend = propBuyItem.base_gold + propBuyItem.step_gold * propUseNum[itemCode.ToString()];
        propGoldSpend = _chanceGoldSpend;
    }


    //Pause UI
    public void OnPauseClick() {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "OnPauseClick");
        AnalysisSup.fabricLog(EventName.GAME, desc);

        game_stage.timer_pause = true;
        game_stage.power_pause = true;
        game_stage.hint_pause = true;

        setMask(true);
        game_stage.pause = true;
        SetButtonsStatus(false);

        Random r = new Random();
        var next = r.Next(1, _tipses.Count());
        var firstOrDefault = _tipses.FirstOrDefault(x => x.id == next);
        if (firstOrDefault != null) pause_text_tips.text = firstOrDefault.name;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(Pause.transform.DOLocalMoveX(-30, 0.5f));
        mySequence.Append(Pause.transform.DOLocalMoveX(0, 0.2f));
    }

    public void OnPlayClick()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "continue");
        AnalysisSup.fabricLog(EventName.GAME, desc);
        game_stage.timer_pause = false;
        game_stage.power_pause = false;
        game_stage.hint_pause = false;

        game_stage.pause = false;
        SetButtonsStatus(true);
        setMask(false);
        Pause.transform.DOLocalMoveX(-missDis, 0.5f)
            .SetEase(Ease.InQuint)
            .SetAutoKill(true)
            .OnComplete(InitPauseUIPos)
            .SetUpdate(true);
    }
    private void InitPauseUIPos()
    {
        Pause.transform.localPosition = new Vector3(missDis, 0, 0);
        Quit.transform.localPosition = new Vector3(missDis, 0, 0);
    }

    private void Restart()
    {
        //SceneManager.LoadScene("Level" + CommonData.current_level);

        CommonData.loading_target_scene = "Level" + CommonData.current_level;
        SceneManager.LoadScene("LoadingInGame");
    }

    public void OnRestartClick()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "OnRestartClick");
        AnalysisSup.fabricLog(EventName.GAME, desc);
        //int heart = LocalDynamicData.GetInstance().GetHeart();

        bool b = PlayerInfoUtil.ConsumeLife();
        if (b)
        {
            //LocalDynamicData.GetInstance().SetHeart(--heart);

            Pause.transform.DOLocalMoveX(-missDis, 0.5f)
                .SetEase(Ease.InQuint)
                .SetAutoKill(true)
                .OnComplete(Restart)
                .SetUpdate(true);

        }
        else
        {
            setMask(false);
            ShopController.instance.show(ShopItem.Heart);
            Debug.Log("No Heart !!!");
        }

    }

    public void OnHomeClick()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "quit");
        AnalysisSup.fabricLog(EventName.GAME, desc);

        Sequence mySequence = DOTween.Sequence();
        Sequence quitSequence = DOTween.Sequence();
        quitSequence.Append(Quit.transform.DOLocalMoveX(-30, 0.5f));
        quitSequence.Append(Quit.transform.DOLocalMoveX(0, 0.2f));
        mySequence.Append(quitSequence);
        mySequence.Insert(0, Pause.transform.DOMoveX(-missDis, 0.5f).OnComplete(resetPopWindow));
    }

    /*
    public void OnMusicClick()
    {
        LocalDynamicData.GetInstance().ChangeMusicOn();
    }

    public void OnSFXClick()
    {
        LocalDynamicData.GetInstance().ChangeSFXOn();
    }

    public void OnHintClick()
    {

    }
    */

    //Affirm UI
    public void OnAffirmHomeClick()
    {
        game_stage.OnFailUpdateData();

        cbs.Stop();

        AudioSourcesManager.GetInstance().Play(level_audio, (audioclip_set == null) ? null : audioclip_set.fail);

//        OnPlayClick();
        if (boardAction != null)
        {
            boardAction(game_stage.score, CommonData.current_level);
        }
        setMask(true);
        UpdateAdbtn();
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(Quit.transform.DOMoveX(-missDis, 0.5f).OnComplete(resetPopWindow));
        Sequence boardSequence = DOTween.Sequence();
        //boardSequence.Append(Fail_Fail.transform.DOLocalMoveX(-30, 0.5f));
        boardSequence.Append(Fail_Fail.transform.DOLocalMoveX(0, 0.5f));
        Transform rankInfo = Fail_Fail.transform.Find("Rank_Info");
        boardSequence.Append(rankInfo.DOLocalMoveY(-8, 0.2f));
        boardSequence.Append(rankInfo.DOPunchPosition(new Vector3(0, -10, 0), 0.5f));
        mySequence.Insert(0, boardSequence);
    }

    void resetPopWindow()
    {
        Pause.transform.localPosition = new Vector3(missDis, 0, 0);
        Fail_Chance.transform.localPosition = new Vector3(missDis, 0, 0);
        //Fail_Fail.transform.localPosition = new Vector3(CommonData.BASE_WIDTH, -65, 0);
    }

    public void OnAffirmPlayClick()
    {
        setMask(false);
        game_stage.pause = false;
        SetButtonsStatus(true);
        Quit.transform.DOLocalMoveX(-missDis, 0.5f)
            .SetEase(Ease.InQuint)
            .SetAutoKill(true)
            .OnComplete(InitPauseUIPos)
            .SetUpdate(true);
    }

    //Success UI
    public void OnSuccessClick()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "OnSuccessClick");
        AnalysisSup.fabricLog(EventName.GAME, desc);
        setMask(true);

        CommonData.loading_target_scene = "Main";
        SceneManager.LoadScene("LoadingInGame");
    }

    public void OnSuccessCloseClick()
    {
        setMask(false);

        CommonData.loading_target_scene = "Main";
        SceneManager.LoadScene("LoadingInGame");
    }

    //Fail Chance UI
    public void OnGiveUpClick() {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "OnGiveUpClick");
        AnalysisSup.fabricLog(EventName.GAME, desc);

        setMask(false);

        ShowFailFailUI();
    }

    public void ShowFailFailUI()
    {
        no_pause_ui = true;

        game_stage.OnFailUpdateData();

        cbs.Stop();

        UpdateAdbtn();
        AudioSourcesManager.GetInstance().Play(level_audio, (audioclip_set == null) ? null : audioclip_set.fail);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(Fail_Chance.transform.DOMoveX(-missDis, 0.5f).OnComplete(resetPopWindow));
        Sequence boardSequence = DOTween.Sequence();
        //boardSequence.Append(Fail_Fail.transform.DOLocalMoveX(-30, 0.5f));
        boardSequence.Append(Fail_Fail.transform.DOLocalMoveX(0, 0.5f));
        Transform rankInfo = Fail_Fail.transform.Find("Rank_Info");
        boardSequence.Append(rankInfo.DOLocalMoveY(-8, 0.2f));
        boardSequence.Append(rankInfo.DOPunchPosition(new Vector3(0, -10, 0), 0.5f));
        mySequence.Insert(0, boardSequence);
    }

    public void OnTenGoldClick() {
        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        if (playerInfo.Gold >= _chanceGoldSpend)
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            desc.Add("OnTenGoldClick", "" + _chanceGoldSpend);
            AnalysisSup.fabricLog(EventName.GAME, desc);
            GamePlusLog.Instance.SpentLog(_chanceGoldSpend, "OnTenGoldClick");

            playerInfo.Gold -= _chanceGoldSpend;
            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

            game_stage.pause = false;
            SetButtonsStatus(true);
            setMask(false);
            GameObject go;

            if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.REDRAW)
            {
                go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill1")) as GameObject;
            }
            else if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.BOMB)
            {
                go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill2")) as GameObject;
            }
            else if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.ADD_TIME)
            {
                go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill3")) as GameObject;
            }
            else if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.COLLECT_JEWELS)
            {
                go = Instantiate(Resources.Load("FX/item4")) as GameObject;
            }

            Fail_Chance.transform.DOLocalMoveX(-missDis, 0.5f)
                .SetEase(Ease.InQuint)
                .SetAutoKill(true)
                .OnComplete(ResetFailUIPos)
                .SetUpdate(true);

            game_stage.timer_pause = false;
            no_pause_ui = false;
        }
        else
        {
            //充值界面
            ShopController.instance.show(ShopItem.Gold);
        }

    }

    private void ResetFailUIPos()
    {
        Fail_Chance.transform.localPosition = new Vector3(missDis, 0, 0);

        if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.REDRAW)
        {
            game_stage.OnRefreshClick();
        }
        else if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.BOMB)
        {
            game_stage.OnBombClick();
        }
        else if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.ADD_TIME)
        {
            game_stage.OnAddTimeClick();
            if (!game_stage.HaveVaildCards())
            {
                  game_stage.OnRefreshClick();
            }
        }
        else if (game_stage.fail_chance_type == FAIL_CHANCE_TYPE.COLLECT_JEWELS)
        {
            Invoke("GetAllJewels", 1.5f);
        }

        game_stage.hint_pause = false;
    }

    //Fail Fail UI
    public void OnFailRetryClick()
    {
        //int heart = LocalDynamicData.GetInstance().GetHeart();
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "OnFailRetryClick");
        AnalysisSup.fabricLog(EventName.GAME, desc);

        bool b = PlayerInfoUtil.ConsumeLife();
        Debug.Log("OnFailRetryClick");
        if (b)
        {
            //LocalDynamicData.GetInstance().SetHeart(--heart);

            //SceneManager.LoadScene("Level" + CommonData.current_level);

            CommonData.loading_target_scene = "Level" + CommonData.current_level;
            SceneManager.LoadScene("LoadingInGame");
        }
        else
        {
            //ShopController.instance.show(ShopItem.Heart);
            Debug.Log("No Heart !!!");
        }
    }

    public void OnFailMainClick()
    {
        CommonData.loading_target_scene = "Main";
        SceneManager.LoadScene("LoadingInGame");
    }

    public void UpdateAdbtn()
    {
        //次数重置
        int refresh = PlayerPrefs.GetInt(REMOTE_ADD_REFRESH, TIME_AD_REFRESH);
        if (PlayerInfoUtil.getSubTime(DAY_FIRST_PLAY) >= refresh)
        {
            PlayerPrefs.SetInt(AD_PLAY_COUNT, 0);
            PlayerPrefs.SetString(DAY_FIRST_PLAY, PlayerInfoUtil.GetTimeStamp());
        }
        int remoteOffset = PlayerPrefs.GetInt(REMOTE_ADD_OFFSET, AD_PLAY_OFFSET);
        int plaed_count = PlayerPrefs.GetInt(AD_PLAY_COUNT, 0);
        if (plaed_count >= remoteOffset)
        {
            Button_Free.transform.Find("Button").gameObject.SetActive(true);
        }
        else
        {
            Button_Free.transform.Find("Button").gameObject.SetActive(false);
        }
    }

    public void OnFailFreeClick()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", "OnFailFreeClick");
        AnalysisSup.fabricLog(EventName.GAME, desc);
        Debug.Log("Fail Free !!!");
        if (PlayerPrefs.GetInt(AD_PLAY_COUNT, 0) >= PlayerPrefs.GetInt(REMOTE_ADD_OFFSET, AD_PLAY_OFFSET))
        {
            //todo 弹出提示框
            Debug.Log("已经超过每日播放次数限制");
            return;
        }
        //        AdSuc();
        gpVideoAd.ShowAd();
        AdloadUtils.Instance.Show();
        gpVideoAd.SetCallback(AdCallbackhanler);
    }

    void AdCallbackhanler(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                AdSuc();
                break;
            default:
                Debug.Log(result.ToString());
                AnalysisSup.LogAd(false, result.ToString());
                break;
        }
        AdloadUtils.Instance.hide();
    }
    private void AdSuc()
    {
        Debug.Log("Ad Finished. Rewarding player...");
        AnalysisSup.LogAd(true);
        PlayerInfoUtil.AddLife(1);
        //记录播放次数
        int num = PlayerPrefs.GetInt(AD_PLAY_COUNT, 0) + 1;
        PlayerPrefs.SetInt(AD_PLAY_COUNT, num);
        SceneManager.LoadScene("Level" + CommonData.current_level);
    }

    //TOP UI
    public void InitTopUI(STAGE_TYPE type)
    {

        text_level.text = "LEVEL " + stage.id.ToString();

        GameStr str;

        switch (type)
        {
            case STAGE_TYPE.KILL_ALL:
                Key_Num.SetActive(false);
                text_card_num.text = game_stage.stage.mahjong_cnt.ToString();

                str = StaticData.GetInstance().GetStringByID(1);

                if (str != null)
                {
                    text_target.text = str.guid_text;
                }
                else
                {
                    text_target.text = "";
                }

                InitTopStars();

                Score.SetActive(true);

                break;
            case STAGE_TYPE.KILL_CHOCOLATE:
                Card_Num.SetActive(false);
                text_key_num.text = game_stage.stage.chocolate_cnt.ToString();

                str = StaticData.GetInstance().GetStringByID(2);

                if (str != null)
                {
                    text_target.text = str.guid_text;
                }
                else
                {
                    text_target.text = "";
                }

                InitTopStars();

                Score.SetActive(true);
                break;
            case STAGE_TYPE.COLLECT_KILL_ALL:
                Key_Num.SetActive(false);
                text_card_num.text = game_stage.stage.mahjong_cnt.ToString();

                InitTopJewels();

                InitTopStars();

                Score.SetActive(true);

                break;
            case STAGE_TYPE.COLLECT_KILL_CHOCOLATE:
                Card_Num.SetActive(false);
                text_key_num.text = game_stage.stage.chocolate_cnt.ToString();

                str = StaticData.GetInstance().GetStringByID(game_stage.stage.type);

                InitTopJewels();

                InitTopStars();

                Score.SetActive(true);
                break;
            case STAGE_TYPE.KILL_ALL_TIMER:
                Key_Num.SetActive(false);
                text_card_num.text = game_stage.stage.mahjong_cnt.ToString();

                str = StaticData.GetInstance().GetStringByID(1);

                if (str != null)
                {
                    text_target.text = str.guid_text;
                }
                else
                {
                    text_target.text = "";
                }

                InitTopStars();

                Timer.SetActive(true);
                text_timer.text = UIUtil.ShowTimeMinute(game_stage.stage.count_down);

                break;
            case STAGE_TYPE.KILL_CHOCOLATE_TIMER:
                Card_Num.SetActive(false);
                text_key_num.text = game_stage.stage.chocolate_cnt.ToString();

                str = StaticData.GetInstance().GetStringByID(2);

                if (str != null)
                {
                    text_target.text = str.guid_text;
                }
                else
                {
                    text_target.text = "";
                }

                InitTopStars();

                Timer.SetActive(true);
                text_timer.text = UIUtil.ShowTimeMinute(game_stage.stage.count_down);

                break;
            case STAGE_TYPE.COLLECT_KILL_ALL_TIMER:
                Key_Num.SetActive(false);
                text_card_num.text = game_stage.stage.mahjong_cnt.ToString();

                InitTopJewels();

                InitTopStars();

                Timer.SetActive(true);
                text_timer.text = UIUtil.ShowTimeMinute(game_stage.stage.count_down);

                break;
            case STAGE_TYPE.COLLECT_KILL_CHOCOLATE_TIMER:
                Card_Num.SetActive(false);
                text_key_num.text = game_stage.stage.chocolate_cnt.ToString();

                str = StaticData.GetInstance().GetStringByID(game_stage.stage.type);

                InitTopJewels();

                InitTopStars();

                Timer.SetActive(true);
                text_timer.text = UIUtil.ShowTimeMinute(game_stage.stage.count_down);

                break;
        }

        fail_level_id.text = "LEVEL " + game_stage.stage.id.ToString();

    }

    float fill_x1;
    float fill_x2;
    float fill_length;

    bool filling = false;

    float fill_old;
    float fill_amount;

    void UpdateFillEffect(float fill)
    {
        fill_effect.transform.localPosition = new Vector3(fill_x1 + fill * fill_length, fill_effect.transform.localPosition.y, 0);
        image_fill.fillAmount = fill;
    }

    void InitTopStars()
    {

        float x1 = go_top_stars[0].GetComponent<RectTransform>().localPosition.x;
        float x2 = go_top_stars[2].GetComponent<RectTransform>().localPosition.x;

        float length = x2 - x1;

        fill_x1 = x1;
        fill_x2 = x2;
        fill_length = length;

        float begin = go_top_stars[0].GetComponent<RectTransform>().localPosition.x;

        fill_effect.transform.localPosition = new Vector3(x1, fill_effect.transform.localPosition.y, fill_effect.transform.localPosition.z);

        fill_old = 0f;

        if (game_stage.timer_level)
        {
            image_fill.fillAmount = 1f;

            fill_old = 1f;

            fill_effect.transform.localPosition = new Vector3(x2, fill_effect.transform.localPosition.y, fill_effect.transform.localPosition.z);

            float radio1 = (float)game_stage.stage.score1 / (float)game_stage.stage.count_down;
            float radio2 = (float)game_stage.stage.score2 / (float)game_stage.stage.count_down;
            float radio3 = (float)game_stage.stage.score3 / (float)game_stage.stage.count_down;

            go_top_stars[0].GetComponent<RectTransform>().localPosition = new Vector3(begin + radio1 * length, go_top_stars[0].GetComponent<RectTransform>().localPosition.y, 0);
            go_top_stars[1].GetComponent<RectTransform>().localPosition = new Vector3(begin + radio2 * length, go_top_stars[1].GetComponent<RectTransform>().localPosition.y, 0);
            go_top_stars[2].GetComponent<RectTransform>().localPosition = new Vector3(begin + radio3 * length, go_top_stars[2].GetComponent<RectTransform>().localPosition.y, 0);

            top_stars[0].SetActive(true);
            top_stars[1].SetActive(true);
            top_stars[2].SetActive(true);

            top_stars_up[0].enabled = true;
            top_stars_up[1].enabled = true;
            top_stars_up[2].enabled = true;

            return;
        }

        float radio4 = (float)game_stage.stage.score1 / (float)game_stage.stage.score3;
        float radio5 = (float)game_stage.stage.score2 / (float)game_stage.stage.score3;

        go_top_stars[0].GetComponent<RectTransform>().localPosition = new Vector3(begin + radio4 * length, go_top_stars[0].GetComponent<RectTransform>().localPosition.y, 0);
        go_top_stars[1].GetComponent<RectTransform>().localPosition = new Vector3(begin + radio5 * length, go_top_stars[1].GetComponent<RectTransform>().localPosition.y, 0);
    }

    public void ShowTopStars()
    {

        filling = true;

        if (game_stage.timer_level)
        {
            /*
            image_fill.fillAmount = game_stage.timer / (float)game_stage.stage.count_down;

            if (image_fill.fillAmount <= 0f)
            {
                image_fill.fillAmount = 0f;
            }
            */
            fill_amount = game_stage.timer / (float)game_stage.stage.count_down;

            if (fill_amount <= 0f)
            {
                fill_amount = 0f;
            }


            //UpdateFillEffect(image_fill.fillAmount);

            if (game_stage.timer < (float)game_stage.stage.score1)
            {
                top_stars[0].SetActive(false);

                top_stars_up[0].enabled = false;
            }

            if (game_stage.timer < (float)game_stage.stage.score2)
            {
                top_stars[1].SetActive(false);

                top_stars_up[1].enabled = false;
            }

            if (game_stage.timer < (float)game_stage.stage.score3)
            {
                top_stars[2].SetActive(false);

                top_stars_up[2].enabled = false;
            }

            return;
        }

        /*
        image_fill.fillAmount = (float)game_stage.score / (float)game_stage.stage.score3;

        if (image_fill.fillAmount >= 1f)
        {
            image_fill.fillAmount = 1f;
        }
        */

        fill_amount = (float)game_stage.score / (float)game_stage.stage.score3;

        if (fill_amount >= 1f)
        {
            fill_amount = 1f;
        }

        //UpdateFillEffect(image_fill.fillAmount);

        if (game_stage.score >= game_stage.stage.score1)
        {
            top_stars[0].SetActive(true);

            top_stars_up[0].enabled = true;
        }

        if (game_stage.score >= game_stage.stage.score2)
        {
            top_stars[1].SetActive(true);

            top_stars_up[1].enabled = true;
        }

        if (game_stage.score >= game_stage.stage.score3)
        {
            top_stars[2].SetActive(true);

            top_stars_up[2].enabled = true;
        }
    }

    void InitTopJewels()
    {
        GameObject go;

        for (int i = 0; i < game_stage.jewels_show.Count; i++)
        {
            if (game_stage.jewels_show[i] == 3)
            {
                go = Instantiate(Resources.Load("Prefabs/Jewel_Club")) as GameObject;

                InitJewels(go, i);
            }
            else if (game_stage.jewels_show[i] == 4)
            {

                go = Instantiate(Resources.Load("Prefabs/Jewel_Diamond")) as GameObject;

                InitJewels(go, i);
            }
            else if (game_stage.jewels_show[i] == 2)
            {
                go = Instantiate(Resources.Load("Prefabs/Jewel_Heart")) as GameObject;

                InitJewels(go, i);
            }
            else if (game_stage.jewels_show[i] == 1)
            {
                go = Instantiate(Resources.Load("Prefabs/Jewel_Spade")) as GameObject;

                InitJewels(go, i);
            }
        }
    }

    void InitJewels(GameObject go, int i)
    {
        go.transform.SetParent(diamonds[i].transform);
        go.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        go.transform.Find("Image2").GetComponent<Image>().enabled = false;

        if (i == game_stage.jewels_show.Count - 1)
        {
            go.transform.Find("Arrow").GetComponent<Image>().enabled = false;
        }

        go_jewels.Add(go);
    }

    public void UpdateJewel(int i, bool b)
    {
        if (!b)
        {
            return;
        }

        GameObject go = go_jewels[i];
        go.transform.Find("Image2").GetComponent<Image>().enabled = b;
    }

    public void ShowAllJewels()
    {
        for (int i = 0; i < go_jewels.Count; i++)
        {
            GameObject go = go_jewels[i];
            go.transform.Find("Image2").GetComponent<Image>().enabled = true;
        }
    }

    public void InitSkillsUI()
    {
        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        text_refresh_num.text = playerInfo.Redraw.ToString();
        text_bomb_num.text = playerInfo.Bomb.ToString();
        text_add_time_num.text = playerInfo.Clock.ToString();
        text_unlock_num.text = playerInfo.Unchain.ToString();
        text_get_diamonds_num.text = playerInfo.Jewel.ToString();
    }

    public void UpdateCardsNum()
    {
        text_card_num.text = (game_stage.cards_total_num - game_stage.cards_now_num).ToString();
    }

    public void UpdateScore()
    {
        score_text_score.text = game_stage.score.ToString();

        ShowTopStars();
    }

    public void UpdateHit(int i)
    {
        text_hit.text = "X" + i.ToString();

    }

    public void UpdateChocolateKeysNum()
    {
        text_key_num.text = (game_stage.chocolate_num - game_stage.chocolate_num_now).ToString();
    }

    public void UpdateTimer(int i)
    {
        text_timer.text = UIUtil.ShowTimeMinute(i);

        ShowTopStars();
    }

    public void OnRedraw()
    {
        if (ClickUtils.IsDoubleClick())return;

        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        bool b = true;

        if (playerInfo.Redraw > 0)
        {
            playerInfo.Redraw--;

            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

            text_refresh_num.text = playerInfo.Redraw.ToString();

            //game_stage.OnRefreshClick();

            game_stage.skill_refresh_num++;

            GameObject go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill1")) as GameObject;

            game_stage.pause = false;

            SetButtonsStatus(true);

            setMask(false);

            Fail_Chance.transform.DOLocalMoveX(-missDis, 0.5f)
                .SetEase(Ease.InQuint)
                .SetAutoKill(true)
                .OnComplete(ResetFailUIPos)
                .SetUpdate(true);

            game_stage.timer_pause = false;
            no_pause_ui = false;
        }
        else
        {
            b = false;
        }

        if (!b)
        {
            BoosterController.instance.ShowBooster("Redraw", propUseNum["Redraw"]);
        }
    }

    public void OnBomb()
    {
        if (ClickUtils.IsDoubleClick()) return;
        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        bool b = true;

        if (playerInfo.Bomb > 0)
        {
            playerInfo.Bomb--;

            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

            text_bomb_num.text = playerInfo.Bomb.ToString();

            //game_stage.OnBombClick();

            game_stage.skill_bomb_num++;

            GameObject go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill2")) as GameObject;

            game_stage.pause = false;

            SetButtonsStatus(true);

            setMask(false);

            Fail_Chance.transform.DOLocalMoveX(-missDis, 0.5f)
                .SetEase(Ease.InQuint)
                .SetAutoKill(true)
                .OnComplete(ResetFailUIPos)
                .SetUpdate(true);

            game_stage.timer_pause = false;
            no_pause_ui = false;
        }
        else
        {
            b = false;
        }

        if (!b)
        {
            BoosterController.instance.ShowBooster("Bomb", propUseNum["Bomb"]);
        }
    }

    public void OnAddTime()
    {
        if (ClickUtils.IsDoubleClick()) return;
        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        bool b = true;

        if (playerInfo.Clock > 0)
        {
            playerInfo.Clock--;

            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

            text_add_time_num.text = playerInfo.Clock.ToString();

            //game_stage.OnAddTimeClick();

            game_stage.skill_add_timer_num++;

            GameObject go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill3")) as GameObject;

            game_stage.pause = false;

            SetButtonsStatus(true);

            setMask(false);

            Fail_Chance.transform.DOLocalMoveX(-missDis, 0.5f)
                .SetEase(Ease.InQuint)
                .SetAutoKill(true)
                .OnComplete(ResetFailUIPos)
                .SetUpdate(true);

            game_stage.timer_pause = false;
            no_pause_ui = false;
        }
        else
        {
            b = false;
        }

        if (!b)
        {
            BoosterController.instance.ShowBooster("Clock", propUseNum["Clock"]);
        }
    }

    public void OnGetAllJewels()
    {
        if (ClickUtils.IsDoubleClick()) return;
        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        bool b = true;

        if (playerInfo.Jewel > 0)
        {
            playerInfo.Jewel--;

            DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

            text_get_diamonds_num.text = playerInfo.Jewel.ToString();

            //game_stage.OnGetDiamondClick();

            //Invoke("GetAllJewels", 2f);

            game_stage.skill_jewels_num++;

            GameObject go = Instantiate(Resources.Load("FX/item4")) as GameObject;

            game_stage.pause = false;

            SetButtonsStatus(true);

            setMask(false);

            Fail_Chance.transform.DOLocalMoveX(-missDis, 0.5f)
                .SetEase(Ease.InQuint)
                .SetAutoKill(true)
                .OnComplete(ResetFailUIPos)
                .SetUpdate(true);

            game_stage.timer_pause = false;
            no_pause_ui = false;
        }
        else
        {
            b = false;
        }

        if (!b)
        {
            BoosterController.instance.ShowBooster("Jewel", propUseNum["Jewel"]);
        }
    }


    public void OnBoosterClick(string item)
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ingame", item);
        AnalysisSup.fabricLog(EventName.GAME, desc);

        if (game_stage.pause)
            return;

        PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First(x => x.id == 1);

        bool b = true;

        if (item.Equals("Jewel"))
        {
            if (playerInfo.Jewel > 0)
            {
                playerInfo.Jewel--;

                DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

                text_get_diamonds_num.text = playerInfo.Jewel.ToString();

                //game_stage.OnGetDiamondClick();

                Invoke("GetAllJewels", 2f);

                game_stage.skill_jewels_num++;

                GameObject go = Instantiate(Resources.Load("FX/item4")) as GameObject;

                propUseNum["Jewel"]++;
            }
            else
            {
                b = false;
            }
        }

        if (item.Equals("Redraw"))
        {
            if (CommonData.current_level == 3 && LocalDynamicData.GetInstance().guide_step3 == 0)
            {
                playerInfo.Redraw++;
                LocalDynamicData.GetInstance().SetGuideStep3(1);
            }
            if (playerInfo.Redraw > 0)
            {
                playerInfo.Redraw--;

                DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

                text_refresh_num.text = playerInfo.Redraw.ToString();

                //game_stage.OnRefreshClick();

                game_stage.skill_refresh_num++;

                GameObject go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill1")) as GameObject;

                propUseNum["Redraw"]++;
                Invoke("Redraw", 2f);
            }
            else
            {
                b = false;
            }
        }

        if (item.Equals("Bomb"))
        {
            if (CommonData.current_level == 5 && LocalDynamicData.GetInstance().guide_step5 == 0)
            {
                playerInfo.Bomb++;

                LocalDynamicData.GetInstance().SetGuideStep5(1);
            }

            if (playerInfo.Bomb > 0)
            {
                playerInfo.Bomb--;

                DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

                text_bomb_num.text = playerInfo.Bomb.ToString();

                game_stage.OnBombClick();

                game_stage.skill_bomb_num++;

                GameObject go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill2")) as GameObject;

                propUseNum["Bomb"]++;
            }
            else
            {
                b = false;
            }
        }

        if (item.Equals("Clock"))
        {
            if (playerInfo.Clock > 0)
            {
                playerInfo.Clock--;

                DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

                text_add_time_num.text = playerInfo.Clock.ToString();

                game_stage.OnAddTimeClick();

                game_stage.skill_add_timer_num++;

                GameObject go = Instantiate(Resources.Load("Effect/Prefab/Inside_skill3")) as GameObject;

                propUseNum["Clock"]++;
            }
            else
            {
                b = false;
            }
        }

        if (item.Equals("Unchain"))
        {
            if (playerInfo.Unchain > 0)
            {
                playerInfo.Unchain--;

                DynamicDataBaseService.GetInstance().UpdateData(playerInfo);

                text_unlock_num.text = playerInfo.Unchain.ToString();

                game_stage.OnUnLockClick();

                game_stage.skill_unlock_num++;

                GameObject go = Instantiate(Resources.Load("FX/item5")) as GameObject;

                propUseNum["Unchain"]++;
            }
            else
            {
                b = false;
            }
        }

        if (!b)
        {
            BoosterController.instance.ShowBooster(item, propUseNum[item]);
        }
    }

    void GetAllJewels()
    {
        game_stage.OnGetDiamondClick();
    }

    void Redraw()
    {
        game_stage.OnRefreshClick();
    }

    public void setMask(bool status)
    {
        Image image = mask.GetComponent<Image>();
        if (status)
        {
            mask.SetActive(true);
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
        mask.SetActive(false);
    }

    void Update()
    {
        if (filling)
        {
            if (game_stage.timer_level)
            {
                if (fill_amount < fill_old)
                {
                    fill_old -= 0.01f;
                }

                if (fill_amount >= fill_old)
                {
                    filling = false;
                }

                if (fill_old <= 0)
                {
                    fill_old = 0;
                }


                UpdateFillEffect(fill_old);
            }
            else
            {
                if (fill_amount > fill_old)
                {
                    fill_old += 0.01f;
                }

                if (fill_amount <= fill_old)
                {
                    filling = false;
                }

                if (fill_old >= 1)
                {
                    fill_old = 1;
                }

                UpdateFillEffect(fill_old);
            }
        }
    }
}













