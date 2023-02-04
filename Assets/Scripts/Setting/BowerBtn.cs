using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BowerBtn : MonoBehaviour
{
    public Button btn;

    public TMP_InputField input;
    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            btn.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
