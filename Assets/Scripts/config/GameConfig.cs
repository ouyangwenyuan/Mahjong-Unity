using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.config
{
    public class GameConfig
    {
#if UNITY_EDITOR
    public const bool InitDebug = true;
#else
        public const bool InitDebug = false;
#endif
    }
}
