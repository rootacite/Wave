using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechnicalPoints : MonoBehaviour
{
    private TextMeshProUGUI _tp;
    // Start is called before the first frame update
    void Start()
    {
        _tp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        double rate = GameScripting.Instance.CurrentMark / (double)LevelBasicInformation.Full;
        _tp.text = (rate * 100d).ToString("00.00") + "%";
    }
}
