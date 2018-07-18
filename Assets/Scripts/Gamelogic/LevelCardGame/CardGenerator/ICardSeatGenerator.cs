using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSeatGenerator //: MonoBehaviour 
{
    void Generator(List<CardSeatMonoHandler> list);
    void Generator(List<CardSeatMonoHandler> list , List<string> card_type, List<int> jewels, List<int> locks , int chocolate_num);
}
