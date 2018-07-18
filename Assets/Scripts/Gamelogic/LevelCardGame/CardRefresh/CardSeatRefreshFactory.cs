using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSeatRefreshFactory //: MonoBehaviour 
{
    static CardSeatRefreshFactory factory;

    public static CardSeatRefreshFactory GetFactory()
    {
        if(factory == null){
            factory = new CardSeatRefreshFactory();
        }

        return factory;
    }

    public ICardSeatRefresh Create(CARD_REFRESH_TYPE type, List<CardSeatMonoHandler> list, List<int> jewels, List<int> locks)
    {
        ICardSeatRefresh refresh = new Test3CardSeatRefresh();

        switch(type){
            case CARD_REFRESH_TYPE.TEST3:
                refresh = new Test3CardSeatRefresh();
                refresh.Generator(list , jewels , locks);
                break;
        }

        return refresh;
    }
}
