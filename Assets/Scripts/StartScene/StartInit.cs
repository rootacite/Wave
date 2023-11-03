using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Dictionary = System.IO.Directory;
public class StartInit : MonoBehaviour
{
    static string[] TipTexts = new string[] 
    {
        "Wave键在被点击前会预先提示生成键位的位置，不同类型的键略有不同。",
        "必须保证触点与将要判定的Drag键重合，才能命中。",
        "如果中途松开Hold键，将会按照Miss判定。",
        "如果中途放开HWave键，其余的按键会全部Miss。",
        "即使命中音符，如果判定结果为Bad，仍会中断Combo。",
        "Slide键需要在触摸的同时向任意方向移动小段距离才能命中。",
        "星形的Slide键，如果将四边向外翻转，仍是圆形！",
        "Wave键的判定评价会影响后续按键吗？答案是:是的。",
        "命中判定不按照时间而是按照节拍计算。所以音乐的BPM越高，判定也就越严格！",
        "任何的音符跑到了星盘中，都会变得有点不一样。但打击方式大体是相同的。",
        "Drag键在星盘外只会以直线形的一串出现，但在其中却会形成反复曲折的星带。",
        "星盘内的按键是按照极坐标分布的。"
    };

    public AudioClip Song;
 //   public TextAsset SongConfig;
    public Texture BackGround;
    public TextAsset SongData;
    public TextAsset SongConfig;

    public GameObject TextTip;

    public Animator animator;
    public TMPro.TextMeshProUGUI TextFloat;

    static bool IsVideo = true;
    static public void SetImage(Texture Image)
    {
        IsVideo = false;
        var image = Canvas.transform.Find("ImageBK");
        image.GetComponent<UnityEngine.UI.RawImage>().texture = Image;
    }

    static public void SetVideo(VideoClip Video)
    {
        IsVideo = true;
        var image = Canvas.transform.Find("ImageBK");
        image.GetComponent<VideoPlayer>().clip = Video;
        image.GetComponent<UnityEngine.UI.RawImage>().texture = image.GetComponent<VideoPlayer>().targetTexture;
    }
    static public void Hide()
    {
        Canvas.GetComponent<Animator>().SetBool("Status", true);
        Canvas.transform.Find("ImageBK").GetComponent<UnityEngine.UI.RawImage>().raycastTarget = false;
        IEnumerator DelayPause()
        {
            yield return new WaitForSeconds(1.5f);
            if (IsVideo)
                Canvas.transform.Find("ImageBK").GetComponent<VideoPlayer>().Stop();
        }
        instance.StartCoroutine(DelayPause());
    }
    static public void Show()
    {
        if (IsVideo)
            Canvas.transform.Find("ImageBK").GetComponent<VideoPlayer>().Play();
        Canvas.transform.Find("ImageBK").GetComponent<UnityEngine.UI.RawImage>().raycastTarget = true;
        Canvas.GetComponent<Animator>().SetBool("Status", false);
        Canvas.GetComponent<Animator>().SetTrigger("MoveIn");
        instance.TextTip.GetComponent<TMPro.TextMeshProUGUI>().text = TipTexts[new System.Random().Next(TipTexts.Length)];
    }

    static public void SetTipText(string s)
    {
        instance.TextTip.GetComponent<TMPro.TextMeshProUGUI>().text = s;
    }
    public static GameObject Canvas = null;
    public static StartInit instance;

    public static void ShowText(string text,float time = 3f)
    {
        instance.TextFloat.text = text;
        instance.animator.SetBool("ShowFT", true);

        IEnumerator DelayHide(float Time)
        {
            yield return new WaitForSeconds(Time);
            instance.animator.SetBool("ShowFT", false);
        }

        instance.StartCoroutine(DelayHide(time));
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Canvas == null)
        {
            transform.Find("ImageBK").GetComponent<VideoPlayer>().Play();
            Canvas = this.gameObject;
            instance = this;
            DontDestroyOnLoad(Canvas);
            Canvas.GetComponent<Animator>().SetTrigger("MoveIn");
            TextTip.GetComponent<TMPro.TextMeshProUGUI>().text = TipTexts[new System.Random().Next(TipTexts.Length)]; ;
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                Screen.SetResolution(2400, 1080, false);
        }
        else
        {
            Destroy(this.gameObject);
        }
        IEnumerator Proc_1()
        {
            yield return new WaitForSeconds(2);
            var Async = SceneManager.LoadSceneAsync(1);
            Async.completed += (v) =>
            {
                Hide();
            };
        }

        if (PlayerPrefs.GetInt("First", 0) == 0)
        {
            var Task = SceneManager.LoadSceneAsync(2);
            Task.completed += (e) =>
            {
                GameScripting.instance.Initialize(Song, SongConfig, SongData, BackGround, null);
                GameScripting.instance.OnBeat += (t) =>
                {
                    if (t == 2)
                    {
                        StartInit.ShowText("Tap键\n跟随节奏，当下落的雨滴打在上面时点击。", 6);
                        return;
                    }

                    if (t == 18)
                    {
                        StartInit.ShowText("Hold键\n当下落的雨滴打在上面时其中心或内圈完全舒展时触摸并保持。待外圈完全展开至虚线边缘时方可松开。", 6);
                        return;
                    }

                    if (t == 35)
                    {
                        StartInit.ShowText("Slide键\n当下落的雨滴打在上面时，点击并向任意方向滑动。", 6);
                        return;
                    }

                    if (t == 47f)
                    {
                        StartInit.ShowText("Wave，当下落的雨滴打在上面时点击，与Tap键不同的是\nWave键被点击时，会生成一个环形的判定区域\"星盘\"\n星盘中的键需要在波纹与其重合时点击。", 12);
                        return;
                    }

                    if (t == 64f)
                    {
                        StartInit.ShowText("Drag键\n紫色按键。\n当判定线划过其中心或外圈完全收缩时触摸或划过。\n", 6);
                        return;
                    }
                };
                StartInit.Hide();

            };
            PlayerPrefs.SetInt("First", 1);
            PlayerPrefs.Save();
        }
        else
        {
            StartCoroutine(Proc_1());
          //  ShowText(Application.persistentDataPath);
        }

        File.WriteAllText(
            Application.persistentDataPath + "/Flag.txt",
            "Hello Wave\n" + System.DateTime.Now
        );

        if (!Dictionary.Exists(CombinPath("Songs"))) Dictionary.CreateDirectory(CombinPath("Songs"));
    }

    public static string CombinPath(string p)
    {
        return Application.persistentDataPath + "/" + p;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Application.targetFrameRate = 360;
        if(PlayerPrefs.GetInt("vSync_Setting", 1) == 1)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        if (PlayerPrefs.GetInt("Low_Setting", 0) == 1) QualitySettings.SetQualityLevel(0);
        else QualitySettings.SetQualityLevel(5);
    }
}
