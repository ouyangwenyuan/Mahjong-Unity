using System;
using Assets.Scripts.Bean.log;
using Assets.Scripts.Data.BeanMap;
using Fabric.Internal.ThirdParty.MiniJSON;
using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.Data.Excel2CS.dynamic
{
    public class LogCache
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string JsonData { get; set; }
        public string UpdateTime { get; set; }
        public string Action { get; set; }

        public LogCache()
        {}

        public LogCache(LogCacheMap logCache)
        {
            id = logCache.id;
            JsonData = logCache.JsonData;
            UpdateTime = logCache.UpdateTime;
            Action = logCache.Action;
        }
    }
}
