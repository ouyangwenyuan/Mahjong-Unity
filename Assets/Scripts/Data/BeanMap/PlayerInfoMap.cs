using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Data.BeanMap
{
    [Serializable]
    public class PlayerInfoMap
    {
        public int id ;
        public int stage ;
        public int Life ;
        public string MaxLifeTime ;
        public int MaxLife ;
        public string Infinite_time ;
        public int Gold ;
        public int Redraw ;
        public int Bomb ;
        public int Clock ;
        public int Jewel ;
        public int Unchain ;
        public string device_id ;
        public string player_update ;
        public string highscore ;

        public  PlayerInfoMap(PlayerInfo info)
        {
            id = info.id;
            stage = info.stage;
            Life = info.Life;
            MaxLife = info.MaxLife;
            Infinite_time = info.Infinite_time;
            Gold = info.Gold;
            Redraw = info.Redraw;
            Bomb = info.Bomb;
            Clock = info.Clock;
            Jewel = info.Jewel;
            Unchain = info.Unchain;
            device_id = info.device_id;
            player_update = info.player_update;
            highscore = info.highscore;
        }
    }
}
