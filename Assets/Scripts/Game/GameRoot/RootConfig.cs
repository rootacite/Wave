using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

static public class Vector2Extence
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

public partial class RootConfig : MonoBehaviour
{
    static public RootConfig instance;
    public event DataReloading Reloading;
    public void Initialize(AudioClip Music, TextAsset SongConfig, TextAsset SongData, Texture Image, VideoClip Video)
    {
        this.Music.clip = Music;
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
            vp.Play();
        }

        XDocument XmlSongConfig = XDocument.Parse(SongConfig.text);

        this.Image = Image;
        BPM = (float)Convert.ToDouble(XmlSongConfig.Root.Attribute("BPM").Value);
        HeadPending = (float)Convert.ToDouble(XmlSongConfig.Root.Attribute("HeadPending").Value);
        Full = Convert.ToInt32(XmlSongConfig.Root.Attribute("Full").Value);
        BeatPerSection = Convert.ToInt32(XmlSongConfig.Root.Attribute("BeatPerSection").Value);
        MusicData = SongData;
        Name = XmlSongConfig.Root.Attribute("Name").Value;
        Music_Length = Convert.ToInt32(XmlSongConfig.Root.Attribute("Length").Value);
        Level = Convert.ToInt32(XmlSongConfig.Root.Attribute("Level").Value);
        Accuracy = Convert.ToInt32(XmlSongConfig.Root.Attribute("Accuracy").Value);

        if (XmlSongConfig.Root.Attribute("WorldCoord") != null)
        {
            UseWorldCoord = XmlSongConfig.Root.Attribute("WorldCoord").Value == "true";
        }

        RestartController.BackGround = Image;
        RestartController.Song = Music;
        RestartController.SongConfig = SongConfig;
        RestartController.SongData = SongData;
        RestartController.Video = Video;

        if(!isDebugMode)
        {
            DebugPlane.SetActive(false);
        }
        else
        {
            OnBeat += (t) =>
            {
                if (this.Music.isPlaying)
                    BeatEditor.text = (t + HeadPending).ToString();
            };
        }
    }
    public void ReloadSongData()
    {
        if (isDebugMode && Reloading != null)
        {
            MusicData = Reloading?.Invoke();
        }
        DataRoot = XDocument.Parse(MusicData.text).Root;

        KeyGroup.Clear();
        foreach (XElement Group in DataRoot.Nodes())
        {
            List<KeyTime> local = new List<KeyTime>();

            foreach (XElement Key in Group.Nodes())
            {
                local.Add(new KeyTime(Key));
            }

            KeyGroup.Add(local);
        }

        while (true)
        {
            if (KeyGroup.Count == 0) break;
            if (KeyGroup[0].Count == 0) { KeyGroup.RemoveAt(0); continue; }
            if (KeyGroup[0][0].Time < CurrentBeat + 1)
                KeyGroup[0].RemoveAt(0);
            else
            {
                break;
            }
        }


    }

    List<List<KeyTime>> KeyGroup = new List<List<KeyTime>>();

    //////////////////////Level Attributes////////////////////////////

    public bool UseWorldCoord { get; set; } = false;
    private Texture Image;
    private float Music_Length;
    private string Name;
    private TextAsset MusicData;
    private int Full = 0;
    private int BeatPerSection = 0;
    public float BPM { get; private set; } = 89f;
    public float HeadPending { get; private set; } = 1f;
    private int Level;
    private int Accuracy;

    private XElement DataRoot;
    ////////////////////////////////////////////////////////////////


    #region Keys
    public GameObject Tap;
    public GameObject Hold;
    public GameObject Slide;
    public GameObject Wave;
    public GameObject Drag;

    public GameObject LineAreaObj;
    #endregion
    #region ISet
    public GameObject KeyLayer;


    public TextMeshProUGUI MarkText;
    public GameObject SongBack;
    public TextMeshProUGUI ComboText;
    public Animator ClearAnimator;
    public TextMeshProUGUI ClearText;
    public TextMeshProUGUI GLevel_Text;
    public TextMeshProUGUI Name_Text;
    public GameObject PrePoint_Obj;

    public Animator RootAnmi;

    public AudioSource Music;

    public GameObject TowardTip;

    #endregion

    bool new_section_flag = true;
    public bool isDebugMode
    {
        get
        {
            return PlayerPrefs.GetInt("Debug_Setting", 0) == 1;
        }
    }
    void Awake()
    {
        instance = this;

        Keys.AutoMode = PlayerPrefs.GetInt("AutoMode_Setting", 0) == 1;
    }
    void Start()
    {
        float ScreenRate = Screen.width / 2400f;


        if (UseDebugResource)
        {
            Initialize(Debug_Music, Debug_SongConfig, Debug_SongData, Debug_Image, Debug_Video);
        }

        // Time.timeScale = 0.3f;
        #region Init
        // Debug.Log(0);
        //  TapController.Creat(Tap, gameObject, SecondPerBeat);
        GLevel_Text.text = "Lv." + Level.ToString();
        Name_Text.text = Name;



        DataRoot = XDocument.Parse(MusicData.text).Root;

        foreach (XElement Group in DataRoot.Nodes())
        {
            List<KeyTime> local = new List<KeyTime>();

            foreach (XElement Key in Group.Nodes())
            {
                local.Add(new KeyTime(Key));
            }

            KeyGroup.Add(local);
        }

        //WorldCoord x:±9.6   y:±5.4 
        #endregion
        if (!UseWorldCoord)
            OnBeat += (t) =>
            {
                while (true)
                {
                    if (KeyGroup.Count == 0) return;
                    if (KeyGroup[0].Count == 0) { new_section_flag = true; KeyGroup.RemoveAt(0); continue; }
                    break;
                }


                bool Reser = false;
                Vector2? Last_Pos_Save = null;

                while (t + HeadPending >= KeyGroup[0][0].Time)
                {
                    bool ns = false;
                    if (new_section_flag) { ns = true; new_section_flag = false; }

                    //在这里创建的音符都在横判定线的一拍后被触发
                    float cy = (float)GetJudgeY(KeyGroup[0][0].Time);
                    if (KeyGroup[0][0].ForceY != null)
                    {
                        cy = (int)KeyGroup[0][0].ForceY.Value;
                    }

                    var wp = Camera.main.ScreenToWorldPoint(new Vector3(Accuracy + Accuracy * (float)KeyGroup[0][0].Pos, cy + 540, 0));

                    if (KeyGroup[0][0].ForceY == null)
                    {
                        wp.y = cy;
                    }

                    Keys ct = null;
                    if (KeyGroup[0][0].Type == KeyType.Tap)
                    {
                        ct = CreateTap(wp, HeadPending);

                        wp.z = -9f;
                        var dc = CreateVecLine(wp, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;

                    }
                    else

                    if (KeyGroup[0][0].Type == KeyType.Hold)
                    {
                        ct = CreateHold(wp, (float)KeyGroup[0][0].Length, HeadPending);

                        wp.z = -9f;
                        var dc = CreateVecLine(wp, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;
                    }
                    else

                    if (KeyGroup[0][0].Type == KeyType.Slide)
                    {
                        ct = CreateSlide(wp, HeadPending);

                        wp.z = -9f;
                        var dc = CreateVecLine(wp, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;
                    }
                    else

                    if (KeyGroup[0][0].Type == KeyType.Wave)
                    {
                        ct = CreateWave(wp, KeyGroup[0][0].Childrens, (float)KeyGroup[0][0].Length, KeyGroup[0][0].LastChildTime, HeadPending, (float)KeyGroup[0][0].WaveScale);
                        wp.z = -9f;

                        var dc = CreateVecLine(wp, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;

                    }
                    else

                    if (KeyGroup[0][0].Type == KeyType.Drag)
                    {

                        StartCoroutine(CreateDrag(KeyGroup[0][0].IDragData.From, KeyGroup[0][0].IDragData.To, KeyGroup[0][0].IDragData.Count, KeyGroup[0][0].Length, HeadPending));

                    }

                    if (ct != null && KeyGroup[0][0].NextToward != null)
                    {
                        var ttObj = Instantiate(TowardTip, wp, KeyLayer.transform.rotation, KeyLayer.transform);
                        ttObj.transform.Rotate(new Vector3(0, 0, (float)KeyGroup[0][0].NextToward), Space.Self);
                        ct.OnInvailded += (s) =>
                        {
                            Destroy(ttObj);
                        };
                    }
                    if (Reser)
                    {
                        var wp1 = Camera.main.ScreenToWorldPoint(new Vector3(Accuracy + Accuracy * Last_Pos_Save.Value.x, 0, 0));
                        wp1.y = Last_Pos_Save.Value.y;
                        var wp2 = Camera.main.ScreenToWorldPoint(new Vector3(Accuracy + Accuracy * (float)(KeyGroup[0][0].Type == KeyType.Drag ? KeyGroup[0][0].IDragData.From : KeyGroup[0][0].Pos), 0, 0));
                        wp2.y = wp.y;

                        wp1.z = 88;
                        wp2.z = 88;

                        LineArea.Create(LineAreaObj, KeyLayer, wp1, wp2, HeadPending * SecondPerBeat);

                        if(Math.Abs(wp1.y) >3.2|| Math.Abs(wp2.y) > 3.2)
                        {
                            Debug.Log("Enter");
                        }
                    }

                    Last_Pos_Save = new Vector2((float)(KeyGroup[0][0].Type == KeyType.Drag ? KeyGroup[0][0].IDragData.From : KeyGroup[0][0].Pos), wp.y);
                    Reser = true;

                    KeyGroup[0].RemoveAt(0);
                    if (KeyGroup[0].Count == 0)
                    {
                        break;
                    }

                }
            };

        else
            OnBeat += (t) =>
            {
                while (true)
                {
                    if (KeyGroup.Count == 0) return;
                    if (KeyGroup[0].Count == 0) { new_section_flag = true; KeyGroup.RemoveAt(0); continue; }
                    break;
                }

                bool Reser = false;
                Vector2? Last_Pos_Save = null;

                while (t + HeadPending >= KeyGroup[0][0].Time)
                {
                    if (new_section_flag) { new_section_flag = false; }

                    //在这里创建的音符都在横判定线的一拍后被触发
                    Vector3 CreateAtHere = new Vector2();

                    if (KeyGroup[0][0].ForceY != null)
                    {
                        CreateAtHere.y = (float)KeyGroup[0][0].ForceY.Value;
                    }
                    else
                        CreateAtHere.y = (float)GetJudgeY(KeyGroup[0][0].Time);

                    CreateAtHere.x = (float)KeyGroup[0][0].Pos;
                    CreateAtHere.z = -9f;


                    Keys ct = null;
                    if (KeyGroup[0][0].Type == KeyType.Tap)
                    {
                        ct = CreateTap(CreateAtHere, HeadPending);

                        var dc = CreateVecLine(CreateAtHere, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;

                    }
                    else

                    if (KeyGroup[0][0].Type == KeyType.Hold)
                    {
                        ct = CreateHold(CreateAtHere, (float)KeyGroup[0][0].Length, HeadPending);

                        var dc = CreateVecLine(CreateAtHere, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;
                    }
                    else

                    if (KeyGroup[0][0].Type == KeyType.Slide)
                    {
                        ct = CreateSlide(CreateAtHere, HeadPending);

                        var dc = CreateVecLine(CreateAtHere, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;
                    }
                    else

                    if (KeyGroup[0][0].Type == KeyType.Wave)
                    {
                        ct = CreateWave(CreateAtHere, KeyGroup[0][0].Childrens, (float)KeyGroup[0][0].Length, KeyGroup[0][0].LastChildTime, HeadPending, (float)KeyGroup[0][0].WaveScale);
                        
                        var dc = CreateVecLine(CreateAtHere, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
                        dc.Key = ct.BAnimation;
                    }
                    else
                    if (KeyGroup[0][0].Type == KeyType.Drag)
                    {

                        StartCoroutine(CreateDrag(KeyGroup[0][0].IDragData.From, KeyGroup[0][0].IDragData.To, KeyGroup[0][0].IDragData.Count, KeyGroup[0][0].Length, HeadPending));

                    }

                    if (ct != null && KeyGroup[0][0].NextToward != null)
                    {
                        var ttObj = Instantiate(TowardTip, CreateAtHere, KeyLayer.transform.rotation, KeyLayer.transform);
                        ttObj.transform.Rotate(new Vector3(0, 0, (float)KeyGroup[0][0].NextToward), Space.Self);
                        ct.OnInvailded += (s) =>
                        {
                            Destroy(ttObj);
                        };
                    }
                    if (Reser)
                    {
                        var wp1 = new Vector3(Last_Pos_Save.Value.x, Last_Pos_Save.Value.y, 88f);
                        var wp2 = new Vector3((float)(KeyGroup[0][0].Type == KeyType.Drag ? KeyGroup[0][0].IDragData.From : KeyGroup[0][0].Pos), CreateAtHere.y, 88f);


                        LineArea.Create(LineAreaObj, KeyLayer, wp1, wp2, HeadPending * SecondPerBeat);
                    }

                    Last_Pos_Save = new Vector2((float)(KeyGroup[0][0].Type == KeyType.Drag ? KeyGroup[0][0].IDragData.From : KeyGroup[0][0].Pos), CreateAtHere.y);
                    Reser = true;

                    KeyGroup[0].RemoveAt(0);
                    if (KeyGroup[0].Count == 0)
                    {
                        break;
                    }

                }
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
            SettlementController.instance.Rate = CurrentMark / (double)Full * 100d;
            SettlementController.instance.Mark = CurrentMark;
            SettlementController.instance.Name = (string)Name.Clone();
            SettlementController.instance.Level = Level;
            SettlementController.instance.AC = AC;
            SettlementController.instance.AP = AP;

            StartInit.Hide();
        };
    }
    bool Stopped = false;
    // Update is called once per frame
    void Update()
    {
        if (Stopped) return;
        if (Music.time >= Music_Length && Music.clip != null)
        {
            Music.Stop();
            Stopped = true;
            StartCoroutine(StopAndClear());
            return;
        }


        if (CurrentBeat != Last_Beat)
        {
            OnBeat?.Invoke(CurrentBeat);
        }
        Last_Beat = CurrentBeat;
    }

    private double Last_Beat = 0d;
    public void OnPrepaired()
    {
        Music.Play();
        //JudegLineAnimator.SetTrigger("Enterance");
    }

    public double JudgeY
    {
        get
        {
            double r = 0;
            double Beat = Music.time / SecondPerBeat + 1;
            double vx;
            switch (BeatPerSection)
            {
                case 4:

                    vx = (Beat - 1d) % 8d + 1;
                    if (vx >= 1 && vx < 5) { r = -1.6d * vx + 4.8d; }
                    if (vx >= 5 && vx < 9) { r = 1.6d * vx - 11.2d; }

                    return r;
                case 2:

                    vx = (Beat - 1d) % 4d + 1;
                    if (vx >= 1 && vx < 3) { r = -3.2d * vx + 6.4d; }
                    if (vx >= 3 && vx < 5) { r = 3.2d * vx - 12.8d; }
                    return r;
                default:
                    throw new Exception("Unsupproted Section.");
            }
        }
    }

    private double GetJudgeY(double Beat)
    {
        double r = 0;
        double vx;
        switch (BeatPerSection)
        {
            case 4:

                vx = (Beat - 1d) % 8d + 1;
                if (vx >= 1 && vx < 5) { r = -1.6d * vx + 4.8d; }
                if (vx >= 5 && vx < 9) { r = 1.6d * vx - 11.2d; }

                return r;
            case 2:

                vx = (Beat - 1d) % 4d + 1;
                if (vx >= 1 && vx < 3) { r = -3.2d * vx + 6.4d; }
                if (vx >= 3 && vx < 5) { r = 3.2d * vx - 12.8d; }
                return r;
            default:
                throw new Exception("Unsupproted Section.");
        }
    }
}
