using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
public class CardMonoHandler : MonoBehaviour {

    public Button button;
    public Image image;
    public Text text;
    //[SerializeField]
    //[HideInInspector]
    public Text text_id;
    public Image icon_jewel;
    public Image icon_lock;
    public Image icon_key;

    public Image image_selected;
    public Image image_invalid;

    public GameObject go_crash;
    public GameObject go_selected;
    public GameObject go_tip;

    public ParticleSystem[] ps_crash;
    public ParticleSystem[] ps_selected;
    //public ParticleSystem[] ps_tip;

    public SkeletonAnimation tip_anim;

    public GameObject score;
    public Text score_text;

    public GameObject score_normal;
    public Text score_normal_text;

    public GameObject jewel_root;

    CardSeatMonoHandler card_seat;

    void Awake() {
        card_seat = transform.parent.gameObject.GetComponent<CardSeatMonoHandler>();
    }

    public void ChangeIconTagActive(bool isActive) {
        icon_jewel.gameObject.SetActive(isActive);
    }

    public void OnSelect() {
        card_seat.OnSelect();
    }
}
