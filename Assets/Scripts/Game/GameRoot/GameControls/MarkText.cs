using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarkText : MonoBehaviour
{
    private TextMeshProUGUI _markText;
    
    // Start is called before the first frame update
    void Start()
    {
        _markText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _markText.text = GameScripting.BuildNum(GameScripting.Instance.CurrentMark);
    }
}
