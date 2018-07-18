using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLevel : MonoBehaviour {

    public int id;

    //bool is_lock = false;
    MainFrontController main_front_controller;
    //Text text_lv;
    //GameObject go_unlock;
    //GameObject go_lock;

    //List<Transform> stars;
    //Sprite star_show;
    //Sprite star_empty;

    void Awake() {

        //go_unlock = transform.Find("Unlock").gameObject;
        //go_lock = transform.Find("Lock").gameObject;

        //stars = new List<Transform>();
        //Transform star;
        for (int i = 1; i <= 3; i++) {
            //star = go_unlock.transform.Find("star" + i);
            //stars.Add(star);
        }

        main_front_controller = GameObject.Find("CanvasFront").GetComponent<MainFrontController>();
        //text_lv = go_unlock.transform.Find("Text_Lv").GetComponent<Text>();
        //text_lv.text = id.ToString();

        int level_done_num = LocalDynamicData.GetInstance().GetLevelDoneNum();

        if (id > level_done_num && id != 1) {
            //is_lock = true;
        }

        /*
        if (is_lock)
        {
            go_unlock.SetActive(false);
            go_lock.SetActive(true);
        }
        else {
            go_unlock.SetActive(true);
            go_lock.SetActive(false);
        }
        */

        //string name = CommonData.TEXTURE_MAP_STAR_SHOW;
        //Texture2D texture_show = (Texture2D)Resources.Load(name);
        //Rect rect_show = new Rect(0, 0, texture_show.width, texture_show.height);
        //star_show = Sprite.Create(texture_show, rect_show, new Vector2(0.5f, 0.5f));

        //name = CommonData.TEXTURE_MAP_STAR_EMPTY;
        //Texture2D texture_empty = (Texture2D)Resources.Load(name);
        //Rect rect_empty = new Rect(0, 0, texture_empty.width, texture_empty.height);
        //star_empty = Sprite.Create(texture_empty, rect_empty, new Vector2(0.5f, 0.5f));

        int star_num = 0;
        float best_time = 0;

        LocalDynamicData.GetInstance().GetLevelDoneStarNBestTime(id ,out star_num , out best_time);

        ShowStars(star_num);
    }

    public void ShowStars(int starcount) {
        /*
        for (int i = 0; i < stars.Count; i++) {
            if (i < starcount)
            {
                stars[i].GetComponent<Image>().sprite = star_show;
            }
            else {
                stars[i].GetComponent<Image>().sprite = star_empty;
            }
        }
         */
    }

    public void OnClick()
    {
        //if (!is_lock)
        {
            main_front_controller.OnLevelClick(id);

            //SceneManager.LoadScene("Level1");
        }
        //else
        {
            
        }
    }
    
}
