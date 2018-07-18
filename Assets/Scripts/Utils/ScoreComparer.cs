using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ScoreComparer : IComparer
{
    public int Compare(object lhs, object rhs)
    {
        DefaultScore aniLhs = lhs as DefaultScore;
        DefaultScore aniRhs = rhs as DefaultScore;
        int result = aniLhs.rank.CompareTo(aniRhs.rank);
        if (result == 0)
        {
            result = aniRhs.score.CompareTo(aniLhs.score);
        }

        return result;
    }
}  
