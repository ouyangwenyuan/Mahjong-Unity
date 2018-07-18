using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class PlayerData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int gold { get; set; }
    public int life { get; set; }
    public int level { get; set; }
    public int maxLife { get; set; }
    public string uploadId { get; set; }
    public string uploadTime { get; set; }
    public string deviceId { get; set; }

//    public PlayerData(int id, int gold, int life, int level, int maxLife, string uploadId, string uploadTime, string deviceId)
//    {
//        this.id = id;
//        this.gold = gold;
//        this.life = life;
//        this.level = level;
//        this.maxLife = maxLife;
//        this.uploadId = uploadId;
//        this.uploadTime = uploadTime;
//        this.deviceId = deviceId;
//    }
//
//    public PlayerData()
//    {
//    }
}
