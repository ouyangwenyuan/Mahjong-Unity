using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CUSTOM_EVENT_TYPE
{
    CHANGE_LIFE_NUM,
    CHANGE_GOLD_NUM,
    RESET_OBSTACLE_BUFF,
    GAME_RESULT,
    CUSTOM_EVENT_MAX
}

public abstract class CustomEventData
{
    public CUSTOM_EVENT_TYPE event_type = CUSTOM_EVENT_TYPE.CUSTOM_EVENT_MAX;
}

public class Event_ChangeLifeNum : CustomEventData
{
    //public CUSTOM_EVENT_TYPE event_type = CUSTOM_EVENT_TYPE.CHANGE_LIFE_NUM;

    public int life_num = 0;
}

public class Event_ChangeGoldNum : CustomEventData
{
    //public CUSTOM_EVENT_TYPE event_type = CUSTOM_EVENT_TYPE.CHANGE_GOLD_NUM;

    public int gold_num = 0;
}

public class Event_GameResult : CustomEventData
{
    //public CUSTOM_EVENT_TYPE event_type = CUSTOM_EVENT_TYPE.GAME_RESULT;

    public bool result = false;
}

public class Event_ResetObstacleBuff : CustomEventData
{
    //public CUSTOM_EVENT_TYPE event_type = CUSTOM_EVENT_TYPE.RESET_OBSTACLE_BUFF;

}

public delegate bool GuideClickEventDelegate(GameObject go);

public class CustomEventSystem //: MonoBehaviour 
{
    public delegate void CustomEventDelegate(CustomEventData ce);
    public CustomEventDelegate[] custom_event_delegate = new CustomEventDelegate[(int)CUSTOM_EVENT_TYPE.CUSTOM_EVENT_MAX];

    static CustomEventSystem instance;

    public static CustomEventSystem GetInstance()
    {
        if(instance == null){
            instance = new CustomEventSystem();
        }

        return instance;
    }

    

	// Use this for initialization
	//void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
