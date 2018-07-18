using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLevelMonoHandler : MonoBehaviour {

    public int ID;

    public Image Passed;
    public Image Current;

    public Text Text_ID;

    public Image Image1_1;

    public Image Image2_1;
    public Image Image2_2;

    public Image Image3_1;
    public Image Image3_2;
    public Image Image3_3;

    public GameObject Head;

    MainFrontController main_front_controller;

    bool b_press = true;

	// Use this for initialization
	void Start () {
        //if(ID == 3){
        //    Debug.Log("ID");
        //}
        
        string sID = "Level" + ID;

        int scene_id = SceneUtility.GetBuildIndexByScenePath("Scenes/" + sID);

        if (scene_id == -1)
        {
            //未解锁
            b_press = false;

            Text_ID.text = "No Loaded" + ID.ToString();

            return;
        }

        main_front_controller = GameObject.Find("CanvasFront").GetComponent<MainFrontController>();

        int level_done_num = DynamicData.GetInstance().GetStagesDoneNum();

        if (ID <= level_done_num) {
            //已完成
            Highscore hs = DynamicData.GetInstance().GetHighScoreByID(ID);
            Stage stage = StaticData.GetInstance().GetStageByID(ID);

            int score_stage = hs.highscore;

            int score1 = stage.score1;
            int score2 = stage.score2;
            int score3 = stage.score3;

            STAGE_TYPE stage_type = (STAGE_TYPE)stage.type;
            bool timer_level = CommonData.ShowTimer(stage_type);

            if (timer_level)
            {
                score_stage = stage.count_down - hs.highscore;
            }

            //else
            {
                if (score_stage < score1)
                {
                    Current.enabled = true;
                }
                else if (score_stage >= score1 && score_stage < score2)
                {
                    Image1_1.enabled = true;

                    Current.enabled = true;
                }
                else if (score_stage >= score2 && score_stage < score3)
                {
                    Image2_1.enabled = true;
                    Image2_2.enabled = true;

                    Current.enabled = true;
                }
                else if (score_stage >= score3)
                {
                    Image3_1.enabled = true;
                    Image3_2.enabled = true;
                    Image3_3.enabled = true;

                    Passed.enabled = true;
                }
            }

            Text_ID.enabled = true;
            Text_ID.text = ID.ToString();
        }
        else if (ID == level_done_num + 1)
        {
            //刚解锁
            Current.enabled = true;

            Text_ID.enabled = true;
            Text_ID.text = ID.ToString();
        }else{
            //未解锁
            b_press = false;

            Text_ID.text = ID.ToString();
        }

        
	}
	
    public void OnClick(){
        if(DragCamera.moving)
        {
            return;
        }

        if(b_press)
            main_front_controller.OnLevelClick(ID);
    }

	// Update is called once per frame
	//void Update () {
		
	//}
}
