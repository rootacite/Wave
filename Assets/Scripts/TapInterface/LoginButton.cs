using System;
using System.Collections;
using System.Collections.Generic;
using TapTap.AntiAddiction;
using TapTap.Bootstrap;
using TapTap.Common;
using TapTap.Login;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    public Loggetion loggetion;
    // Start is called before the first frame update
    void Start()
    {
       GetComponent<Button>().onClick.AddListener(async () =>
       {
           try
           {
               var tdsUser = await TDSUser.LoginWithTapTap();
               Debug.Log($"login success:{tdsUser}");
               // 获取 TDSUser 属性
               var objectId = tdsUser.ObjectId; 
               var nickname = tdsUser["nickname"]; 
               var avatar = tdsUser["avatar"];
               var profile = await TapLogin.FetchProfile();
               
               Loggetion.UserIdentifier = profile.unionid;
               AvatarPic.SetAvatar(profile.avatar);
               AntiAddictionUIKit.StartupWithTapTap(Loggetion.UserIdentifier);
           }
           catch (Exception e)
           {
               if (e is TapException tapError)  // using TapTap.Common
               {
                   Debug.Log($"encounter exception:{tapError.code} message:{tapError.message}");
                   if (tapError.code == (int)TapErrorCode.ERROR_CODE_BIND_CANCEL)
                   {
                       Debug.Log("登录取消");
                   }
               }
           }
       });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
