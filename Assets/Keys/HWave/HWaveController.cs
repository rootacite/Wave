using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HWaveController : Keys
{
    public GameObject ScaleObj;

    public GameObject DragLineOrigin;
    public GameObject Center;
    public GameObject Grade;
    public GameObject Exp;

    private Vector2 SavePosition;
    private Animator TAnimation;
    public AnimationClip HoldedClip;

    event Action EndForFirstDrag;
    bool DifficultMode = false;
    public event Action OnHold;
    private float Scale;
    private List<CircularKey> Childrens;
    private List<Keys> Saved_Keys = new();

    private Creator _creator;

    void MissExistsKeys()
    {
        foreach (var i in Saved_Keys)
        {
            if (!i.Invalided)
            {
                i.MissEvent();
            }
        }
    }
    static public HWaveController Creat(Creator creator, Vector3 Position, GameObject Origin, GameObject TransfronParent, float Length, float SecondPerBeat, List<CircularKey> Childrens, float LastTime, float BeatOffset = 1f, float Scale = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        r.transform.position = new Vector3(r.transform.position.x, r.transform.position.y, Position.z);
        var Controller = r.GetComponent<HWaveController>();

        Controller.SavePosition = new Vector2(0, 0);;
        Controller.ScaleObj.transform.localScale = new Vector3(Scale, Scale, 1);
        Controller.Scale = Scale;
        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.ExplandTime = SecondPerBeat * Length * 60f;
        Controller.Z = r.transform.position.z;
        Controller.Childrens = Childrens;
        Controller._creator = creator;

        return Controller;
    }
    public float RealRod
    {
        get
        {
            return 8f * Scale;
        }
    }
    public float Length; 
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
        if (Invalided) return;

        TAnimation.speed = 1 / (BeatPerSecond * HeadPending); //如果已经进入判定区域，则应该把速度重置为节拍速度
        OnPrefect();
        //TAnimation.SetTrigger("Perfect");
        Invalided = true;
        StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
    }
    public void Exp_EndEvent()
    {
        TAnimation.speed = 1 / (BeatPerSecond * HeadPending);

        if (AutoMode)
        {
            if (Invalided || IsHold) return;
            IsHold = true;
        }
    }
    override public void MissEvent()
    {
        if (Invalided) return;

        TAnimation.SetTrigger("Miss");
      //  if (!IsHold)
           
     //   else
     //       StartCoroutine(DelayDestroy(0.33f / TAnimation.speed));
        OnMiss();

        Invalided = true;
        StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
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

            var anmiley = gameObject.AddComponent<Animation>(); //使用animation的原因是，animator组件很难用常规操作单独修改某个动画的速度
 
            anmiley.playAutomatically = false;
            anmiley.AddClip(HoldedClip, "1");
            foreach(AnimationState i in anmiley)
            {
                i.speed = 0.75f / (Offset * BeatPerSecond);
            }
            anmiley.Play("1");

            foreach (var i in Childrens)
            {
                Keys BK = null;
                switch (i.Type)
                {
                    case KeyType.Tap:
                        BK = _creator.CreateTap(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), (float)i.WaveOffset, gameObject);
                        break;
                    case KeyType.Hold:
                        BK = _creator.CreateHold(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), (float)i.Length, (float)i.WaveOffset, gameObject);
                        break;
                    case KeyType.Slide:
                        BK = _creator.CreateSlide(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), (float)i.WaveOffset, gameObject);
                        break;
                    case KeyType.Wave:
                        BK = _creator.CreateWave(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), i.Children, (float)i.Length, i.TimeOfLastChildren, i.Rotate, (float)i.WaveOffset, (float)i.WaveScale, gameObject);
                        break;
                    case KeyType.HWave:
                        BK = _creator.CreateHWave(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), i.Children, (float)i.Length, i.TimeOfLastChildren, (float)i.WaveOffset, (float)i.WaveScale, gameObject);
                        break;
                }
                if (BK != null)
                {
                    Saved_Keys.Add(BK);
                    BK.SetWaveEffect();
                    BK.isInWave = true;
                }
            }
            if (Childrens[0].Type == KeyType.Drag) EndForFirstDrag?.Invoke();
            TAnimation.speed = 60f / ExplandTime;
        };
        Bad += () =>
        {
            MissExistsKeys();
        };

        Great += () =>
        {
            MissExistsKeys();
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
                        if (Invalided) return false;
                        if (Status == 0)
                        {
                            OnPrefect();
                           // TAnimation.SetTrigger("Perfect");

                            Invalided = true;
                            StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
                        }
                        else if (Status == 1)
                        {
                            OnGreat();
                            TAnimation.SetTrigger("Miss");
                            Invalided = true;
                            StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
                        }
                        else
                        {
                            OnBad();
                            TAnimation.SetTrigger("Miss");
                            Invalided = true;
                            StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
                        }
                    }
                }
            }
            return false;
        }
        if (t == TouchPhase.Began)
        {
            if (Invalided || IsHold) return false;
            IsHold = true;
            return true;
        }
        if(t== TouchPhase.Ended)
        {
            if (Invalided || !IsHold) return false;

            if (Status == 0)
            {
                OnPrefect();
                //TAnimation.SetTrigger("Perfect");
               
                Invalided = true;
                StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
            }
            else if (Status == 1)
            {
                OnGreat();
                //TAnimation.SetTrigger("Great");
                Invalided = true;
                StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
            }
            else
            {
                OnBad();
                //TAnimation.SetTrigger("Bad");
                Invalided = true;
                StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
            }
            //TAnimation.speed = 1 / BeatPerSecond;
            return true;
        }
        return false;
    }
    override public void PrefectEvent()
    {
        //Prefect键的end时机不在这里，所以不能在这里重置速度
        Status = 0;

        if(AutoMode)
        {
            OnPrefect();
            //TAnimation.SetTrigger("Perfect");
            Invalided = true;
            StartCoroutine(DelayDestroy(TotalLength * BeatPerSecond));
        }
    }


    override public void SetWaveEffect()
    {
        base.SetWaveEffect();
    }
    
    private double GetChildrenLength(List<CircularKey> chs)
    {
        CircularKey lk = null; 
        double r = 0;
        foreach (var k in chs)
        {
            if (k.WaveOffset > r) r = k.WaveOffset;
        }

        foreach (var k in chs)
        {
            if (Math.Abs(k.WaveOffset - r) < 0.001d) lk = k;
        }

        if (lk.Type != KeyType.Wave && lk.Type != KeyType.HWave)
        {
            return lk.WaveOffset;
        }
        else if (lk.Type == KeyType.Drag || lk.Type == KeyType.Hold)
        {
            return lk.Length;
        }
        else
        {
            return lk.WaveOffset + GetChildrenLength(lk.Children);
        }
    }
    
    public override double TotalLength
    {
        get
        {
            return GetChildrenLength(Childrens);
        }
    }
    
    private double StLength => LastKey.WaveOffset;
    private CircularKey LastKey
    {
        get
        {
            double r = 0;
            foreach (var k in Childrens)
            {
                if (k.WaveOffset > r) r = k.WaveOffset;
            }

            foreach (var k in Childrens)
            {
                if (Math.Abs(k.WaveOffset - r) < 0.001d) return k;
            }

            return null;
        }
    }
}
