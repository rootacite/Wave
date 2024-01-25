using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatCalc : MonoBehaviour
{

    public GameScripting Scripter;
    public TMP_InputField Text;

    private bool Flag = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool Status = Input.GetKeyDown(KeyCode.Space);
        if (!Status)
        {
            Flag = false;
        }
        else
        {
            if (Flag) return;

            Text.text = (Scripter.Metronome.CurrentBeat - 0.125).ToString();

            Flag = true;
        }
    }
}
