using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.GamePlus.utils;

public class DynamicData
{
    static DynamicData dynamic_data;

    public static DynamicData GetInstance()
    {
        if (dynamic_data == null)
        {
            dynamic_data = new DynamicData();
            dynamic_data.LoadData();
        }

        return dynamic_data;
    }

    public void Clear()
    {
        dynamic_data = null;
    }

    List<Highscore> high_scorce;

    void LoadData() {
        var data_source = DynamicDataBaseService.GetInstance().GetHighscores();
        if (data_source != null)
        {
            high_scorce = data_source.ToList<Highscore>();
        }
    }

    public Highscore GetHighScoreByID(int id)
    {
        if (high_scorce != null && high_scorce.Count > 0 && id <= high_scorce.Count)
        {
            return high_scorce[id - 1];
        }

        return null;
    }

    public int GetHighStage()
    {
        return high_scorce.Count;
    }

    public int GetStagesDoneNum()
    {
        int sum = 0;

        if (high_scorce != null)
        {
            for (int i = 0; i < high_scorce.Count;i++ )
            {
                if(high_scorce[i].highscore <= 0){
                    break;
                }

                sum++;
            }
        }

        return sum;
    }

    public void UpdateHighScorce(Highscore score)
    {
        DynamicDataBaseService.GetInstance().UpdateData(score);
        try
        {
            high_scorce[score.id - 1] = score;
        }
        catch (Exception)
        {
            Debug.Log("score.id " + score.id + " high_scorce.Count " + high_scorce.Count);
        }
    }

    public void InsertHighScorce(Highscore score)
    {
        DynamicDataBaseService.GetInstance().InsertData(score);

        high_scorce.Add(score);
    }
}
