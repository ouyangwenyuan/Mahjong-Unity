using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadMonoHandler : MonoBehaviour {

    public int Chapter_ID = 0;

    Image image;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();

        int level_done_num = DynamicData.GetInstance().GetStagesDoneNum();
        int stage_num = StaticData.GetInstance().GetStagesNum();

        int level_new = level_done_num + 1;

        if (level_new > stage_num)
        {
            image.enabled = true;

            return;
        }

        Stage stage = StaticData.GetInstance().GetStageByID(level_new);

        int chapter = stage.chapter;

        if (chapter > Chapter_ID)
        {
            image.enabled = true;
        }
	}
	
	// Update is called once per frame
	//void Update () {
	//	
	//}
}
