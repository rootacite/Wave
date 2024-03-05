using System;
using System.Collections;
using System.Collections.Generic;
using TapTap.AntiAddiction;
using TapTap.Bootstrap;
using TapTap.Common;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class LogoutButton : MonoBehaviour
{
    public Loggetion loggetion;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(async () =>
        {
            await TDSUser.Logout();
            loggetion.SwitchMode(0);
            Loggetion.UserIdentifier = null;
            AntiAddictionUIKit.Exit();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
