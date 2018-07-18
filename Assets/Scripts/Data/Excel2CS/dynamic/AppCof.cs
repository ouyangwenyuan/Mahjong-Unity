using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class AppCof
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string jsonData { get; set; }

    public AppCof()
    {}
}
