using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboText : MonoBehaviour
{
    private TextMeshProUGUI _comboText;

    private void Start()
    {
        _comboText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _comboText.text = GameScripting.Instance.ComboCount.ToString();
        if (GameScripting.Instance.ComboCount < 20)
            _comboText.color = Color.white;
        else if (GameScripting.Instance.ComboCount >= 20 && GameScripting.Instance.ComboCount < 50)
            _comboText.color = new Color(0.4f, 0.4f, 0.85f, 0.39f);
        else if (GameScripting.Instance.ComboCount >= 50 && GameScripting.Instance.ComboCount < 100)
            _comboText.color = Color.green;
        else if (GameScripting.Instance.ComboCount >= 100)
            _comboText.color = Color.red;

        var oc = _comboText.color;
        oc.a = 0.39f;
        _comboText.color = oc;
    }
}
