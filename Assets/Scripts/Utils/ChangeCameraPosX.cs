using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraPosX : MonoBehaviour {

    public GameObject Map;
    public Transform camera_mid;

    List<GameLevelMonoHandler> levels;

    int level_done = 0;
    int level_total_num = 0;
    int level_new = 0;

    float distance = 0f;
    float distance_total = 0f;

    void Awake() {
        if (Map == null)
        {
            return;
        }

        level_done = DynamicData.GetInstance().GetStagesDoneNum();

        /*
        if (level_done < 2)
        {
            return;
        }
        */

        levels = new List<GameLevelMonoHandler>(Map.GetComponentsInChildren<GameLevelMonoHandler>());

        if (levels.Count == 0)
        {
            return;
        }

        level_total_num = StaticData.GetInstance().GetStagesNum();

        level_new = level_done + 1;


        //ChangeX();
        ChangeY();
    }

    void ChangeY() {
        //return;


        /*
        if (level_new < 2) {
            return;
        }
        */

        if (level_new > level_total_num)
        {
            level_new = level_total_num;
        }

        Transform target = levels[level_new - 1].transform;
        Transform begin = levels[0].transform;

        if (level_new <= levels.Count)
        {
            distance = target.position.y - begin.position.y;
            distance_total = levels[levels.Count - 1].transform.position.y - begin.position.y;
        }

        float move_y = CommonData.CAMERA_MAP_BEGIN_Y + (CommonData.CAMERA_MAP_END_Y - CommonData.CAMERA_MAP_BEGIN_Y) * (distance / distance_total);

        transform.position = new Vector3(transform.position.x, move_y, transform.position.z);

        //move_x = ((move_x - CommonData.CAMERA_MAP_BEGIN) / (CommonData.CAMERA_MAP_END - CommonData.CAMERA_MAP_BEGIN)) * (CommonData.CAMERA_MID_MAP_END - CommonData.CAMERA_MID_MAP_BEGIN) + CommonData.CAMERA_MID_MAP_BEGIN;
        //camera_mid.position = new Vector3(move_x, camera_mid.position.y, camera_mid.position.z);
    }

    void ChangeX()
    {
        float x = levels[level_new - 1].transform.position.x - levels[0].transform.position.x;

        if(x < 0){
            transform.position = new Vector3(CommonData.CAMERA_MAP_BEGIN_X, transform.position.y, transform.position.z);
        }
        else if (x > 0)
        {
            transform.position = new Vector3(CommonData.CAMERA_MAP_END_X, transform.position.y, transform.position.z);
        }
    }
}
