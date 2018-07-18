using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bean
{
    public abstract class BeanDic
    {
        public abstract Dictionary<string, object> ToDictionary();
    }
}
