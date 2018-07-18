
using Assets.Script.gameplus.define;
using UnityEngine;

namespace Assets.GamePlus.ironsrc
{
    public class MyAppStart : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("MyAppStart Start called");

            //IronSource tracking sdk
            IronSource.Agent.reportAppStarted();

            //Dynamic config example
            IronSourceConfig.Instance.setClientSideCallbacks(true);

            string id = IronSource.Agent.getAdvertiserId();
            Debug.Log("IronSource.Agent.getAdvertiserId : " + id);

            Debug.Log("IronSource.Agent.validateIntegration");
            IronSource.Agent.validateIntegration();

            Debug.Log("unity version" + IronSource.unityVersion());
            //SDK init
            Debug.Log("IronSource.Agent.init");
//            IronSource.Agent.setUserId("uniqueUserId");
            IronSource.Agent.init(AppConfig.IRONSRC_APIKEY);
        }

        void OnApplicationPause(bool isPaused)
        {
            Debug.Log("OnApplicationPause = " + isPaused);
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}
