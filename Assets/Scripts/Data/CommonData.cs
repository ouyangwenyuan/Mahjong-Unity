using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARD_STATUS
{
    INVAILD,    //不能点击
    VAILD,      //能点击
    SELECTED,   //选中
    UNSELECTED, //未选中
    FLY,        //被消除,开始起飞
    DONE        //成功消除
}

public enum FIGURE_TYPE     //剧情相关
{
    captain,
    mechanic,
    doctor,
    mate
}

public enum DEFAULT_NPC     //剧情相关
{
    Sandy,
    Tom,
    Joy,
    userdefault
}

public enum LEVEL_TYPE
{
    level,                  //普通关卡
    cave                    //洞穴关卡
}

public enum STAGE_TYPE 
{
    KILL_ALL = 1,                         //消除所有牌
    KILL_CHOCOLATE,                             //消除关键牌
    COLLECT_KILL_ALL,                     //收集并消除全部麻将
    COLLECT_KILL_CHOCOLATE,                     //收集并消除全部巧克力
    KILL_ALL_TIMER,                       //消除所有牌在倒计时时间内
    KILL_CHOCOLATE_TIMER,                       //消除关键牌在倒计时时间内
    COLLECT_KILL_ALL_TIMER,               //收集并消除全部麻将在倒计时时间内
    COLLECT_KILL_CHOCOLATE_TIMER                //收集并消除全部巧克力在倒计时时间内
}

public enum CARD_SHOW_TYPE  //关卡生成算法
{
    TEST1,
    TEST2,
    TEST3,
    TEST4
}

public enum CARD_REFRESH_TYPE   //洗牌算法
{
    TEST2,
    TEST3,
    TEST4
}

public enum CARD_TYPE_SEASON    //四季牌
{
    s_spring,//chun,
    s_summer,//xia,
    s_autumn,//qiu,
    s_winter//dong
}

public enum CARD_TYPE_PLANT     //植物牌
{
    f_orchid,//lan,
    f_mum,//ju,
    f_plum,//mei,
    f_bamboo//zhu
}

public enum JEWEL_TYPE
{
    NONE = 0,
    TYPE_SPADE,
    TYPE_HEART,
    TYPE_CLUB,
    TYPE_DIAMOND
}

public enum LOCK_TYPE
{
    NONE = 0,
    TYPE_1,
    TYPE_2,
    TYPE_3,
    TYPE_4,
    TYPE_5
}

public enum KEY_TYPE
{
    NONE = 0,
    TYPE_1,
    TYPE_2,
    TYPE_3,
    TYPE_4,
    TYPE_5
}

public enum CARD_TYPE
{
    pin_1 = 0,//yibing = 0, //饼
    pin_2,//erbing,
    pin_3,//sanbing,
    pin_4,//sibing,
    pin_5,//wubing,
    pin_6,//liubing,
    pin_7,//qibing,
    pin_8,//babing,
    pin_9,//jiubing,
    //
    w_north,//beifeng,    //风
    w_west,//xifeng,
    w_south,//nanfeng,
    w_east,//dongfeng,
    d_red,//zhong,
    d_green,//fa,
    d_white,//baipi,      //白皮
    //
    wan_1,//yiwan,      //万
    wan_2,//erwan,
    wan_3,//sanwan,
    wan_4,//siwan,
    wan_5,//wuwan,
    wan_6,//liuwan,
    wan_7,//qiwan,
    wan_8,//bawan,
    wan_9,//jiuwan,
    //
    sow_1,//yitiao,     //条
    sow_2,//ertiao,
    sow_3,//santiao,
    sow_4,//sitiao,
    sow_5,//wutiao,
    sow_6,//liutiao,
    sow_7,//qitiao,
    sow_8,//batiao,
    sow_9,//jiutiao,
    //
    //chocolate,            //巧克力
    //
    plant,      //植物
    season,     //四季

    max
}

public enum DEATH_POS_TYPE
{
    NONE,
    I,
    X
}


public enum AUDIO_TYPE { 
    MUSIC,
    SFX
}

public enum FAIL_CHANCE_TYPE { 
    NONE,
    REDRAW,
    BOMB,
    ADD_TIME,
    COLLECT_JEWELS,
}

public class CardFlyPoint {
    public Vector2 point_begin;
    public Vector2 point_middle;
    public Vector2 point_end;
    public float timer;

    public bool is_left = false;
}

public class CommonData
{
    public static bool b_init = false;

    // 当前最高游戏关卡
    public const string BEST_LEVEL = "BestLevel";
    // 体力值
    public const string POWER = "Heart";
    // 目标时间戳
    public const string AIM_TIME = "AimTime";
    public static float delta_time = -1;

    public static List<string> season_list;
    public static List<string> plant_list;
    // 当前所在关卡
    public static string loading_target_scene = "Main";
    public static int current_level = 1;
    public static int cave_level;
    public static LEVEL_TYPE current_level_type = LEVEL_TYPE.level;
    public const int HEART_MAX = 5;
    public static int BASE_WIDTH = 720;//Screen.width;
    public static int BASE_HEIGHT = 1280;//Screen.height;

    public static float ADJUST_WIDTH = 0f;//Screen.width;
    public static float ADJUST_HEIGHT = 0f;//Screen.height;

    public const float SHOW_SUCCESS_FAIL_TIME = 1f;
    // 体力恢复时间
    public const int HEART_COOLING_TIME = 30;

    //是否闯关成功
    public static bool is_success_level = false;

    //增加时间技能 增加的时间
    public static float ADD_TIME = 30f;

    //纹理路径
    public const string TEXTURE_MAP_STAR_SHOW = "Texture/Atlas/star";
    public const string TEXTURE_MAP_STAR_EMPTY = "Texture/Atlas/star_empty";
    public const string MAHJONG_TILE_SET = "Texture/Atlas/smooth";
    public const string FIGURE_PATH = "Texture/figure/";
    public const string CHOCOLATE_PATH = "Texture/Cards/chocolate";
    public const string DGREEN_PATH = "Texture/Cards/d_green";
    public const string IMG_PROP_PATH = "Texture/UI/";

    //牌飞行
    public const float fly_radius = 34f;
    public const float fly_speed = 20000f;
    public const int fly_points_num = 200;

    public const float CAMERA_MAP_BEGIN = -58f;
    public const float CAMERA_MAP_END = 6268f;

    public const float CAMERA_MID_MAP_BEGIN = 823f;
    public const float CAMERA_MID_MAP_END = 4582f;

    public const float CAMERA_MAP_ADJUST = 160f;

    public const float CAMERA_MAP_BEGIN_X = -2224f;
    public const float CAMERA_MAP_END_X = -2031f;

    public const float CAMERA_MAP_BEGIN_Y = -136f;
    public const float CAMERA_MAP_END_Y = 1907f;

    public static AudioClip level_clip = null;

    public const string LEVEL_PATH = "CanvasPath/Button/Level";
    public const float HEAD_FLY_TIME = 1f;

    public static bool ShowTimer(STAGE_TYPE  type) {
        if (type == STAGE_TYPE.KILL_ALL_TIMER 
            || type == STAGE_TYPE.KILL_CHOCOLATE_TIMER 
            || type == STAGE_TYPE.COLLECT_KILL_ALL_TIMER 
            || type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE_TIMER)
        {
            return true;
        }

        return false;
    }

    public static bool ShowDiamonds(STAGE_TYPE type)
    {
        if (type == STAGE_TYPE.COLLECT_KILL_ALL
            || type == STAGE_TYPE.COLLECT_KILL_ALL_TIMER
            || type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE
            || type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE_TIMER)
        {
            return true;
        }

        return false;
    }

    public static List<int> ShowDiamondsOrder(string order)
    {
        List<int> orders = new List<int>();

        string[] sArray = order.Split('|');

        for (int i = 0; i < sArray.Length; i++) {
            int j = Convert.ToInt32(sArray[i]);

            orders.Add(j);
        }

        return orders;
    }

    public static bool IsSeason(CARD_TYPE_SEASON type)
    {
        if (type == CARD_TYPE_SEASON.s_autumn
            || type == CARD_TYPE_SEASON.s_spring
            || type == CARD_TYPE_SEASON.s_summer
            || type == CARD_TYPE_SEASON.s_winter)
        {
            return true;
        }

        return false;
    }

    public static bool IsPlant(CARD_TYPE_PLANT type)
    {
        if (type == CARD_TYPE_PLANT.f_bamboo
            || type == CARD_TYPE_PLANT.f_mum
            || type == CARD_TYPE_PLANT.f_orchid
            || type == CARD_TYPE_PLANT.f_plum)
        {
            return true;
        }

        return false;
    }

    public static bool IsChocolateLevel(STAGE_TYPE type) {
        if (type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE || type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE_TIMER || type == STAGE_TYPE.KILL_CHOCOLATE || type == STAGE_TYPE.KILL_CHOCOLATE_TIMER)
        {
            return true;
        }

        return false;
    }
}