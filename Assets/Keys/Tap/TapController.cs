using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = System.Object;

sealed public class TapController : Keys
{
    public GameObject Border;
    public GameObject Center;

    public Sprite Border_sp;
    public Sprite Center_sp;

    private Animator TAnimation;

    public GameObject Effect;

    static public TapController Creat(Vector3 Position, GameObject Origin, GameObject TransfronParent, float SecondPerBeat, float BeatOffset = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        var Controller = r.GetComponent<TapController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.Z = Position.z;
        return Controller;
    }
    
    override protected bool TouchEvent(TouchPhase t, Vector2 p)
    {
        if (t == TouchPhase.Began)
        {
            var RayHit = Physics2D.Raycast(p, Vector2.zero);
            if (RayHit.collider == gameObject.GetComponent<Collider2D>())
            {
                if (Invalided) return false;

                if (Status == 0)
                {
                    OnPrefect();
                    TAnimation.SetTrigger("Prefect");
                    Invalided = true;
                    StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                }
                else if (Status == 1)
                {
                    OnGreat();
                    TAnimation.SetTrigger("Great");
                    Invalided = true;
                    StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                }
                else
                {
                    OnBad();
                    TAnimation.SetTrigger("Bad");
                    Invalided = true;
                    StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                }
                return true;
            }

            return false;
        }
        return false;
    }

    override public void SetWaveEffect()
    {
        base.SetWaveEffect();
        Border.GetComponent<SpriteRenderer>().sprite = Border_sp;
        Center.GetComponent<SpriteRenderer>().sprite = Center_sp;
    }

    protected override void Start()
    {
        base.Start();
        TAnimation = GetComponent<Animator>();

        OnInvalided += (s) =>
        {
            TAnimation.SetBool("Invailed", true);
        };

        Prefect += () =>
        {
            Effect.SetActive(true);
        };
    }


    public void EndEvent()
    {
        if (Invalided) return;
        TAnimation.speed = 1 / (BeatPerSecond * HeadPending); //����Ѿ������ж�������Ӧ�ð��ٶ�����Ϊ�����ٶ�
        if (AutoMode)
        {
            OnPrefect();
            TAnimation.SetTrigger("Prefect");
            Invalided = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
        }
    }
}
