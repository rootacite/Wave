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

    // Update is called once per frame
    void Update()
    {
        
    }
}
