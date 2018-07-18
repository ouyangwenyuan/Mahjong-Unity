﻿using System;
﻿using System.Diagnostics;
﻿using Assets.Scripts.Utils;
﻿using UnityEngine;
﻿using Debug = UnityEngine.Debug;

namespace Assets.GamePlus.utils
{
    public class ClickUtils:MonoBehaviour
    {
        private static double _lastClickTime;
        public static bool IsDoubleClick(double limit)
        {
            double time = PlayerInfoUtil.GetTimeDouble();
            Debug.Log(time - _lastClickTime);
            if (Math.Abs(time - _lastClickTime) < limit)
            {
                return true;
            }
            _lastClickTime = time;
            return false;
        }

        public static bool IsDoubleClick()
        {
            return IsDoubleClick(3);
        }

        public static double PrintTime()
        {
            double time = PlayerInfoUtil.GetTimeDouble();
            if (time - _lastClickTime < 0.5d)
            {
                return time - _lastClickTime;
            }
            _lastClickTime = time;
            return time - _lastClickTime;
        }
    }
}