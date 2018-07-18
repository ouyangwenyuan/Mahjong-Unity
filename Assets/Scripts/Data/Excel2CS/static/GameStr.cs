using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class GameStr
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string guid_text { get; set; }

    public GameStr()
    {}
}
