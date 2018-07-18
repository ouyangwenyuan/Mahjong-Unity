using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDiamondItem :ItemBase{
    public override void Execute(GameStage game)
    {
        //base.Execute(game);

        CardsProcess.GetInstance().CleanJewelsPos(game.all_list);
        game.GetAllJevels();
    }
}
