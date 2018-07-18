using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class Item
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string name { get; set; }
    public int img_res { get; set; }
    public int img_gray { get; set; }
    public string animation { get; set; }
    public string gamestr { get; set; }

    public Item()
    {}
}
