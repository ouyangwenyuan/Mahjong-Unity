using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetVersion : MonoBehaviour {
    Text version;
	// Use this for initialization
    void Start()
    {

#if SHOW_VERSION

		version = gameObject.GetComponent<Text>();

        if(version != null){
            version.text = Application.version;
        }
#endif
    }
}
