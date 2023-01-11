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
    private List<CirculKey> Childrens;
    static public HWaveController Creat(GameScripting rootConfig,Vector3 Position, GameObject Origin, GameObject TransfronParent, float Length, float SecondPerBeat, List<CirculKey> Childrens, float LastTime, float BeatOffset = 1f, float Scale = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;

        var Controller = r.GetComponent<HWaveController>();

        Controller.SavePosition = Position;
        Controller.ScaleObj.transform.localScale = new Vector3(Scale, Scale, 1);
        Controller.Scale = Scale;
        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.ExplandTime = SecondPerBeat * Length * 60f;
        Controller.Z = Position.z;
        Controller.rootConfig = rootConfig;
        Controller.Childrens = Childrens;

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
        if (Invailded) return;

        TAnimation.speed = 1 / (BeatPerSecond * rootConfig.HeadPending); //����Ѿ������ж�������Ӧ�ð��ٶ�����Ϊ�����ٶ�
        OnPrefect();
        //TAnimation.SetTrigger("Perfect");
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

            foreach (var i in Childrens)
            {
                Keys BK = null;
                switch (i.Type)
                {
                    case KeyType.Tap:
                        BK = rootConfig.CreateTap(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), (float)i.WaveOffset);
                        break;
                    case KeyType.Hold:
                        BK = rootConfig.CreateHold(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), (float)i.Length, (float)i.WaveOffset);
                        break;
                    case KeyType.Slide:
                        BK = rootConfig.CreateSlide(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), (float)i.WaveOffset);
                        break;
                    case KeyType.Wave:
                        BK = rootConfig.CreateWave(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), i.Childrens, (float)i.Length, i.TimeOfLastChildren, (float)i.WaveOffset, (float)i.WaveScale);
                        break;
                    case KeyType.HWave:
                        BK = rootConfig.CreateHWave(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), i.Childrens, (float)i.Length, i.TimeOfLastChildren, (float)i.WaveOffset, (float)i.WaveScale);
                        break;
                    case KeyType.Drag:

                        List<Vector3> Points = new List<Vector3>();
                        DragLine? DL = null;
                        if (i.IDragData.DragRoute.Count == 0)
                        {
                            double initR = RealRod * (i.WaveOffset / Length);
                            double endR = RealRod * ((i.Length + i.WaveOffset) / Length);
                            int Crond = (int)((endR - initR) * 80d);
                            for (int p = 0; p <= Crond; p++)
                            {
                                double CurrentAngle = i.IDragData.From + (i.IDragData.To - i.IDragData.From) / Crond * p;
                                double CurrentR = initR + (endR - initR) / Crond * p;

                                Vector3 pv = SavePosition.Offset((float)CurrentAngle, (float)CurrentR);
                                pv.z = 88f;
                                Points.Add(pv);
                            }

                            DL = DragLine.Create(DragLineOrigin, rootConfig.KeyLayer, Points.ToArray());

                            for (double p = 0; p <= i.IDragData.Count; p++)
                            {
                                double CurrentAngle = i.IDragData.From + (i.IDragData.To - i.IDragData.From) / i.IDragData.Count * p;
                                double CurrentR = initR + (endR - initR) / i.IDragData.Count * p;
                                double CurrentOffset = i.WaveOffset + i.Length / i.IDragData.Count * p;
                                var dcr = rootConfig.CreateDragAngle(SavePosition, (float)CurrentAngle, (float)CurrentR, (float)CurrentOffset);

                                if (p == 0 || p == i.IDragData.Count)
                                {
                                    dcr.SetWaveEffect();
                                    if (p == 0)
                                    {
                                        EndForFirstDrag += () =>
                                        {
                                            dcr.EndEvent();
                                        };
                                    }
                                }
                                else
                                    dcr.SetNodeMode(-(float)CurrentAngle, (float)(i.Length / (double)i.IDragData.Count));

                                double Rate = 1d - p / i.IDragData.Count;

                                dcr.OnInvailded += (s) =>
                                {
                                    DL?.Sub(Rate);
                                };

                            }
                        }
                        else
                        {
                            var StartPos = SavePosition.Offset((float)i.IDragData.From, (float)(RealRod * (i.WaveOffset / Length)));
                            List<Polar2> KFrameList = new List<Polar2>();

                            foreach (var z in i.IDragData.DragRoute)
                            {
                                KFrameList.Add(new Polar2(Polar2.d2r(z.d��), RealRod * ((i.WaveOffset + z.��) / Length)));
                            }
                            double PointLimit = (double)(35 * (KFrameList.Count - 1)) / i.IDragData.Count;
                            double flag_limit = 0;
                            PolarSystem.EnumPolarRoute((p, ii) =>
                            {
                                double CurrentOffset = i.WaveOffset + i.Length / (35 * (KFrameList.Count - 1)) * (ii + 1);

                                Vector3 ccp = p.ToVector();
                                ccp = (Vector2)ccp + StartPos;
                                ccp.z = 88f;
                                Points.Add(ccp);

                                if (ii > flag_limit)
                                {
                                    var dcr = rootConfig.CreateDrag_Single(ccp, (float)CurrentOffset);

                                    if (ii == 0 || ii == 35 * (KFrameList.Count - 1))
                                    {
                                        dcr.SetWaveEffect();
                                    }
                                    else
                                        dcr.SetNodeMode(-(float)p.��, (float)(i.Length / (double)i.IDragData.Count));

                                    double Rate = 1d - ii / (double)(35 * (KFrameList.Count - 1));

                                    dcr.OnInvailded += (s) =>
                                    {
                                        DL?.Sub(Rate);
                                    };

                                    flag_limit += PointLimit;
                                }

                            }, new Polar2(0, 0), KFrameList.ToArray(), 35);
                            DL = DragLine.Create(DragLineOrigin, rootConfig.KeyLayer, Points.ToArray());


                        }
                        break;
                }
                if (BK != null)
                {

                    BK.SetWaveEffect();
                    BK.IsInWave = true;
                }
            }
            if (Childrens[0].Type == KeyType.Drag) EndForFirstDrag?.Invoke();
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
                           // TAnimation.SetTrigger("Perfect");

                            Invailded = true;
                            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                        }
                        else if (Status == 1)
                        {
                            OnGreat();
                            TAnimation.SetTrigger("Miss");
                            Invailded = true;
                            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                        }
                        else
                        {
                            OnBad();
                            TAnimation.SetTrigger("Miss");
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
                //TAnimation.SetTrigger("Perfect");
               
                Invailded = true;
                StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            }
            else if (Status == 1)
            {
                OnGreat();
                //TAnimation.SetTrigger("Great");
                Invailded = true;
                StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            }
            else
            {
                OnBad();
                //TAnimation.SetTrigger("Bad");
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
            //TAnimation.SetTrigger("Perfect");
            Invailded = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
        }
    }


    override public void SetWaveEffect()
    {
        base.SetWaveEffect();
    }
}