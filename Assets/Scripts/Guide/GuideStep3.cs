using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideStep3 : MonoBehaviour, IGuideStep, IPointerUpHandler{
    public GameObject hand;

    GameObject redraw;
    void Awake()
    {
        redraw = GameObject.Find("CanvasFront/Button_Refresh");

        Camera uicamera = Camera.main;
        Image image = GetComponent<Image>();

        Transform t = redraw.transform;

        UIUtil.ShowHighLightCircle(uicamera, image, t, 80f, "_Center", "_Silder");

        UIUtil.CloseHighLightCircle(image , "_CenterB", "_SilderB");

        Canvas canvas = GameObject.Find("CanvasFront").gameObject.GetComponent<Canvas>();

        FrontController front = GameObject.Find("CanvasFront").gameObject.GetComponent<FrontController>();
        front.OpenRedrawButton();

        UIUtil.ShowArrow(uicamera, canvas, t, hand.transform, 0f, 150f);

    }

    bool ClickEvent(GameObject go)
    {
        if (go == redraw)
        {
            //LocalDynamicData.GetInstance().SetGuideStep3(1);
            
            return true;
        }
        return false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool b = UIUtil.PassEvent(eventData, ExecuteEvents.pointerClickHandler , ClickEvent , true);

        if(b) GameObject.Destroy(gameObject);
    }

    public void Init(CardSeatMonoHandler card1, CardSeatMonoHandler card2) { }
    public void Execute() { }
}
