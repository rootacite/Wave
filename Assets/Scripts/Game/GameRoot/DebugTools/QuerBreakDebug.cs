using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuerBreakDebug : MonoBehaviour
{
    public UnityEngine.UI.Button Button;
    public RootConfig rootConfig;

    double BreakAt = 0;

    public TMPro.TMP_InputField Input;
    // Start is called before the first frame update

    void BeatProc(double t)
    {
        if (t == BreakAt)
        {
            rootConfig.Music.Pause();
            Time.timeScale = 0;

            rootConfig.OnBeat -= BeatProc;
        }
    }
    void Start()
    {
        Button.onClick.AddListener(() =>
        {
            double BreakTime = Convert.ToDouble(Input.text);

            if (BreakTime <= 0 || BreakTime % 0.25d != 0)
            {
                StartInit.ShowText("无效的节拍数", 2f);
                return;
            }

            BreakAt = rootConfig.CurrentBeat + BreakTime;

            if (Time.timeScale != 0)
            {
                StartInit.ShowText("只在挂起状态下有效。", 2f);
                return;
            }

            rootConfig.Music.UnPause();
            Time.timeScale = 1;

            rootConfig.OnBeat += BeatProc;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
