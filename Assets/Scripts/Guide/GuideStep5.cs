using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideStep5 : MonoBehaviour, IGuideStep, IPointerUpHandler{
    public GameObject hand;

    GameObject bomb;
    void Awake()
    {
        bomb = GameObject.Find("CanvasFront/Button_Bomb");

        Camera uicamera = Camera.main;
        Transform t = bomb.transform;

        Image image = GetComponent<Image>();

        UIUtil.ShowHighLightCircle(uicamera, image, t, 80f, "_Center", "_Silder");

        UIUtil.CloseHighLightCircle(image, "_CenterB", "_SilderB");

        Canvas canvas = GameObject.Find("CanvasFront").gameObject.GetComponent<Canvas>();

        FrontController front = GameObject.Find("CanvasFront").gameObject.GetComponent<FrontController>();
        front.OpenBombButton();

        UIUtil.ShowArrow(uicamera, canvas, t, hand.transform, 0f, 150f);
    }

    public void Init(CardSeatMonoHandler card1, CardSeatMonoHandler card2) { }
    public void Execute() { }

    bool ClickEvent(GameObject go)
    {
        if (go == bomb)
        {
            //LocalDynamicData.GetInstance().SetGuideStep5(1);

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
