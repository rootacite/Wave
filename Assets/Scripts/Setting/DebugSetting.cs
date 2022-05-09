using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var cbb = GetComponent<CheckBoxBehavior>();
        cbb.ValueChanged += (s) =>
        {
            if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
            {
                StartInit.ShowText("开发模式仅在PC平台中被支持。");
                cbb.SetValue(false);
                return;
            }
            PlayerPrefs.SetInt("Debug_Setting", s ? 1 : 0);
        };

        cbb.SetValue(PlayerPrefs.GetInt("Debug_Setting", 0) == 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
