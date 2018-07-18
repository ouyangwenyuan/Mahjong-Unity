using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUtil //: MonoBehaviour 
{
    public static string ShowTimeHour(long t) {
        string time = "";

        int hour = (int)(t / 3600);

        if (hour < 10)
        {
            time += "0";
        }

        time += hour.ToString() + " : ";

        int minute = (int)((t - hour * 3600)/ 60);

        if (minute < 10)
        {
            time += "0";
        }

        time += minute.ToString() + " : ";

        int second = (int)((t - hour * 3600)  % 60);

        if (second < 10)
        {
            time += "0";
        }

        time += second.ToString();

        return time; 
    }

    public static string ShowTimeMinute(int t)
    {
        string time = "";

        int minute = (int)(t / 60);

        if (minute < 10)
        {
            time += "0";
        }

        time += minute.ToString() + " : ";

        int second = (int)((t - minute * 60) % 60);

        if (second < 10)
        {
            time += "0";
        }

        time += second.ToString();

        return time;
    }

    public static string GetDateDiff(string date_count_donw)
    {
        long duration = 0;
        long seconds_diff = 0;

        if (date_count_donw.Equals("Done"))
        {
            return date_count_donw;
        }

        DateTime date = Convert.ToDateTime(date_count_donw);

        long seconds_done = date.Ticks / 10000000 + duration;
        long seconds_now = DateTime.Now.Ticks / 10000000;

        if (seconds_now >= seconds_done) //计时完成
        {
            return "Done";
        }
        else {
            seconds_diff = seconds_done - seconds_now;
        }

        return seconds_diff.ToString();
    }

    public static Sprite GetSprite(string name){
        Texture2D texture_show = (Texture2D)Resources.Load(name);

        if (texture_show == null)
        {
            Debug.Log("Not Texture Name : " + name);

            return null;
        }

        Rect rect_show = new Rect(0, 0, texture_show.width, texture_show.height);
        Sprite s_show = Sprite.Create(texture_show, rect_show, new Vector2(0.5f, 0.5f));

        return s_show;
    }

    public static void CalculateCurve(int segment_num , CardFlyPoint p , List<Vector2> list)
    {
        if(list == null){
            return;
        }

        list.Clear();

        for (int i = 1; i <= segment_num; i++)
        {
            float t = i / (float)segment_num;
            
            Vector2 point = CalculateCubicBezierPoint(t, p.point_begin, 
                                                         p.point_middle, 
                                                         p.point_end);

            list.Add(point);
        }

    }

    public static Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    public static void ShowArrow(Camera uicamera, Canvas canvas, Transform t, Transform arrow_t, float adjust_x, float adjust_y)
    {
        float xh = uicamera.WorldToScreenPoint(t.position).x;
        float yh = uicamera.WorldToScreenPoint(t.position).y;

        Vector3 center_hand = new Vector3(xh + adjust_x, yh + adjust_y, 0f);

        RectTransform rectTransform = arrow_t as RectTransform;
        
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, center_hand, canvas.worldCamera, out pos))
        {
            rectTransform.anchoredPosition = pos;
        }
    }

    public static void ShowHighLightCircle(Camera uicamera , Image image , Transform t , float silder , string para1 , string para2) {
        float x = uicamera.WorldToScreenPoint(t.position).x * CommonData.ADJUST_WIDTH - CommonData.BASE_WIDTH / 2;
        float y = uicamera.WorldToScreenPoint(t.position).y * CommonData.ADJUST_HEIGHT - CommonData.BASE_HEIGHT / 2;

        Vector4 center = new Vector4(x, y, 0f, 0f);
        Material material = image.material;
        material.SetVector(para1, center);
        material.SetFloat(para2, 80f);
    }

    public static void CloseHighLightCircle(Image image , string para1, string para2)
    {
        Vector4 center = new Vector4(0f, 0f, 0f, 0f);
        Material material = image.material;
        material.SetVector(para1, center);
        material.SetFloat(para2, 0f);
    }

    public static bool PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function, GuideClickEventDelegate guide_event = null , bool go_execute = false) where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        //GameObject current = data.pointerCurrentRaycast.gameObject;

        for (int i = 0; i < results.Count; i++)
        {
            //ExecuteEvents.Execute(results[i].gameObject, data, function);
            //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。

            //if (current != results[i].gameObject)
            GameObject go = results[i].gameObject;

            if (guide_event != null)
            {
                bool b = guide_event(go);

                if (b)
                {
                    if (go_execute)
                    {
                        ExecuteEvents.Execute(go, data, function);
                    }

                    return true;
                }
            }
        }

        return false;
    }
}
