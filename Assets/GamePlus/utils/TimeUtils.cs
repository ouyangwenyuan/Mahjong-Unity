using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.utils
{
    public class TimeUtils
    {
        public static double getUnixTime(long ticks){
            double epoch = (ticks - 621355968000000000) / 10000000;
            return epoch;
        }
    }
}
