using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using Assets.GamePlus.utils;
using UnityEngine;

namespace Assets.Scripts.Bean.log
{
    public enum ActionLog
    {
        Register=1,
        Spent = 2,
        Playtime = 3,
        Bind = 4,
        Login = 5,
        Charge = 6
    }
    [Serializable]
    public class BaseLog
    {
        public WWWForm Form;
        public Dictionary<string, object> Args;
        public BaseLog(Dictionary<string, object> arDictionary)
        {
            Args = arDictionary;
        }

        public WWWForm GenerateForm()
        {
            Form = new WWWForm();
            foreach (var arg in Args)
            {
                Form.AddField(arg.Key,(string) arg.Value);
            }
            return Form;
        }
    }
}
