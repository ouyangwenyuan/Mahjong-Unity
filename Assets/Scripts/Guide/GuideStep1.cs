using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideStep1 : MonoBehaviour, IGuideStep, IPointerUpHandler
{
    public Image image;

    public GameObject stage1;
    public GameObject stage2;

    public GameObject hand1;
    public GameObject hand2;

    CardSeatMonoHandler card1;
    CardSeatMonoHandler card2;

    public void Init(CardSeatMonoHandler card_1, CardSeatMonoHandler card_2)
    {
        if (card_1 == null || card_2 == null)
        {
            Debug.Log("No Targets");
        }

        card1 = card_1;
        card2 = card_2;

        Camera uicamera = Camera.main;
        Transform t = card1.transform;
        UIUtil.ShowHighLightCircle(uicamera, image, t, 70f, "_Center", "_Silder");

        Transform t2 = card2.transform;
        UIUtil.ShowHighLightCircle(uicamera, image, t2, 70f, "_CenterB", "_SilderB");

        stage2.SetActive(false);

        Canvas canvas = GameObject.Find("CanvasFront").gameObject.GetComponent<Canvas>() ;

        UIUtil.ShowArrow(uicamera, canvas, card1.transform, hand1.transform , -100f , 0f);
        UIUtil.ShowArrow(uicamera, canvas, card2.transform, hand2.transform , -100f , 0f);
    }

    bool ClickEvent(GameObject go)
    {
        while (go != null)
        {
            if (go == card1.gameObject)
            {
                card1.OnSelect();

                return true;
            }

            if (go == card2.gameObject)
            {
                card2.OnSelect();

                return true;
            }

            go = (go.transform.parent != null) ? go.transform.parent.gameObject : null;
        }

        return false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIUtil.PassEvent(eventData, ExecuteEvents.pointerUpHandler, ClickEvent);
    }

    public void Execute() {
        image.enabled = false;
        stage1.SetActive(false);

        hand1.SetActive(false);
        hand2.SetActive(false);

        Invoke("OnStage2" , 0.7f);
    }

    void OnStage2() {
        image.enabled = true;
        stage2.SetActive(true);

        Transform t = card1.transform;
        UIUtil.CloseHighLightCircle(image,"_Center", "_Silder");

        Transform t2 = card2.transform;
        UIUtil.CloseHighLightCircle(image, "_CenterB", "_SilderB");
    }

    public void Done() {
        LocalDynamicData.GetInstance().SetGuideStep1(1);

        GameObject.Destroy(gameObject);
    }
}
