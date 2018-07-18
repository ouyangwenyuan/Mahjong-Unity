using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideStep2 : MonoBehaviour, IGuideStep
{
    public GameObject stage1;
    public GameObject stage2;
    public GameObject hand;
    public void Stage1() {
        stage1.SetActive(false);

        GameObject.Find("CanvasFront/Guide_Mask").GetComponent<Image>().enabled = true;

        stage2.SetActive(true);
        hand.SetActive(true);
    }

    public void Stage2()
    {
        LocalDynamicData.GetInstance().SetGuideStep2(1);

        GameObject.Find("CanvasFront/Guide_Mask").GetComponent<Image>().enabled = false;

        GameObject.Destroy(gameObject);
    }

    public void Init(CardSeatMonoHandler card1, CardSeatMonoHandler card2) { }
    public void Execute() { }
}
