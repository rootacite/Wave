using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CardController : MonoBehaviour
{
    public AudioClip Ef1;
    // Start is called before the first frame update
    public GameObject AnimatorObject;
    public GameObject AudioObject;
    public GameObject ImageObject;

    public Sprite BackGroundSprite;

    public AudioClip TitleAudio;

    public Sprite Pic;
    public Texture BackGround;
    public VideoClip Video;
    public TextAsset SongConfig;
    public TextAsset SongData;
    IEnumerator LoadSong()
    {
        string Route = "Songs/" + InsideName + "/" + InsideName;
        AudioClip Song = (AudioClip)Resources.Load(Route, typeof(AudioClip));

        if (Video == null)
            StartInit.SetImage(BackGround);
        else
            StartInit.SetImage(BackGround);
        StartInit.Show();

        yield return new WaitForSeconds(2.5f);

        var Task = SceneManager.LoadSceneAsync(2);
        Task.completed += (e) =>
        {
            GameScripting.instance.Initialize(Song, SongConfig, SongData, BackGround, Video);
            StartInit.Hide();

        };
    }
    string Name
    {
        get
        {
            return XDocument.Parse(SongConfig.text).Root.Attribute("Name").Value;
        }
    }

    string InsideName
    {
        get
        {
            return XDocument.Parse(SongConfig.text).Root.Attribute("InsideName").Value;
        }
    }
    IEnumerator PicExpland()
    {
        if (ImageObject.GetComponent<UnityEngine.UI.Image>().sprite != BackGroundSprite)
        {
            yield return new WaitForSeconds(1.25f);
        }
        AnimatorObject.GetComponent<Animator>().SetTrigger("UIExit");
        yield return new WaitForSeconds(1f);

        StartCoroutine(LoadSong());
    }
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => 
        {
            StartCoroutine(PicExpland());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        StartCoroutine(SetTitle());
        TempPlay.PlayAudioOnce(Ef1);
    }

    IEnumerator SetTitle()
    {
        if (ImageObject.GetComponent<UnityEngine.UI.Image>().sprite == BackGroundSprite) yield break;

        AnimatorObject.GetComponent<Animator>().SetBool("ShowBK", false);
        yield return new WaitForSeconds(0.75f);
        ImageObject.GetComponent<UnityEngine.UI.Image>().sprite = BackGroundSprite;
        AudioObject.GetComponent<AudioSource>().Stop();
        AudioObject.GetComponent<AudioSource>().clip = TitleAudio;
        AudioObject.GetComponent<AudioSource>().Play();

        try
        {
            string Mark = PlayerPrefs.GetString(XDocument.Parse(SongConfig.text).Root.Attribute("Name").Value + "_Mark", "0000000");
            string TP = PlayerPrefs.GetString(XDocument.Parse(SongConfig.text).Root.Attribute("Name").Value + "_TP", "00.00%");

            if (PlayerPrefs.GetInt(Name + "_AP", 0) == 1) TP += " AP";
            else
            if (PlayerPrefs.GetInt(Name + "_AC", 0) == 1) TP += " AC";


            AnimatorObject.GetComponent<MenuRoot>().SetMark(
                Mark,
                TP,
                "Lv." + XDocument.Parse(SongConfig.text).Root.Attribute("Level").Value
                );

        }catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        AnimatorObject.GetComponent<Animator>().SetBool("ShowBK", true);
        yield return new WaitForSeconds(0.5f);
    }
}
