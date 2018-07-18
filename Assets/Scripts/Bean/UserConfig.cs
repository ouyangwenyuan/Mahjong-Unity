using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bean
{
    [Serializable]
    public class UserConfig:BeanDic
    {
        //游戏广告刷新时间
       public string GameAdCountDown;
       public int GameAdPlayCount;
       //商店广告倒计时
       public string ShopAdCountDown;
        //音乐
       public int MusicOn;
        //按键音效
       public int SoundOn;
        //消息
       public int NotifictionOn;
       //是否完成新手
       public int TutorialOn;
       //新手引导
        public string GuidSteps;
        //最大生命购买次数
        public int MaxlifeBuyTime;
        //更新时间
       public string Time;

       public override Dictionary<string, object> ToDictionary()
       {
           Dictionary<string, object> result = new Dictionary<string, object>();
           result["GameAdCountDown"] = GameAdCountDown;
           result["ShopAdCountDown"] = ShopAdCountDown;
           result["MusicOn"] = MusicOn;
           result["SoundOn"] = SoundOn;
           result["NotifictionOn"] = NotifictionOn;
           result["TutorialOn"] = TutorialOn;
           result["Time"] = Time;
           result["GuidSteps"] = GuidSteps;
           result["MaxlifeBuyTime"] = MaxlifeBuyTime;
           result["GameAdPlayCount"] = GameAdPlayCount;
           return result;
       }
    }
}
