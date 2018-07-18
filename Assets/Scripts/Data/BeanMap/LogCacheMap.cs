using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Data.Excel2CS.dynamic;

namespace Assets.Scripts.Data.BeanMap
{
    public class LogCacheMap
    {
        public int id ;
        public string JsonData ;
        public string UpdateTime ;
        public string Action ;

        public LogCacheMap(LogCache logCache)
        {
            id = logCache.id;
            JsonData = logCache.JsonData;
            UpdateTime = logCache.UpdateTime;
            Action = logCache.Action;
        }
    }
}
