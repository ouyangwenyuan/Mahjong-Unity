using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CardsProcess
    {
        private static CardsProcess cardsProcess;

        public static CardsProcess GetInstance()
        {
            if (cardsProcess == null)
            {
                cardsProcess = new CardsProcess();
            }
            return cardsProcess;
        }

        int death_pos_i_num = 0;    //有多少I型坑
        int death_pos_x_num = 0;    //有多少X型坑

        public void SetDeathPosINum(int num) {
            death_pos_i_num = num;
        }

        public void SetDeathPosXNum(int num)
        {
            death_pos_x_num = num;
        }

        List<String> special_cards_season;
        List<String> special_cards_plant;

        public void ReSetSpecialCardType() {
            if (special_cards_season != null) special_cards_season.Clear();
            if (special_cards_plant != null) special_cards_plant.Clear();

            special_cards_season = new List<String>(Enum.GetNames(typeof(CARD_TYPE_SEASON)));
            special_cards_plant = new List<String>(Enum.GetNames(typeof(CARD_TYPE_PLANT)));
        }

        System.Random random = new System.Random();

        public Dictionary<string, List<string>> GetCardsTypes(List<CardSeatMonoHandler> list)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

            string key = null;

            List<string> type = new List<string>();

            foreach (CardSeatMonoHandler card in list)
            {
                if (card.GetStatus() != CARD_STATUS.DONE && card.GetStatus() != CARD_STATUS.FLY)
                {
                    key = card.card_type;

                    if (Enum.IsDefined(typeof(CARD_TYPE_SEASON), card.card_type))
                        key = CARD_TYPE.season.ToString();// "season";

                    if (Enum.IsDefined(typeof(CARD_TYPE_PLANT), card.card_type))
                        key = CARD_TYPE.plant.ToString();
                    
                    if (!dic.ContainsKey(key))
                    {
                        type = new List<string>();

                        type.Add(card.card_type);

                        dic.Add(key , type);
                    }
                    else {
                        dic[key].Add(card.card_type);
                    }
                }
            }

            return dic;
        }

        public List<CardSeatMonoHandler> GetCardsPairValid(List<CardSeatMonoHandler> list)
        {
            Dictionary<string , List<CardSeatMonoHandler>> dic =new Dictionary<string,List<CardSeatMonoHandler>>();

            List<CardSeatMonoHandler> pair ;

            string key;

            foreach(CardSeatMonoHandler card in list){
                if (card.GetStatus() != CARD_STATUS.DONE && card.GetStatus() != CARD_STATUS.FLY && card.IsVaild() && card.lock_type == LOCK_TYPE.NONE)
                {
                    key = card.card_type;

                    if (Enum.IsDefined(typeof(CARD_TYPE_SEASON), card.card_type))
                        key = CARD_TYPE.season.ToString();

                    if (Enum.IsDefined(typeof(CARD_TYPE_PLANT), card.card_type))
                        key = CARD_TYPE.plant.ToString();

                    if (dic.ContainsKey(key))
                    {
                        dic[key].Add(card);
                    }
                    else {
                        pair = new List<CardSeatMonoHandler>();
                        pair.Add(card);

                        dic.Add(key,pair);
                    }
                }
            }

            foreach (KeyValuePair<string, List<CardSeatMonoHandler>> kv in dic)
            {
                if(kv.Value!=null && kv.Value.Count >= 2){
                    return kv.Value;
                }
            }

            return null;
        }

        public void Generator(List<CardSeatMonoHandler> list, List<string> types, List<int> jewels, List<int> locks, int chocolate_num)
        {
            //////////////////////////////////////////////////////////////////////////////////////////
            List<string> types_list;

            if (types == null || types.Count <= 0)
            {
                types_list = ProduceCardType((list.Count >> 1) - (chocolate_num >> 1));
            }
            else
            {
                types_list = types;
            }

            ReSetSpecialCardType();

            //////////////////////////////////////////////////////////////////////////////////////////

            MakeDeathPos(list, types_list);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //生成容器
            List<List<CardSeatMonoHandler>> list_list = ProduceLogicCardList(list);

            list.Sort(Sort);



            //for (int i = 0; i < list.Count; i++) {
            //    Debug.Log(list[i].index_sort.ToString());
            //}

            //发牌 设置巧克力
            Deal(list_list, types_list, chocolate_num);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            SetJewelsPos(list, jewels); //设置宝石序列
            
            SetLocksPos(list , locks);
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        void MakeDeathPos(List<CardSeatMonoHandler> list , List<string> types_list)
        {
            List<CardSeatMonoHandler> temp_list = new List<CardSeatMonoHandler>();
            List<int> pos_list = new List<int>();

            if(death_pos_i_num > 0){
                temp_list.Clear();
                pos_list.Clear();

                foreach(CardSeatMonoHandler card in list){
                    if(card.death_pos_type == DEATH_POS_TYPE.I){
                        temp_list.Add(card);
                    }
                }

                if (death_pos_i_num > (temp_list.Count / 2) || temp_list.Count == 0)
                {
                    Debug.LogError("No More Death Pos I !!!!!!!!!!!!!!!!!!");
                }
                else {

                    for (int j = 1; j <= (temp_list.Count / 2); j++ )
                    {
                        pos_list.Add(j);
                    }

                    for (int i = 1; i <= death_pos_i_num;  i++ )
                    {
                        int pos = random.Next() % (pos_list.Count);
                        int pos_index = pos_list[pos];
                        pos_list.RemoveAt(pos);

                        int type_pos = random.Next() % (types_list.Count);
                        string type = types_list[type_pos];
                        types_list.RemoveAt(type_pos);

                        foreach (CardSeatMonoHandler card in temp_list)
                        {
                            if(card.death_pos_index == pos_index){
                                card.Init(DealWithSpecialCard(type));

                                card.is_death_pos = true;

                                //list.Remove(card);
                            }
                        }
                    }

                }
            }

            Dictionary<int, List<CardSeatMonoHandler>> tmp_dic = new Dictionary<int, List<CardSeatMonoHandler>>();

            List<CardSeatMonoHandler> x_list;

            if (death_pos_x_num > 0)
            {
                tmp_dic.Clear();
                temp_list.Clear();
                pos_list.Clear();

                foreach (CardSeatMonoHandler card in list)
                {
                    if (card.death_pos_type == DEATH_POS_TYPE.X)
                    {
                        if(tmp_dic.ContainsKey(card.death_pos_index)){
                            tmp_dic[card.death_pos_index].Add(card);
                        }else{
                            x_list = new List<CardSeatMonoHandler>();

                            x_list.Add(card);

                            tmp_dic.Add(card.death_pos_index, x_list);
                        }
                    }
                }

                for (int j = 1; j <= (tmp_dic.Count); j++)
                {
                    pos_list.Add(j);
                }

                for (int i = 1; i <= death_pos_x_num; i++)
                {
                    int pos = random.Next() % (pos_list.Count);
                    int pos_index = pos_list[pos];
                    pos_list.RemoveAt(pos);

                    int type_pos1 = random.Next() % (types_list.Count);
                    string type1 = types_list[type_pos1];
                    types_list.RemoveAt(type_pos1);

                    int type_pos2 = random.Next() % (types_list.Count);
                    string type2 = types_list[type_pos2];
                    types_list.RemoveAt(type_pos2);

                    foreach(KeyValuePair<int, List<CardSeatMonoHandler>> kvp in tmp_dic){
                        if(kvp.Key == pos_index){

                            foreach (CardSeatMonoHandler card in kvp.Value)
                            {
                                if (card.death_pos_x_sub_index == 1)
                                {
                                    card.Init(DealWithSpecialCard(type1));

                                    card.is_death_pos = true;

                                }else{
                                    card.Init(DealWithSpecialCard(type2));

                                    card.is_death_pos = true;
                                }

                                //list.Remove(card);
                            }

                        }
                    }
                }
            }
        }

        private static int Sort(CardSeatMonoHandler a1, CardSeatMonoHandler a2)
        {
            if (a1.index_sort > a2.index_sort)
            {
                return 1;
            }
            else if (a1.index_sort < a2.index_sort)
            {
                return -1;
            }

            return 0;
        }

        //生成牌的花色 从两幅牌选 有可能一种花色出现两边 
        List<string> ProduceCardType(int type_length)
        {
            List<String> type_names = new List<String>(Enum.GetNames(typeof(CARD_TYPE)));

            // 所有牌色
            type_names.RemoveAt(type_names.Count - 1);

            if (type_length > type_names.Count) 
            {
                type_names.AddRange(type_names);
            }

            return type_names;
        }

        //产生逻辑层
        List<List<CardSeatMonoHandler>> ProduceLogicCardList(List<CardSeatMonoHandler> full_list)
        {
            List<CardSeatMonoHandler> list = new List<CardSeatMonoHandler>();
            List<CardSeatMonoHandler> level_list;

            int index = 1;

            foreach (CardSeatMonoHandler card in full_list)
            {
                if (card.GetStatus() != CARD_STATUS.DONE)
                {
                    list.Add(card);
                }
            }

            List<List<CardSeatMonoHandler>> list_list = new List<List<CardSeatMonoHandler>>();

            while (true)
            {
                level_list = new List<CardSeatMonoHandler>();

                foreach (CardSeatMonoHandler card in list)
                {
                    if (card.CanMove())
                    {
                        card.index_sort = index;

                        index++;
                        level_list.Add(card);
                    }
                }

                list_list.Add(level_list);

                foreach (CardSeatMonoHandler card in level_list)
                {
                    card.RemoveRely();

                    list.Remove(card);
                }

                if (list.Count <= 0)
                {
                    break;
                }
            }

            foreach (List<CardSeatMonoHandler> temp_list in list_list)
            {
                /*
                foreach (CardSeatMonoHandler card in temp_list)
                {
                    card.RecoverRely();
                }
                 * */

                for (int i = temp_list.Count - 1; i >= 0; i-- )
                {
                    temp_list[i].RecoverRely();

                    if (temp_list[i].is_death_pos)
                    {
                        temp_list.RemoveAt(i);
                    }
                }
            }

            return list_list;

        }

        public void SetJewelsPos(List<CardSeatMonoHandler> list, List<int> jewels)
        {
            if(jewels == null || jewels.Count <= 0){
                return;
            }

            if(jewels[0] == 0){
                return;
            }

            List<CardSeatMonoHandler> temp_list = new List<CardSeatMonoHandler>();

            foreach (CardSeatMonoHandler card in list)
            {

                if (card.GetStatus() != CARD_STATUS.DONE && card.GetStatus() != CARD_STATUS.FLY && !card.is_chocolate)
                {
                    card.jewel_type = JEWEL_TYPE.NONE;

                    card.ShowJewel(false);

                    temp_list.Add(card);
                }
            }

            List<string> type_exclude = new List<string>();
            type_exclude.Clear();

            foreach(int i in jewels){
                int pos = random.Next() % (temp_list.Count);

                CardSeatMonoHandler card = temp_list[pos];

                //宝石不能同花色
                while(true){

                    if (type_exclude.Contains(card.card_type))
                    {
                        pos = random.Next() % (temp_list.Count);

                        card = temp_list[pos];
                    }
                    else {

                        type_exclude.Add(card.card_type);

                        break;
                    }

                }

                card.jewel_type = (JEWEL_TYPE)i;
                card.ShowJewel(true);

                for (int j = temp_list.Count - 1; j >= 0; j--)
                {
                    if (temp_list[j].card_type == card.card_type)
                        temp_list.Remove(temp_list[j]);
                }

            }
        }

        public void CleanJewelsPos(List<CardSeatMonoHandler> list)
        {
            foreach (CardSeatMonoHandler card in list)
            {
                card.jewel_type = JEWEL_TYPE.NONE;
                card.ShowJewel(false);
            }
        }

        public void SetLocksPos(List<CardSeatMonoHandler> list , List<int> locks)
        {
            if(locks == null || locks.Count <=0){
                return;
            }

            List<CardSeatMonoHandler> temp_list = new List<CardSeatMonoHandler>();
            List<string> type_exclude = new List<string>();
            type_exclude.Clear();

            foreach (CardSeatMonoHandler card in list)
            {
                if (card.GetStatus() != CARD_STATUS.DONE && card.GetStatus() != CARD_STATUS.FLY && !card.is_chocolate)
                {
                    if(card.jewel_type != JEWEL_TYPE.NONE){
                        card.lock_type = LOCK_TYPE.NONE;
                        card.key_type = KEY_TYPE.NONE;

                        card.ShowLock(false);
                        card.ShowKey(false);

                        if(!type_exclude.Contains(card.card_type)){
                            type_exclude.Add(card.card_type);
                        }

                        continue;
                    }

                    card.lock_type = LOCK_TYPE.NONE;
                    card.key_type = KEY_TYPE.NONE;

                    card.ShowLock(false);
                    card.ShowKey(false);

                    temp_list.Add(card);
                }
            }

            CardSeatMonoHandler card_lock;
            CardSeatMonoHandler card_key;

            for (int i = 0; i < locks.Count; i++ )
            {
                int j = 1;
                card_lock = temp_list[(temp_list.Count - j)];

                while(true){
                    if (!type_exclude.Contains(card_lock.card_type))
                    {
                        type_exclude.Add(card_lock.card_type);

                        temp_list.RemoveAt(temp_list.Count - j);

                        break;
                    }
                    else {
                        j++;

                        card_lock = temp_list[(temp_list.Count - j)];
                    }
                }

                card_lock.lock_type = (LOCK_TYPE)(locks[i]);

                int pos = random.Next() % (temp_list.Count);
                card_key = temp_list[pos];

                while (true)
                {
                    if (!type_exclude.Contains(card_key.card_type))
                    {
                        type_exclude.Add(card_key.card_type);
                        temp_list.RemoveAt(pos);

                        break;
                    }
                    else
                    {
                        pos = random.Next() % (temp_list.Count);
                        card_key = temp_list[pos];
                    }
                }

                card_key.key_type = (KEY_TYPE)(locks[i]);
                card_key.who_i_unlock = card_lock;

                card_lock.ShowLock(true);

                Debug.Log(card_lock.ID.ToString() +  " " + card_lock.index_sort.ToString());
                Debug.Log(card_key.ID.ToString() + " " + card_key.index_sort.ToString());

                card_key.ShowKey(true);

                if(temp_list.Count < 3){
                    break;
                }
            }
        }

        public void CleanLocksNKeysPos(List<CardSeatMonoHandler> list , List<int> locks)
        {
            foreach (CardSeatMonoHandler card in list)
            {
                if (card.lock_type != LOCK_TYPE.NONE)
                {
                    card.lock_type = LOCK_TYPE.NONE;
                    card.ShowLock(false);
                }

                if (card.key_type != KEY_TYPE.NONE)
                {
                    card.key_type = KEY_TYPE.NONE;
                    card.ShowKey(false);
                }
            }

            locks.Clear();
        }

        public void Refresh(List<CardSeatMonoHandler> list, Dictionary<string, List<string>> dic, List<int> jewels, List<int> locks)
        {
            List<CardSeatMonoHandler> level_list_vaild = new List<CardSeatMonoHandler>();
            List<CardSeatMonoHandler> level_list = new List<CardSeatMonoHandler>();

            foreach (CardSeatMonoHandler card in list)
            {
                if (card.GetStatus() != CARD_STATUS.DONE && card.GetStatus() != CARD_STATUS.FLY)
                {
                    if(card.IsVaild()){
                        level_list_vaild.Add(card);
                    }
                    else{
                        level_list.Add(card);
                    }
                }
            }

            level_list.Sort(Sort);

            /*
            for (int i = 0; i < level_list.Count; i++ )
            {
                Debug.Log(level_list[i].index_sort.ToString());
            }
            */

            /*
            List<string> type_list = new List<string>();

            foreach (KeyValuePair<string, List<string>> pair in dic)
            {
                
                //for (int i = 0; i < pair.Value.Count; i++)
                //{

                //}
                

                if (pair.Value.Count == 2)
                {
                    type_list.Add(pair.Key);
                }

                if (pair.Value.Count == 4)
                {
                    type_list.Add(pair.Key);
                    type_list.Add(pair.Key);
                }
            }

            Generator(level_list, type_list, jewels, locks, 0);
            */
            //int type_pos;
            //string type;

            
            int pos;

            foreach (KeyValuePair<string, List<string>> pair in dic) {
                for (int i = 0; i < pair.Value.Count; i++ )
                {
                    /*
                    if(i == 0){
                        pos = 0;
                    }else if(i == 1){
                        pos = 0; 
                    }else{
                        pos = random.Next() % (level_list.Count);
                    }

                    level_list[pos].ResetType(pair.Value[i]);
                    level_list.RemoveAt(pos);
                    */

                    if (level_list_vaild.Count > 0)
                    {
                        pos = random.Next() % (level_list_vaild.Count);
                        level_list_vaild[pos].ResetType(pair.Value[i]);
                        level_list_vaild.RemoveAt(pos);
                    }
                    else{
                        pos = random.Next() % (level_list.Count);
                        level_list[pos].ResetType(pair.Value[i]);
                        level_list.RemoveAt(pos);
                    }
                }
            }

            SetJewelsPos(list, jewels);

            list.Sort(Sort);
            SetLocksPos(list, locks);
        }

        public List<CardSeatMonoHandler> GetBombTargetLessTen(List<CardSeatMonoHandler> list)
        {
            Dictionary<string, List<CardSeatMonoHandler>> dic = new Dictionary<string, List<CardSeatMonoHandler>>();
            List<CardSeatMonoHandler> pair;

            string key;

            foreach (CardSeatMonoHandler card in list)
            {
                bool b1 = card.GetStatus() != CARD_STATUS.DONE;
                bool b2 = card.GetStatus() != CARD_STATUS.FLY;
                bool b3 = card.lock_type == LOCK_TYPE.NONE;
             
                if (b1 && b2 && b3)
                {
                    key = card.card_type;

                    if (Enum.IsDefined(typeof(CARD_TYPE_SEASON), card.card_type))
                        key = CARD_TYPE.season.ToString();

                    if (Enum.IsDefined(typeof(CARD_TYPE_PLANT), card.card_type))
                        key = CARD_TYPE.plant.ToString();

                    if (dic.ContainsKey(key))
                    {
                        dic[key].Add(card);
                    }
                    else
                    {
                        pair = new List<CardSeatMonoHandler>();
                        pair.Add(card);

                        dic.Add(key, pair);
                    }
                }
            }

            foreach (KeyValuePair<string, List<CardSeatMonoHandler>> kv in dic)
            {
                if (kv.Value != null && kv.Value.Count >= 2)
                {
                    return kv.Value;
                }
            }

            return null;

        }

        public List<CardSeatMonoHandler> GetBombTarget(List<CardSeatMonoHandler> list)
        {
            Dictionary<string, List<CardSeatMonoHandler>> dic = new Dictionary<string, List<CardSeatMonoHandler>>();
            List<CardSeatMonoHandler> pair;

            string key;

            foreach (CardSeatMonoHandler card in list)
            {
                //Debug.Log(card.GetStatus().ToString());

                bool b1 = card.GetStatus() != CARD_STATUS.DONE;
                bool b2 = card.GetStatus() != CARD_STATUS.FLY;
                bool b3 = card.lock_type == LOCK_TYPE.NONE;
                bool b4 = !card.WhoOnTopMe();

                if (b1 && b2 && b3 && b4)
                {
                    key = card.card_type;

                    if (Enum.IsDefined(typeof(CARD_TYPE_SEASON), card.card_type))
                        key = CARD_TYPE.season.ToString();

                    if (Enum.IsDefined(typeof(CARD_TYPE_PLANT), card.card_type))
                        key = CARD_TYPE.plant.ToString();

                    if (dic.ContainsKey(key))
                    {
                        dic[key].Add(card);
                    }
                    else
                    {
                        pair = new List<CardSeatMonoHandler>();
                        pair.Add(card);

                        dic.Add(key, pair);
                    }
                }
            }

            //处理只剩两张 压着的牌的情况
            if (dic.Count == 1)
            {
                foreach(KeyValuePair<string, List<CardSeatMonoHandler>> kv in dic){
                    if (kv.Value != null && kv.Value.Count == 1) {
                        if (kv.Value[0].who_are_under_me_be_hindered != null)
                        {
                            for (int i = 0; i < kv.Value[0].who_are_under_me_be_hindered.Length; i++)
                            {
                                if (kv.Value[0].who_are_under_me_be_hindered[i] != null)
                                {
                                    CardSeatMonoHandler card_a = kv.Value[0];
                                    CardSeatMonoHandler card_b = kv.Value[0].who_are_under_me_be_hindered[i].GetComponent<CardSeatMonoHandler>();

                                    bool b = IsCardMatch(card_a, card_b);

                                    if(b){
                                        kv.Value.Add(card_b);
                                        return kv.Value;
                                    }

                                    return null;
                                }
                            }
                        }

                    }
                }
            }

            foreach (KeyValuePair<string, List<CardSeatMonoHandler>> kv in dic)
            {
                if (kv.Value != null && kv.Value.Count >= 2)
                {
                    return kv.Value;
                }
            }

            return null;
        }

        void Deal(List<List<CardSeatMonoHandler>> list_list, List<string> types_list, int chocolate_num)
        {
            //ReSetSpecialCardType();

            string type = null;

            int length = list_list.Count;

            List<CardSeatMonoHandler> list_item;
            List<CardSeatMonoHandler> list_temp = new List<CardSeatMonoHandler>();

            CardSeatMonoHandler card_seat1;
            CardSeatMonoHandler card_seat2;

            for (int i = length - 1; i >= 0; i--)
            {
                list_item = list_list[i];

                //Debug.Log("List :　" + list_item.Count.ToString());

                int item_length = list_item.Count;

                bool b1 = false;                            //上一层是否有奇数牌

                if (list_temp.Count > 0)
                {
                    b1 = true;

                    list_item.Add(list_temp[0]);            //理论上只有一张奇数牌
                }

                list_temp.Clear();

                if (item_length < 1)
                {
                    continue;
                }
                else if (item_length < 2)
                {
                    list_temp.Add(list_item[0]);            //此层只有一张牌的话 并入下一层

                    list_item.Clear();
                }
                else
                {
                    item_length = list_item.Count;

                    while (true)
                    {
                        bool is_chocolate = false;
                        //每种花色的牌 只限制最多出现两次
                        if(chocolate_num > 0){
                            is_chocolate = true;

                            chocolate_num -= 2;

                            type = null;
                        }
                        else
                        {
                            int type_pos = random.Next() % (types_list.Count);

                            type = types_list[type_pos];

                            types_list.RemoveAt(type_pos);
                        }

                        if (b1)
                        {
                            card_seat1 = list_item[item_length - 1];    //下层奇数项 加在最后
                            list_item.RemoveAt(item_length - 1);

                            int pos2 = random.Next() % (list_item.Count);
                            card_seat2 = list_item[pos2];

                            //找一个不压着第一张牌的                        
                            while (true)
                            {
                                while (true)
                                {
                                    bool b3 = false;

                                    for (int jj = 0; jj < card_seat2.who_are_under_me_be_hindered.Length; jj++)
                                    {
                                        if (card_seat2.who_are_under_me_be_hindered[jj] == card_seat1.gameObject)
                                        {
                                            b3 = true;

                                            break;
                                        }
                                    }

                                    if (b3)
                                    {
                                        pos2 = random.Next() % (list_item.Count);
                                        card_seat2 = list_item[pos2];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                {
                                    list_item.RemoveAt(pos2);

                                    if (is_chocolate)
                                    {
                                        card_seat1.SetChocolate();
                                        card_seat2.SetChocolate();
                                    }
                                    else
                                    {
                                        string type1 = DealWithSpecialCard(type);
                                        string type2 = DealWithSpecialCard(type);

                                        //if (!card_seat1.is_death_pos) 
                                            card_seat1.Init(type1);
                                        //if (!card_seat2.is_death_pos) 
                                            card_seat2.Init(type2);
                                    }

                                    break;
                                }
                            }

                        }
                        else
                        {
                            int pos1 = random.Next() % (list_item.Count);
                            card_seat1 = list_item[pos1];

                            list_item.RemoveAt(pos1);

                            int pos2 = random.Next() % (list_item.Count);
                            card_seat2 = list_item[pos2];

                            list_item.RemoveAt(pos2);

                            if (is_chocolate)
                            {
                                card_seat1.SetChocolate();
                                card_seat2.SetChocolate();
                            }
                            else
                            {
                                string type1 = DealWithSpecialCard(type);
                                string type2 = DealWithSpecialCard(type);

                                //if (!card_seat1.is_death_pos) 
                                    card_seat1.Init(type1);
                                //if (!card_seat2.is_death_pos) 
                                    card_seat2.Init(type2);
                            }

                        }

                        item_length = list_item.Count;

                        card_seat1 = null;
                        card_seat2 = null;

                        if (item_length < 1)
                        {
                            break;
                        }

                        if (item_length < 2)
                        {
                            list_temp.Add(list_item[0]);

                            list_item.Clear();

                            break;
                        }
                    }
                }
            }
        }

        string DealWithSpecialCard(string type)
        {
            if (type == CARD_TYPE.season.ToString())
            {
                int typeIndex = random.Next() % (special_cards_season.Count);
                type = special_cards_season[typeIndex];

                special_cards_season.RemoveAt(typeIndex);
            }
            else if (type == CARD_TYPE.plant.ToString())
            {
                int typeIndex = random.Next() % (special_cards_plant.Count);
                type = special_cards_plant[typeIndex];

                special_cards_plant.RemoveAt(typeIndex);
            }

            return type;
        }

        // 是否配对
        public bool IsCardMatch(CardSeatMonoHandler card_A, CardSeatMonoHandler card_B)
        {
            bool result = false;
            if (card_A.is_chocolate && card_B.is_chocolate)
            {
                result = true;
            }
            else if ((Enum.IsDefined(typeof(CARD_TYPE), card_A.card_type) && card_A.card_type == card_B.card_type))
            {
                result = true;
            }
            else if (Enum.IsDefined(typeof(CARD_TYPE_PLANT), card_A.card_type) && Enum.IsDefined(typeof(CARD_TYPE_PLANT), card_B.card_type))
            {
                result = true;
            }
            else if (Enum.IsDefined(typeof(CARD_TYPE_SEASON), card_A.card_type) && Enum.IsDefined(typeof(CARD_TYPE_SEASON), card_B.card_type))
            {
                result = true;
            }

            return result;
        }

        public void Fly(CardSeatMonoHandler c1, CardSeatMonoHandler c2 , GameObject Root)
        {
            CardSeatMonoHandler c_left;
            CardSeatMonoHandler c_right;

            float distance = (c1.transform.localPosition - c2.transform.localPosition).magnitude;

            if (c1.transform.localPosition.x < c2.transform.localPosition.x)
            {
                c_left = c1;
                c_right = c2;
            }
            else if (c1.transform.localPosition.x > c2.transform.localPosition.x)
            {
                c_left = c2;
                c_right = c1;
            }
            else if (c1.transform.localPosition.y > c2.transform.localPosition.y)
            {
                c_left = c1;
                c_right = c2;
            }
            else
            {
                c_left = c2;
                c_right = c1;
            }

            c_left.transform.SetParent(Root.transform);
            c_right.transform.SetParent(Root.transform);

            float x_distance = 0f;
            float y_distance = 0f;

            x_distance = c_right.transform.localPosition.x - c_left.transform.localPosition.x;

            bool b_left = true; //左牌更高

            if (c_left.transform.localPosition.y >= c_right.transform.localPosition.y)
            {
                y_distance = c_left.transform.localPosition.y - c_right.transform.localPosition.y;
            }
            else
            {
                b_left = false;

                y_distance = c_right.transform.localPosition.y - c_left.transform.localPosition.y;
            }

            float x = 0f;
            float y = 0f;

            ///////////////////////////////////////////////////////////////////////////////
            x = c_left.transform.localPosition.x;
            y = c_left.transform.localPosition.y;

            Vector2 p_l_b = new Vector2(x, y);

            //左边飞
            if (b_left)
            {
                y = c_left.transform.localPosition.y - y_distance * (1f / 6f);
            }
            else
            {
                y = c_left.transform.localPosition.y + y_distance * (1f / 6f);
            }

            x = c_left.transform.localPosition.x - CommonData.fly_radius * 4f;

            Vector2 p_l_m = new Vector2(x, y);

            if (b_left)
            {
                y = c_left.transform.localPosition.y - y_distance * (1f / 2f);
            }
            else
            {
                y = c_left.transform.localPosition.y + y_distance * (1f / 2f);
            }

            x = c_left.transform.localPosition.x - CommonData.fly_radius + x_distance * (1f / 2f);

            Vector2 p_l_e = new Vector2(x, y);

            CardFlyPoint point_l = new CardFlyPoint();
            point_l.point_middle = p_l_m;
            point_l.point_end = p_l_e;
            point_l.point_begin = p_l_b;
            //point_l.timer = timer;
            point_l.is_left = true;

            c_left.Fly(point_l);

            ///////////////////////////////////////////////////////////////////////////////
            x = c_right.transform.localPosition.x;
            y = c_right.transform.localPosition.y;

            Vector2 p_r_b = new Vector2(x, y);

            //右边飞
            if (b_left)
            {
                y = c_right.transform.localPosition.y + y_distance * (1f / 6f);
            }
            else
            {
                y = c_right.transform.localPosition.y - y_distance * (1f / 6f);
            }

            x = c_right.transform.localPosition.x + CommonData.fly_radius * 4f;

            Vector2 p_r_m = new Vector2(x, y);

            if (b_left)
            {
                y = c_right.transform.localPosition.y + y_distance * (1f / 2f);
            }
            else
            {
                y = c_right.transform.localPosition.y - y_distance * (1f / 2f);
            }

            x = c_right.transform.localPosition.x + CommonData.fly_radius - x_distance * (1f / 2f);

            Vector2 p_r_e = new Vector2(x, y);

            CardFlyPoint point_r = new CardFlyPoint();
            point_r.point_middle = p_r_m;
            point_r.point_end = p_r_e;
            point_r.point_begin = p_r_b;

            c_right.Fly(point_r);
        }

    }        
