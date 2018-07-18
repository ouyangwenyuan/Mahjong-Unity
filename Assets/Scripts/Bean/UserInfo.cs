using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Bean
{
    [Serializable]
    public class UserInfo:BeanDic
    {
        public int Stage;
        public int Life;
        public string MaxLifeTime;
        public int MaxLife;
        public string InfiniteTime;
        public int Gold;
        public int Redraw;
        public int Bomb;
        public int Clock;
        public int Jewel;
        public int Unchain;
        public string Update;
        public string Fbid;
        public string Uid;
        public string DeviceId;
        public string OsType;

        public UserInfo()
        {
        }

        public UserInfo(PlayerInfo playerInfo)
        {
            Stage = playerInfo.stage;
            Life = playerInfo.Life;
            MaxLifeTime = playerInfo.MaxLifeTime;
            MaxLife = playerInfo.MaxLife;
            InfiniteTime = playerInfo.Infinite_time;
            Gold = playerInfo.Gold;
            Redraw = playerInfo.Redraw;
            Bomb = playerInfo.Bomb;
            Clock = playerInfo.Clock;
            Jewel = playerInfo.Jewel;
            Unchain = playerInfo.Unchain;
            Update = playerInfo.player_update;
            DeviceId = playerInfo.device_id;
        }

        public override Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["Stage"] = Stage;
            result["Life"] = Life;
            result["MaxLifeTime"] = MaxLifeTime;
            result["MaxLife"] = MaxLife;
            result["InfiniteTime"] = InfiniteTime;
            result["Gold"] = Gold;
            result["Redraw"] = Redraw;
            result["Bomb"] = Bomb;
            result["Clock"] = Clock;
            result["Jewel"] = Jewel;
            result["Unchain"] = Unchain;
            result["Update"] = Update;
            result["Fbid"] = Fbid;
            result["Uid"] = Uid;
            result["DeviceId"] = DeviceId;
            result["OsType"] = OsType;
            return result;
        }
    }
}
