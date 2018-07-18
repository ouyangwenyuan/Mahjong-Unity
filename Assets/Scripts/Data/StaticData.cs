using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StaticData
{
    static StaticData static_data;

    public static StaticData GetInstance()
    {
        if (static_data == null)
        {
            static_data = new StaticData();
            static_data.LoadData();
        }

        return static_data;
    }

    List<Stage> stages;
    List<GameStr> game_strs;

    void LoadData() {
        var data_source1 = StaticDataBaseService.GetInstance().GetStages();
        if (data_source1 != null)
        {
            stages = data_source1.ToList<Stage>();

        }

        var data_source2 = StaticDataBaseService.GetInstance().GetGameStr();
        if (data_source2 != null)
        {
            game_strs = data_source2.ToList<GameStr>();

        }
    }

    public int GetStagesNum() {
        if(stages != null){
            return stages.Count;
        }

        return 0;
    }

    public Stage GetStageByID(int id) { 
        if (stages != null && stages.Count > 0 && id <= stages.Count)
        {
            return stages[id - 1];
        }

        return null;
    }

    public GameStr GetStringByID(int id)
    {
        if (game_strs != null && game_strs.Count > 0 && id <= game_strs.Count)
        {
            return game_strs[id - 1];
        }

        return null;
    }
}
