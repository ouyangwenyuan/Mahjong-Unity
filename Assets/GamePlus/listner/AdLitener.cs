using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.listner
{
    public interface AdLitener
    {
        void onFinished();
        void onFailed(string msg);
    }
}
