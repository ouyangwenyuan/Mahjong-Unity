using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.listner
{
    public interface NativeAdLitener<T>
    {
        void onNativeLoad(T info);
        void onNativeFail();
    }
}
