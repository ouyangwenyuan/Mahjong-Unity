using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class Payment
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int gold { get; set; }
    public float discount { get; set; }
    public int text { get; set; }
    public int item { get; set; }
    public int count { get; set; }
    public int infinite_life { get; set; }
    public int infinite_time { get; set; }

    public Payment()
    {}
}
