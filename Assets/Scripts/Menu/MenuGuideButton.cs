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

        GameObject corObj = new ("GuideObj"); // Create the new obj to contain Guide Coroutine
        var sgc = corObj.AddComponent<SimpleGuideContrion>();
        DontDestroyOnLoad(corObj);

        var Task = SceneManager.LoadSceneAsync(2);
        Task.completed += (e) =>
        {
            GameScripting.Instance.Initialize(Song, SongConfig, SongData, BackGround, null);
            sgc.StartGuide(GameScripting.Instance);
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
