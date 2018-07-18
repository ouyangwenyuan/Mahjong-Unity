using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ITEM_TYPE {
    REFREASH = 0,
    BOMB,
    ADD_TIME,
    GET_DIAMOND,
    UNLOCK
}

public class ItemManager{
    static ItemManager factory;

    public static ItemManager GetInstance()
    {
        if (factory == null)
        {
            factory = new ItemManager();
        }

        return factory;
    }

    Dictionary<int, ItemBase> items_dic = new Dictionary<int, ItemBase>();

    ItemManager() {
        ItemBase item = new RefreashItem(); 
        items_dic.Add((int)ITEM_TYPE.REFREASH,item);
        item = new BombItem();
        items_dic.Add((int)ITEM_TYPE.BOMB, item);
        item = new AddTimeItem();
        items_dic.Add((int)ITEM_TYPE.ADD_TIME, item);
        item = new GetDiamondItem();
        items_dic.Add((int)ITEM_TYPE.GET_DIAMOND, item);
        item = new UnlockItem();
        items_dic.Add((int)ITEM_TYPE.UNLOCK, item);
    }

    public void Execute(ITEM_TYPE type , GameStage stage) {

        items_dic[(int)type].Execute(stage);

    }
}
