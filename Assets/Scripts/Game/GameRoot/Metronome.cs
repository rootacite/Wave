using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Definitions
    public delegate void BeatProc(double time);
    
    // Events
    public event BeatProc OnBeat;
    public event Action OnEnd;
    
    // Unity
    public AudioSource Music { get; private set; }
    
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
            if (CurrentBeat - _lastBeat > 0.125)
            {
                OnBeat?.Invoke(CurrentBeat - 0.125);
            }
            OnBeat?.Invoke(CurrentBeat);
        }
        _lastBeat = CurrentBeat;
    }

    public void Active()
    {
        Music.Play();
        _actived = true;
    }
    
    
    public double JudgeY
    {
        get
        {
            double r = 0;
            double Beat = Music.time / Metronome.BeatSpeed + 1;
            double vx;
            switch (LevelBasicInformation.BeatPerSection)
            {
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

    public double GetJudgeY(double Beat)
    {
        double r = 0;
        double vx;
        switch (LevelBasicInformation.BeatPerSection)
        {
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
