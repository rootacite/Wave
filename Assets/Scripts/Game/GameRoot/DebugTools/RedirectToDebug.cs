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
        // if(Application.platform == RuntimePlatform.WindowsPlayer)
        //    KeyboardHandler.StartCapture();

        KeyboardHandler.GlobalKeyDown += (vk) =>
        {
            if (vk == 37)
            {
                RootConfig.Metronome.Music.time = (float)RootConfig.Metronome.Music.time - 3f;
                RootConfig.FlushSongData();
            }
            else if (vk == 39)
            {
                RootConfig.Metronome.Music.time = (float)RootConfig.Metronome.Music.time + 3f;
                RootConfig.FlushSongData();
            }
        };
        Button.onClick.AddListener(() =>
        {
            if (RootConfig.Metronome.Music.isPlaying)
            {
                return;
            }

            var BeatP = Convert.ToDouble(RootConfig.BeatEditor.text);
            if (BeatP < 1)
            {
                StartInit.ShowText("??งน???????");
                return;
            }

            double TargetTime = Metronome.BeatSpeed * (BeatP - 2);
            RootConfig.Metronome.Music.time = (float)TargetTime;
            RootConfig.FlushSongData();
        });
    }
    void OnDestroy()
    {
        //if(Application.platform == RuntimePlatform.WindowsPlayer)
        //    KeyboardHandler.StartCapture();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
