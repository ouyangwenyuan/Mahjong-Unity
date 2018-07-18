using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class LevelData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int star1 { get; set; }
    public int star2 { get; set; }
    public int star3 { get; set; }
    public int max { get; set; }
    public string target { get; set; }

    public LevelData()
    {}
}
