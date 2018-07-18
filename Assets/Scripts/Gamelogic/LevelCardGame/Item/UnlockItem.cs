using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockItem :ItemBase{
    public override void Execute(GameStage game)
    {
        //base.Execute(game);

        CardsProcess.GetInstance().CleanLocksNKeysPos(game.all_list , game.lock_list);
    }
}