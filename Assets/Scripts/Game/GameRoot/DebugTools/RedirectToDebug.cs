using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedirectToDebug : MonoBehaviour
{
    public Button Button;
    public GameScripting RootConfig;
    // Start is called before the first frame update
    void Start()
    {
        Button.onClick.AddListener(() =>
        {
            if (RootConfig.Music.isPlaying)
            {
                return;
            }

            var BeatP = Convert.ToDouble(RootConfig.BeatEditor.text);
            if (BeatP < 1)
            {
                StartInit.ShowText("无效的数字。");
                return;
            }

            double TargetTime = RootConfig.SecondPerBeat * (BeatP - 2);
            RootConfig.Music.time = (float)TargetTime;
            RootConfig.ReloadSongData();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
