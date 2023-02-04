using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using Dictionary = System.IO.Directory;
public class MenuRoot : MonoBehaviour
{
    public bool isDebugMode
    {
        get
        {
            return PlayerPrefs.GetInt("Debug_Setting", 0) == 1;
        }
    }
    public GameObject MarkText;
    public GameObject TpText;
    public GameObject LevelText;

    public GameObject DebugTools;

    public Transform Songs;

    public GameObject MusicCard;

    public GameObject Blank;

    public GameObject ImageObject;

    public GameObject Audiobject;
    // Start is called before the first frame update

    public void SetMark(string Mark, string TP, string Level)
    {
        MarkText.GetComponent<TMPro.TextMeshProUGUI>().text = Mark;
        TpText.GetComponent<TMPro.TextMeshProUGUI>().text = TP;
        LevelText.GetComponent<TMPro.TextMeshProUGUI>().text = Level;
    }

    private IEnumerator AddMusic(string name,string path)
    {
        
        var bk = new WWW("file://" + path + "/" + name + ".png");
        yield return bk;
        Texture2D tx2d = new Texture2D(1920, 1080);
        tx2d.LoadImage(bk.bytes);
        
        bk.Dispose();

        var svb = File.ReadAllBytes(path + "/" + name + "_title.wav");
        var au = new WWW("file://" + path + "/" + name + "_title.wav");
        yield return au;
        
        var newCard = Instantiate(MusicCard, Songs);
        newCard.transform.SetAsLastSibling();

        var Controller = newCard.GetComponent<CardController>();
        Controller.transform.Rotate(Vector3.forward,38);
        Controller.AnimatorObject = gameObject;
        Controller.AudioObject = Audiobject;
        Controller.ImageObject = ImageObject;
        Controller.BackGround = tx2d;
        Controller.SongConfig = new TextAsset(File.ReadAllText(path + "/" + name + ".xml"));
        Controller.SongData = new TextAsset(File.ReadAllText(path + "/" + name + "_data" + ".xml"));
        
        Controller.SetText(Controller.Name);


        Controller.TitleAudio = au.GetAudioClip();
        Controller.isExternal = true;
        Controller.ExternalPath = "file://" + path + "/" + name + ".wav";
        
        au.Dispose();
        yield break;
    }

    private IEnumerator LoadMusics()
    {
        
        foreach (var i in System.IO.Directory.EnumerateDirectories(StartInit.CombinPath("Songs")))
        {
            FileInfo info = new FileInfo(i);
            yield return StartCoroutine( AddMusic(info.Name, i));
        }
        
        for (int i = 0; i < 4; i++)
        {
            var tr = Instantiate(Blank, Songs).transform;
            tr.SetAsLastSibling();
            tr.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        
        yield break;
    }
    void Start()
    {
        SetMark(
            PlayerPrefs.GetString("三色绘恋S" + "_Mark", "0000000"),
            PlayerPrefs.GetString("三色绘恋S" + "_TP", "00.00%"), "Lv.3");

        StartCoroutine(LoadMusics());
    }
    private void Awake()
    {
        if (!isDebugMode)
        {
            DebugTools.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
