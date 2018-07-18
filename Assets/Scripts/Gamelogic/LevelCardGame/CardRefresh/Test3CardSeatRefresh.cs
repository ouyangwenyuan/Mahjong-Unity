using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Test3CardSeatRefresh : ICardSeatRefresh 
{
    //List<string> set_types;

    //位置
    //Dictionary<int, List<CardSeatMonoHandler>> posObj = new Dictionary<int, List<CardSeatMonoHandler>>();

    public void Generator(List<CardSeatMonoHandler> list)
    {
        Generator(list, null,null);
    }

    public void Generator(List<CardSeatMonoHandler> list, List<int> jewels, List<int> locks)
    {
        Dictionary<string, List<string>> dic = CardsProcess.GetInstance().GetCardsTypes(list);

        CardsProcess.GetInstance().Refresh(list, dic , jewels , locks);
    }
}