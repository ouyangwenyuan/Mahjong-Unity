using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.Bean
{
    public class LeaderBoardData
    {
        private ulong mLeaderBaoardCount;
        private int mUserRank;
        private string mUserID;
        private long mboardScore;

        public ulong leaderBaoardCount
        {
            get
            {
                return mLeaderBaoardCount;
            }

            internal set
            {
                mLeaderBaoardCount = value;
            }
        }

        public long boardScore
        {
            get
            {
                return mboardScore;
            }

            internal set
            {
                mboardScore = value;
            }
        }

        public string userID
        {
            get
            {
                return mUserID;
            }

            internal set
            {
                mUserID = value;
            }
        }
        public int userRank
        {
            get
            {
                return mUserRank;
            }

            internal set
            {
                mUserRank = value;
            }
        }


        public string getBeatPercent() {
            string percentStr = "0%";
            double percent = (mLeaderBaoardCount - (ulong)mUserRank) * 1.0 / mLeaderBaoardCount;
            percentStr = Math.Round(percent, 2) * 100 + "%";
            return percentStr;
        }
    }
}
