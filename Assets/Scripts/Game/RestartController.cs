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

    static public Texture BackGround;
    static public AudioClip Song;
    static public VideoClip Video;

    static public TextAsset SongData;
    static public TextAsset SongConfig;
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
                RootConfig.instance.Initialize(Song, SongConfig, SongData, BackGround, Video);

                StartInit.Hide();

            };
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
