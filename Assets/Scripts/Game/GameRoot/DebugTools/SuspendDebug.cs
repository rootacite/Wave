using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspendDebug : MonoBehaviour
{
    float Old_Time_Scale;

    public Button Button;
    public GameScripting RootConfig;
    // Start is called before the first frame update
    void Start()
    {
        Old_Time_Scale = Time.timeScale;
        Button.onClick.AddListener(() =>
        {
            if (RootConfig.Metronome.Music.isPlaying)
            {
                RootConfig.Metronome.Music.Pause();
                RootConfig.BeatEditor.readOnly = false;
                Old_Time_Scale = Time.timeScale;
                Time.timeScale = 0;
                StopWatchManager.HangAll();
            }
            else
            {
                RootConfig.Metronome.Music.UnPause();
                RootConfig.BeatEditor.readOnly = true;
                if (Time.timeScale != Old_Time_Scale) Time.timeScale = Old_Time_Scale;
                StopWatchManager.ResumeAll();
            }

        });

        KeyboardHandler.GlobalKeyDown += (vk) =>
        {
            if (vk == 32)
            {
                if (RootConfig.Metronome.Music.isPlaying)
                {
                    RootConfig.Metronome.Music.Pause();
                    RootConfig.BeatEditor.readOnly = false;
                    Old_Time_Scale = Time.timeScale;
                    Time.timeScale = 0;
                }
                else
                {
                    RootConfig.Metronome.Music.UnPause();
                    RootConfig.BeatEditor.readOnly = true;
                    if (Time.timeScale != Old_Time_Scale) Time.timeScale = Old_Time_Scale;
                }
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
