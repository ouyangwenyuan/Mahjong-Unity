using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using Assets.GamePlus.support;
using Assets.Script.gameplus.define;
using Assets.Scripts.UIController;
using Assets.Scripts.Utils;

public class MainFrontController : MonoBehaviour {

    const string MAX = "MAX";

    public Text Text_Gold;
    public Text Text_Heart;
    public Text Text_Timer;
    public Text Text_Level;
    public Image Image_Heart;
    public GameObject Settings;
    public GameObject PlayBoard;
    public GameObject DialBoard;
    public GameObject mask;
    public GameObject Img_unlimit;
    public Transform buttonHeart;
    public Transform buttonGold;
    public Transform buttonSetting;
    public Transform CloudTransform;

    public Transform MapLV;
    public GameObject Figure_Icon;//关卡进度头像位置

    public GameObject Guide_Root;

    DateTime aim_time;

    string time_str;
    //int current_heart = 0;
    Vector3 move_pos = new Vector3(0, 140, 0);
    public static long CoolTimes;
    private bool isCountDown = false;
    //GameLevel[] game_levels = new GameLevel[] { };

    //Camera main_camera;
    DragCamera drag_camera;
    private float missDis;
    void Start () {
        drag_camera = Camera.main.GetComponent<DragCamera>();
        AdapterScreen();
        StartTimeUpdate();
        InitPos();
        InitFigurePos();
        initGlobalCamara();
        GuideManager.GetInstance().GuideMapInit(Guide_Root.transform);
        GameObject music = Instantiate(Resources.Load("Prefabs/Music")) as GameObject;
    }

    void AdapterScreen()
    {
        missDis = Screen.width * 2;
        CanvasScaler CanvasBack = GameObject.Find("CanvasFront").GetComponent<CanvasScaler>();
        if (Splash.IsIpad)
        {
//            CanvasBack.matchWidthOrHeight = 0f;
            buttonHeart.localPosition = new Vector3(-184, 429, 0);
            buttonGold.localPosition = new Vector3(211, 429, 0);
            buttonSetting.localPosition = new Vector3(-281, -396, 0);
            GameObject.Find("GobalUI/Shop").transform.localScale=new Vector3(1,0.75f,1);
        }
        else
        {
//            CanvasBack.matchWidthOrHeight = 1f;
            buttonHeart.localPosition = new Vector3(-160f, 570, 0);
            buttonGold.localPosition = new Vector3(195, 570, 0);
            buttonSetting.localPosition = new Vector3(-281, -505, 0);
        }
    }

    void initGlobalCamara()
    {
        Canvas canvas = GameObject.Find("GobalUI").GetComponent<Canvas>();
        CanvasScaler CanvasScaler = GameObject.Find("GobalUI").GetComponent<CanvasScaler>();
        if (Screen.width < Screen.height)
        {
            CanvasScaler.matchWidthOrHeight = 0f;
        }
        else
        {
            CanvasScaler.matchWidthOrHeight = 1f;
        }
        canvas.renderMode=RenderMode.ScreenSpaceCamera;
        canvas.worldCamera=Camera.main;
    }

    void OnDestroy()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("ui_main", SecitonUtils.CountSection(5,(int) Time.timeSinceLevelLoad));
        AnalysisSup.fabricLog(EventName.UI_SHOW_TIME, desc);
    }

    void InitFigurePos() {
        if(MapLV == null){
            return;
        }

        int new_level = LocalDynamicData.GetInstance().GetNewLevel();//PlayerPrefs.GetInt(CommonData.BEST_LEVEL, 0);
        int lv_count = MapLV.childCount;

        Vector3 pos_new;
        if (lv_count > new_level)
            pos_new = MapLV.GetChild(new_level - 1).transform.localPosition;
        else
            pos_new = MapLV.GetChild(lv_count - 1).transform.localPosition;

        if (CommonData.is_success_level)
        {
            // 闯关成功，显示星星，移动头像。
            int pre_level = new_level - 1;
            Vector3 pos_pre;
            if (lv_count > pre_level)
                pos_pre = MapLV.GetChild(pre_level).transform.localPosition;
            else
                pos_pre = MapLV.GetChild(lv_count - 1).transform.localPosition;

            CommonData.is_success_level = false;

            Figure_Icon.transform.localPosition = pos_pre + move_pos;
            Figure_Icon.transform.DOLocalMove(pos_new + move_pos, 0.5f)
                .SetEase(Ease.Linear)
                .SetAutoKill(true)
                .SetUpdate(true);

        }
        else
        {
            Figure_Icon.transform.localPosition = pos_new + move_pos;
        }
    }

    //回到地图
    public void StartTimeUpdate()
    {
        PlayerInfo info = PlayerInfoUtil.GetLifeInfo();
        Text_Gold.text = "" + info.Gold;
        Debug.Log("StartTimeUpdate " + PlayerInfoUtil.Countdown);
        if (PlayerInfoUtil.InfiniteHeart)
        {
            Debug.Log("InfiniteHeart");
            Text_Heart.text = "";
            Img_unlimit.SetActive(true);
//            Text_Heart.transform.Rotate(new Vector3(0, 0, 90));
            CoolTimes = PlayerInfoUtil.UnlimiteCountdown;
            if (!isCountDown)
            {
                StartCoroutine(UpdateTime());
            }
        }
        else
        {
//            CoolTimes = 1800;
//            StartCoroutine(UpdateTime());
            HeartCountdown(info);
        }
    }

    private void HeartCountdown(PlayerInfo info)
    {
        Img_unlimit.SetActive(false);
        Text_Heart.text = info.Life + "/" + info.MaxLife;
        if (Math.Abs(PlayerInfoUtil.Countdown) < 1)
        {
            Text_Timer.text = "";
            CoolTimes = 0;
            Debug.Log("体力已满，取消倒计时");
        }
        else
        {
            //更新倒计时
            CoolTimes = PlayerInfoUtil.Countdown;
            if (!isCountDown)
            {
                Debug.Log("体力未满，开始倒计时");
                StartCoroutine(UpdateTime());
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) return;
//        Debug.Log("Pause time"+ SupportListener.pauseTime);
//        CoolTimes -= SupportListener.pauseTime;
        StartTimeUpdate();
    }

    //倒计时
    public IEnumerator UpdateTime()
    {
        while (CoolTimes >= 0)
        {
            isCountDown = true;
            FormatTime();
            if (CoolTimes==0)
            {
                UpdateHeartUI();
            }
            CoolTimes--;
            yield return new WaitForSeconds(1);
        }
        isCountDown = false;
        Debug.Log("UpdateTime");
    }

    private void UpdateHeartUI()
    {
        PlayerInfo info = DynamicDataBaseService.GetInstance().GetPlayerInfo().First();
        if (PlayerInfoUtil.InfiniteHeart)
        {
            info.Infinite_time = "0";
        }
        else
        {
            info.Life += 1;
            Debug.Log("增加体力");
            if (info.Life < info.MaxLife)
            {
                Text_Heart.text = info.Life + "/" + info.MaxLife;
                CoolTimes += CommonData.HEART_COOLING_TIME*60;
            }
            else
            {
                info.Life = info.MaxLife;
                Text_Timer.text = "";
                info.MaxLifeTime = "0";
                Text_Heart.text = info.MaxLife + "/" + info.MaxLife;
                CoolTimes = 0;
            }
        }
        DynamicDataBaseService.GetInstance().UpdateData(info);
    }

    private void FormatTime()
    {
        int day = (int) (CoolTimes/(3600*24));
        int hours = (int) (CoolTimes/3600%24);
        int minutes = (int) (CoolTimes%3600/60);
        int seconds = (int) (CoolTimes%3600%60);
        if (day!=0)
        {
            Text_Timer.text = PadZero(day) + "D" + PadZero(hours) + "H";
        }
        else if (hours!=0)
        {
            Text_Timer.text = PadZero(hours) + "H" + PadZero(minutes) + "M";
        }
        else
        {
            Text_Timer.text =PadZero(minutes) + "M" + PadZero(seconds)+"S";
        }
    }

    string PadZero(int value)
    {
        return value.ToString().PadLeft(2, '0');
    }

    public void OnFacebookClick()
    {
    }

    public void OnGameCenterClick()
    {
    }

    public void OnLevelClick(int level)
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("level_main", "level_click_"+level);
        AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

        drag_camera.enabled = false;

        Debug.Log("OnLevelClick:" + level);

        CommonData.current_level = level;

        PlayBoard.GetComponent<PlayBoardController>().Init(level);
    }

    public void OnCaveClick(int level)
    {


        PlayBoard.GetComponent<PlayBoardController>().Init(level);
    }

    private void InitPos()
    {
        Settings.transform.localPosition = new Vector3(missDis, 0, 0);
        PlayBoard.transform.localPosition = new Vector3(missDis, -65, 0);
        DialBoard.transform.localPosition = new Vector3(missDis, 0, 0);
    }

    public void OnSettingClick()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("type", "OnSettingClick");
        AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

        Settings.GetComponent<SettingController>().UpdateFB();
        setMask(true);
        drag_camera.enabled = false;
//        GameObject.Find("GobalUI").GetComponent<DialogController>().show("aaaaaaaaaaaaaa");
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(Settings.transform.DOLocalMoveX(-30, 0.5f));
        mySequence.Append(Settings.transform.DOLocalMoveX(0, 0.2f));
    }

    public void OnSettingClose()
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("type", "OnSettingClose");
        AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

        setMask(false);
        drag_camera.enabled = true;

        Settings.transform.DOLocalMoveX(- missDis, 0.5f)
            .SetEase(Ease.InQuint)
            .OnComplete(InitPos)
            .SetAutoKill(true)
            .SetUpdate(true);
    }

    public void OnDialClick()
    {
        PlayerInfoUtil.ConsumeLife();
        StartTimeUpdate();
        DialBoard.transform.DOLocalMoveX(0, 0.5f)
            .SetEase(Ease.InQuint)
            .SetAutoKill(true)
            .SetUpdate(true);
    }

    public void OnDialClose()
    {
        DialBoard.transform.DOLocalMoveX(-missDis, 0.5f)
            .SetEase(Ease.InQuint)
            .OnComplete(InitPos)
            .SetAutoKill(true)
            .SetUpdate(true);
    }

    public void OnGoldClick()
    {
        drag_camera.enabled = false;

        ShopController.instance.show(ShopItem.Gold);
    }

    public void OnHeartClick()
    {
        drag_camera.enabled = false;

        ShopController.instance.show(ShopItem.Heart);
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
}
