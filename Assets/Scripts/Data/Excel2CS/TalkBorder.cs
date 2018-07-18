using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class TalkBorder
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int chapterid { get; set; }
    public int figure_type { get; set; }
    public string content { get; set; }
    public int isLeft { get; set; }

    public TalkBorder()
    {}

    public Sprite getSprite()
    {
        string name = CommonData.FIGURE_PATH + Enum.GetName(typeof(FIGURE_TYPE), figure_type);
        Texture2D texture2d = (Texture2D)Resources.Load(name);
        Rect rect = new Rect(0, 0, texture2d.width, texture2d.height);
        Sprite sp = Sprite.Create(texture2d, rect, new Vector2(0.5f, 0.5f));

        return sp;
    }
}
