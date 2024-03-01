using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AdvanceBeat
{
    public int Measure;
    public int Beat;
    public double Detail;

    public override string ToString()
    {
        return $"{Measure}:{Beat}:{Detail.ToString("0.000")}";
    }

    public double ToDouble()
    {
        return (double)Measure * LevelBasicInformation.BeatPerSection + (double)Beat + Detail + 1.0d;
    }

    public static AdvanceBeat Parse(double s)
    {
        AdvanceBeat r = new();
        
        int currentbeat = (int)s - 1;
        r.Measure = (currentbeat - currentbeat % LevelBasicInformation.BeatPerSection) / LevelBasicInformation.BeatPerSection;
        r.Beat = currentbeat % LevelBasicInformation.BeatPerSection;
        r.Detail = s - (int)s;

        return r;
    }

    public static AdvanceBeat Parse(string s)
    {
        var n = s.Split(':');
        return new AdvanceBeat()
        {
            Measure = Convert.ToInt32(n[0]),
            Beat = Convert.ToInt32(n[1]),
            Detail = Convert.ToDouble(n[2])
        };
    }

    // 加法
    public static AdvanceBeat operator +(AdvanceBeat a, AdvanceBeat b)
    {
        AdvanceBeat result = new AdvanceBeat();
        result.Detail = a.Detail + b.Detail;
        result.Beat = a.Beat + b.Beat + (int)result.Detail;
        result.Measure = a.Measure + b.Measure + result.Beat / LevelBasicInformation.BeatPerSection;
        result.Beat %= LevelBasicInformation.BeatPerSection;
        result.Detail %= 1.0;
        return result;
    }

    // 减法
    public static AdvanceBeat operator -(AdvanceBeat a, AdvanceBeat b)
    {
        AdvanceBeat result = new AdvanceBeat();
        result.Detail = a.Detail - b.Detail;
        result.Beat = a.Beat - b.Beat - (int)result.Detail;
        result.Measure = a.Measure - b.Measure + result.Beat / LevelBasicInformation.BeatPerSection;
        result.Beat = (result.Beat + LevelBasicInformation.BeatPerSection) % LevelBasicInformation.BeatPerSection;
        result.Detail %= 1.0;
        return result;
    }

    // 比较
    public static bool operator ==(AdvanceBeat a, AdvanceBeat b)
    {
        return a.Measure == b.Measure && a.Beat == b.Beat && a.Detail == b.Detail;
    }

    public static bool operator !=(AdvanceBeat a, AdvanceBeat b)
    {
        return !(a == b);
    }

    public static bool operator <(AdvanceBeat a, AdvanceBeat b)
    {
        if (a.Measure != b.Measure)
            return a.Measure < b.Measure;
        if (a.Beat != b.Beat)
            return a.Beat < b.Beat;
        return a.Detail < b.Detail;
    }

    public static bool operator >(AdvanceBeat a, AdvanceBeat b)
    {
        if (a.Measure != b.Measure)
            return a.Measure > b.Measure;
        if (a.Beat != b.Beat)
            return a.Beat > b.Beat;
        return a.Detail > b.Detail;
    }

    public static bool operator <=(AdvanceBeat a, AdvanceBeat b)
    {
        return a < b || a == b;
    }

    public static bool operator >=(AdvanceBeat a, AdvanceBeat b)
    {
        return a > b || a == b;
    }
}
public class Metronome : MonoBehaviour
{
    // Interfaces
    private float Bpm => LevelBasicInformation.Bpm;
    public static float BeatSpeed => 60f / LevelBasicInformation.Bpm;
    public double CurrentBeat
    {
        get
        {
            double BeatInDouble = Music.time / Metronome.BeatSpeed + 1;
            return BeatInDouble - BeatInDouble % 0.125d;
        }
    }

    public AdvanceBeat CurrentBeatAdvance
    {
        get
        {
            AdvanceBeat r = new();
            
            int currentbeat = (int)CurrentBeat - 1; // Old Metronome don't has the zero beat, we reduce it by 1
            r.Measure = (currentbeat - currentbeat % LevelBasicInformation.BeatPerSection) / LevelBasicInformation.BeatPerSection;
            r.Beat = currentbeat % LevelBasicInformation.BeatPerSection;
            r.Detail = CurrentBeat - (int)CurrentBeat;

            return r;
        }
    }
    
    public double BeatInterval => 1d / (double)LevelBasicInformation.BeatPerSection;

    // Definitions
    public delegate void BeatProc(double time);

    public delegate void AdvanceBeatProc(AdvanceBeat beat);
    
    // Events
    public event AdvanceBeatProc OnBeatAdvance;
    public event BeatProc OnBeat;
    public event Action OnEnd;
    
    // Unity
    public AudioSource Music { get; private set; }

    private AdvanceBeat _advancelastbeat = new(){ Measure = 0, Beat = 0, Detail = 0};
    private double _lastBeat = 0d;
    private bool _stopped = false;
    private bool _actived = false;
    void Awake()
    {
        Music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_stopped || (!_actived)) return;
        if (Music.time >= LevelBasicInformation.MusicDuration)
        {
            _stopped = true;
            Music.Stop();
            OnEnd?.Invoke();
            return;
        }
        
        if (CurrentBeat != _lastBeat)
        {
            if (CurrentBeat - _lastBeat > 0.125d)
            {
                OnBeat?.Invoke(CurrentBeat - 0.125d);
            }
            OnBeat?.Invoke(CurrentBeat);
        }
        _lastBeat = CurrentBeat;
        
        //Constructor for Advance Beat
        if (CurrentBeatAdvance != _advancelastbeat)
        {
            if (CurrentBeatAdvance - _advancelastbeat < AdvanceBeat.Parse("0:0:0.125"))
            {
                OnBeatAdvance?.Invoke(CurrentBeatAdvance - AdvanceBeat.Parse("0:0:0.125"));
            }

            OnBeatAdvance?.Invoke(CurrentBeatAdvance);
        }

        _advancelastbeat = CurrentBeatAdvance;
    }

    public void Active()
    {
        Music.Play();
        _actived = true;
    }
    public double GetJudgeY(double Beat) // Range +- 3.2
    {
        double r = 0;
        double vx;
        switch (LevelBasicInformation.BeatPerSection)
        {
            case 3:
                
                vx = (Beat - 1d) % 6d + 1; // The Term is 3 * 2 = 6 Beat
                if (vx >= 1 && vx < 4) {
                    r = 3.2 - (vx - 1) / 3.0 * 6.4;
                }
                if (vx >= 4 && vx < 7) {
                    r = -3.2 + (vx - 4) / 3.0 * 6.4;
                }
                
                return r;
            case 4:

                vx = (Beat - 1d) % 8d + 1;
                if (vx >= 1 && vx < 5) { r = -1.6d * vx + 4.8d; }
                if (vx >= 5 && vx < 9) { r = 1.6d * vx - 11.2d; }

                return r;
            case 2:

                vx = (Beat - 1d) % 4d + 1;
                if (vx >= 1 && vx < 3) { r = -3.2d * vx + 6.4d; }
                if (vx >= 3 && vx < 5) { r = 3.2d * vx - 12.8d; }
                return r;
            default:
                throw new Exception("Unsupproted Section.");
        }
    }
}
