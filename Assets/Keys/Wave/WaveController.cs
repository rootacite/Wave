using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


sealed public class WaveController : Keys
{
    public float RealRod
    {
        get
        {
            return 8f * Scale;
        }
    }
    float LastChildTime;

    public GameObject DragLineOrigin;

    public GameObject ScaleObj;
    public GameObject Border;
    public Sprite Border_sp;
    private float Scale;

    private Vector2 SavePosition;//
    private Animator TAnimation;
    private List<CirculKey> Childrens;
    static public WaveController Creat(RootConfig rootConfig, Vector3 Position, GameObject Origin, GameObject TransfronParent, float SecondPerBeat, List<CirculKey> Childrens, float LastTime, float BeatOffset = 1f, float Scale = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        var Controller = r.GetComponent<WaveController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.Z = Position.z;
        Controller.Childrens = Childrens;
        Controller.rootConfig = rootConfig;
        Controller.SavePosition = Position;
        Controller.ScaleObj.transform.localScale = new Vector3(Scale, Scale, 1);
        Controller.Scale = Scale;
        Controller.LastChildTime = LastTime;

        return Controller;
    }
    
    public float Length; //Should be Setted when Init

    override protected IEnumerator DelayDestroy(float Time)
    {
        yield return new WaitForSeconds(Time - LastChildTime * BeatPerSecond);
        GetComponent<Animator>().SetTrigger("RangeHide");

        yield return new WaitForSeconds(LastChildTime * BeatPerSecond);

        yield return new WaitForSeconds(0.33f * BeatPerSecond);
        Destroy(this.gameObject);
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
                    StartCoroutine(DelayDestroy((Length + LastChildTime) * BeatPerSecond));
                    Invailded = true;
                }
                else if (Status == 1)
                {
                    OnGreat();
                    TAnimation.SetTrigger("Great");
                    StartCoroutine(DelayDestroy((Length + LastChildTime) * BeatPerSecond));
                    Invailded = true;
                }
                else
                {
                    OnBad();
                    TAnimation.SetTrigger("Bad");
                    StartCoroutine(DelayDestroy((Length + LastChildTime) * BeatPerSecond));
                    Invailded = true;
                }
                return true;
            }

            return false;
        }
        return false;
    }

    protected override void Start()
    {
        base.Start();
        TAnimation = GetComponent<Animator>();
        if (rootConfig != null)
            OnInvailded += (s) =>
            {
                if (s == 3) return;
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
                            BK = rootConfig.CreateWave(SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), i.Childrens, (float)i.Length, i.LastChildTime, (float)i.WaveOffset, (float)i.WaveScale);
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
                                    KFrameList.Add(new Polar2(Polar2.d2r(z.dθ),RealRod * ((i.WaveOffset + z.ρ) / Length)));
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
                                            dcr.SetNodeMode(-(float)p.θ, (float)(i.Length / (double)i.IDragData.Count));

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
                
                TAnimation.speed = 1 / (Length * BeatPerSecond);
                if (s == 3) return;
                TAnimation.SetTrigger("JudgeStart");
            };
        else
        {
            
        }
    }

    protected override void Update()
    {
        base.Update();
        if (Invailded)
        {
           TAnimation.speed = 1 / (Length * BeatPerSecond);
        }
    }

    override public void SetWaveEffect()
    {
        base.SetWaveEffect();
        Border.GetComponent<SpriteRenderer>().sprite = Border_sp;
    }

    public void EndEvent()
    {
        if (Invailded) return;
        TAnimation.speed = 1 / (BeatPerSecond * rootConfig.HeadPending); //如果已经进入判定区域，则应该把速度重置为节拍速度
        if (AutoMode)
        {
            OnPrefect();
            TAnimation.SetTrigger("Prefect");
            StartCoroutine(DelayDestroy((Length + LastChildTime) * BeatPerSecond));
            Invailded = true;
        }
    }
}
