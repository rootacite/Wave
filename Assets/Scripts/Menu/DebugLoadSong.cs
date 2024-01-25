using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;

public class DebugLoadSong : MonoBehaviour
{
    public Button Button;
    public TMP_InputField Input;

    IEnumerator LoadData(string Name)
    {

        Texture2D Image = GetImage(Name);
        yield return StartCoroutine(LoadExternalAudioWebRequest(Name));

        TextAsset SongConfig = new TextAsset(File.ReadAllText(@"Data\" + Name + @".xml"));
        TextAsset SongData = new TextAsset(File.ReadAllText(@"Data\" + Name + @"_data.xml"));

        StartInit.SetImage(Image);
        StartInit.Show();

        yield return new WaitForSeconds(1f);

        var Task = SceneManager.LoadSceneAsync(2);
        Task.completed += (e) =>
        {
            GameScripting.Instance.Initialize(MusicClip, SongConfig, SongData, Image, null);
            GameScripting.Instance.Reloading += () =>
            {
                return new TextAsset(File.ReadAllText(@"Data\" + Name + @"_data.xml"));
            };
            StartInit.Hide();

        };
    }
    // Start is called before the first frame update
    void Start()
    {
        Button.onClick.AddListener(() =>
        {
            string Name = Input.text;
            StartCoroutine(LoadData(Name));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator LoadExternalAudioWebRequest(string Name, AudioType _audioType = AudioType.WAV)
    {
        string readPath = Environment.CurrentDirectory + "\\" + "Data" + "\\" + Name + ".wav";//��ȡ�ļ���·��

        yield return null;
        UnityWebRequest _unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(readPath, _audioType);
        yield return _unityWebRequest.SendWebRequest();

        if (_unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(_unityWebRequest.error.ToString());
        }
        else
        {
            MusicClip = DownloadHandlerAudioClip.GetContent(_unityWebRequest);
        }
    }

    Texture2D GetImage(string Name, int Height = 1080, int Width = 1920)
    {
        byte[] Data = File.ReadAllBytes("Data\\" + Name + ".png");
        Texture2D txr = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        txr.LoadImage(Data);

        return txr;
    }

    AudioClip MusicClip = null;
}
