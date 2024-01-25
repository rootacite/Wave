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
                    StartInit.ShowText("Drag��\n��ɫ������\n���ж��߻��������Ļ���Ȧ��ȫ����ʱ�����򻮹���\n",6);
                    return;
                }
            };
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
