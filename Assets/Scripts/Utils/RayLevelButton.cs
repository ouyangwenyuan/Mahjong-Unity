using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLevelButton : MonoBehaviour {

	// Use this for initialization
	//void Start () {
	//	
	//}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
         {
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             RaycastHit hitInfo;
             if(Physics.Raycast(ray,out hitInfo))
             {
                 Debug.DrawLine(ray.origin,hitInfo.point);
                 GameObject gameObj = hitInfo.collider.gameObject;
                 Debug.Log("click object name is " + gameObj.name);

                 if (gameObj)
                 {
                     GameLevel level = gameObj.GetComponent<GameLevel>();

                     level.OnClick();
                 }

             }
         }
	}
}
