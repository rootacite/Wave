using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class HorPortController : MonoBehaviour
{ 
    public AudioSource titleAudioSource;
    public TouchManager touchManager;
    public RawImage background;
    public RawImage backgroundIns;
    public RawImage songImage;
    public RawImage songImageIns;
    public TextMeshProUGUI title;
    public TextMeshProUGUI additionInfo;
    public Material blurMat;
    public GameObject comingSoonText;

    private Vector2 _lastPoint;

    private bool _touching = false;
    
    private float _xMaxLimit = 0, _xMinLimit = 0;

    private float _lastX = 0;
    private BlurEffect _blurEffect;

    private bool _unMoved = false;

    IEnumerator LoadSong(string InsideName, Texture BackGround, TextAsset SongConfig, TextAsset SongData)
    {
        string Route = "Origin/" + InsideName + "/" + InsideName;
        AudioClip Song = (AudioClip)Resources.Load(Route, typeof(AudioClip));


        StartInit.SetImage(BackGround);
        StartInit.Show();

        yield return new WaitForSeconds(2.5f);

        var Task = SceneManager.LoadSceneAsync(2);
        Task.completed += (e) =>
        {
            GameScripting.Instance.Initialize(Song, SongConfig, SongData, BackGround, null);
            StartInit.Hide();

        };
    }

    public (SongInfo, AudioClip) GetCurrentSong(out bool comingSoon)
    {
        var tx = Mathf.Round(transform.localPosition.x / 60.0f) * 60.0f;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            if (Mathf.Abs(c.localPosition.x - tx) < 1f)
            {
                UnityEngine.Debug.Log(c.name);
                var lm = c.GetComponent<MusicPoint>();
                comingSoon = lm.comingSoon;
                return (new SongInfo()
                {
                    Song = lm.song,
                    BackGround = lm.backGround,
                    SongConfig = lm.songConfig,
                    SongData = lm.songData
                }, lm.titleAudio);
            }
        }

        comingSoon = true;
        return (new SongInfo(){ Song = null }, null);
    }
    void FingerMoved(Vector2 p)
    {
        var lp = transform.localPosition;

        if (lp.x + p.x * 20f < _xMinLimit || lp.x + p.x * 20f > _xMaxLimit)
            return;
        
        lp.x += p.x * 20f;
        transform.localPosition = lp;
    }

    IEnumerator SwitchMusic(SongInfo song, AudioClip titleShort)
    { 
        XDocument xmlSongConfig = XDocument.Parse(song.SongConfig.text);
        
        Stopwatch stw = new Stopwatch();
        stw.Reset();
        stw.Start();
        backgroundIns.gameObject.SetActive(true);
        backgroundIns.color = new Color(1, 1, 1, 0);
        backgroundIns.texture = _blurEffect.IO(song.BackGround);
        
        songImageIns.gameObject.SetActive(true);
        songImageIns.color = new Color(1, 1, 1, 0);
        songImageIns.texture = song.BackGround;
        
        while (true)
        {
            yield return null;
            double r = stw.ElapsedMilliseconds / 250d;
            background.color = new Color(1, 1, 1, (float)(1 - r));
            backgroundIns.color = new Color(1, 1, 1, (float)r);
            
            songImage.color = new Color(1, 1, 1, (float)(1 - r));
            songImageIns.color = new Color(1, 1, 1, (float)r);
            if (r > 1f) break;
        }

        background.texture = backgroundIns.texture;
        background.color = new Color(1, 1, 1, 1);
        backgroundIns.gameObject.SetActive(false);
        
        songImage.texture = songImageIns.texture;
        songImage.color = new Color(1, 1, 1, 1);
        songImageIns.gameObject.SetActive(false);
        
        title.text = xmlSongConfig.Root.Attribute("Name").Value;
        string Mark = PlayerPrefs.GetString(xmlSongConfig.Root.Attribute("Name").Value + "_Mark", "0000000");
        string TP = PlayerPrefs.GetString(xmlSongConfig.Root.Attribute("Name").Value + "_TP", "00.00%");
        
        additionInfo.text = $"Rank : { Mark }\nLevel : { xmlSongConfig.Root.Attribute("Level").Value }\nTP : { TP }";
        
        titleAudioSource.Stop();
        titleAudioSource.clip = titleShort;
        titleAudioSource.Play();
        stw.Restart();

        yield break;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            if (c.localPosition.x < _xMinLimit)
                _xMinLimit = c.localPosition.x;

            if (c.localPosition.x > _xMaxLimit)
                _xMaxLimit = c.localPosition.x;
        }

        touchManager.OnTouch.Add((t, p) =>
        {
            switch (t)
            {
                case TouchPhase.Began:
                    _lastPoint = p;
                    _touching = true;
                    _unMoved = true;
                    break;
                case TouchPhase.Moved:
                    FingerMoved(p - _lastPoint);
                    _lastPoint = p;
                    _unMoved = false;
                    break;
                case TouchPhase.Ended:
                    _touching = false;
                    var tx = Mathf.Round(transform.localPosition.x / 60.0f) * 60.0f;

                    var cs = GetCurrentSong(out bool comingSoon);
                    if (Math.Abs(_lastX - tx) > 1f)
                    {
                        if (cs.Item1.Song == null) break;
                        if (comingSoon) comingSoonText.SetActive(true);
                        else comingSoonText.SetActive(false);
                        StartCoroutine(SwitchMusic(cs.Item1, cs.Item2));
                    }

                    if (_unMoved && (!comingSoon))
                    {
                        StartCoroutine(LoadSong(
                            XDocument.Parse(cs.Item1.SongConfig.text).Root.Attribute("InsideName").Value,
                            cs.Item1.BackGround, cs.Item1.SongConfig, cs.Item1.SongData));
                        break;
                    }

                    _lastX = tx;

                    break;
            }

            return true;
        });

        GameObject blurObj = new GameObject("blurEffect");
        _blurEffect = blurObj.AddComponent<BlurEffect>();
        _blurEffect.blurMat = blurMat;
        _blurEffect.blurCount = 24;
        _blurEffect.blurSize = 0.65f;

        _blurEffect.isOpen = false;

        var cs = GetCurrentSong(out bool comingSoon);

        if (comingSoon) comingSoonText.SetActive(true);
        else comingSoonText.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (!_touching)
        {
            // 当标志为true时，使transform.localPosition.x线性地向最接近的可以被60整除的数变化
            float targetX;
            targetX = Mathf.Round(transform.localPosition.x / 60.0f) * 60.0f;
            
            transform.localPosition = new Vector3(Mathf.MoveTowards(transform.localPosition.x, targetX, Time.deltaTime * 250.0f), transform.localPosition.y, transform.localPosition.z);
        }
    }
}
