using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearLocalSave : MonoBehaviour {

    [MenuItem("LocalSave/Clear")]
    public static void Clear() {
        PlayerPrefs.DeleteAll();
    }
}
