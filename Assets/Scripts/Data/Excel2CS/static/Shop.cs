using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class Shop
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int type { get; set; }
    public string product_code { get; set; }
    public float usd { get; set; }
    public float benefit { get; set; }
    public int limit_pur { get; set; }
    public int img_res { get; set; }
    public int text { get; set; }
    public int size { get; set; }
    public int item1 { get; set; }
    public int count1 { get; set; }
    public int item2 { get; set; }
    public int count2 { get; set; }
    public int item3 { get; set; }
    public int count3 { get; set; }
    public int item4 { get; set; }
    public int count4 { get; set; }
    public int infinite_life { get; set; }
    public int infinite_time { get; set; }

    public Shop()
    {}
}
