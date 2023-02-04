using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldController : Keys
{
    public GameObject Center;
    public GameObject Grade;
    public GameObject Exp;

    public Sprite Center_sp;
    public Sprite Grade_sp;

    private Animator TAnimation;
    public AnimationClip HoldedClip;

    bool DifficultMode = false;
    public event Action OnHold;

    static public HoldController Creat(GameScripting rootConfig,Vector3 Position, GameObject Origin, GameObject TransfronParent, float Length, float SecondPerBeat, float BeatOffset = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;

        var Controller = r.GetComponent<HoldController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.ExplandTime = SecondPerBeat * Length * 60f;
        Controller.Z = Position.z;
        Controller.rootConfig = rootConfig;

        return Controller;
    }
    float ExplandTime = 60; // Game Tick
    private bool _IsHold = false;
    public bool IsHold 
    {
        get 
        { 
            return _IsHold;
        }

        set
        {
            _IsHold = value;
            if(_IsHold)
            {
                OnHold?.Invoke();
            }
        }
    }
    public void EndEvent()
    {
        if (DifficultMode) return;
        if (Invailded) return;

        TAnimation.speed = 1 / (BeatPerSecond * rootConfig.HeadPending); //����Ѿ������ж�������Ӧ�ð��ٶ�����Ϊ�����ٶ�
        OnPrefect();
        TAnimation.SetTrigger("Perfect");
        Invailded = true;
        StartCoroutine(DelayDestroy(1f / TAnimation.speed));
    }
    public void Exp_EndEvent()
    {
        TAnimation.speed = 1 / (BeatPerSecond * rootConfig.HeadPending);

        if (AutoMode)
        {
            if (Invailded || IsHold) return;
            IsHold = true;
        }
    }
    override public void MissEvent()
    {
        if (Invailded) return;

        TAnimation.SetTrigger("Miss");
      //  if (!IsHold)
           
     //   else
     //       StartCoroutine(DelayDestroy(0.33f / TAnimation.speed));
        OnMiss();

        Invailded = true;
        StartCoroutine(DelayDestroy(1f / TAnimation.speed));
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        TAnimation = GetComponent<Animator>();
        OnHold += () =>
        {
            TAnimation.SetTrigger("Holded");
            //TAnimation.SetTrigger("OnHoldEffect");

            var anmiley = gameObject.AddComponent<Animation>(); //ʹ��animation��ԭ���ǣ�animator��������ó�����������޸�ĳ���������ٶ�
 
            anmiley.playAutomatically = false;
            anmiley.AddClip(HoldedClip, "1");
            foreach(AnimationState i in anmiley)
            {
                i.speed = 0.75f / (Offset * BeatPerSecond);
            }
            anmiley.Play("1");

            TAnimation.speed = 60f / ExplandTime;
        };
    }
    override protected bool TouchEvent(TouchPhase t, Vector2 p)
    {
        var RayHit = Physics2D.Raycast(p, Vector2.zero);
        if (RayHit.collider != gameObject.GetComponent<Collider2D>()) 
        {
           if(IsHold && !AutoMode)
            {
                if (Input.touchCount > 0)
                {
                    bool HoldingFlag = false;
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        Vector2 touch_pos = Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));

                        if (Physics2D.Raycast(touch_pos, Vector2.zero).collider == gameObject.GetComponent<Collider2D>())
                        {
                            HoldingFlag = true;
                            break;
                        }
                    }
                    if (!HoldingFlag)
                    {
                        if (Invailded) return false;
                        if (Status == 0)
                        {
                            OnPrefect();
                            TAnimation.SetTrigger("Perfect");

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
                    }
                }
            }
            return false;
        }
        if (t == TouchPhase.Began)
        {
            if (Invailded || IsHold) return false;
            IsHold = true;
            return true;
        }
        if(t== TouchPhase.Ended)
        {
            if (Invailded || !IsHold) return false;

            if (Status == 0)
            {
                OnPrefect();
                TAnimation.SetTrigger("Perfect");
               
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
            //TAnimation.speed = 1 / BeatPerSecond;
            return true;
        }
        return false;
    }
    override public void PrefectEvent()
    {
        //Prefect����endʱ������������Բ��������������ٶ�
        Status = 0;

        if(AutoMode)
        {
            OnPrefect();
            TAnimation.SetTrigger("Perfect");
            Invailded = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
        }
    }


    override public void SetWaveEffect()
    {
        base.SetWaveEffect();
        Center.GetComponent<SpriteRenderer>().sprite = Center_sp;
        Grade.GetComponent<SpriteRenderer>().sprite = Grade_sp;
    }
}
