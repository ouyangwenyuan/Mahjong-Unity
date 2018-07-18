using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager{

    IGuideStep guide_now;

    public static bool is_guide_now = false;

    public IGuideStep Guide_Now
    {
        get {
            return guide_now;
        }
        set {
            guide_now = value;
        }
    }

    static GuideManager manager;

    public static GuideManager GetInstance() {
        if (manager == null)
        {
            manager = new GuideManager();
        }

        return manager;
    }

    public void GuideMapInit(Transform tran)
    {
        if (tran == null)
        {
            return;
        }

        if (LocalDynamicData.GetInstance().guide_step8 == 0 && DynamicData.GetInstance().GetStagesDoneNum() == 1)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep8")) as GameObject;

            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }


    public void GuideLevelInit(Transform tran, CardSeatMonoHandler card1 = null, CardSeatMonoHandler card2 = null)
    {
        if(tran == null){
            return;
        }

        if (LocalDynamicData.GetInstance().guide_step1 == 0 && CommonData.current_level == 1)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep1")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep1>();

            Guide_Now.Init(card1, card2);
        }

        if (LocalDynamicData.GetInstance().guide_step2 == 0 && CommonData.current_level == 2)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep2")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep2>();
        }

        /*
        if (LocalDynamicData.GetInstance().guide_step3 == 0 && CommonData.current_level == 3)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep3")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep3>();
        }
        */

        if (LocalDynamicData.GetInstance().guide_step4 == 0 && CommonData.current_level == 4)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep4")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep4>();
        }

        /*
        if (LocalDynamicData.GetInstance().guide_step5 == 0 && CommonData.current_level == 5)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep5")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep5>();
        }
        */

        if (LocalDynamicData.GetInstance().guide_step6 == 0 && CommonData.current_level == 7)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep6")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep6>();
        }

        if (LocalDynamicData.GetInstance().guide_step7 == 0 && CommonData.current_level == 19)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep7")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep7>();
        }

        if (Guide_Now != null)
        {
            is_guide_now = true;
        }
    }

    public void GuideLevelOver(Transform tran, CardSeatMonoHandler card1 = null, CardSeatMonoHandler card2 = null)
    {
        if (tran == null)
        {
            return;
        }

        if (LocalDynamicData.GetInstance().guide_step3 == 0 && CommonData.current_level == 3)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep3")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep3>();
        }

        if (LocalDynamicData.GetInstance().guide_step5 == 0 && CommonData.current_level == 5)
        {
            GameObject go = Object.Instantiate(Resources.Load("Prefabs/Guide/GuideStep5")) as GameObject;
            go.transform.SetParent(tran);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.transform.localPosition = new Vector3(0f, 0f, 0f);

            Guide_Now = go.GetComponent<GuideStep5>();
        }

        if (Guide_Now != null)
        {
            is_guide_now = true;
        }
    }


    public IGuideStep GetCurrentGuide() {
        return Guide_Now;
    }
}
