using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideStep7 : MonoBehaviour, IGuideStep{
    public GameObject hand;

    GameObject jewels;
    void Awake()
    {
        jewels = GameObject.Find("CanvasFront/Top_New/Target");

        Camera uicamera = Camera.main;
        Transform t = jewels.transform;

        Image image = GetComponent<Image>();

        UIUtil.ShowHighLightCircle(uicamera, image, t, 100f, "_Center", "_Silder");

        UIUtil.CloseHighLightCircle(image, "_CenterB", "_SilderB");

        Canvas canvas = GameObject.Find("CanvasFront").gameObject.GetComponent<Canvas>();

        UIUtil.ShowArrow(uicamera, canvas, t, hand.transform, 0f, -150f);
    }

    public void Init(CardSeatMonoHandler card1, CardSeatMonoHandler card2) { }
    public void Execute() { }

    public void OnClick() {
        LocalDynamicData.GetInstance().SetGuideStep7(1);
        GameObject.Destroy(gameObject);
    }

}
