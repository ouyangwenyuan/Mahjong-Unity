using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class DefaultScore
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int level { get; set; }
    public int rank { get; set; }
    public int npcid { get; set; }
    public int score { get; set; }

    public DefaultScore()
    {}

    public DefaultScore(int npcid, int rank, int score)
    {
        this.npcid = npcid;
        this.rank = rank;
        this.score = score;
    }

    public string getName()
    {
        return Enum.GetName(typeof(DEFAULT_NPC), npcid);
    }

    public Sprite getSprite()
    {
        string name = CommonData.FIGURE_PATH + Enum.GetName(typeof(DEFAULT_NPC), npcid);
        Texture2D texture2d = (Texture2D)Resources.Load(name);
        Rect rect = new Rect(0, 0, texture2d.width, texture2d.height);
        Sprite sp = Sprite.Create(texture2d, rect, new Vector2(0.5f, 0.5f));

        return sp;
    }
}
