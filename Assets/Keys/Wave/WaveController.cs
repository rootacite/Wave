using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public GameObject DragLineOrigin;

    public GameObject ScaleObj;
    public GameObject Border;
    public SpriteRenderer OldRange;
    public CircularLine NewRange;
    public Sprite Border_sp;
    private float Scale;

    private Vector2 SavePosition;//
    private Animator TAnimation;
    private List<CircularKey> Childrens;

    private Creator _creator;
    private double _rotate;
    static public WaveController Creat(Creator creator, Vector3 Position, GameObject Origin, GameObject TransfromParent, float SecondPerBeat, List<CircularKey> Childrens, float LastTime, float BeatOffset = 1f, float Scale = 1f, double rotate = 0d)
    {
        var r = Instantiate(Origin, TransfromParent.transform);
        r.transform.localPosition = Position;
        r.transform.position = new Vector3(r.transform.position.x, r.transform.position.y, Position.z);
        var Controller = r.GetComponent<WaveController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.Z = r.transform.position.z;
        Controller.Childrens = Childrens;
        Controller.SavePosition = new Vector2(0, 0);
        Controller.ScaleObj.transform.localScale = new Vector3(Scale, Scale, 1);
        Controller.Scale = Scale;
        Controller._creator = creator;
        Controller._rotate = rotate;

        return Controller;
    }
    
    public float Length; //Should be Setted when Init

    override protected IEnumerator DelayDestroy(double time)
    {
        yield return new WaitForSeconds((float)time);
        if(LastKey.Type == KeyType.Hold || LastKey.Type == KeyType.Drag)
            yield return new WaitForSeconds((float)(LastKey.Length * BeatPerSecond));
        GetComponent<Animator>().SetTrigger("RangeHide");

        yield return new WaitForSeconds((float)((TotalLength - time) * BeatPerSecond));

        yield return new WaitForSeconds(0.33f * BeatPerSecond);
        StopWatchManager.RemoveEntity(_stw);
        Destroy(this.gameObject);
    }
    event Action EndForFirstDrag;
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
                    StartCoroutine(DelayDestroy(StLength * BeatPerSecond));
                    Invalided = true;
                }
                else if (Status == 1)
                {
                    OnGreat();
                    TAnimation.SetTrigger("Great");
                    StartCoroutine(DelayDestroy(StLength * BeatPerSecond));
                    Invalided = true;
                }
                else
                {
                    OnBad();
                    TAnimation.SetTrigger("Bad");
                    StartCoroutine(DelayDestroy(StLength * BeatPerSecond));
                    Invalided = true;
                }
                if (Childrens[0].Type == KeyType.Drag) EndForFirstDrag?.Invoke();
                return true;
            }

            return false;
        }
        return false;
    }

    readonly Stopwatch _stw = new Stopwatch();
    IEnumerator Rotating()
    {
        StopWatchManager.AddEntity(_stw);
        _stw.Reset();
        _stw.Start();
        
        while (true)
        {
            yield return null;
            Vector3 currentEulerAngles = transform.eulerAngles;
            // 增加1度到Y轴的旋转角度
            currentEulerAngles.z = (float)(_stw.ElapsedMilliseconds / 1000d * _rotate);
            // 将修改后的欧拉角应用到物体的Transform组件
            transform.eulerAngles = currentEulerAngles;
        }
    }

    protected override void Start()
    {
        base.Start();
        NewRange.radius = RealRod;
        TAnimation = GetComponent<Animator>();
        OnInvalided += (s) =>
        {
            if (s == 3) return;

            foreach (var i in Childrens)
            {
                Keys BK = null;
                switch (i.Type)
                {
                    case KeyType.Tap:
                        BK = _creator.CreateTap(
                            SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)),
                            (float)i.WaveOffset, gameObject);
                        break;
                    case KeyType.Hold:
                        BK = _creator.CreateHold(
                            SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), (float)i.Length,
                            (float)i.WaveOffset, gameObject);
                        break;
                    case KeyType.Slide:
                        BK = _creator.CreateSlide(
                            SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)),
                            (float)i.WaveOffset, gameObject);
                        break;
                    case KeyType.Wave:
                        BK = _creator.CreateWave(
                            SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), i.Children,
                            (float)i.Length, i.TimeOfLastChildren, i.Rotate, (float)i.WaveOffset, (float)i.WaveScale, gameObject);
                        break;
                    case KeyType.HWave:
                        BK = _creator.CreateHWave(
                            SavePosition.Offset(i.Angle, RealRod * ((float)i.WaveOffset / Length)), i.Children,
                            (float)i.Length, i.TimeOfLastChildren, (float)i.WaveOffset, (float)i.WaveScale, gameObject);
                        break;
                    case KeyType.Drag:

                        List<Vector3> Points = new List<Vector3>();
                        DragLine? DL = null;
                        if (i.DragData.DragRoute.Count == 0)
                        {
                            double initR = RealRod * (i.WaveOffset / Length);
                            double endR = RealRod * ((i.Length + i.WaveOffset) / Length);
                            int Crond = (int)((endR - initR) * 8d);
                            for (int p = 0; p <= Crond; p++)
                            {
                                double CurrentAngle =
                                    i.DragData.From + (i.DragData.To - i.DragData.From) / Crond * p;
                                double CurrentR = initR + (endR - initR) / Crond * p;

                                Vector3 pv = SavePosition.Offset((float)CurrentAngle, (float)CurrentR);
                                pv.z = 5f;
                                Points.Add(pv);
                            }

                            DL = DragLine.Create(DragLineOrigin, gameObject, Points.ToArray());
                            //Debug.Log(Points.ToArray());

                            for (double p = 0; p <= i.DragData.Count; p++)
                            {
                                double CurrentAngle = i.DragData.From +
                                                      (i.DragData.To - i.DragData.From) / i.DragData.Count * p;
                                double CurrentR = initR + (endR - initR) / i.DragData.Count * p;
                                double CurrentOffset = i.WaveOffset + i.Length / i.DragData.Count * p;
                                var dcr = _creator.CreateDragAngle(SavePosition, (float)CurrentAngle, (float)CurrentR,
                                    (float)CurrentOffset, gameObject);

                                if (p == 0 || p == i.DragData.Count)
                                {
                                    dcr.SetWaveEffect();
                                    if (p == 0)
                                    {
                                        EndForFirstDrag += () => { dcr.EndEvent(); };
                                        dcr.OnInvalided += (s) => { DL?.StartShrink(i.Length * BeatPerSecond); };
                                    }
                                }
                                else
                                    dcr.SetNodeMode(-(float)CurrentAngle,
                                        (float)(i.Length / (double)i.DragData.Count));

                                double Rate = 1d - p / i.DragData.Count;
                            }
                        }
                        else
                        {
                            var StartPos = SavePosition.Offset((float)i.DragData.From,
                                (float)(RealRod * (i.WaveOffset / Length)));
                            List<Polar2> KFrameList = new List<Polar2>();

                            foreach (var z in i.DragData.DragRoute)
                            {
                                KFrameList.Add(new Polar2(Polar2.d2r(z.dsita),
                                    RealRod * ((i.WaveOffset + z.rou) / Length)));
                            }

                            double PointLimit = (double)(35 * (KFrameList.Count - 1)) / i.DragData.Count;
                            double flag_limit = 0;
                            DragController save_dcr = null;
                            PolarSystem.EnumPolarRoute((p, ii) =>
                            {
                                double CurrentOffset =
                                    i.WaveOffset + i.Length / (35 * (KFrameList.Count - 1)) * (ii + 1);

                                Vector3 ccp = p.ToVector();
                                ccp = (Vector2)ccp + StartPos;
                                ccp.z = 5f;
                                Points.Add(ccp);

                                if (ii > flag_limit)
                                {
                                    var dcr = _creator.CreateDrag_Single(ccp, (float)CurrentOffset, gameObject);
                                    if (save_dcr == null) save_dcr = dcr;
                                    if (ii == 0 || ii == 35 * (KFrameList.Count - 1))
                                    {
                                        dcr.SetWaveEffect();
                                    }
                                    else
                                        dcr.SetNodeMode(-(float)p.rou, (float)(i.Length / (double)i.DragData.Count));

                                    double Rate = 1d - ii / (double)(35 * (KFrameList.Count - 1));
                                    flag_limit += PointLimit;
                                }

                            }, new Polar2(0, 0), KFrameList.ToArray(), 35);
                            DL = DragLine.Create(DragLineOrigin, gameObject, Points.ToArray());
                            save_dcr.OnInvalided += (s) => { DL?.StartShrink(i.Length * BeatPerSecond); };

                        }

                        break;
                }

                if (BK != null)
                {
                    BK.SetWaveEffect();
                    BK.isInWave = true;

                    BK.gameObject.transform.localScale = new Vector3((float)i.Scale, (float)i.Scale, 1);
                }
            }

            TAnimation.speed = 1 / (Length * BeatPerSecond);
            if (s == 3) return;
            TAnimation.SetTrigger("JudgeStart");
        };

        //OldRange.enabled = false;

        if (_rotate != 0)
        {
            StartCoroutine(Rotating());
        }
    }

    protected override void Update()
    {
        base.Update();
        //NewRange.transparency = OldRange.color.a;
        if (Invalided)
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
        if (Invalided) return;
        TAnimation.speed = 1 / (BeatPerSecond * HeadPending); //???????????????????????????????????????
        if (AutoMode)
        {
            OnPrefect();
            TAnimation.SetTrigger("Prefect");
            StartCoroutine(DelayDestroy(StLength * BeatPerSecond));
            Invalided = true;
        }
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
        
        if (lk.Type == KeyType.Wave || lk.Type == KeyType.HWave)
        {
            return lk.WaveOffset + GetChildrenLength(lk.Children) + 1;
        }
        else if (lk.Type == KeyType.Drag || lk.Type == KeyType.Hold)
        {
            return lk.WaveOffset + lk.Length + 1;
        }
        else
        {
            return lk.WaveOffset + 1;
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
