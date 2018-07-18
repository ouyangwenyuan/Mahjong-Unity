using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class FriendData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string fb_id { get; set; }
    public string gs_id { get; set; }
    public string name { get; set; }
    public string head_LocPath { get; set; }
    public string head_WebPath { get; set; }
    public int stage { get; set; }

    public FriendData()
    {}
}
