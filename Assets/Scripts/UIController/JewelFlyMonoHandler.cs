using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelFlyMonoHandler : MonoBehaviour {

    public GameObject target;
    public bool is_broken;

    public GameObject go_fly;
    public GameObject go_broken;

    Vector3 v3_target;
    Vector3 v3;

    float TIME_LENGTH = 1f;

    float stepX = 1f;
    float stepY = 1f;

    float len_x = 0f;
    float len_y = 0f;

    bool fly = false;

    float time = 0f;

    Camera uicamera;

	// Use this for initialization
	void Start () {
        uicamera = Camera.main;
        Transform t = target.transform;

        float x = uicamera.WorldToScreenPoint(t.position).x;
        float y = uicamera.WorldToScreenPoint(t.position).y;

        v3_target = new Vector3(x, y, 0);


        fly = true;

        //Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        float x1 = uicamera.WorldToScreenPoint(transform.position).x;
        float y1 = uicamera.WorldToScreenPoint(transform.position).y;

        v3 = new Vector3(x1, y1, 0);

        float x_target = v3_target.x;
        float y_target = v3_target.y;

        len_x = (x_target - x1);
        len_y = (y_target - y1);

        stepX = len_x / TIME_LENGTH;
        stepY = len_y / TIME_LENGTH;

        time = 0f;

        go_fly.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        if (fly)
        {
            time += Time.deltaTime;

            if (time >= TIME_LENGTH)
            {
                fly = false;

                //Destroy(this);

                go_fly.SetActive(false);
                if(is_broken)
                    go_broken.SetActive(true);
                else
                    target.transform.Find("Image2").GetComponent<Image>().enabled = true;
            }
            else
            {
                float x = v3.x + (stepX * Time.deltaTime);
                float y = v3.y + (stepY * Time.deltaTime);

                v3.x = x;
                v3.y = y;

                float x1 = uicamera.ScreenToWorldPoint(new Vector3(x, y, 0f)).x;
                float y1 = uicamera.ScreenToWorldPoint(new Vector3(x, y, 0f)).y;

                transform.position = new Vector3(x1, y1, transform.position.z);

            }
        }
	}
}
