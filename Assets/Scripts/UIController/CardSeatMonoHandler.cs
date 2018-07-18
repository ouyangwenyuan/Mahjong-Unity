using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;


public class CardSeatMonoHandler : MonoBehaviour {

    public GameStage stage;

    public GameObject [] who_are_on_top_of_me;               //我的上面是XX XX是要填的值
    public GameObject [] who_are_on_my_left;             //我的左边是XX
    public GameObject [] who_are_on_my_right;            //我的右边是XX

    List<GameObject> tmp_who_are_on_top_of_me = new List<GameObject>();
    List<GameObject> tmp_who_are_on_my_left = new List<GameObject>();
    List<GameObject> tmp_who_are_on_my_right = new List<GameObject>();          

    public GameObject [] who_are_under_me_be_hindered;    //我的下面有被我压着的XX    
    public GameObject [] who_are_on_my_left_be_hindered;    //我的左边有被我挡着的XX
    public GameObject [] who_are_on_my_right_be_hindered;   //我的右边有被我挡着的XX

    public string ID;

    private bool are_someguy_on_top_of_me;          //上面是否有东西挡着我
    private bool are_someguy_on_my_left;        //左边是否有东西挡着我
    private bool are_someguy_on_my_right;       //右边是否有东西挡着我

    public bool is_taken = false;       //是否生成了

    public string card_type ;//= CARD_TYPE.yibing;

    public bool is_chocolate = false;                               //是否是巧克力关键牌

    public JEWEL_TYPE jewel_type = JEWEL_TYPE.NONE;           //收集牌类型

    public LOCK_TYPE lock_type = LOCK_TYPE.NONE;                    //锁类型
    public KEY_TYPE key_type = KEY_TYPE.NONE;                       //钥匙

    public CardSeatMonoHandler who_i_unlock;

    public bool is_lock_pos = false;
    public bool is_key_pos = false;

    //public CardSeatMonoHandler card_key;

    CARD_STATUS status = CARD_STATUS.INVAILD;
    private CardMonoHandler card;

    public bool is_left = false;

    public int index_sort = 0;

    public DEATH_POS_TYPE death_pos_type;
    public int death_pos_index = 0;
    public int death_pos_x_sub_index = 0;
    public bool is_death_pos = false;

    public GameObject jewel_target;
    public bool is_jewel_broken = true;

    public CardMonoHandler GetCard() {
        return card;
    }

    public CARD_STATUS GetStatus()
    {
        return status;
    }

    void Awake() {
        card = gameObject.GetComponentInChildren<CardMonoHandler>();

        card.ps_crash = card.go_crash.GetComponentsInChildren<ParticleSystem>();
        card.ps_selected = card.go_selected.GetComponentsInChildren<ParticleSystem>();
        //card.ps_tip = card.go_tip.GetComponentsInChildren<ParticleSystem>();

        card.tip_anim = card.go_tip.GetComponentInChildren<SkeletonAnimation>();

        ID = gameObject.name;

        card.text_id.enabled = false;

        if (AreSomeGuysOnTopOfMe())
        {
            are_someguy_on_top_of_me = true;
        }

        if (AreSomeGuysOnMyLeft())
        {
            are_someguy_on_my_left = true;
        }

        if (AreSomeGuysOnMyRight())
        {
            are_someguy_on_my_right = true;
        }

        BackUp();
    }

    public void SetChocolate() {
        is_chocolate = true;
        card.image.sprite = AtlasManager.GetInstance().GetCardSprite("chocolate");
        //card.image.sprite = Resources.Load<Sprite>("Texture/Cards/chocolate");

        if (IsVaild())
        {

            ChangeStatus(CARD_STATUS.VAILD);
        }
        else
        {

            ChangeStatus(CARD_STATUS.INVAILD);
        }
    }

    public bool IsVaild() {
        if(are_someguy_on_top_of_me){
            return false;
        }

        if(are_someguy_on_my_left&&are_someguy_on_my_right){
            return false;
        }

        return true;
    }

    public void ChangeStatus(CARD_STATUS _status) {

        status = _status;

        if(status == CARD_STATUS.INVAILD){            
            card.image_invalid.enabled = true;

            card.button.targetGraphic = card.image_invalid;

            card.text.enabled = false;
        }
        else if(status == CARD_STATUS.VAILD){            
            card.image_invalid.enabled = false;
            
            card.text.enabled = false;
        }
        else if (status == CARD_STATUS.SELECTED)
        {
            card.image_selected.enabled = true;

            /*
            for (int i = 0; i < card.ps_selected.Length; i++)
            {
                card.ps_selected[i].Play();
            }
            */
            card.text.enabled = true;
        }
        else if (status == CARD_STATUS.UNSELECTED)
        {
            card.image_selected.enabled = false;

            card.text.enabled = false;
        }
        else if (status == CARD_STATUS.FLY)
        {
            //card.image_selected.enabled = false;

            card.text.enabled = false;
            card.button.enabled = false;

            DisableTipAnim();

            OnDone();
        }
        else if (status == CARD_STATUS.DONE)
        {
            //stage.UpdatePowerSound();

            card.image_selected.enabled = false;

            card.text_id.enabled = false;
            card.icon_jewel.enabled = false;
            card.text.enabled = false;
            card.image.enabled = false;

            card.go_tip.SetActive(false);

            if (is_left)
            {
                //stage.front_controller.audio
                
                AudioSourcesManager.GetInstance().Play(stage.front_controller.level_audio, (stage.front_controller.audioclip_set == null) ? null : stage.front_controller.audioclip_set.card_remove);

                for (int i = 0; i < card.ps_crash.Length; i++)
                {
                    card.ps_crash[i].Play();
                }

                if (!stage.timer_level)
                {
                    if (stage.power > 1)
                    {
                        card.score.SetActive(true);
                        card.score_text.text = stage.score_match.ToString();
                    }
                    else
                    {
                        card.score_normal.SetActive(true);
                        card.score_normal_text.text = stage.score_match.ToString();
                    }
                }
            }

            if (jewel_type == JEWEL_TYPE.TYPE_CLUB)
            {
                SetJewelFly("Effect/Prefab/Inside_UI_gem_fly_broken_club");
            }

            if (jewel_type == JEWEL_TYPE.TYPE_DIAMOND)
            {
                SetJewelFly("Effect/Prefab/Inside_UI_gem_fly_broken_diamond");
            }

            if (jewel_type == JEWEL_TYPE.TYPE_HEART)
            {             
                SetJewelFly("Effect/Prefab/Inside_UI_gem_fly_broken_heart");
            }

            if (jewel_type == JEWEL_TYPE.TYPE_SPADE)
            {
                SetJewelFly("Effect/Prefab/Inside_UI_gem_fly_broken_spade");
            }
        }
    }

    void SetJewelFly(string prefab_name) {
        //Debug.Log("ID : " + ID);

        GameObject go = Instantiate(Resources.Load(prefab_name)) as GameObject;
        go.transform.SetParent(card.jewel_root.transform);
        go.transform.localPosition = new Vector3(0f,0f,0f);

        JewelFlyMonoHandler fly = go.GetComponent<JewelFlyMonoHandler>();
        fly.target = jewel_target;
        fly.is_broken = is_jewel_broken;

        AudioSourcesManager.GetInstance().Play(stage.front_controller.audio_jewel_fly, (stage.front_controller.audioclip_set == null) ? null : stage.front_controller.audioclip_set.diamond_fly);

        stage.CheckJewelsStatus();
    }

    public bool WhoOnTopMe() {
        if (who_are_on_top_of_me == null)
        {
            return false;
        }

        for (int i = 0; i < who_are_on_top_of_me.Length; i++ )
        {
            if (who_are_on_top_of_me[i] != null)
            {
                return true;
            }
        }

        return false;
    }

    public void Init(string type) {
        card_type = type;

        card.image.sprite = AtlasManager.GetInstance().GetCardSprite(card_type);
        //card.image.sprite = Resources.Load<Sprite>("Texture/Cards/" + card_type);

        if (IsVaild())
        {

            ChangeStatus(CARD_STATUS.VAILD);
        }
        else
        {

            ChangeStatus(CARD_STATUS.INVAILD);
        }
    }

    public void SetButtonStatus(bool b) {
        card.button.enabled = b;
    }

    public void ResetType(string type)
    {
        card_type = type;

        card.image.sprite = AtlasManager.GetInstance().GetCardSprite(card_type);
        //card.image.sprite = Resources.Load<Sprite>("Texture/Cards/" + card_type);
    }

    public bool CanMove()
    {
        if (!AreSomeGuysOnTopOfMe() && (!AreSomeGuysOnMyLeft() || !AreSomeGuysOnMyRight()))
            return true;
        else
            return false;

    }

    public bool AreSomeGuysOnTopOfMe() {
        for (int i = 0; i < who_are_on_top_of_me.Length; i++)
        {
            if (who_are_on_top_of_me[i] != null)
                return true;
        }

        return false;
    }

    public bool AreSomeGuysOnMyLeft()
    {
        for (int i = 0; i < who_are_on_my_left.Length; i++)
        {
            if (who_are_on_my_left[i] != null)
                return true;
        }

        return false;
    }

    public bool AreSomeGuysOnMyRight()
    {
        for (int i = 0; i < who_are_on_my_right.Length; i++)
        {
            if (who_are_on_my_right[i] != null)
                return true;
        }

        return false;
    }

    public bool AreSomeGuysOnMyLeftTaken()
    {
        for (int i = 0; i < who_are_on_my_left.Length; i++)
        {
            if (who_are_on_my_left[i] != null && who_are_on_my_left[i].GetComponent<CardSeatMonoHandler>().is_taken)
                return true;
        }

        return false;
    }

    public bool AreSomeGuysOnMyRightTaken()
    {
        for (int i = 0; i < who_are_on_my_right.Length; i++)
        {
            if (who_are_on_my_right[i] != null && who_are_on_my_right[i].GetComponent<CardSeatMonoHandler>().is_taken)
                return true;
        }

        return false;
    }

    public bool CanMoveMiddle()
    {
        if (!AreSomeGuysOnMyLeft() || !AreSomeGuysOnMyRight())
        {
            return true;
        }
        else {
            if (!AreSomeGuysOnMyLeftTaken() || !AreSomeGuysOnMyRightTaken())
            {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public bool IsRelyUp(CardSeatMonoHandler card)
    {
        bool b1 = false,b2 = false;

        for (int i = 0 ; i < who_are_on_top_of_me.Length ; i++){
            if (who_are_on_top_of_me[i] == card) {
                b1 = true;
                break;
            }
        }

        for (int i = 0; i < who_are_under_me_be_hindered.Length; i++){
            if (who_are_under_me_be_hindered[i] == card)
            {
                b2 = true;
                break;
            }
        }

        if (b1 || b2)
            return true;
        else
            return false;
    }

    public void RecoverRely()
    {
        for (int i = 0; i < who_are_on_top_of_me.Length; i ++ )
        {
            who_are_on_top_of_me[i] = tmp_who_are_on_top_of_me[i];
        }

        for (int i = 0; i < who_are_on_my_left.Length; i++)
        {
            who_are_on_my_left[i] = tmp_who_are_on_my_left[i];
        }

        for (int i = 0; i < who_are_on_my_right.Length; i++)
        {
            who_are_on_my_right[i] = tmp_who_are_on_my_right[i];
        }
    }

    public void RemoveRely()
    {
        CardSeatMonoHandler card_seat;

        for (int i = 0; i < who_are_under_me_be_hindered.Length; i++) {
            if (who_are_under_me_be_hindered[i] != null)
            {
                card_seat = who_are_under_me_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                if (card_seat == null)
                {
                    continue;
                }
                
                for (int j = 0; j < card_seat.who_are_on_top_of_me.Length; j++) {
                    if (card_seat.who_are_on_top_of_me[j] == this.gameObject) {
                        card_seat.who_are_on_top_of_me[j] = null;

                        break;
                    }
                }
            }
        }

        for (int i = 0; i < who_are_on_my_left_be_hindered.Length; i++)
        {
            if (who_are_on_my_left_be_hindered[i] != null)
            {
                card_seat = who_are_on_my_left_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                if (card_seat == null)
                {
                    continue;
                }

                for (int j = 0; j < card_seat.who_are_on_my_right.Length; j++)
                {
                    if (card_seat.who_are_on_my_right[j] == this.gameObject)
                    {
                        card_seat.who_are_on_my_right[j] = null;

                        break;
                    }
                }
            }
        }

        for (int i = 0; i < who_are_on_my_right_be_hindered.Length; i++)
        {
            if (who_are_on_my_right_be_hindered[i] != null)
            {
                card_seat = who_are_on_my_right_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                if (card_seat == null)
                {
                    continue;
                }

                for (int j = 0; j < card_seat.who_are_on_my_left.Length; j++)
                {
                    if (card_seat.who_are_on_my_left[j] == this.gameObject)
                    {
                        card_seat.who_are_on_my_left[j] = null;

                        break;
                    }
                }
            }
        }

    }

     public void BackUp() {
        tmp_who_are_on_top_of_me.Clear();
        tmp_who_are_on_my_left.Clear();
        tmp_who_are_on_my_right.Clear();

        for (int i = 0; i < who_are_on_top_of_me.Length; i++)
        {
            tmp_who_are_on_top_of_me.Add(who_are_on_top_of_me[i]);
        }

        for (int i = 0; i < who_are_on_my_left.Length; i++)
        {
            tmp_who_are_on_my_left.Add(who_are_on_my_left[i]);
        }

        for (int i = 0; i < who_are_on_my_right.Length; i++)
        {
            tmp_who_are_on_my_right.Add(who_are_on_my_right[i]);
        }

    }

    public void ClearUp(GameObject go) {

        for (int i = 0; i < who_are_on_top_of_me.Length; i++ )
        {
            if (who_are_on_top_of_me[i] == go) {
                who_are_on_top_of_me[i] = null;
            }
        }

        are_someguy_on_top_of_me = AreSomeGuysOnTopOfMe();

        if (status == CARD_STATUS.INVAILD && IsVaild())
        {
            ChangeStatus(CARD_STATUS.VAILD);
        }
    }

    public void ClearLeft(GameObject go){

        for (int i = 0; i < who_are_on_my_left.Length; i++)
        {
            if (who_are_on_my_left[i] == go)
            {
                who_are_on_my_left[i] = null;
            }
        }

        are_someguy_on_my_left = AreSomeGuysOnMyLeft();

        if (status == CARD_STATUS.INVAILD && IsVaild())
        {
            ChangeStatus(CARD_STATUS.VAILD);
        }
    }

    public void ClearRight(GameObject go)
    {
        for (int i = 0; i < who_are_on_my_right.Length; i++)
        {
            if (who_are_on_my_right[i] == go)
            {
                who_are_on_my_right[i] = null;
            }
        }

        are_someguy_on_my_right = AreSomeGuysOnMyRight();

        if (status == CARD_STATUS.INVAILD && IsVaild())
        {
            ChangeStatus(CARD_STATUS.VAILD);
        }
    }

    public void ClearLeftHindered(GameObject go)
    {

        for (int i = 0; i < who_are_on_my_left_be_hindered.Length; i++)
        {
            if (who_are_on_my_left_be_hindered[i] == go)
            {
                who_are_on_my_left_be_hindered[i] = null;
            }
        }
    }

    public void ClearRightHindered(GameObject go)
    {
        for (int i = 0; i < who_are_on_my_right_be_hindered.Length; i++)
        {
            if (who_are_on_my_right_be_hindered[i] == go)
            {
                who_are_on_my_right_be_hindered[i] = null;
            }
        }
    }

    //牌从无效变有效
    public void BeVaild() {
        CardSeatMonoHandler card_seat;

        for (int i = 0; i < who_are_under_me_be_hindered.Length; i++)
        {
            if (who_are_under_me_be_hindered[i] != null)
            {
                card_seat = who_are_under_me_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                card_seat.ClearUp(gameObject);
            }
        }

        for (int i = 0; i < who_are_on_my_left.Length; i++)
        {
            if (who_are_on_my_left[i] != null)
            {
                card_seat = who_are_on_my_left[i].GetComponent<CardSeatMonoHandler>();

                card_seat.ClearRightHindered(gameObject);
            }
        }

        for (int i = 0; i < who_are_on_my_right.Length; i++)
        {
            if (who_are_on_my_right[i] != null)
            {
                card_seat = who_are_on_my_right[i].GetComponent<CardSeatMonoHandler>();

                card_seat.ClearLeftHindered(gameObject);
            }
        }

        are_someguy_on_top_of_me = false;
        are_someguy_on_my_left = false;
        are_someguy_on_my_right = false;   

        ChangeStatus(CARD_STATUS.VAILD);
    }

    public void OnDone() {
        CardSeatMonoHandler card_seat;

        for (int i = 0; i < who_are_under_me_be_hindered.Length; i++ )
        {
            if (who_are_under_me_be_hindered[i] != null) {
                card_seat = who_are_under_me_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                card_seat.ClearUp(gameObject);
            }
        }

        for (int i = 0; i < who_are_on_my_left_be_hindered.Length; i++)
        {
            if (who_are_on_my_left_be_hindered[i] != null)
            {
                card_seat = who_are_on_my_left_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                card_seat.ClearRight(gameObject);
            }
        }

        for (int i = 0; i < who_are_on_my_right_be_hindered.Length; i++)
        {
            if (who_are_on_my_right_be_hindered[i] != null)
            {
                card_seat = who_are_on_my_right_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                card_seat.ClearLeft(gameObject);
            }
        }
    }

    public void OnSelect() {

        if(lock_type != LOCK_TYPE.NONE){
            return;
        }

        if (stage.pause)
            return;

        if (status == CARD_STATUS.INVAILD)
        {
            AudioSourcesManager.GetInstance().Play(stage.front_controller.audio_card_click, (stage.front_controller.audioclip_set == null) ? null : stage.front_controller.audioclip_set.click_card_invalid);
            return;
        }
        else {
            AudioSourcesManager.GetInstance().Play(stage.front_controller.audio_card_click, (stage.front_controller.audioclip_set == null) ? null : stage.front_controller.audioclip_set.click_card_valid);
        }

        stage.OnSelect(ID);
    }

    public void HighLight() {
        /*
        for (int i = 0; i < card.ps_tip.Length; i++)
        {
            card.ps_tip[i].Play();
        }
         */

        card.tip_anim.enabled = true;

        Invoke("DisableTipAnim",2);
    }

    void DisableTipAnim() {
        card.tip_anim.enabled = false;
    }

    public void ShowLock(bool b) {
        card.icon_lock.enabled = b;

        if (b)
        {
            card.icon_lock.sprite = AtlasManager.GetInstance().GetCardSprite("det_lock" + ((int)lock_type).ToString());//UIUtil.GetSprite("Texture/UI/det_lock" + ((int)lock_type).ToString());
        }
    }

    public void ShowKey(bool b)
    {
        card.icon_key.enabled = b;

        if (b)
        {
            card.icon_key.sprite = AtlasManager.GetInstance().GetCardSprite("det_key" + ((int)key_type).ToString());//UIUtil.GetSprite("Texture/UI/det_key" + ((int)key_type).ToString());
        }
    }

    public void ShowJewel(bool b) {
        card.icon_jewel.enabled = b;

        if(b){
            string name = null;
            switch (jewel_type)
            {
                case JEWEL_TYPE.TYPE_SPADE:
                    name = "det_spade1";//"Texture/UI/det_spade1"; 
                    break;
                case JEWEL_TYPE.TYPE_DIAMOND:
                    name = "det_diamond1";//"Texture/UI/det_diamond1";
                    break;
                case JEWEL_TYPE.TYPE_HEART:
                    name = "det_heart1";//"Texture/UI/det_heart1";
                    break;
                case JEWEL_TYPE.TYPE_CLUB:
                    name = "det_club1";//"Texture/UI/det_club1";
                    break;
            }

            card.icon_jewel.sprite = AtlasManager.GetInstance().GetCardSprite(name);//UIUtil.GetSprite(name);
        }
    }

    void Update() {
        if (is_fly)
        {
            //timer += Time.deltaTime;

            //if (timer >= point_set.timer) {
            //    timer = 0;

            v = list[index];
            transform.localPosition = new Vector3(v.x, v.y, transform.localPosition.z);

            index += CommonData.fly_points_num / 21;

            if (index >= CommonData.fly_points_num)
            {
                is_fly = false;

                ChangeStatus(CARD_STATUS.DONE);
            }

            if (index + CommonData.fly_points_num / 21 >= CommonData.fly_points_num)
            {
                v = list[CommonData.fly_points_num - 1];
                transform.localPosition = new Vector3(v.x, v.y, transform.localPosition.z);
            }                
            //}
        }
    }

    Vector2 v;
    int index = 0;
    //float timer = 0f;

    //CardFlyPoint point_set;

    bool is_fly = false;

    List<Vector2> list = new List<Vector2>();

    public void Fly(CardFlyPoint p) {
        is_fly = true;

        //point_set = p;

        is_left = p.is_left;

        UIUtil.CalculateCurve(CommonData.fly_points_num, p, list);
    }
}
