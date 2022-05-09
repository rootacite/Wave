using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckBoxBehavior))]
public class LowPerfSetting : MonoBehaviour
{

    CheckBoxBehavior cbb;
    // Start is called before the first frame update
    void Start()
    {
        cbb = GetComponent<CheckBoxBehavior>();
        cbb.ValueChanged += (s) =>
        {
            PlayerPrefs.SetInt("Low_Setting", s ? 1 : 0);

            if (s) QualitySettings.SetQualityLevel(0);
            else QualitySettings.SetQualityLevel(5);
        };

        PlayerPrefs.SetInt("Low_Setting", QualitySettings.GetQualityLevel() == 0 ? 1 : 0);
        cbb.SetValue(PlayerPrefs.GetInt("Low_Setting", 0) == 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
