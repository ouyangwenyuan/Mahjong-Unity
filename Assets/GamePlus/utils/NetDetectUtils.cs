
using System;
using System.ComponentModel;
using System.Net;
using UnityEngine;

namespace Assets.GamePlus.utils
{
    public class NetDetectUtils
    {
        public static event Action<bool> PingPass; 
        public static void Detect(string url)
        {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
            request.Timeout = 10000;
            request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
            request.Method = "HEAD";
            var worker = new BackgroundWorker();
            string result;
            worker.DoWork += (sender, args) =>
            {
                args.Result = request.GetResponse();
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    result = "Error: " + e.Error.Message;
                    Debug.LogError(result);
                    PingPass(false);
                }
                else
                {
                    result = "Connectivity OK";
                    Debug.Log(result);
                    PingPass(true);
                }
            };
            Debug.Log("Testing Connectivity");
            worker.RunWorkerAsync();
        }
    }
}