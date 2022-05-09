using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowResuSwitch : MonoBehaviour
{
    public UnityEngine.UI.Button Button;
    // Start is called before the first frame update
    void Start()
    {
        Button.onClick.AddListener(() =>
        {
            Screen.SetResolution(1600, 720, false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
