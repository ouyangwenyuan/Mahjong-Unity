using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem :ItemBase {
    public override void Execute(GameStage game)
    {
        //base.Execute(game);

        game.Bomb();
    }
}