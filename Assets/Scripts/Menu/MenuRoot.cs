using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoot : MonoBehaviour
{
    public bool isDebugMode
    {
        get
        {
            return PlayerPrefs.GetInt("Debug_Setting", 0) == 1;
        }
    }
    public GameObject MarkText;
    public GameObject TpText;
    public GameObject LevelText;

    public GameObject DebugTools;
    // Start is called before the first frame update

    public void SetMark(string Mark, string TP, string Level)
    {
        MarkText.GetComponent<TMPro.TextMeshProUGUI>().text = Mark;
        TpText.GetComponent<TMPro.TextMeshProUGUI>().text = TP;
        LevelText.GetComponent<TMPro.TextMeshProUGUI>().text = Level;
    }
    void Start()
    {
        SetMark(
            PlayerPrefs.GetString("三色绘恋S" + "_Mark", "0000000"),
            PlayerPrefs.GetString("三色绘恋S" + "_TP", "00.00%"), "Lv.3");

       
    }
    private void Awake()
    {
        if (!isDebugMode)
        {
            DebugTools.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
