using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDynamicData {
    static LocalDynamicData dynamic_data;

    public static LocalDynamicData GetInstance()
    {
        if (dynamic_data == null)
        {
            dynamic_data = new LocalDynamicData();
            dynamic_data.LoadData();
        }

        return dynamic_data;
    }

    private int heart = 0;
    private int gold = 0;
    private bool hint_on = true;
    private bool music_on = true;
    private bool sfx_on = true;
    private bool newbie_done = false;
    private bool ads_done = false;
    //最大生命购买次数
    public int maxlifeBuyTime = 0;
    private List<int> avatars = new List<int>();
    private int avatar_id = 0;
    private int avatars_count = 0;

    private int new_level = 0;
    private int level_done_num = 0;
    private List<int> levels_star = new List<int>();
    private List<float> levels_best_time = new List<float>();

    //private int first_revive = 0; // 还没有失败过 0 失败过了 1 

    public string count_down = "";    //开始的时间戳

    public int count_down_id;
    public int count_down_time;
    public int count_down_gold;

    public string aim_time_heart = "";

    public int guide_step1 = 0;
    public int guide_step2 = 0;
    public int guide_step3 = 0;
    public int guide_step4 = 0;
    public int guide_step5 = 0;
    public int guide_step6 = 0;
    public int guide_step7 = 0;
    public int guide_step8 = 0;
    public int guide_step9 = 0;

    private void LoadData() {
        //first_revive = PlayerPrefs.GetInt("FIRST_ADS");

        new_level = PlayerPrefs.GetInt("NEW_LEVEL");

        if(new_level == 0){
            new_level = 1;
        }

        aim_time_heart = PlayerPrefs.GetString("AIM_TIME_HEART");
        
        heart = PlayerPrefs.GetInt("HEART");
        gold = PlayerPrefs.GetInt("GOLD");
        hint_on = PlayerPrefs.GetInt("HINT_ON") == 0 ? true : false;
        music_on = PlayerPrefs.GetInt("MUSIC_ON") == 0?true:false;
        sfx_on = PlayerPrefs.GetInt("SFX_ON") == 0 ? true : false;
        newbie_done = PlayerPrefs.GetInt("NEWBIE_DONE") == 1 ? true : false;
        ads_done = PlayerPrefs.GetInt("ADS_DONE") == 1 ? true : false;

        avatar_id = PlayerPrefs.GetInt("CLOTH_ID");
        avatars_count = PlayerPrefs.GetInt("CLOTHS_COUNT");
        maxlifeBuyTime = PlayerPrefs.GetInt("MAXLIFE_BUY_NUM",0);

        for (int i = 1; i <= avatars_count; i++ )
        {
            int id = PlayerPrefs.GetInt("CLOTH_" + i.ToString());
            avatars.Add(id);
        }

        if (avatar_id == 0)  // 第一次游戏 默认
        {
            UpdateAvatarId(1);
            AddAvatarId(1);
        }

        level_done_num = PlayerPrefs.GetInt("LEVEL_DONE_NUM");

        for (int i = 1; i <= level_done_num; i++)
        {
            int star = PlayerPrefs.GetInt("LEVEL_STAR_" + i.ToString());
            float time = PlayerPrefs.GetFloat("LEVEL_BEST_TIME_" + i.ToString());

            levels_star.Add(star);
            levels_best_time.Add(time);
        }

        count_down = PlayerPrefs.GetString("COUNT_DOWN");

        count_down_id =  PlayerPrefs.GetInt("COUNT_DOWN_ID");
        count_down_time = PlayerPrefs.GetInt("COUNT_DOWN_TIME");
        count_down_gold = PlayerPrefs.GetInt("COUNT_DOWN_GOLD");

        if (count_down.Equals(""))
        {
            SetCountDown(DateTime.Now.ToString());

            count_down_id = 1;

            //CommonData.GetGiftTime(count_down_id , out count_down_time , out count_down_gold);

            SetCountDownID(count_down_id);
            SetCountDownTime(count_down_time);
            SetCountDownGold(count_down_gold);
        }

        guide_step1 = PlayerPrefs.GetInt("GUIDE_STEP_1");
        guide_step2 = PlayerPrefs.GetInt("GUIDE_STEP_2");
        guide_step3 = PlayerPrefs.GetInt("GUIDE_STEP_3");
        guide_step4 = PlayerPrefs.GetInt("GUIDE_STEP_4");
        guide_step5 = PlayerPrefs.GetInt("GUIDE_STEP_5");
        guide_step6 = PlayerPrefs.GetInt("GUIDE_STEP_6");
        guide_step7 = PlayerPrefs.GetInt("GUIDE_STEP_7");
        guide_step8 = PlayerPrefs.GetInt("GUIDE_STEP_8");
        guide_step9 = PlayerPrefs.GetInt("GUIDE_STEP_9");
    }

    public void Clear() {
        heart = 0;
        gold = 0;
        hint_on = true;
        music_on = true;
        sfx_on = true;
        newbie_done = false;
        ads_done = false;

        avatars.Clear();
        avatar_id = 0;
        avatars_count = 0;

        level_done_num = 0;
        levels_star.Clear();
        levels_best_time.Clear();

        count_down = "";
    }

    /*
    public int GetFirstAds()
    {
        return first_revive;
    }

    public void SetFirstAds(int s)
    {
        first_revive = s;

        PlayerPrefs.SetInt("FIRST_ADS", s);
    }
    */

    public int GetNewLevel() {
        return new_level;
    }

    public void SetNewLevel(int n)
    {
        new_level = n;

        PlayerPrefs.SetInt("NEW_LEVEL", n);
    }

    public string GetAimTimeHeart()
    {
        return aim_time_heart;
    }

    public void SetAimTimeHeart(string a)
    {
        aim_time_heart = a;

        PlayerPrefs.SetString("AIM_TIME_HEART", a);
    }

    public int GetHeart()
    {
        return heart;
    }

    public void SetHeart(int h)
    {
        heart = h;

        PlayerPrefs.SetInt("HEART", h);
    }

    public string GetCountDown() {
        return count_down;
    }

    public void SetCountDown(string s) {
        count_down = s;

        PlayerPrefs.SetString("COUNT_DOWN", s);
    }

    public int GetCountDownID()
    {
        return count_down_id;
    }

    public void SetCountDownID(int b)
    {
        //if (b > 6) {
        //    b = 6;
        //}

        count_down_id = b;

        PlayerPrefs.SetInt("COUNT_DOWN_ID", b);
    }

    public int GetCountDownTime()
    {
        return count_down_time;
    }

    public void SetCountDownTime(int b)
    {
        count_down_time = b;

        PlayerPrefs.SetInt("COUNT_DOWN_TIME", b);
    }

    public int GetCountDownGold()
    {
        return count_down_gold;
    }

    public void SetCountDownGold(int b)
    {
        count_down_gold = b;

        PlayerPrefs.SetInt("COUNT_DOWN_GOLD", b);
    }

    public void ChangeHintOn()
    {
        hint_on = hint_on ? false : true;

        SetHintOn(hint_on);
    }

    public void ChangeMusicOn() {
        music_on = music_on ? false : true;

        SetMusicOn(music_on);

        AudioSourcesManager.GetInstance().ChangeStatus(music_on,AUDIO_TYPE.MUSIC);
    }

    public void ChangeSFXOn()
    {
        sfx_on = sfx_on ? false : true;

        SetSFXOn(sfx_on);

        AudioSourcesManager.GetInstance().ChangeStatus(sfx_on, AUDIO_TYPE.SFX);
    }

    public int GetGold()
    {
        return gold;
    }

    public void SetGold(int num ) {
    
        gold += num;

        PlayerPrefs.SetInt("GOLD",gold);
    }

    public void SetMaxLifeBuyNum(int num)
    {

        maxlifeBuyTime += num;

        PlayerPrefs.SetInt("MAXLIFE_BUY_NUM", maxlifeBuyTime);
    }

    public void SetHintOn(bool done)
    {
        hint_on = done;

        PlayerPrefs.SetInt("HINT_ON", hint_on ? 0 : 1);
    }

    public void SetMusicOn(bool done)
    {
        music_on = done;

        PlayerPrefs.SetInt("MUSIC_ON", music_on?0:1);
    }

    public void SetSFXOn(bool done)
    {
        sfx_on = done;

        PlayerPrefs.SetInt("SFX_ON", sfx_on ? 0 : 1);
    }

    public bool GetHintOn()
    {
        return hint_on;
    }

    public bool GetMusicOn() {
        return music_on;
    }

    public bool GetSFXOn()
    {
        return sfx_on;
    }

    public bool GetNewbieDone() {
        return newbie_done;
    }

    public void SetNewbieDone(bool done)
    {
        newbie_done = done;

        PlayerPrefs.SetInt("NEWBIE_DONE", newbie_done ? 1 : 0);
    }

    public void SetAdsDone(bool done)
    {
        ads_done = done;

        PlayerPrefs.SetInt("ADS_DONE", ads_done ? 1 : 0);
    }

    public void UpdateAvatarId(int id)
    {
        avatar_id = id;

        PlayerPrefs.SetInt("CLOTH_ID", id);
    }


    public void AddAvatarId(int id)
    {
        avatars.Add(id);
        avatars_count += 1;

        PlayerPrefs.SetInt("CLOTH_" + id.ToString(), id);
        PlayerPrefs.SetInt("CLOTHS_COUNT", avatars_count);
    }

    public int GetAvatarID() {
        return avatar_id;
    }

    public List<int> GetAvatarsList(){
        return avatars;
    }

    public void UpdateLevelData(int level_id , int level_star , float level_best_time) {

        levels_star[level_id - 1] = level_star;
        levels_best_time[level_id - 1] = level_best_time;

        PlayerPrefs.SetInt("LEVEL_STAR_" + level_id.ToString(), level_star);
        PlayerPrefs.SetFloat("LEVEL_BEST_TIME_" + level_id.ToString(), level_best_time);
    }

    public void AddLevelData(int level_id, int level_star, float level_best_time)
    {
        level_done_num += 1;

        levels_star.Add(level_star);
        levels_best_time.Add(level_best_time);

        PlayerPrefs.SetInt("LEVEL_DONE_NUM", level_done_num);
        UpdateLevelData(level_id, level_star, level_best_time);
    }

    public int GetLevelDoneNum() {
        return level_done_num;
    }

    public void GetLevelDoneStarNBestTime(int id , out int star , out float best_time ) {
        star = 0;
        best_time = 0f;

        if (id > level_done_num && id < 1)
        {
            return;
        }

        if (id > levels_star.Count) {
            return;
        }

        star = levels_star[id-1];
        best_time = levels_best_time[id-1];
    }

    public int GetAllStars() {
        int sum = 0;

        foreach (int i in levels_star) {
            sum += i;
        }

        return sum;
    }

    public void SetGuideStep1(int i) {
        guide_step1 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_1", i);
    }

    public void SetGuideStep2(int i)
    {
        guide_step2 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_2", i);
    }

    public void SetGuideStep3(int i)
    {
        guide_step3 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_3", i);
    }

    public void SetGuideStep4(int i)
    {
        guide_step4 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_4", i);
    }

    public void SetGuideStep5(int i)
    {
        guide_step5 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_5", i);
    }

    public void SetGuideStep6(int i)
    {
        guide_step6 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_6", i);
    }

    public void SetGuideStep7(int i)
    {
        guide_step7 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_7", i);
    }

    public void SetGuideStep8(int i)
    {
        guide_step8 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_8", i);
    }

    public void SetGuideStep9(int i)
    {
        guide_step9 = i;

        GuideManager.GetInstance().Guide_Now = null;
        GuideManager.is_guide_now = false;

        PlayerPrefs.SetInt("GUIDE_STEP_9", i);
    }
}