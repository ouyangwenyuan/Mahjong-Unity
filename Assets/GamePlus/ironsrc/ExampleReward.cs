
using Assets.GamePlus.advertise;

namespace Assets.GamePlus.ironsrc
{
    public class ExampleReward : AbstractGpVideo
    {
        void OnStart()
        {
            _typeTizer=VideoTizer.Example;
        }

        void failload()
        {
            isLoad = false;
        }

        void sucload()
        {
            isLoad = true;
        }

        void finish()
        {
            ResultCallback(ShowResult.Finished);
            //播放结束加载
            load();
        }

        void load()
        {
            
        }

        void show()
        {
            if (isLoad)
            {
                //播放
            }
            else
            {
                //播放失败加载
                load();
            }
        }

        public override void ShowVideoAd()
        {
            
        }

        public override void LoadVideoAd()
        {
            
        }
    }
}
