using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragCamera : MonoBehaviour {

    //public Transform camera_mid;

	// Use this for initialization
	void Start () {
        m_camera = GetComponent<Camera>();

        cam_transform = Camera.main.transform;

        //x = cam_transform.position.x;
        z = cam_transform.position.z;
	}

    Camera m_camera;
    Transform cam_transform;

    float x = 0f;
    float y = 0f;
    float z = 0f;

    float move_x = 0f;
    float pre_x = 0f;
    float now_x = 0f;

    float move_y = 0f;
    float pre_y = 0f;
    float now_y = 0f;

    bool follow = false;

    public static bool moving = false;

    float down_x = 0f;
    float down_y = 0f;

    float up_x = 0f;
    float up_y = 0f;

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            //Debug.Log("Down : " + Input.mousePosition.x.ToString());
            //Debug.Log("Down : " + Input.mousePosition.y.ToString());

            down_x = pre_x = Input.mousePosition.x;
            down_y = pre_y = Input.mousePosition.y;



            follow = true;
        }

        if (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            //Debug.Log("Move : " + Input.mousePosition.x.ToString());

            now_x = Input.mousePosition.x;

            if (now_x == pre_x)
            {
                move_x = 0f;
            }
            else {
                Vector3 v_1 = m_camera.ScreenToWorldPoint(new Vector3((now_x), 0, 0));
                Vector3 v_2 = m_camera.ScreenToWorldPoint(new Vector3((pre_x), 0, 0));

                move_x = v_1.x - v_2.x;
                move_x = 0f - move_x;

                if (System.Math.Abs(move_x) < 10)
                {
                    moving = false;
                }
                else{
                    moving = true;
                }
            }
            

            pre_x = now_x;

            ////////////////////////////////////////////////////

            now_y = Input.mousePosition.y;

            if (now_y == pre_y)
            {
                move_y = 0f;
            }
            else
            {
                Vector3 v_1 = m_camera.ScreenToWorldPoint(new Vector3(0, (now_y), 0));
                Vector3 v_2 = m_camera.ScreenToWorldPoint(new Vector3(0, (pre_y), 0));

                move_y = v_1.y - v_2.y;
                move_y = 0 - move_y;

                if (System.Math.Abs(move_y) < 10)
                {
                    moving = false;
                }
                else
                {
                    moving = true;
                }
            }


            pre_x = now_x;
            pre_y = now_y;
        }

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            //Debug.Log("Up : " + Input.mousePosition.x.ToString());

            //Debug.Log("Up : " + Input.mousePosition.x.ToString());
            //Debug.Log("Up : " + Input.mousePosition.y.ToString());

            up_x = Input.mousePosition.x;
            up_y = Input.mousePosition.y;

            follow = false;
            moving = false;

            move_x = 0f;
            pre_x = 0f;
            now_x = 0f;

            move_y = 0f;
            pre_y = 0f;
            now_y = 0f;

            if (!x_min_back && !x_max_back && !y_min_back && !y_max_back)
            {
                x = 0f;
                y = 0f;

                if (up_x > down_x)
                {
                    if(up_x - down_x > 10f)
                        x = -50;
                }
                else if (up_x < down_x)
                {
                    if (down_x - up_x > 10f)
                        x = 50f;
                }

                if (up_y > down_y)
                {
                    if (up_y - down_y > 10f)
                        y = -50;
                }
                else if (up_y < down_y)
                {
                    if (down_y - up_y > 10f)
                        y = 50f;
                }

                float pos_x = cam_transform.position.x + x;

                if (pos_x <= CommonData.CAMERA_MAP_BEGIN_X)
                {
                    x = CommonData.CAMERA_MAP_BEGIN_X - cam_transform.position.x;
                }

                if (pos_x >= CommonData.CAMERA_MAP_END_X)
                {
                    x = CommonData.CAMERA_MAP_END_X - cam_transform.position.x;
                }

                float pos_y = cam_transform.position.y + y;

                if (pos_y <= CommonData.CAMERA_MAP_BEGIN_Y)
                {
                    y = CommonData.CAMERA_MAP_BEGIN_Y - cam_transform.position.y;
                }

                if (pos_y >= CommonData.CAMERA_MAP_END_Y)
                {
                    y = CommonData.CAMERA_MAP_END_Y - cam_transform.position.y;
                }

                if (!Splash.IsIpad)
                {
                    MoveCamera(x, y);
                }
            }
            else{
                bool is_back = false;

                x = 0f;
                y = 0f;

                if(x_min_back){
                
                    is_back = true;

                    x = 50f;
                
                    x_min_back = false;

                }
            
                if(x_max_back){
                
                    is_back = true;

                    x = -50f;
                
                    x_max_back = false;

                }    

                if(y_min_back){
                
                    is_back = true;

                    y = 50f;

                    y_min_back = false;

                }
            
                if(y_max_back){

                    is_back = true;

                    y = -50f;

                    y_max_back = false;

                }

                if (is_back && !Splash.IsIpad)
                {
                    MoveCamera(x,y);
                }
            }
        }
	}

    void OnDisable()
    {
        follow = false;
        moving = false;

        move_x = 0f;
        pre_x = 0f;
        now_x = 0f;

        move_y = 0f;
        pre_y = 0f;
        now_y = 0f;
    }

    bool x_min_back = false;
    bool x_max_back = false;

    bool y_min_back = false;
    bool y_max_back = false;

    void LateUpdate() {
        if (follow)
        {
            x_min_back = false;
            x_max_back = false;

            y_min_back = false;
            y_max_back = false;

            var pos_x = AdapterScreen();

            float pos_y = cam_transform.position.y + move_y;

            if (pos_y <= CommonData.CAMERA_MAP_BEGIN_Y){
                pos_y = CommonData.CAMERA_MAP_BEGIN_Y;

                y_min_back = true;
            }

            if (pos_y >= CommonData.CAMERA_MAP_END_Y)
            {
                pos_y = CommonData.CAMERA_MAP_END_Y;

                y_max_back = true;
            }

            Vector3 v_target = new Vector3(pos_x, pos_y, z);

            cam_transform.position = v_target;
        }
    }

    private float AdapterScreen()
    {
        if (!Splash.IsIpad)
        {
            float pos_x = cam_transform.position.x + move_x;

            if (pos_x <= CommonData.CAMERA_MAP_BEGIN_X)
            {
                pos_x = CommonData.CAMERA_MAP_BEGIN_X;

                x_min_back = true;
            }

            if (pos_x >= CommonData.CAMERA_MAP_END_X)
            {
                pos_x = CommonData.CAMERA_MAP_END_X;

                x_max_back = true;
            }
            return pos_x;
        }
        return cam_transform.position.x;
    }

    void MoveCamera(float x , float y)
    {
        Vector3 v_target = new Vector3(cam_transform.position.x + x, cam_transform.position.y + y, z);

        Camera.main.transform.DOMove(v_target, 0.2f);
    }
}
