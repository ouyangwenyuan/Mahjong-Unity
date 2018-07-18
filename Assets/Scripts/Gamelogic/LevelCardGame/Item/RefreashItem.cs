using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreashItem : ItemBase{
    public override void Execute(GameStage game)
    {
        //base.Execute(game);

        game.OnRefreshClick();
    }
}
