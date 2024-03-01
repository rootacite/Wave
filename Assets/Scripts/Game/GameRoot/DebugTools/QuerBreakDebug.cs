using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuerBreakDebug : MonoBehaviour
{
    public UnityEngine.UI.Button Button;
    public GameScripting rootConfig;

    double BreakAt = 0;

    public TMPro.TMP_InputField Input;
    // Start is called before the first frame update

    void BeatProc(double t)
    {
        if (t == BreakAt)
        {
            rootConfig.Metronome.Music.Pause();
            Time.timeScale = 0;

            //rootConfig.Metronome.OnBeat -= BeatProc;
        }
    }
    void Start()
    {
        Button.onClick.AddListener(() =>
        {
            double BreakTime = Convert.ToDouble(Input.text);

            if (BreakTime <= 0 || BreakTime % 0.25d != 0)
            {
                StartInit.ShowText("��Ч�Ľ�����", 2f);
                return;
            }

            BreakAt = rootConfig.Metronome.CurrentBeat + BreakTime;

            if (Time.timeScale != 0)
            {
                StartInit.ShowText("ֻ�ڹ���״̬����Ч��", 2f);
                return;
            }

            rootConfig.Metronome.Music.UnPause();
            Time.timeScale = 1;

            //rootConfig.Metronome.OnBeat += BeatProc;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
