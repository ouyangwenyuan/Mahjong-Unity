using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideStep8 : MonoBehaviour, IGuideStep, IPointerUpHandler{
    public GameObject hand;

    GameObject level2;
    void Awake()
    {
        level2 = GameObject.Find("CanvasPath/Button/Level2");

        Camera uicamera = Camera.main;
        Transform t = level2.transform;

        
        Image image = GetComponent<Image>();

        UIUtil.ShowHighLightCircle(uicamera, image, t, 80f, "_Center", "_Silder");

        UIUtil.CloseHighLightCircle(image, "_CenterB", "_SilderB");

        //Canvas canvas = GameObject.Find("CanvasFront").gameObject.GetComponent<Canvas>();

        //UIUtil.ShowArrow(uicamera, canvas, t, hand.transform, 0f, 300f);
         
    }
    public void Init(CardSeatMonoHandler card1, CardSeatMonoHandler card2) { }
    public void Execute() { }

    bool ClickEvent(GameObject go)
    {
        if (go == level2)
        {
            LocalDynamicData.GetInstance().SetGuideStep8(1);

            return true;
        }
        return false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool b = UIUtil.PassEvent(eventData, ExecuteEvents.pointerClickHandler, ClickEvent, true);

        if (b) GameObject.Destroy(gameObject);
    }
}
