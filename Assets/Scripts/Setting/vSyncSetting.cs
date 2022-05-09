using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckBoxBehavior))]
public class vSyncSetting : MonoBehaviour
{
    CheckBoxBehavior cbb;
    // Start is called before the first frame update
    void Start()
    {
        cbb = GetComponent<CheckBoxBehavior>();
        cbb.ValueChanged += (s) =>
        {
            PlayerPrefs.SetInt("vSync_Setting", s ? 1 : 0);

            if (s) QualitySettings.vSyncCount = 1;
            else QualitySettings.vSyncCount = 0;

            PlayerPrefs.Save();
        };

        PlayerPrefs.SetInt("vSync_Setting", QualitySettings.vSyncCount == 1 ? 1 : 0);
        cbb.SetValue(PlayerPrefs.GetInt("vSync_Setting", 1) == 1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
