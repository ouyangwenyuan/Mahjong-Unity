using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class BuyItem
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int item { get; set; }
    public int base_gold { get; set; }
    public int step_gold { get; set; }
    public float discount { get; set; }
    public int descTxt { get; set; }
    public int count { get; set; }
    public int infinite_life { get; set; }
    public int infinite_time { get; set; }

    public BuyItem()
    {}
}
