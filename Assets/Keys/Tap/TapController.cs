using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

sealed public class TapController : Keys
{
    public GameObject Border;
    public GameObject Center;

    public Sprite Border_sp;
    public Sprite Center_sp;

    private Animator TAnimation;

    static public TapController Creat(RootConfig rootConfig,Vector3 Position, GameObject Origin, GameObject TransfronParent, float SecondPerBeat, float BeatOffset = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        var Controller = r.GetComponent<TapController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.Z = Position.z;
        Controller.rootConfig = rootConfig;

        return Controller;
    }
    
    override protected bool TouchEvent(TouchPhase t, Vector2 p)
    {
        if (t == TouchPhase.Began)
        {
            var RayHit = Physics2D.Raycast(p, Vector2.zero);
            if (RayHit.collider == gameObject.GetComponent<Collider2D>())
            {
                if (Invailded) return false;

                if (Status == 0)
                {
                    OnPrefect();
                    TAnimation.SetTrigger("Prefect");
                    Invailded = true;
                    StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                }
                else if (Status == 1)
                {
                    OnGreat();
                    TAnimation.SetTrigger("Great");
                    Invailded = true;
                    StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                }
                else
                {
                    OnBad();
                    TAnimation.SetTrigger("Bad");
                    Invailded = true;
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
    }


    public void EndEvent()
    {
        if (Invailded) return;
        TAnimation.speed = 1 / (BeatPerSecond * rootConfig.HeadPending); //如果已经进入判定区域，则应该把速度重置为节拍速度
        if (AutoMode)
        {
            OnPrefect();
            TAnimation.SetTrigger("Prefect");
            Invailded = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
        }
    }
}
