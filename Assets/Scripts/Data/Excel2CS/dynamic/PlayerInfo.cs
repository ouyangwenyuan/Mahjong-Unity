using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Bean;
using UnityEngine;
using SQLite4Unity3d;

public class PlayerInfo
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int stage { get; set; }
    public int Life { get; set; }
    public string MaxLifeTime { get; set; }
    public int MaxLife { get; set; }
    public string Infinite_time { get; set; }
    public int Gold { get; set; }
    public int Redraw { get; set; }
    public int Bomb { get; set; }
    public int Clock { get; set; }
    public int Jewel { get; set; }
    public int Unchain { get; set; }
    public string device_id { get; set; }
    public string player_update { get; set; }
    public string highscore { get; set; }
    public PlayerInfo(UserInfo info)
    {
        id = 1;
        stage = info.Stage;
        Life = info.Life;
        MaxLife = info.MaxLife;
        Infinite_time = info.InfiniteTime;
        Gold = info.Gold;
        Redraw = info.Redraw;
        Bomb = info.Bomb;
        Clock = info.Clock;
        Jewel = info.Jewel;
        Unchain = info.Unchain;
        player_update = info.Update;
        device_id = info.DeviceId;
        MaxLifeTime = info.MaxLifeTime;
    }
    public PlayerInfo()
    {}
}
