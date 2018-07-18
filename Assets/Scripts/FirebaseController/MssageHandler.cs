using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.GamePlus.FireBaseManager;
using Assets.Scripts.Bean;
using Assets.Scripts.FireBaseManager;
using Fabric.Internal.ThirdParty.MiniJSON;
using UnityEngine;

namespace Assets.Scripts.FirebaseController
{
    public class MssageHandler:MonoBehaviour
    {

        private void OnReceiveMsg(string json)
        {
            FireMessage fireMessage = JsonUtility.FromJson<FireMessage>(json);
            MsgType type = (MsgType) Enum.Parse(typeof (MsgType), fireMessage.Type);
            switch (type)
            {
                case MsgType.AdminPrivate:
                    HandleAdminPrivate(fireMessage);
                    break;
                case MsgType.UserPrivate:
                    HandlerUserPrivate(fireMessage);
                    break;
                case MsgType.AdminPublic:
                    HandleAdminPublic(fireMessage);
                    break;
            }
        }

        //todo 依据当前场景更新UI
        private void HandleAdminPrivate(FireMessage fireMessage)
        {
            List<object> propsObjects = (List<object>) Json.Deserialize(fireMessage.Content);
            foreach (var propsObject in propsObjects)
            {
                Dictionary<string, object> propsDictionary = (Dictionary<string, object>)propsObject;
                PlayerInfo playerInfo = DynamicDataBaseService.GetInstance().GetPlayerInfo().First();
                foreach (var i in propsDictionary)
                {
                    int value;
                    int.TryParse((string) i.Value, out value);
                    SetReflectValue(playerInfo, i.Key, value);
                }
                DynamicDataBaseService.GetInstance().UpdateData(playerInfo);
            }
        }

        private void HandlerUserPrivate(FireMessage fireMessage)
        {
            
        }

        private void HandleAdminPublic(FireMessage fireMessage)
        {

        }

        private static void SetReflectValue(object obj, string itemKey, int num)
        {
            PropertyInfo prop = obj.GetType().GetProperty(itemKey, BindingFlags.Public | BindingFlags.Instance);
            int prop_num = (int)prop.GetValue(obj, null);
            prop.SetValue(obj, prop_num + num, null);
        }

        void OnEnable()
        {
            MsgManager.MessageAction += OnReceiveMsg;
        }

        void OnDisable()
        {
            MsgManager.MessageAction -= OnReceiveMsg;
        }
    }
}
