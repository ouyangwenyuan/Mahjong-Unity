using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test3CardSeatGenerator : ICardSeatGenerator//: MonoBehaviour 
{
    public void Generator(List<CardSeatMonoHandler> list)
    {
        Generator(list, null ,null,null ,0);
    }

    public void Generator(List<CardSeatMonoHandler> list, List<string> types, List<int> jewels, List<int> locks, int chocolate_num)
    {
        CardsProcess.GetInstance().Generator(list, types, jewels, locks, chocolate_num);
    }

}
