using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.Video;
using TextAsset = UnityEngine.TextAsset;

public class RestartController : MonoBehaviour
{
    public GameObject Audio;

    public static Texture BackGround;
    public static AudioClip Song;
    public static VideoClip Video;

    public static TextAsset SongData;
    public static TextAsset SongConfig;
    // Start is called before the first frame update
    void Start()
    {
        var btn = GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() =>
        {
            Audio.GetComponent<AudioSource>().Stop();
            Time.timeScale = 1.0f;
            StartInit.Show();
            var Task = SceneManager.LoadSceneAsync(2);
            Task.completed += (e) =>
            {
                GameScripting.Instance.Initialize(Song, SongConfig, SongData, BackGround, Video);

                StartInit.Hide();

            };
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
