using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Assets.GamePlus.utils;
using Assets.Script.gameplus.define;
using Assets.Scripts.Bean;
using Assets.Scripts.Data.BeanMap;
using Fabric.Internal.ThirdParty.MiniJSON;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ATest
{
    [Serializable]
    public class Cube
    {
       public string name;
       public string color;
       public int score;

        public Cube(string name, string color, int score)
        {
            this.name = name;
            this.color = color;
            this.score = score;
        }
    }

    public class Cube2 : Cube
    {
        public Cube2(string name, string color, int score) : base(name, color, score)
        {
        }
    }
    public class Test2 : MonoBehaviour
    {
        void Start()
        {
        }
        void Update()
        {

        }

        public void OnListJson()
        {
            List<Cube> scoList = new List<Cube> {new Cube("tangle", "red",10), new Cube("circle", "blue",100)};
            CubeInfo info =new CubeInfo();
            info.CubeList = scoList;
            //实体类需要加序列化
            string str = JsonUtility.ToJson(new Serialization<Cube>(scoList));
            Debug.Log(str);
            List<Cube> scoList2 = JsonUtility.FromJson<Serialization<Cube>>(str).ToList();
            Debug.Log(scoList2[0].color);
        }

        public void OnUserInfo()
        {
//            Dictionary<string,object> infoDictionary= JsonUtil.ReadLocalJson("UserInfo.json");
//            Debug.Log(infoDictionary["name"]);
        }

        public void OnSort()
        {
            var cubes = new List<Cube>
            {
                new Cube("tangle1", "red", 20),
                new Cube("tangle2", "red", 10),
                new Cube("tangle3", "red", 30)
            };
            cubes.Sort((x,y)=>y.score.CompareTo(x.score));
            foreach (var cube in cubes)
            {
                Debug.Log(cube.score);
            }
        }

        public void OnCode()
        {
//            string bbb =
//                "{\"target\":[{\"Tag\":1000,\"id\":101,\"ItemOrder\":1},{\"Tag\":0,\"id\":102,\"ItemOrder\":2},{\"Tag\":1,\"id\":103,\"ItemOrder\":3},{\"Tag\":0,\"id\":104,\"ItemOrder\":4},{\"Tag\":0,\"id\":105,\"ItemOrder\":5},{\"Tag\":0,\"id\":106,\"ItemOrder\":6},{\"Tag\":2,\"id\":201,\"ItemOrder\":7},{\"Tag\":0,\"id\":202,\"ItemOrder\":8},{\"Tag\":0,\"id\":203,\"ItemOrder\":9}]}";
//            List<ShopOrderMap> shopOrders =
//                JsonUtility.FromJson<Serialization<ShopOrderMap>>(bbb).ToList();
//            List<ShopOrder> shopOrderNew = shopOrders.ConvertAll(x => new ShopOrder(x));
//            DynamicDataBaseService.GetInstance().Connection.UpdateAll(shopOrderNew);
            NetDetectUtils.Detect("http://console.firebase.google.com");
        }

        void Worker()
        {
            while (true)
            {
                Debug.Log("Worker");
                Thread.Sleep(1000);
            }
        }

        public void UpdateTable()
        {
            var addCol = "alter table Highscore add column aaaa";
            DynamicDataBaseService.GetInstance().Connection.CreateCommand(addCol).ExecuteNonQuery();
        }
    }

    [Serializable]
    public class CubeInfo
    {
        public List<Cube> CubeList;
    }
}
