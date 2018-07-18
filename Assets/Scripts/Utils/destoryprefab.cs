using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destoryprefab : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GameObject.Destroy(gameObject, 2.5f);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
