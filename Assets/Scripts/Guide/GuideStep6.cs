using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideStep6 : MonoBehaviour, IGuideStep{
    public GameObject hand;

    GameObject timer;
    void Awake()
    {
        timer = GameObject.Find("CanvasFront/Top_New/Figure");

        Camera uicamera = Camera.main;
        Transform t = timer.transform;

        Image image = GetComponent<Image>();

        UIUtil.ShowHighLightCircle(uicamera, image, t, 100f, "_Center", "_Silder");

        UIUtil.CloseHighLightCircle(image, "_CenterB", "_SilderB");

        Canvas canvas = GameObject.Find("CanvasFront").gameObject.GetComponent<Canvas>();

        UIUtil.ShowArrow(uicamera, canvas, t, hand.transform, 0f, -150f);
    }

    // Update is called once per frame
    //void Update () {

    //}

    public void OnClick() {
        LocalDynamicData.GetInstance().SetGuideStep6(1);
        GameObject.Destroy(gameObject);
    }

    public void Init(CardSeatMonoHandler card1, CardSeatMonoHandler card2) { }
    public void Execute() { }

}
