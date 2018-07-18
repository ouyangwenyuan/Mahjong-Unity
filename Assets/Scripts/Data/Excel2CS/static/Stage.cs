using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class Stage
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int chapter { get; set; }
    public int type { get; set; }
    public int mahjong_cnt { get; set; }
    public int chocolate_cnt { get; set; }
    public string jewel_all { get; set; }
    public string collect_order { get; set; }
    public int count_down { get; set; }
    public int lock_key { get; set; }
    public string deadpoint_all { get; set; }
    public int deadpoint_I { get; set; }
    public int deadpoint_X { get; set; }
    public int score1 { get; set; }
    public int score2 { get; set; }
    public int score3 { get; set; }
    public int reward { get; set; }
    public int count { get; set; }
    public int guide_if { get; set; }
    public string guide_id { get; set; }
    public string background { get; set; }

    public Stage()
    {}
}
