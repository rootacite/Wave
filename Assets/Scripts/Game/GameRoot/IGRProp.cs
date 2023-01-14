using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class GameScripting
{
    public TextMeshProUGUI TPText;

    private int _CurrentMark = 0;
    public int CurrentMark
    {
        get
        {
            return _CurrentMark;
        }
        private set
        {
            _CurrentMark = value;

            double rate = _CurrentMark / (double)Full;
            TPText.text = (rate * 100d).ToString("00.00") + "%";


            MarkText.text = BulidNum(_CurrentMark);
        }
    }

    private int _ComboCount = 0;

    public int ComboCount
    {
        get
        {
            return _ComboCount;
        }
        private set
        {
            _ComboCount = value;
            ComboText.text = _ComboCount.ToString();
            if (_ComboCount < 20)
                ComboText.color = Color.white;
            else if (_ComboCount >= 20 && _ComboCount < 50)
                ComboText.color = new Color(0.4f, 0.4f, 0.85f, 0.39f);
            else if (_ComboCount >= 50 && _ComboCount < 100)
                ComboText.color = Color.green;
            else if (_ComboCount >= 100)
                ComboText.color = Color.red;

            var oc = ComboText.color;
            oc.a = 0.39f;
            ComboText.color = oc;
            //  ComboAnimator.GetComponent<Animator>().SetTrigger("Show");
        }
    }
    public float SecondPerBeat
    {
        get
        {
            return 60f / BPM;
        }
    }

    public double CurrentBeat
    {
        get
        {
            double BeatInDouble = Music.time / SecondPerBeat + 1;
            return BeatInDouble - BeatInDouble % 0.125d;
        }
    }

    private bool _AC = true;
    public bool AC
    {
        get { return _AC; }
        set
        {
            if(_AC && !value)
            {
                RootAnmi.SetTrigger("AC_Lost");
            }

            _AC = value;
        }
    }

    public bool _AP = true;
    public bool AP
    {
        get { return _AP; }
        set
        {
            if (_AP && !value)
            {
                RootAnmi.SetTrigger("AP_Lost");
            }

            _AP = value;
        }
    }
}
