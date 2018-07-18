using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class ImgRes
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string describe { get; set; }
    public string img_res { get; set; }

    public ImgRes()
    {}
}
