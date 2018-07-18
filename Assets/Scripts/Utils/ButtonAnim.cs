using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Assets.Scripts.Utils
{
    public class ButtonAnim : MonoBehaviour
    {
        public Transform TargeTransform;
        public bool AnimChild = false;
        public float delay = 0;
        readonly Dictionary<int, Vector3> _scaleInts = new Dictionary<int, Vector3>();
        //不使用动画的按钮
        private List<string> AnimIgnore = new List<string>()
        {
            "Guide",
            "loadmask"
        }; 
        void Start()
        {
            Invoke("CheckType", delay);
        }
        private void CheckType()
        {
            if (AnimChild)
            {
                Button[] buttons = GetComponentsInChildren<Button>();
                int i = 0;
                foreach (var button in buttons)
                {
					if (IgnoreAnim(button.name)) {
						continue;
					}
                    //唯一id标识
                    _scaleInts.Add(button.gameObject.GetInstanceID(), button.transform.localScale);
                    var button1 = button;
                    button.gameObject.AddComponent<EventTrigger>();
                    EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data, button1.transform); });
                    trigger.triggers.Add(entry);
                    EventTrigger.Entry entry2 = new EventTrigger.Entry();
                    entry2.eventID = EventTriggerType.PointerUp;
                    entry2.callback.AddListener(data => { OnPointerUpDelegate((PointerEventData)data, button1.transform); });
                    trigger.triggers.Add(entry2);
                    i++;
                }
//                Debug.Log("-----------" + gameObject.name + "--------------- " + buttons.Length);
            }
            else
            {
                GetComponent<Button>().onClick.AddListener(delegate { Onclick(transform); });
            }
        }

        bool IgnoreAnim(string name)
        {
            return AnimIgnore.Any(name.Contains);
        }

        public void OnPointerDownDelegate(PointerEventData data, Transform transform)
        {
            Vector3 preScale = transform.localScale;
            transform.DOScale(new Vector3(preScale.x - 0.1f, preScale.y - 0.1f, preScale.z), 0.1f);
        }
        public void OnPointerUpDelegate(PointerEventData data, Transform transform)
        {
            transform.DOScale(_scaleInts[transform.gameObject.GetInstanceID()], 0.1f);
        }
        void Onclick(Transform forTransform)
        {
            if (TargeTransform == null)
            {
                Anim(forTransform);
            }
            else
            {
                Anim(forTransform);
            }
        }
        void Anim(Transform form)
        {
            Vector3 preScale = form.localScale;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(form.DOScale(new Vector3(preScale.x - 0.1f, preScale.y - 0.1f, preScale.z), 0.1f));
            mySequence.Append(form.DOScale(preScale, 0.1f));
        }
    }
}