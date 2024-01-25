using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettlementController : MonoBehaviour
{
#region Externce

    public Texture Image;
    public double Rate;
    public string Name;
    public int Mark;
    [FormerlySerializedAs("Level")] public int Difficulty;

    public bool AC;
    public bool AP;

    #endregion

#region Local

    public GameObject BackToTitle;
    public GameObject BackGround;
    public GameObject CG;

    public GameObject NameRange;
    public GameObject LevelRange;
    public GameObject MarkRange;
    public GameObject RateRange;

    #endregion

    IEnumerator Title()
    {
        StartInit.Show();
        yield return new WaitForSecondsRealtime(1);
        var Async = SceneManager.LoadSceneAsync(1);
        Async.completed += (v) =>
        {
            StartInit.Hide();
        };
    }
    static public SettlementController instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        NameRange.GetComponent<TextMeshProUGUI>().text = Name;
        LevelRange.GetComponent<TextMeshProUGUI>().text = "Lv." + Difficulty.ToString();
        MarkRange.GetComponent<TextMeshProUGUI>().text = Mark.ToString();
        RateRange.GetComponent<TextMeshProUGUI>().text = Rate.ToString("0.00") + "%";

        if (AP) RateRange.GetComponent<TextMeshProUGUI>().text += " AP";
        else
          if (AC) RateRange.GetComponent<TextMeshProUGUI>().text += " AC";

        int OldMark = Convert.ToInt32(PlayerPrefs.GetString(Name + "_Mark", "0000000"));
        double OldTP = Convert.ToDouble(PlayerPrefs.GetString(Name + "_TP", "00.00%").Substring(0, 5));

        if (!Keys.AutoMode)
        {
            if (Mark > OldMark)
            {
                PlayerPrefs.SetString(Name + "_Mark", Mark.ToString());
            }

            if (Rate > OldTP)
            {
                PlayerPrefs.SetString(Name + "_TP", Rate.ToString("0.00") + "%");
            }
            if(AC)
            {
                PlayerPrefs.SetInt(Name + "_AC", 1);
            }
            if (AP)
            {
                PlayerPrefs.SetInt(Name + "_AP", 1);
            }
        }
        PlayerPrefs.Save();

        BackGround.GetComponent<RawImage>().texture = Image;
        CG.GetComponent<RawImage>().texture = Image;
        BackToTitle.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(Title());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
