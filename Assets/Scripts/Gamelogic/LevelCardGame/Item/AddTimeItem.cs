using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTimeItem :ItemBase{
    public override void Execute(GameStage game)
    {
        //base.Execute(game);

        //Debug.Log("Add Time");

        game.AddTime();
    }
}
