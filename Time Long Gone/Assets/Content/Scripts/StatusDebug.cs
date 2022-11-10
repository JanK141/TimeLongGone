using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusDebug : MonoBehaviour
{
    public static StatusDebug Instance;

    void Awake() => Instance = this;

    public void UpdateText(string txt)
    {
        GetComponent<Text>().text = txt;
    }
}
