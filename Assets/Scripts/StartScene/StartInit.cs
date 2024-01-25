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
        "Wave���ڱ����ǰ��Ԥ����ʾ���ɼ�λ��λ�ã���ͬ���͵ļ����в�ͬ��",
        "���뱣֤�����뽫Ҫ�ж���Drag���غϣ��������С�",
        "�����;�ɿ�Hold�������ᰴ��Miss�ж���",
        "�����;�ſ�HWave��������İ�����ȫ��Miss��",
        "��ʹ��������������ж����ΪBad���Ի��ж�Combo��",
        "Slide����Ҫ�ڴ�����ͬʱ�����ⷽ���ƶ�С�ξ���������С�",
        "���ε�Slide����������ı����ⷭת������Բ�Σ�",
        "Wave�����ж����ۻ�Ӱ����������𣿴���:�ǵġ�",
        "�����ж�������ʱ����ǰ��ս��ļ��㡣�������ֵ�BPMԽ�ߣ��ж�Ҳ��Խ�ϸ�",
        "�κε������ܵ��������У��������е㲻һ�����������ʽ��������ͬ�ġ�",
        "Drag����������ֻ����ֱ���ε�һ�����֣���������ȴ���γɷ������۵��Ǵ���",
        "�����ڵİ����ǰ��ռ�����ֲ��ġ�"
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
                GameScripting.Instance.Initialize(Song, SongConfig, SongData, BackGround, null);
                GameScripting.Instance.Metronome.OnBeat += (t) =>
                {
                    if (t == 2)
                    {
                        StartInit.ShowText("Tap��\n������࣬���������δ�������ʱ�����", 6);
                        return;
                    }

                    if (t == 18)
                    {
                        StartInit.ShowText("Hold��\n���������δ�������ʱ�����Ļ���Ȧ��ȫ��չʱ���������֡�����Ȧ��ȫչ�������߱�Եʱ�����ɿ���", 6);
                        return;
                    }

                    if (t == 35)
                    {
                        StartInit.ShowText("Slide��\n���������δ�������ʱ������������ⷽ�򻬶���", 6);
                        return;
                    }

                    if (t == 47f)
                    {
                        StartInit.ShowText("Wave�����������δ�������ʱ�������Tap����ͬ����\nWave�������ʱ��������һ�����ε��ж�����\"����\"\n�����еļ���Ҫ�ڲ��������غ�ʱ�����", 12);
                        return;
                    }

                    if (t == 64f)
                    {
                        StartInit.ShowText("Drag��\n��ɫ������\n���ж��߻��������Ļ���Ȧ��ȫ����ʱ�����򻮹���\n", 6);
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
