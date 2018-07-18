using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Data.BeanMap
{
    [Serializable]
    public class HighscoreMap
    {
        public int id;
        public int stage_id;
        public int highscore;

        public HighscoreMap(Highscore score)
        {
            id = score.id;
            stage_id = score.id;
            highscore = score.highscore;
        }

        public HighscoreMap()
        {
        }
    }
}
