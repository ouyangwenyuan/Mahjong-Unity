using System;
using System.Collections.Generic;
using System.Linq;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.utils;
using Assets.Scripts.config;
using Assets.Scripts.FirebaseController;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Init : MonoBehaviour {

	// Use this for initialization
    public Text procesText;
    public Text tipDesc;
    public Transform ButtonTransform;
    //重复登录超时时间
    private float _loginTime = 5;
    //第一次进入超时时间
    public static float _loginTime2 = 10;
    private IEnumerable<Tips> _tipses;
    private int i = 1;
    //firebase出现超时立即结束登录
    public static bool LoginCompleted;
    public GameObject aa;
    void Start ()
	{
	    _tipses = StaticDataBaseService.GetInstance().GetTips();
        InvokeRepeating("setTips",0f,3f);
        CommonData.b_init = true;
        if (GameConfig.InitDebug)
        {
            Invoke("EnterScene", 3f);
        }
        else
        {
            NetDetectUtils.PingPass += PingPass;
            NetDetectUtils.Detect("http://firebase.google.com");
        }
	}

    void Bug()
    {
        
    }

    void PingPass(bool connectivity)
    {
        if (connectivity)
        {
            LoginController.Instance.StartLogin();
        }
        else
        {
            EnterScene();
        }
    }

    public void ReSetTimeout()
    {
        Debug.Log("老用户下载所有数据,重置超时时间 " + (_loginTime2 + 10));
        CancelInvoke("EnterScene");
        Invoke("EnterScene", _loginTime2+10);
    }

    void setTips()
    {
        int index = i%_tipses.Count();
        var firstOrDefault = _tipses.FirstOrDefault(x => x.id == index);
        if (firstOrDefault != null) tipDesc.text = firstOrDefault.name;
        i++;
    }

    void EnterScene()
    {
        DynamicDataBaseService.GetInstance();
        StaticDataBaseService.GetInstance();
        Debug.Log("进入游戏");
        if (LocalDynamicData.GetInstance().guide_step1 == 0)
        {
            CommonData.current_level = 1;
            SceneManager.LoadScene("Level1");
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void TipUp()
    {
        Vector3 preScale = ButtonTransform.localScale;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(ButtonTransform.DOScale(new Vector3(1, 1, 1), 0.05f));
        mySequence.Append(ButtonTransform.DOPunchScale(new Vector3(0.1f, 0.1f, preScale.z), 0.5f, 8));
    }

    public void TipDown()
    {
        if (ClickUtils.IsDoubleClick())
        {
            setTips();
            Vector3 preScale = ButtonTransform.localScale;
            ButtonTransform.DOScale(new Vector3(preScale.x - 0.1f, preScale.y - 0.1f, preScale.z), 0.1f);
        }
        else
        {
            Debug.Log("重复点击");
        }
    }

	// Update is called once per frame
	void Update () {
	    if (LoginCompleted)
	    {
	        EnterScene();
	    }
	}
}
