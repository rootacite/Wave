using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeLinePrevious : MonoBehaviour
{
    public RectTransform rectTransform;
    public int BeatPerSection = 4;
    public GameScripting rootConfig;
    public bool Status { get; private set; } = true;

    public void UpDown()
    {
        Status = true;
    }
    public void DownUp()
    {
        Status = false;
    }

    public double GetValue(double AtBeat)
    {
        double r = 0;
        double vx;
        switch (BeatPerSection)
        {
            case 4:

                vx = (AtBeat - 1d) % 8d + 1;
                if (vx >= 1 && vx < 5) { r = -160d * vx + 480; UpDown(); }
                if (vx >= 5 && vx < 9) { r = 160d * vx - 1120; DownUp(); }

                return r;
            case 2:

                vx = (AtBeat - 1d) % 4d + 1;
                if (vx >= 1 && vx < 3) { r = -320d * vx + 640; UpDown(); }
                if (vx >= 3 && vx < 5) { r = 320d * vx - 1280; DownUp(); }
                return r;
            default:
                throw new Exception("Unsupproted Section.");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
