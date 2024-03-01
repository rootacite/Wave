using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public static class Vector2Extence
{
    static public Vector2 Offset(this Vector2 Origin, float Angle, float R)
    {
        return new Vector2(Origin.x + R * MathF.Cos(MathF.PI / 180f * Angle), Origin.y + R * MathF.Sin(MathF.PI / 180f * Angle));
    }

    static public Vector2 Offset(this Vector2 Origin, Vector2 Toward, float R)
    {
        float Distance = (Toward - Origin).magnitude;
        float Rate = R / Distance;
        return new Vector2(Origin.x + (Toward - Origin).x * Rate, Origin.y + (Toward - Origin).y * Rate);
    }
}

public partial class GameScripting : MonoBehaviour
{
    // Initialization
    public void Initialize(AudioClip Music, TextAsset SongConfig, TextAsset SongData, Texture Image, VideoClip Video)
    {
        this.Image = Image;
        this.MusicData = SongData;
        
        if (Video == null)
        {
            SongBack.GetComponent<UnityEngine.UI.RawImage>().texture = Image;

        }
        else
        {
            RenderTexture Tx = new RenderTexture(2400, 1350, 32, RenderTextureFormat.ARGB32);

            var vp = SongBack.AddComponent<VideoPlayer>();
            vp.clip = Video;
            vp.isLooping = true;
            vp.playOnAwake = false;
            vp.waitForFirstFrame = true;
            vp.skipOnDrop = true;
            vp.renderMode = VideoRenderMode.RenderTexture;
            vp.audioOutputMode = VideoAudioOutputMode.None;
            vp.targetTexture = Tx;
            SongBack.GetComponent<UnityEngine.UI.RawImage>().texture = Tx;
            SongBack.GetComponent<BlurEffect>().isOpen = false;

            vp.Stop();
            vp.Prepare();
        }

        XDocument XmlSongConfig = XDocument.Parse(SongConfig.text);
        LevelBasicInformation.Bpm = (float)Convert.ToDouble(XmlSongConfig.Root.Attribute("BPM").Value);
        LevelBasicInformation.HeadPending = (float)Convert.ToDouble(XmlSongConfig.Root.Attribute("HeadPending").Value);
        LevelBasicInformation.Full = Convert.ToInt32(XmlSongConfig.Root.Attribute("Full").Value);
        LevelBasicInformation.BeatPerSection = Convert.ToInt32(XmlSongConfig.Root.Attribute("BeatPerSection").Value);
        LevelBasicInformation.Name = XmlSongConfig.Root.Attribute("Name").Value;
        LevelBasicInformation.MusicDuration = Convert.ToInt32(XmlSongConfig.Root.Attribute("Length").Value);
        LevelBasicInformation.Difficulty = Convert.ToInt32(XmlSongConfig.Root.Attribute("Level").Value);
        LevelBasicInformation.Accuracy = Convert.ToInt32(XmlSongConfig.Root.Attribute("Accuracy").Value);

        LevelBasicInformation.UseWorldCoordinate = false;
        if (XmlSongConfig.Root.Attribute("WorldCoord") != null)
        {
            LevelBasicInformation.UseWorldCoordinate = XmlSongConfig.Root.Attribute("WorldCoord").Value == "true";
        }
        
        LevelBasicInformation.UseAdvanceBeat = false;
        if (XmlSongConfig.Root.Attribute("UseAdvanceBeat") != null)
        {
            LevelBasicInformation.UseAdvanceBeat = XmlSongConfig.Root.Attribute("UseAdvanceBeat").Value == "true";
        }

        RestartController.BackGround = Image;
        RestartController.Song = Music;
        RestartController.SongConfig = SongConfig;
        RestartController.SongData = SongData;
        RestartController.Video = Video;
    }
    public void FlushSongData()
    {
        if (IsDebugMode && Reloading != null)
        {
            MusicData = Reloading?.Invoke();
        }
        DataRoot = XDocument.Parse(MusicData.text).Root;
        KeyLayer.GetComponent<Creator>().Flush(DataRoot);
    }
    
    //////////////////////Level Attributes////////////////////////////
    private Texture Image;
    private TextAsset MusicData;
    private XElement DataRoot;
    ////////////////////////////////////////////////////////////////
    
    #region ISet
    public Metronome Metronome;
    public GameObject KeyLayer;  // Creator
   
    public GameObject SongBack;
    public Animator ClearAnimator;
    public TextMeshProUGUI ClearText;
    public TextMeshProUGUI GLevel_Text;
    public TextMeshProUGUI Name_Text;
    public Animator RootAnmi;
    #endregion

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Metronome.Music.clip = RestartController.Song;
        if(!IsDebugMode)
        {
            DebugPlane.SetActive(false);
        }
        else
        {
            BeatEditor.textComponent.alignment = TextAlignmentOptions.Center;
            Metronome.OnBeat += (t) =>
            {
                if (Metronome.Music.isPlaying)
                    BeatEditor.text = AdvanceBeat.Parse(t).ToString();
            };
        }
        if (UseDebugResource)
        {
            Initialize(Debug_Music, Debug_SongConfig, Debug_SongData, Debug_Image, Debug_Video);
        }

        #region Init
        GLevel_Text.text = "Lv." + LevelBasicInformation.Difficulty.ToString();
        Name_Text.text = LevelBasicInformation.Name;
        FlushSongData();    // Load Script
        //WorldCoord x:??9.6   y:??5.4 
        #endregion
        KeyLayer.SetActive(true);
        Metronome.OnEnd += () =>
        {
            StartCoroutine(StopAndClear());
        };
    }
    IEnumerator StopAndClear()
    {
        ClearText.text = "Clear!";
        ClearAnimator.SetTrigger("Show");

        yield return new WaitForSeconds(2.5f);

        StartInit.Show();
        yield return new WaitForSeconds(1f);

        var Async = SceneManager.LoadSceneAsync(4);
        Async.completed += (v) =>
        {
            SettlementController.instance.Image = Image;
            SettlementController.instance.Rate = CurrentMark / (double)LevelBasicInformation.Full * 100d;
            SettlementController.instance.Mark = CurrentMark;
            SettlementController.instance.Name = (string)LevelBasicInformation.Name.Clone();
            SettlementController.instance.Difficulty = LevelBasicInformation.Difficulty;
            SettlementController.instance.AC = AC;
            SettlementController.instance.AP = AP;

            StartInit.Hide();
        };
    }
    public void OnPrepared()
    {
        VideoPlayer vp = null;
        Metronome.Active();
        if ((vp = SongBack.GetComponent<VideoPlayer>()) != null)
        {
            vp.Play();
        }
    }
    
    public bool IsDebugMode => PlayerPrefs.GetInt("Debug_Setting", 0) == 1;
    public int CurrentMark
    {
        get;
        set;
    }
    public int ComboCount
    {
        get;
        set;
    }
    private bool _AC = true;
    public bool AC
    {
        get { return _AC; }
        set
        {
            if(_AC && !value)
            {
                RootAnmi.SetTrigger("AC_Lost");
            }

            _AC = value;
        }
    }
    public bool _AP = true;
    public bool AP
    {
        get { return _AP; }
        set
        {
            if (_AP && !value)
            {
                RootAnmi.SetTrigger("AP_Lost");
            }

            _AP = value;
        }
    }
}
