using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CheckBoxBehavior))]
public class AutoModeButton : MonoBehaviour
{
    CheckBoxBehavior cbb;
    // Start is called before the first frame update
    void Start()
    {
        cbb = GetComponent<CheckBoxBehavior>();
        cbb.ValueChanged += (s) =>
        {
            PlayerPrefs.SetInt("AutoMode_Setting", s ? 1 : 0);
        };

        cbb.SetValue(PlayerPrefs.GetInt("AutoMode_Setting", 0) == 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
