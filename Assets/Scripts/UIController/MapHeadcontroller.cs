using System.Collections;
using System.Collections.Generic;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MapHeadcontroller : MonoBehaviour {
    public GameObject head;

    GameObject pos_begin;
    GameObject pos_end;

    int index_begin;
    int index_end;

    bool fly = false;

    float x_step = 0;
    float y_step = 0;
    private Image headerImage;
    private ImageUtil imageUtil;
	// Use this for initialization
	void Start () {
        headerImage = head.transform.Find("mask").Find("panda").GetComponent<Image>();
        imageUtil = GameObject.Find("GamePlus").GetComponent<ImageUtil>();
        SetHeade();
        int level_done_num = DynamicData.GetInstance().GetStagesDoneNum();
        int level_total_num = StaticData.GetInstance().GetStagesNum();

        if(level_done_num == 0){
            index_begin = 1;

            SetHeadPos(index_begin);
        }
        else if (level_done_num == level_total_num)
        {
            index_begin = level_total_num;

            SetHeadPos(index_begin);
        }
        else{
            index_begin = level_done_num;
            index_end = level_done_num + 1;

            Highscore score = DynamicData.GetInstance().GetHighScoreByID(index_end);
            Stage stage = StaticData.GetInstance().GetStageByID(index_end);

            if (score == null) {
                score = new Highscore();

                score.id = index_end;
                score.stage_id = index_end;
                
                score.try_num = 0;
                
                score.time_cost = 0f;
                
                score.skill_refresh_num = 0;
                score.skill_bomb_num = 0;
                score.skill_add_timer_num = 0;
                score.skill_jewels_num = 0;
                score.skill_unlock_num = 0;

                score.deadpoint_I = stage.deadpoint_I;
                score.deadpoint_X = stage.deadpoint_X;

                score.headicon = 0;

                DynamicData.GetInstance().InsertHighScorce(score);
            }

            if (score.headicon == 1)
            {
                SetHeadPos(index_end);
            }
            else {
                fly = true;

                score.headicon = 1;

                DynamicData.GetInstance().UpdateHighScorce(score);

                pos_begin = GameObject.Find(CommonData.LEVEL_PATH + index_begin.ToString());
                pos_end = GameObject.Find(CommonData.LEVEL_PATH + index_end.ToString());

                GameLevelMonoHandler level = pos_begin.GetComponent<GameLevelMonoHandler>();
                GameObject head_pos = level.Head;

                float x = head_pos.transform.localPosition.x + level.transform.localPosition.x;
                float y = head_pos.transform.localPosition.y + level.transform.localPosition.y;

                head.transform.localPosition = new Vector3(x, y, 0f);

                level = pos_end.GetComponent<GameLevelMonoHandler>();
                head_pos = level.Head;

                x = head_pos.transform.localPosition.x + level.transform.localPosition.x;
                y = head_pos.transform.localPosition.y + level.transform.localPosition.y;

                Debug.Log("X : " + x.ToString());
                Debug.Log("Y : " + y.ToString());

                x_step = (x - head.transform.localPosition.x) / CommonData.HEAD_FLY_TIME;
                y_step = (y - head.transform.localPosition.y) / CommonData.HEAD_FLY_TIME;
            }
        }
	}

    void SetHeadPos(int index) {
        pos_begin = GameObject.Find(CommonData.LEVEL_PATH + index.ToString());

        GameLevelMonoHandler level = pos_begin.GetComponent<GameLevelMonoHandler>();

        GameObject head_pos = level.Head;

        float x = head_pos.transform.localPosition.x + level.transform.localPosition.x;
        float y = head_pos.transform.localPosition.y + level.transform.localPosition.y;

        head.transform.localPosition = new Vector3(x, y, 0f);
    }

    float timer = 0f;

	// Update is called once per frame
	void Update () {
		if(fly){

            timer += Time.deltaTime;

            float x = head.transform.localPosition.x + x_step * Time.deltaTime;
            float y = head.transform.localPosition.y + y_step * Time.deltaTime;

            head.transform.localPosition = new Vector3(x, y, 0f);

            if(timer >= CommonData.HEAD_FLY_TIME){
                MainFrontController main_front_controller = GameObject.Find("CanvasFront").GetComponent<MainFrontController>();

                main_front_controller.OnLevelClick(index_end);

                fly = false;
            }
        }

	}

    public void SetHeade()
    {
        if (FB.IsLoggedIn)
        {
            var curFbid = PlayerPrefs.GetString(Constance.FACEBOOK_ID, "");
            if (string.IsNullOrEmpty(curFbid))
            {
                headerImage.sprite = Resources.Load<Sprite>("Texture/UI/head_sculpture/img_panda");
            }
            else
            {
                imageUtil.SetHeadImage(curFbid, headerImage);
            }
        }
        else
        {
            headerImage.sprite = Resources.Load<Sprite>("Texture/UI/head_sculpture/img_panda");
        }
    }

}
