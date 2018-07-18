using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class ChapterInfo
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int locks { get; set; }
    public int condition { get; set; }
    public int map_res { get; set; }
    public int bg_res { get; set; }

    public ChapterInfo()
    {}
}
