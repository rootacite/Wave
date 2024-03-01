using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CardController : MonoBehaviour
{
    public AudioClip Ef1;
    public TextMeshProUGUI NameText;
    
    // Start is called before the first frame update
    public GameObject AnimatorObject;
    public GameObject AudioObject;
    public GameObject ImageObject;


    public AudioClip TitleAudio;
    public Texture BackGround;
    public VideoClip Video;
    public TextAsset SongConfig;
    public TextAsset SongData;
    private Sprite BackGroundSprite;

    public string[] additionalInfo;

    public bool isExternal = false;
    public string ExternalPath = "";
    public void SetText(string t)
    {
        NameText.text = t;
    }
    private Texture2D TextureToTexture2D(Texture texture) {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }
    
    IEnumerator LoadSong()
    {
        AudioClip Song = null;

        if (!isExternal)
        {
            string Route = "Songs/" + InsideName + "/" + InsideName;
            Song = (AudioClip)Resources.Load(Route, typeof(AudioClip));
        }
        else
        {
            var au = new WWW(ExternalPath);
            yield return au;
            Song = au.GetAudioClip();
            au.Dispose();
        }

        if (Video == null)
            StartInit.SetImage(BackGround);
        else
            StartInit.SetImage(BackGround);
        StartInit.Show();

        yield return new WaitForSeconds(2.5f);

        var Task = SceneManager.LoadSceneAsync(2);
        Task.completed += (e) =>
        {
            GameScripting.Instance.Initialize(Song, SongConfig, SongData, BackGround, Video);
            StartInit.Hide();

        };
    }
    public string Name
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
        var tx2d = TextureToTexture2D(BackGround);
        BackGroundSprite =  Sprite.Create(tx2d, new Rect(0, 0, tx2d.width,tx2d.height), Vector2.zero);
        
        
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

            if (additionalInfo != null)
            {
                string cx = "";
                foreach (var t in additionalInfo)
                {
                    cx += t + "\n";
                }
                
                MenuRoot.SetAdditionText(cx);
            }

        }catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        AnimatorObject.GetComponent<Animator>().SetBool("ShowBK", true);
        yield return new WaitForSeconds(0.5f);
    }
}
