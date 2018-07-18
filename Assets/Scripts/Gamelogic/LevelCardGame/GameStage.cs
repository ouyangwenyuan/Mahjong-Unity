using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GameStage : MonoBehaviour {

    // 生成算法
    public List<CARD_SHOW_TYPE> gen_types;
    // 刷新算法
    public CARD_REFRESH_TYPE refresh_type = CARD_REFRESH_TYPE.TEST3;

    // 用Test4生成的一定可解路径。
    //List<CardSeatMonoHandler> card_ways = new List<CardSeatMonoHandler>();
    public FrontController front_controller;
    CardSeatMonoHandler[] card_seat_list = new CardSeatMonoHandler[] { };

    public List<CardSeatMonoHandler> all_list;

    public Stage stage;
    public STAGE_TYPE stage_type = STAGE_TYPE.KILL_ALL;

    public int cards_total_num = 0;
    public int cards_now_num = 0;
    public int score = 0;

    public int score_match = 0;

    //public int lock_key_num = 0;        //锁

    public int chocolate_num = 0;                             //巧克力
    public int chocolate_num_now = 0;
    
    public List<int> jewels_show = new List<int>();
    public List<int> jewels_set_pos = new List<int>();

    public List<int> lock_list = new List<int>();

    public int jewels_count = 0;
    public bool is_jewels_done = true;

    public bool pause = false;  //游戏暂停,不能点击

    public bool timer_level = false;
    public bool jewels_level = false;
    public bool locks_level = false;

    public bool timer_pause = false;
    public bool power_pause = false;
    public bool hint_pause = false;
    public float timer = 0f;

    public GameObject Root;

    public int power = 1;
    int power_conter = 0;
    int step = 0;
    bool is_power = false;
    float timer_power = 0f;

    float timer_hint = 0f;

    List<CardSeatMonoHandler> hint_pairs;
    //float hint_timer = 0f;

    bool is_hint = true;
    bool b_timer_audio = false;
    bool b_timer_audio_on = false;

    /////////////////////////////////////////////////////////////////
    //数据分析的变量
    public int star_num = 0;
    public int try_num = 0;
    public float time_cost = 0f;
    public int skill_refresh_num = 0;
    public int skill_bomb_num = 0;
    public int skill_add_timer_num = 0;
    public int skill_jewels_num = 0;
    public int skill_unlock_num = 0;

    public FAIL_CHANCE_TYPE fail_chance_type = FAIL_CHANCE_TYPE.NONE;

    bool level_done = false;

    void CalculateScore() {
        step++;

        score_match = 20 * power;

        score += score_match;// (int)Math.Pow(power, step);
    }

    void Awake()
    {
        is_hint = LocalDynamicData.GetInstance().GetHintOn();

        pause = false;

        front_controller = GameObject.Find("CanvasFront").GetComponent<FrontController>();
        card_seat_list = gameObject.GetComponentsInChildren<CardSeatMonoHandler>();

        all_list = new List<CardSeatMonoHandler>(card_seat_list);

        cards_total_num = card_seat_list.Length;

        stage = StaticData.GetInstance().GetStageByID(CommonData.current_level);
        stage_type = (STAGE_TYPE)stage.type;

        timer_level = CommonData.ShowTimer(stage_type);
        jewels_level = CommonData.ShowDiamonds(stage_type);

        timer = (float)stage.count_down;
        //lock_key_num = stage.lock_key;

        if (stage.lock_key != 0)
        {
            lock_list.Add(stage.lock_key);

            locks_level = true;
        }

        chocolate_num = stage.chocolate_cnt;

        jewels_show = CommonData.ShowDiamondsOrder(stage.collect_order);
        jewels_set_pos = CommonData.ShowDiamondsOrder(stage.jewel_all);
        jewels_count = jewels_show.Count;

        Highscore hs = DynamicData.GetInstance().GetHighScoreByID(CommonData.current_level);

        if (hs != null)
        {
            CardsProcess.GetInstance().SetDeathPosINum(hs.deadpoint_I);
            CardsProcess.GetInstance().SetDeathPosXNum(hs.deadpoint_X);
        }
        else {
            CardsProcess.GetInstance().SetDeathPosINum(stage.deadpoint_I);
            CardsProcess.GetInstance().SetDeathPosXNum(stage.deadpoint_X); 
        }
    }

	void Start () {
        Init();
	}

    void Update() {

        if (is_hint && timer_hint < 15f && !hint_pause)
        {
            timer_hint += Time.deltaTime;

            if (timer_hint >= 15f)
            {
                timer_hint = 0f;

                int sum = 0;

                if (hint_pairs != null)
                {
                    foreach (CardSeatMonoHandler card in hint_pairs)
                    {
                        sum++;
                        card.HighLight();

                        if (sum == 2)
                        {
                            break;
                        }
                    }
                }
            }
        }

        if (timer_level && timer > 0f && !timer_pause)
        {
            if (timer >= 0f)
            {
                timer -= Time.deltaTime;
            }

            if (timer <= 30f)
            {
                if (!b_timer_audio)
                b_timer_audio = true;
            }

            if (timer <= 0f)
            {
                Invoke("OnTimerLevelFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
            }

            front_controller.UpdateTimer((int)timer);
        }
        else {
            if(!power_pause){
                if (!is_power)
                {
                    if (timer_power >= 1f)
                    {
                        timer_power = 0f;

                        if (power_conter > 0)
                        {
                            power_conter--;

                            if (power_conter < 0)
                                power_conter = 0;

                            front_controller.image_hit.fillAmount = power_conter / 30f;
                        }
                    }
                    else
                    {
                        timer_power += Time.deltaTime;
                    }
                }
                else
                {
                    if (timer_power >= 1f)
                    {
                        timer_power = 0f;

                        if (power_conter > 0)
                        {
                            power_conter -= 3;

                            if (power_conter <= 0)
                            {
                                is_power = false;

                                power = 1;

                                front_controller.UpdateHit(power);

                                power_conter = 0;

                                front_controller.image_hit.fillAmount = power_conter / 30f;
                            }

                            front_controller.image_hit.fillAmount = power_conter / 30f;
                        }
                    }
                    else
                    {
                        timer_power += Time.deltaTime;
                    }
                }
            }
        }

        if(b_timer_audio && !level_done){
            if (!b_timer_audio_on) {
                b_timer_audio_on = true;

                AudioSourcesManager.GetInstance().Play(front_controller.level_audio, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.timer);
            }

        }

        time_cost += Time.deltaTime;
    }

    void Init()
    {
        CardShow();

        GuideManager.GetInstance().GuideLevelInit(front_controller.guide_root.transform , hint_pairs[0] , hint_pairs[1]);
    }

    void CollectJewel(int j){

        bool b = true;

        //命中目标，删除目标
        if (jewels_show[0] == j)
        {
            jewels_show.RemoveAt(0);
        }
        else {
            b = false;
        }

        //目标全消，任务完成
        if (jewels_show.Count > 0)
        {
            is_jewels_done = false;
        }
        else {
            is_jewels_done = true;

            //任务完成去掉所有多余子弹
            CardsProcess.GetInstance().CleanJewelsPos(all_list);
        }

        int i = jewels_count - jewels_show.Count - 1;    

        //画面表现
        front_controller.UpdateJewel(i , b);
    }

    public void GetAllJevels() {
        jewels_set_pos.Clear();
        jewels_show.Clear();

        is_jewels_done = true;

        front_controller.ShowAllJewels();

        if (cards_total_num == cards_now_num)
        {
            OnSuccess();
        }
        else if(chocolate_num == chocolate_num_now){
            OnSuccess();
        }
    }

    void CardShow()
    {
        List<CardSeatMonoHandler> list = new List<CardSeatMonoHandler>(card_seat_list);

        CardSeatGeneratorFactory.GetFactory().Create(gen_types[0], list, null, jewels_set_pos, lock_list,stage.chocolate_cnt);

        hint_pairs = CardsProcess.GetInstance().GetCardsPairValid(all_list);
    }

    //根据牌的属性分成几个大堆
    void SeparateCardsByBigPos()
    {
    }

    CardSeatMonoHandler card_first;
    CardSeatMonoHandler card_second;

    public void OnSelect(string _id)
    {
        int id = Convert.ToInt32(_id);


        if (card_first == null)
        {
            card_first = GetCardById(id);
            card_first.ChangeStatus(CARD_STATUS.SELECTED);

            return;
        }

        if (card_first != null)
        {
            card_second = GetCardById(id);

            if (card_second.ID.Equals(card_first.ID))
            {
                card_second = null;

                return;
            }

            if (IsCancelSelect(card_second, card_first))
            {
                card_first.ChangeStatus(CARD_STATUS.UNSELECTED);
                card_first = card_second;
                card_first.ChangeStatus(CARD_STATUS.SELECTED);
                card_second = null;

                return;
            }

            if (CardsProcess.GetInstance().IsCardMatch(card_second, card_first))
            {
                if(LocalDynamicData.GetInstance().guide_step1 == 0){
                    IGuideStep guide = GuideManager.GetInstance().GetCurrentGuide();

                    if (guide != null) {
                        guide.Execute();
                    }                
                }

                timer_hint = 0f;

                card_second.ChangeStatus(CARD_STATUS.SELECTED);

                card_first.ChangeStatus(CARD_STATUS.FLY);
                card_second.ChangeStatus(CARD_STATUS.FLY);

                CardsProcess.GetInstance().Fly(card_first, card_second , Root);

                UpdateScore();

                if(!timer_level){
                    UpdatePower();

                    UpdatePowerFX();
                }                

                //巧克力
                if (card_first.is_chocolate)
                {
                    int status = UpdateChocolate(card_first);

                    if (status == -1) { Invoke("OnJewelsLevelFail", CommonData.SHOW_SUCCESS_FAIL_TIME); ; return; }
                    if (status == 1) { Invoke("OnSuccess", CommonData.SHOW_SUCCESS_FAIL_TIME); return; }
                }

                //钥匙
                if (card_first.key_type != KEY_TYPE.NONE || card_second.key_type != KEY_TYPE.NONE)
                {
                    UpdateKey(card_first, card_second);
                }

                //收集宝石
                if (jewels_level && (card_first.jewel_type != JEWEL_TYPE.NONE) || (card_second.jewel_type != JEWEL_TYPE.NONE))
                {
                    UpdateJewels(card_first, card_second);
                }

                card_first = null;
                card_second = null;

                if(cards_total_num == cards_now_num){

                    if (jewels_level)
                    {
                        if (!is_jewels_done)
                        {
                            //失败
                            //front_controller.ShowFailUI();
                            //Invoke("OnFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
                            Invoke("OnJewelsLevelFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
                            return;
                        }
                    }

                    //胜利
                    //OnSuccess();

                    Invoke("OnSuccess", CommonData.SHOW_SUCCESS_FAIL_TIME);

                    return;
                }

                GetHint();
            }
        }
    }

    public bool HaveVaildCards() {
        if (hint_pairs != null && hint_pairs.Count >= 2)
        {
            return true;
        }

        return false;
    }

    void GetHint() {
        if (hint_pairs != null)
        {
            hint_pairs.Clear();
        }

        //每一步都判断下一步有没有可点击牌
        hint_pairs = CardsProcess.GetInstance().GetCardsPairValid(all_list);

        if (hint_pairs == null || hint_pairs.Count < 2)
        {
            GuideManager.GetInstance().GuideLevelOver(front_controller.guide_root.transform);

            if (!GuideManager.is_guide_now)
            {
                pause = true;
                front_controller.SetButtonsStatus(false);

                /*
                if(cards_total_num - cards_now_num == 2){
                    Invoke("OnTimerLevelFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
                }
                else{
                    Invoke("OnFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
                }
                */

                //新的死局逻辑
                //消除所有牌
                //消除关键牌
                //收集并消除全部麻将
                //收集并消除全部巧克力
                //消除所有牌在倒计时时间内
                //消除关键牌在倒计时时间内
                //收集并消除全部麻将在倒计时时间内
                //收集并消除全部巧克力在倒计时时间内
                if(stage_type == STAGE_TYPE.KILL_ALL || stage_type == STAGE_TYPE.KILL_CHOCOLATE){
                    if(cards_total_num - cards_now_num > 10){
                        //洗牌
                        fail_chance_type = FAIL_CHANCE_TYPE.REDRAW;
                    }
                    else if (cards_total_num - cards_now_num <= 10)
                    {
                        //炸弹
                        fail_chance_type = FAIL_CHANCE_TYPE.BOMB;
                    }
                }                         
       
                if(stage_type == STAGE_TYPE.COLLECT_KILL_ALL || stage_type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE){
                    if (cards_total_num - cards_now_num > 10)
                    {
                        //洗牌
                        fail_chance_type = FAIL_CHANCE_TYPE.REDRAW;
                    }
                    else if (cards_total_num - cards_now_num <= 10)
                    {
                        //炸弹
                        fail_chance_type = FAIL_CHANCE_TYPE.BOMB;
                    }
                }
            
                if(stage_type == STAGE_TYPE.KILL_ALL_TIMER || stage_type == STAGE_TYPE.KILL_CHOCOLATE_TIMER){

                    if (cards_total_num - cards_now_num > 10)
                    {
                        //洗牌
                        fail_chance_type = FAIL_CHANCE_TYPE.REDRAW;
                    }
                    else if (cards_total_num - cards_now_num <= 10)
                    {
                        //炸弹
                        fail_chance_type = FAIL_CHANCE_TYPE.BOMB;
                    }
                }

                if(stage_type == STAGE_TYPE.COLLECT_KILL_ALL_TIMER || stage_type == STAGE_TYPE.COLLECT_KILL_CHOCOLATE_TIMER){

                    if (cards_total_num - cards_now_num > 10)
                    {
                        //洗牌
                        fail_chance_type = FAIL_CHANCE_TYPE.REDRAW;
                    }
                    else if (cards_total_num - cards_now_num <= 10)
                    {
                        //炸弹
                        fail_chance_type = FAIL_CHANCE_TYPE.BOMB;
                    }
                }

                Invoke("OnFail", CommonData.SHOW_SUCCESS_FAIL_TIME);                                                   
            }
        }
    }

    public void OnFailUpdateData() {
        Highscore hs = DynamicData.GetInstance().GetHighScoreByID(CommonData.current_level);

        if (hs != null)
        {
            {
                if (hs.highscore <= 0)
                {
                    hs.try_num = hs.try_num + 1;
                }

                hs.time_cost = hs.time_cost > time_cost ? hs.time_cost : time_cost;

                hs.skill_refresh_num = hs.skill_refresh_num >= skill_refresh_num ? hs.skill_refresh_num : skill_refresh_num;
                hs.skill_bomb_num = hs.skill_bomb_num >= skill_bomb_num ? hs.skill_bomb_num : skill_bomb_num;
                hs.skill_add_timer_num = hs.skill_add_timer_num >= skill_add_timer_num ? hs.skill_add_timer_num : skill_add_timer_num;
                hs.skill_jewels_num = hs.skill_jewels_num >= skill_jewels_num ? hs.skill_jewels_num : skill_jewels_num;
                hs.skill_unlock_num = hs.skill_unlock_num >= skill_unlock_num ? hs.skill_unlock_num : skill_unlock_num;

                if (hs.deadpoint_I > 0)
                {
                    hs.deadpoint_I --;
                }
                else if (hs.deadpoint_X > 0)
                {
                    hs.deadpoint_X --;
                }                

                DynamicData.GetInstance().UpdateHighScorce(hs);
            }
        }
        else
        {
            hs = new Highscore();

            hs.id = CommonData.current_level;
            hs.stage_id = CommonData.current_level;

            hs.try_num = hs.try_num + 1;

            hs.time_cost = time_cost;

            hs.skill_refresh_num = skill_refresh_num;
            hs.skill_bomb_num = skill_bomb_num;
            hs.skill_add_timer_num = skill_add_timer_num;
            hs.skill_jewels_num = skill_jewels_num;
            hs.skill_unlock_num = skill_unlock_num;

            if (stage.deadpoint_I > 0) {
                hs.deadpoint_I = stage.deadpoint_I - 1;
            }
            else if (stage.deadpoint_X > 0)
            {
                hs.deadpoint_X = stage.deadpoint_X - 1;
            }

            DynamicData.GetInstance().InsertHighScorce(hs);
        }

        level_done = true;
    }

    void OnFail() {
        timer_pause = true;

        //front_controller.ShowFailUI();

        Invoke("ShowFailChanceUI", 1f);
    }

    void OnTimerLevelFail()
    {
        hint_pause = true;
        timer_pause = true;

        //front_controller.ShowFailFailUI();

        fail_chance_type = FAIL_CHANCE_TYPE.ADD_TIME;
        front_controller.ShowFailUI();
    }

    void OnJewelsLevelFail()
    {
        hint_pause = true;
        timer_pause = true;

        //front_controller.ShowFailFailUI();

        fail_chance_type = FAIL_CHANCE_TYPE.COLLECT_JEWELS;
        front_controller.ShowFailUI();
    }

    void ShowFailChanceUI() {
        front_controller.ShowFailUI();
    }

    void OnSuccess() {
        timer_pause = true;

        CommonData.is_success_level = true;

        int stars_count = 0;

        int score_stage = score;

        int score1 = stage.score1;
        int score2 = stage.score2;
        int score3 = stage.score3;

        if (timer_level)
        {

            score_stage = (int)timer;

            if (score_stage >= score1 && score_stage < score2)
            {
                stars_count = 1;
            }
            else if (score_stage >= score2 && score_stage < score3)
            {
                stars_count = 2;
            }
            else if (score_stage >= score3)
            {
                stars_count = 3;
            }
        }
        else
        {
            if (score_stage >= score1 && score_stage < score2)
            {
                stars_count = 1;
            }
            else if (score_stage >= score2 && score_stage < score3)
            {
                stars_count = 2;
            }
            else if (score_stage >= score3)
            {
                stars_count = 3;
            }
        }


        Highscore hs = DynamicData.GetInstance().GetHighScoreByID(CommonData.current_level);

        if (hs != null)
        {
            {
                if (timer_level)
                {
                    if (hs.highscore == 0)
                    {
                        hs.highscore = stage.count_down - (int)timer;
                    }else{
                        hs.highscore = hs.highscore <= (stage.count_down - (int)timer) ? hs.highscore : stage.count_down - (int)timer;
                    }
                    
                }
                else
                {
                    hs.highscore = hs.highscore >= score ? hs.highscore : score;
                }

                hs.star_num = hs.star_num >= stars_count ? hs.star_num : stars_count;
                hs.time_cost = hs.time_cost > time_cost ? hs.time_cost : time_cost;

                hs.skill_refresh_num = hs.skill_refresh_num >= skill_refresh_num ? hs.skill_refresh_num : skill_refresh_num;
                hs.skill_bomb_num = hs.skill_bomb_num >= skill_bomb_num ? hs.skill_bomb_num : skill_bomb_num;
                hs.skill_add_timer_num = hs.skill_add_timer_num >= skill_add_timer_num ? hs.skill_add_timer_num : skill_add_timer_num;
                hs.skill_jewels_num = hs.skill_jewels_num >= skill_jewels_num ? hs.skill_jewels_num : skill_jewels_num;
                hs.skill_unlock_num = hs.skill_unlock_num >= skill_unlock_num ? hs.skill_unlock_num : skill_unlock_num;

                hs.deadpoint_I = stage.deadpoint_I;
                hs.deadpoint_X = stage.deadpoint_X;
                
                if(CommonData.current_level == 3 || CommonData.current_level == 5){
                    hs.deadpoint_I = 0;
                    hs.deadpoint_X = 0;
                }

                DynamicData.GetInstance().UpdateHighScorce(hs);
            }
        }
        else {
            hs = new Highscore();

            hs.id = CommonData.current_level;
            hs.stage_id = CommonData.current_level;

            if (timer_level)
            {
                hs.highscore = stage.count_down - (int)timer;
            }
            else
            {
                hs.highscore = score;
            }

            hs.star_num = stars_count;
            hs.time_cost = time_cost;

            hs.skill_refresh_num = skill_refresh_num;
            hs.skill_bomb_num = skill_bomb_num;
            hs.skill_add_timer_num = skill_add_timer_num;
            hs.skill_jewels_num = skill_jewels_num;
            hs.skill_unlock_num = skill_unlock_num;

            hs.deadpoint_I = stage.deadpoint_I;
            hs.deadpoint_X = stage.deadpoint_X;

            if (CommonData.current_level == 3 || CommonData.current_level == 5)
            {
                hs.deadpoint_I = 0;
                hs.deadpoint_X = 0;
            }

            DynamicData.GetInstance().InsertHighScorce(hs);
        }

        PlayerInfoUtil.AddLife(1);

        level_done = true;

        front_controller.EndLevel(stars_count);
    }

    CardSeatMonoHandler GetCardById(int id) { 
        for(int i=0;i<card_seat_list.Length;i++){
            if (card_seat_list[i].ID == id+"") {
                return card_seat_list[i];
            }
        }
        return null;
    }

    // 是否取消选择
    bool IsCancelSelect(CardSeatMonoHandler card_A, CardSeatMonoHandler card_B)
    {
        bool result = false;
        if (((Enum.IsDefined(typeof(CARD_TYPE), card_A.card_type) || Enum.IsDefined(typeof(CARD_TYPE), card_B.card_type)) && card_A.card_type != card_B.card_type))
        {
            result = true;
        }
        else if (Enum.IsDefined(typeof(CARD_TYPE_SEASON), card_A.card_type) && Enum.IsDefined(typeof(CARD_TYPE_PLANT), card_B.card_type))
        {
            result = true;
        }
        else if (Enum.IsDefined(typeof(CARD_TYPE_PLANT), card_A.card_type) && Enum.IsDefined(typeof(CARD_TYPE_SEASON), card_B.card_type))
        {
            result = true;
        }
        return result;
    }


    public void OnRefreshClick()
    {
        List<CardSeatMonoHandler> list = new List<CardSeatMonoHandler>(card_seat_list);
        //card_ways = new List<CardSeatMonoHandler>();

        for (int i = list.Count - 1; i >= 0; i-- )
        {
            if (list[i].is_chocolate) {
                list.RemoveAt(i);
            }
        }

        CardSeatRefreshFactory.GetFactory().Create(refresh_type, list, jewels_set_pos , lock_list);

        GetHint();
    }

    public void AddTime() {
        timer += CommonData.ADD_TIME;
    }

    public void OnAddTimeClick()
    {
        ItemManager.GetInstance().Execute(ITEM_TYPE.ADD_TIME, this);
    }

    public void OnGetDiamondClick()
    {
        ItemManager.GetInstance().Execute(ITEM_TYPE.GET_DIAMOND, this);
    }

    public void OnUnLockClick()
    {
        ItemManager.GetInstance().Execute(ITEM_TYPE.UNLOCK,this);
    }

    public void OnBombClick()
    {
        ItemManager.GetInstance().Execute(ITEM_TYPE.BOMB, this);
    }

    public void Bomb() {
        bomb_index = 1;

        pause = true;
        front_controller.SetButtonsStatus(false);

        if(cards_total_num - cards_now_num <= 10){
            only_10_cards = true;
        }

        Invoke("BombBegin", 1.5f);
    }

    void BombBegin() {
        InvokeRepeating("OneBomb", 0, 0.5f);
    }

    List<CardSeatMonoHandler> bomb_pair;
    int bomb_index = 1;

    bool only_10_cards = false;

    public void OneBomb() {

        if (only_10_cards)
        {
            bomb_pair = CardsProcess.GetInstance().GetBombTargetLessTen(all_list);
        }else{
            bomb_pair = CardsProcess.GetInstance().GetBombTarget(all_list);
        }

        bomb_index++;

        if (bomb_pair == null || bomb_pair.Count < 2)
        {
            CancelInvoke("OneBomb");
            Invoke("OnFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
        }
        else {
            timer_hint = 0f;

            card_first = bomb_pair[0];
            card_second = bomb_pair[1];

            if(!card_first.IsVaild()){
                card_first.BeVaild();
            }

            if (!card_second.IsVaild())
            {
                card_second.BeVaild();
            }

            card_first.ChangeStatus(CARD_STATUS.SELECTED);
            card_second.ChangeStatus(CARD_STATUS.SELECTED);

            card_first.ChangeStatus(CARD_STATUS.FLY);
            card_second.ChangeStatus(CARD_STATUS.FLY);

            CardsProcess.GetInstance().Fly(card_first, card_second , Root);

            UpdateScore();

            if (!timer_level)
            {
                UpdatePower();

                UpdatePowerFX();

            }

            //巧克力
            if (card_first.is_chocolate)
            {
                int status = UpdateChocolate(card_first);

                if (status == -1) { Invoke("OnJewelsLevelFail", CommonData.SHOW_SUCCESS_FAIL_TIME); CancelInvoke("OneBomb"); return; }
                if (status == 1) { Invoke("OnSuccess", CommonData.SHOW_SUCCESS_FAIL_TIME); CancelInvoke("OneBomb"); return; }
            }

            //钥匙
            if (card_first.key_type != KEY_TYPE.NONE || card_second.key_type != KEY_TYPE.NONE)
            {
                UpdateKey(card_first, card_second);
            }

            //收集宝石
            if (jewels_level && (card_first.jewel_type != JEWEL_TYPE.NONE) || (card_second.jewel_type != JEWEL_TYPE.NONE))
            {
                UpdateJewels(card_first, card_second);
            }

            card_first = null;
            card_second = null;

            if (cards_total_num == cards_now_num)
            {

                if (jewels_level)
                {
                    if (!is_jewels_done)
                    {
                        //Invoke("OnFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
                        Invoke("OnJewelsLevelFail", CommonData.SHOW_SUCCESS_FAIL_TIME);
                        CancelInvoke("OneBomb");
                        return;
                    }
                }

                Invoke("OnSuccess", CommonData.SHOW_SUCCESS_FAIL_TIME);
                CancelInvoke("OneBomb");
                return;
            }

        }

        if (bomb_index > 5)
        {
            pause = false;
            front_controller.SetButtonsStatus(true);

            CancelInvoke("OneBomb");

            GetHint();
        }
    }

    void UpdateScore() {
        cards_now_num += 2;
        front_controller.UpdateCardsNum();
    }

    void UpdatePower() {
        if (is_power)
        {
            power++;

            if (power > 10)
            {
                power = 10;
            }

            if(power >= 10){
                power_conter += 12;

                if (power_conter >= 30)
                {
                    power_conter = 30;
                }

                front_controller.image_hit.fillAmount = power_conter / 30f;
            }

            front_controller.UpdateHit(power);
        }
        else
        {
            power_conter += 12;

            if (power_conter >= 30)
            {
                power_conter = 30;
                is_power = true;
            }

            front_controller.image_hit.fillAmount = power_conter / 30f;
        }

        CalculateScore();
        front_controller.UpdateScore();
    }

    int UpdateChocolate(CardSeatMonoHandler card) {
        chocolate_num_now += 2;
        front_controller.UpdateChocolateKeysNum();

        if (chocolate_num_now == chocolate_num)
        {
            if (jewels_level)
            {
                if (!is_jewels_done)
                {
                    return -1;
                }
            }

            //胜利
            return 1;
        }

        return 0;
    }

    void UpdateKey(CardSeatMonoHandler card_first, CardSeatMonoHandler card_second)
    {
        if (card_first.key_type != KEY_TYPE.NONE)
        {
            card_first.ShowKey(false);

            card_first.who_i_unlock.ShowLock(false);
            card_first.who_i_unlock.lock_type = LOCK_TYPE.NONE;

            lock_list.Remove((int)card_first.key_type);

            card_first.key_type = KEY_TYPE.NONE;
        }

        if (card_second.key_type != KEY_TYPE.NONE)
        {
            card_second.ShowKey(false);

            card_second.who_i_unlock.ShowLock(false);
            card_second.who_i_unlock.lock_type = LOCK_TYPE.NONE;

            lock_list.Remove((int)card_second.key_type);

            card_second.key_type = KEY_TYPE.NONE;
        }
    }

    void UpdateJewels(CardSeatMonoHandler card_first, CardSeatMonoHandler card_second)
    {
        JEWEL_TYPE jt = JEWEL_TYPE.NONE;

        if (card_first.jewel_type != JEWEL_TYPE.NONE)
        {
            jt = card_first.jewel_type;
        }

        if (card_second.jewel_type != JEWEL_TYPE.NONE)
        {
            jt = card_second.jewel_type;
        }

        //消耗一发子弹 不一定命中
        jewels_set_pos.Remove((int)jt);

        //card_first.ShowJewel(false);
        //card_second.ShowJewel(false);

        //CollectJewel((int)jt);

        bool b = true;

        //命中目标，删除目标
        if (jewels_show[0] == (int)jt)
        {
            jewels_show.RemoveAt(0);
        }
        else
        {
            b = false;
        }

        //目标全消，任务完成
        if (jewels_show.Count > 0)
        {
            is_jewels_done = false;
        }
        else
        {
            is_jewels_done = true;
        }

        int i;

        if(b)
            i = jewels_count - jewels_show.Count - 1;
        else
            i = jewels_count - jewels_show.Count;

        if (card_first.jewel_type != JEWEL_TYPE.NONE)
        {
            card_first.jewel_target = front_controller.GetJewelPos(i);
            card_first.is_jewel_broken = !b;
        }

        if (card_second.jewel_type != JEWEL_TYPE.NONE)
        {
            card_second.jewel_target = front_controller.GetJewelPos(i);
            card_second.is_jewel_broken = !b;
        }

        //画面表现
        //front_controller.UpdateJewel(i, b);
    }

    public void CheckJewelsStatus() {
        if (is_jewels_done)
            //任务完成去掉所有多余子弹
            CardsProcess.GetInstance().CleanJewelsPos(all_list);
    }

    void UpdatePowerFX() {
        GameObject go = null;

        if(power == 3){
            go = Instantiate(Resources.Load("Effect/Prefab/Inside_Cam_Combo_Good")) as GameObject;

        }
        else if(power == 8){
            go = Instantiate(Resources.Load("Effect/Prefab/Inside_Cam_Combo_Perfect")) as GameObject;

        }
        else if (power == 5)
        {
            go = Instantiate(Resources.Load("Effect/Prefab/Inside_Cam_Combo_Excellent")) as GameObject;

        }

        if (go != null) {
            go.transform.SetParent(front_controller.fx_root);
            go.transform.SetParent(front_controller.fx_root);
            go.transform.localPosition = new Vector3(25f, 0f, 0f);
            go.transform.localScale = new Vector3(100f, 100f, 1f);
        }
    }

    public void UpdatePowerSound() {
        if (power == 1)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo1);
        }
        else if (power == 2)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo2);
        }
        else if (power == 3)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo3);
        }
        else if (power == 4)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo4);
        }
        else if (power == 5)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo5);
        }
        else if (power == 6)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo6);
        }
        else if (power == 7)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo7);
        }
        else if (power == 8)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo8);
        }
        else if (power == 9)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo9);
        }
        else if (power == 10)
        {
            AudioSourcesManager.GetInstance().Play(front_controller.audio_combo, (front_controller.audioclip_set == null) ? null : front_controller.audioclip_set.combo10);
        }
    }
}