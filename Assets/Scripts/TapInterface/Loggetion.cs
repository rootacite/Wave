using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TapTap.AntiAddiction;
using TapTap.AntiAddiction.Model;
using TapTap.Bootstrap;
using TapTap.Common;
using TapTap.Login;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Loggetion : MonoBehaviour
{
    public async void Anticallback(int code, string errorMsg)
    {
        // code == 500;   // ��¼�ɹ�
        // code == 1000;  // �û��ǳ�
        // code == 1001;  // �л��˺�
        // code == 1030;  // �û���ǰ�޷�������Ϸ
        // code == 1050;  // ʱ������
        // code == 9002;  // ʵ�������е���˹ر�ʵ����
        UnityEngine.Debug.Log($"code: {code} error Message: {errorMsg}");

        if (code == 1001)
        {
            await TDSUser.Logout();
            SwitchMode(0);
            Loggetion.UserIdentifier = null;
            AntiAddictionUIKit.Exit();
            return;
        }
        else if (code == 1030 || code == 9002)
        {
            SwitchMode(2);
        }
        else if (code == 500)
        {
            SwitchMode(1);
        }else if (code == 1000)
        {
            AvatarPic.SetAvatar("");
        }
    }

    public static TapConfig GlobalConfig;

    public static TDSUser CurrentUser = null;

    public static string UserIdentifier = "null";

    public GameObject notLogging;

    public GameObject logging;

    public GameObject playButton;

    public void SwitchMode(int mode)
    {
        switch (mode)
        {
            case 0: // not log in
                notLogging.SetActive(true);
                logging.SetActive(false);
                playButton.GetComponent<Button>().interactable = false;
                break;
            case 1: // logged in
                notLogging.SetActive(false);
                logging.SetActive(true);
                playButton.GetComponent<Button>().interactable = true;
                break;
            case 2: // logged in but can not play
                notLogging.SetActive(false);
                logging.SetActive(true);
                playButton.GetComponent<Button>().interactable = false;
                break;
            default:
                throw new Exception();
        }
    }

    // Start is called before the first frame update
    async void Start()
    {
        AntiAddictionConfig config = new AntiAddictionConfig()
        {
            gameId = "frubo1m4rhfgppbusm", // TapTap ���������Ķ�Ӧ Client ID
            showSwitchAccount = true, // �Ƿ���ʾ�л��˺Ű�ť
        };
        TapTap.AntiAddiction.TapTapAntiAddictionManager.AntiAddictionConfig.gameId = "frubo1m4rhfgppbusm";
        AntiAddictionUIKit.Init(config, Anticallback);


        Loggetion.GlobalConfig = new TapConfig.Builder()
            .ClientID("frubo1m4rhfgppbusm") // ���룬���������Ķ�Ӧ Client ID
            .ClientToken("kKDg6linCutpZuplhHcavsWAvZeT636wiAv1OHuI") // ���룬���������Ķ�Ӧ Client Token
            .ServerURL("https://frubo1m4.cloud.tds1.tapapis.cn") // ���룬���������� > �����Ϸ > ��Ϸ���� > ������Ϣ > �������� > API
            .RegionType(RegionType.CN) // �Ǳ��룬CN ��ʾ�й���½��IO ��ʾ�������һ����
            .AntiAddictionConfig(true)
            .ConfigBuilder();
        TapBootstrap.Init(Loggetion.GlobalConfig);
        //AntiAddictionUIKit.SetTestEnvironment(true); // Remove This After Pack

        Loggetion.CurrentUser = await TDSUser.GetCurrent();
        if (Loggetion.CurrentUser == null)
        {
            SwitchMode(0);
        }
        else
        {
            var profile = await TapLogin.FetchProfile();
            UserIdentifier = profile.unionid;
            AvatarPic.SetAvatar(profile.avatar);
            AntiAddictionUIKit.StartupWithTapTap(UserIdentifier);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetRemainTime()
    {
        while (true)
        {
            Debug.LogWarning( AntiAddictionUIKit.RemainingTime.ToString());
            yield return new WaitForSeconds(1f);
        }
    }

}
