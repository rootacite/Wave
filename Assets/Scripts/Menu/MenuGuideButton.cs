using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGuideButton : MonoBehaviour
{
    IEnumerator Proc_2()
    {
        StartInit.Show();

        yield return new WaitForSeconds(1f);

        var Task = SceneManager.LoadSceneAsync(2);
        Task.completed += (e) =>
        {
            GameScripting.Instance.Initialize(Song, SongConfig, SongData, BackGround, null);

            GameScripting.Instance.Metronome.OnBeat += (t) =>
            {
                if (t == 2)
                {
                    StartInit.ShowText("Tap键\n跟随节奏，当下落的雨滴打在上面时点击。", 6);
                    return;
                }
                if (t == 18)
                {
                    StartInit.ShowText("Hold键\n当下落的雨滴打在上面时其中心或内圈完全舒展时触摸并保持。待外圈完全展开至虚线边缘时方可松开。", 6);
                    return;
                }
                if (t == 35)
                {
                    StartInit.ShowText("Slide键\n当下落的雨滴打在上面时，点击并向任意方向滑动。", 6);
                    return;
                }
                if (t == 47f)
                {
                    StartInit.ShowText("Wave，当下落的雨滴打在上面时点击，与Tap键不同的是\nWave键被点击时，会生成一个环形的判定区域\"星盘\"\n星盘中的键需要在波纹与其重合时点击。", 12);
                    return;
                }
                if (t == 64f)
                {
                    StartInit.ShowText("Drag键\n紫色按键。\n当判定线划过其中心或外圈完全收缩时触摸或划过。\n",6);
                    return;
                }
            };
            StartInit.Hide();

        };

    }

    public AudioClip Song;
    //   public TextAsset SongConfig;
    public Texture BackGround;
    public TextAsset SongData;
    public TextAsset SongConfig;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            StartCoroutine(Proc_2());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
