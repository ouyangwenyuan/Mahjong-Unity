using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Bean;
using UnityEngine;
using SQLite4Unity3d;

public class Highscore
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int stage_id { get; set; }
    public int highscore { get; set; }
    public int star_num { get; set; }
    public int try_num { get; set; }
    public float time_cost { get; set; }
    public int skill_refresh_num { get; set; }
    public int skill_bomb_num { get; set; }
    public int skill_add_timer_num { get; set; }
    public int skill_jewels_num { get; set; }
    public int skill_unlock_num { get; set; }
    public int headicon { get; set; }
    public int deadpoint_I { get; set; }
    public int deadpoint_X { get; set; }

    public Highscore()
    {}

    public Highscore(UserScore score)
    {
        highscore = (int) score.Score;
        id = score.Level;
        stage_id = score.Level;
    }
}
