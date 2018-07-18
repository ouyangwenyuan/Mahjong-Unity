using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class ScoreRecord
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int rank { get; set; }
    public int score { get; set; }
    public int star { get; set; }

    public ScoreRecord()
    {}

    public ScoreRecord(int id, int rank, int score, int star)
    {
        this.id = id;
        this.rank = rank;
        this.score = score;
        this.star = star;
    }
}
