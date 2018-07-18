using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSeatRefresh //: MonoBehaviour 
{
    void Generator(List<CardSeatMonoHandler> list);
    void Generator(List<CardSeatMonoHandler> list, List<int> jewels, List<int> locks);
}
