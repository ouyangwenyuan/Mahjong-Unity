using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utils
{
    public class SecitonUtils
    {
        public static string CountSection(int step, int time)
        {
            string prefix = "section_";
            if (time==0)return "0";
            int section = time/step;
            return prefix + section;
        }
    }
}
