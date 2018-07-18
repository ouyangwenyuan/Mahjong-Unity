using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using System;
using Assets.Script.gameplus.define;
using Assets.Scripts.Utils;

public class PlayBoardController : MonoBehaviour {

    public Text text_id;
    public GameObject[] stars;
    
    public GameObject go_single;
    public GameObject go_multi;

    public GameObject mainMask;

    public Image single_card;
    public Image multi_card;

    public Text text_card_num_single;
    public Text text_card_num;
    public Text text_timer;
    public Transform Rank_Info;
    Stage stage;
    Highscore high_score;
    public static event Action<int,int> SucAction; 
	void Awake () {

        //Init(CommonData.current_level);
	}

    public void Init(int level)
    {
        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("board", "OnBoardOpen");
        AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

        Close();

        stage = StaticData.GetInstance().GetStageByID(level);

        if (stage == null)
        {
            Debug.LogError("Not Level : " + level.ToString());

            return;
        }

        text_id.text = "LEVEL " + stage.id.ToString();

        bool single = true;

        if (!(stage.type == (int)STAGE_TYPE.KILL_ALL
            || stage.type == (int)STAGE_TYPE.KILL_CHOCOLATE
            || stage.type == (int)STAGE_TYPE.COLLECT_KILL_ALL
            || stage.type == (int)STAGE_TYPE.COLLECT_KILL_CHOCOLATE))
        {
            single = false;
        }

        bool b = CommonData.IsChocolateLevel((STAGE_TYPE)stage.type);

        if(single){
            go_single.SetActive(true);

            

            if (b)
            {
                single_card.sprite = AtlasManager.GetInstance().GetCardSprite("chocolate");//Resources.Load<Sprite>(CommonData.CHOCOLATE_PATH);
                
                text_card_num_single.text = "X" + stage.chocolate_cnt.ToString();
            }
            else {
                single_card.sprite = AtlasManager.GetInstance().GetCardSprite("d_green");//Resources.Load<Sprite>(CommonData.DGREEN_PATH);

                text_card_num_single.text = "X" + stage.mahjong_cnt.ToString();
            }
        }else{
            go_multi.SetActive(true);

            if (b)
            {
                multi_card.sprite = AtlasManager.GetInstance().GetCardSprite("chocolate");//Resources.Load<Sprite>(CommonData.CHOCOLATE_PATH); ;

                text_card_num.text = "X" + stage.chocolate_cnt.ToString();
            }
            else
            {
                multi_card.sprite = AtlasManager.GetInstance().GetCardSprite("d_green");//Resources.Load<Sprite>(CommonData.DGREEN_PATH); ;

                text_card_num.text = "X" + stage.mahjong_cnt.ToString();
            }

            
            text_timer.text = UIUtil.ShowTimeMinute(stage.count_down);
        }

        //text_timer.text = stage.count_down.ToString();

        int stars_count = 0;

        high_score = DynamicData.GetInstance().GetHighScoreByID(level);

        int score_stage = high_score != null ? high_score.highscore : 0;

        int score1 = stage.score1;
        int score2 = stage.score2;
        int score3 = stage.score3;

        if (score_stage >= score1 && score_stage < score2)
        {
            stars_count = 1;
        }
        else if (score_stage >= score2 && score_stage < score3)
        {
            stars_count = 2;
        }
        else if (score_stage >= score3)
        {
            stars_count = 3;
        }

        if (score_stage==0)
        {
            stars_count = 0;
        }

        for (int i = 0; i < stars_count; i++)
        {
            stars[i].SetActive(true);
        }

        if (SucAction != null) SucAction(0, level);
        GameObject.Find("CanvasFront").GetComponent<MainFrontController>().setMask(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOLocalMoveX(-30, 0.5f));
        mySequence.Append(transform.DOLocalMoveX(0, 0.2f));
        mySequence.Append(Rank_Info.DOLocalMoveY(-8, 0.2f));
        mySequence.Append(Rank_Info.DOPunchPosition(new Vector3(0,-10,0),0.5f,8));
    }

    public void OnBoardClose() {

        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("board", "OnBoardClose");
        AnalysisSup.fabricLog(EventName.BUTTON_CLICK, desc);

        GameObject.Find("CanvasFront").GetComponent<MainFrontController>().setMask(false);
        Camera.main.GetComponent<DragCamera>().enabled = true;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOLocalMoveX(-CommonData.BASE_WIDTH, 0.5f).OnComplete(InitPos));
        mySequence.Append(Rank_Info.DOLocalMoveY(254, 0.3f));
    }

    private void InitPos()
    {
        transform.localPosition = new Vector3(CommonData.BASE_WIDTH, -65, 0);
    }

    public void OnPlayClick()
    {
        if (!PlayerInfoUtil.ConsumeLife())
        {
            return;
        }
        if (stage == null)
        {
            return;
        }

        transform.DOLocalMoveX(-CommonData.BASE_WIDTH, 0.5f)
            .SetEase(Ease.InQuint)
            .OnComplete(EnterGame)
            .SetAutoKill(true)
            .SetUpdate(true);
    }

    private void EnterGame()
    {
        transform.localPosition = new Vector3(CommonData.BASE_WIDTH, 0, 0);

        //SceneManager.LoadScene("Level" + stage.id.ToString());

        CommonData.loading_target_scene = "Level" + stage.id.ToString();
        SceneManager.LoadScene("LoadingInGame");
    }

    void Close() {
        
        for (int i = 0; i < 3; i++)
        {
            stars[i].SetActive(false);
        }

        go_single.SetActive(false);
        go_multi.SetActive(false);
    }
}