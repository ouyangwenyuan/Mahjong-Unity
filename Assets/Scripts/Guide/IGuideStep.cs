using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGuideStep{
    //void OnClick();

    void Init(CardSeatMonoHandler card1, CardSeatMonoHandler card2);
    void Execute();
}
