using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KeyRoute = System.Collections.Generic.List<System.Collections.Generic.List<CirculKey>>;
public partial class RootConfig
{
    public GameObject Drop;
    public HoldController CreateHold(Vector2 Pos, float Length, float Offset = 1, GameObject Parent = null)
    {
        float max_z = 0;
        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invailded == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _hold = HoldController.Creat(this, new Vector3(Pos.x, Pos.y, max_z + 0.01f), Hold, Parent == null ? KeyLayer : Parent, Length, SecondPerBeat, Offset);

        _hold.Prefect += () =>
        {
            CurrentMark += 500;
            ComboCount += 1;
        };
        _hold.Great += () =>
        {
            CurrentMark += 200;
            ComboCount += 1;
        };
        _hold.Bad += () =>
        {
            CurrentMark += 50;
            ComboCount = 0;
        };
        _hold.Miss += () =>
        {
            CurrentMark += 0;
            ComboCount = 0;
        };

        return _hold;
    }
    public TapController CreateTap(Vector2 Pos, float Offset = 1, GameObject Parent = null)
    {
        float max_z = 0;

        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invailded == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _tap = TapController.Creat(this, new Vector3(Pos.x, Pos.y, max_z + 0.01f), Tap, Parent == null ? KeyLayer : Parent, SecondPerBeat, Offset);

        _tap.Prefect += () =>
        {
            CurrentMark += 500;
            ComboCount += 1;
        };
        _tap.Great += () =>
        {
            CurrentMark += 200;
            ComboCount += 1;
        };
        _tap.Bad += () =>
        {
            CurrentMark += 50;
            ComboCount = 0;
        };
        _tap.Miss += () =>
        {
            CurrentMark += 0;
            ComboCount = 0;
        };

        return _tap;
    }
    public SlideController CreateSlide(Vector2 Pos, float Offset = 1, GameObject Parent = null)
    {
        float max_z = 0;

        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invailded == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _slide = SlideController.Creat(this, new Vector3(Pos.x, Pos.y, max_z + 0.01f), Slide, Parent == null ? KeyLayer : Parent, SecondPerBeat, Offset);

        _slide.Prefect += () =>
        {
            CurrentMark += 500;
            ComboCount += 1;
        };
        _slide.Great += () =>
        {
            CurrentMark += 200;
            ComboCount += 1;
        };
        _slide.Bad += () =>
        {
            CurrentMark += 50;
            ComboCount = 0;
        };
        _slide.Miss += () =>
        {
            CurrentMark += 0;
            ComboCount = 0;
        };

        return _slide;
    }
    public WaveController CreateWave(Vector2 Pos, List<CirculKey> Childrens, float Length, float LastTime, float Offset = 1, float Scale = 1, GameObject Parent = null)
    {
        float max_z = 0;

        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invailded == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _wave = WaveController.Creat(this, new Vector3(Pos.x, Pos.y, max_z + 0.01f), Wave, Parent == null ? KeyLayer : Parent, SecondPerBeat, Childrens, LastTime, Offset, Scale);
        #region Behavior
        _wave.Length = Length;
        _wave.Prefect += () =>
        {
            CurrentMark += 500;
            ComboCount += 1;
        };
        _wave.Great += () =>
        {
            CurrentMark += 200;
            ComboCount += 1;
        };
        _wave.Bad += () =>
        {
            CurrentMark += 50;
            ComboCount = 0;
        };
        _wave.Miss += () =>
        {
            CurrentMark += 0;
            ComboCount = 0;
        };
        #endregion

        var OriginPoint = new Vector3(Pos.x, Pos.y, max_z);
        (Vector3 Head, Vector3 End) DrawDragRoute(CirculKey i)
        {
            Vector2 OriginPos = new Vector3(Pos.x, Pos.y, max_z + 0.01f);
            List<Vector3> Points = new List<Vector3>();

            if (i.IDragData.DragRoute.Count == 0)
            {
                double initR = _wave.RealRod * (i.WaveOffset / Length);
                double endR = _wave.RealRod * ((i.Length + i.WaveOffset) / Length);
                for (int p = 0; p <= i.IDragData.Count; p++)
                {
                    double CurrentAngle = i.IDragData.From + (i.IDragData.To - i.IDragData.From) / i.IDragData.Count * p;
                    double CurrentR = initR + (endR - initR) / i.IDragData.Count * p;

                    Vector3 ccp = OriginPos.Offset((float)CurrentAngle, (float)CurrentR);
                    ccp.z = max_z;
                    Points.Add(ccp);

                    if (p == 0)
                    {
                        var pcc = PrePoint.Create(ccp, PrePoint_Obj, KeyLayer, Offset * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), KeyType.Drag);
                        if (i.IDragData.DragRoute.Count == 0)
                            pcc.Angle = (float)i.IDragData.From;


                    }

                    if (p == i.IDragData.Count)
                    {
                        var pcc = PrePoint.Create(ccp, PrePoint_Obj, KeyLayer, Offset * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), KeyType.Drag);
                        if (i.IDragData.DragRoute.Count == 0)
                            pcc.Angle = (float)i.IDragData.To;
                    }

                }
            }
            else
            {
                var StartPos = OriginPos.Offset((float)i.IDragData.From, (float)(_wave.RealRod * (i.WaveOffset / Length)));
                List<Polar2> KFrameList = new List<Polar2>();


                foreach (var z in i.IDragData.DragRoute)
                {
                    KFrameList.Add(new Polar2(Polar2.d2r(z.d¦È), _wave.RealRod * ((i.WaveOffset + z.¦Ñ) / Length)));
                }
                PolarSystem.EnumPolarRoute((p, ii) =>
                {
                    Vector3 ccp = p.ToVector();
                    ccp = (Vector2)ccp + StartPos;
                    ccp.z = max_z;
                    Points.Add(ccp);

                }, new Polar2(0, 0), KFrameList.ToArray(), 35);
            }
            double towardHead = Polar2.FromVector(Points[1] - Points[0]).¦È;
            double towardEnd= Polar2.FromVector(Points[Points.Count - 1] - Points[Points.Count - 2]).¦È;

            PrePoint.Create(Points[0], PrePoint_Obj, KeyLayer, Offset * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), KeyType.Drag).Angle = (float)Polar2.r2d(towardHead);
            PrePoint.Create(Points[Points.Count - 1], PrePoint_Obj, KeyLayer, Offset * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), KeyType.Drag).Angle = (float)Polar2.r2d(towardEnd);
            LineArea.Create(LineAreaObj, KeyLayer, Points.ToArray(), Offset * SecondPerBeat);

            return (Points[0], Points[Points.Count - 1]);
        }

        void DrawSingleRoute(KeyRoute Route/**/)
        {
            for (int v = 0; v < Route.Count; v++)
            {
                if (v == 0)
                {
                    foreach(var Key in Route[0])
                    {
                        if(Key.Type==KeyType.Drag)
                        {
                            var np = DrawDragRoute(Key);
                            Connect(OriginPoint, np.Head, (float)Offset);
                            
                            continue;
                        }

                        PrePoint.Create(Pos.Offset(Key.Angle, _wave.RealRod * ((float)Key.WaveOffset / Length)), PrePoint_Obj, KeyLayer, (float)(Key.WaveOffset + Offset) * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), Key.Type);
                        Connect(OriginPoint, ((Vector2)OriginPoint).Offset(Key.Angle, _wave.RealRod * ((float)Key.WaveOffset / _wave.Length)), (float)Offset);
                    }
                    continue;
                }


                ForEachPoint(Route[v - 1].ToArray(), Route[v].ToArray(), (p1, p2) =>
                  {
                      Connect(p1.GetPositionInDicar(OriginPoint,_wave),p2.GetPositionInDicar(OriginPoint, _wave), (float)(p1.WaveOffset + Offset));
                  });

                foreach(var i in Route[v])
                {
                    if(i.Type==KeyType.Drag)
                    {
                        DrawDragRoute(i);
                        continue;
                    }

                    PrePoint.Create(Pos.Offset(i.Angle, _wave.RealRod * ((float)i.WaveOffset / Length)), PrePoint_Obj, KeyLayer, (float)(i.WaveOffset + Offset) * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), i.Type);
                }

            }
        }

        var Routes = GetRoutes(Childrens);

        foreach (var Route in Routes)
        {
            DrawSingleRoute(Route);
        }

       

        return _wave;
    }
    public DragController CreateDrag_Single(Vector2 Pos, float Offset = 1, GameObject Parent = null)
    {
        float max_z = 0;

        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invailded == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _Drag = DragController.Creat(this, new Vector3(Pos.x, Pos.y, max_z + 0.01f), Drag, Parent == null ? KeyLayer : Parent, SecondPerBeat, Offset);

        _Drag.Prefect += () =>
        {
            CurrentMark += 500;
            ComboCount += 1;
        };
        _Drag.Great += () =>
        {
            CurrentMark += 200;
            ComboCount += 1;
        };
        _Drag.Bad += () =>
        {
            CurrentMark += 50;
            ComboCount = 0;
        };
        _Drag.Miss += () =>
        {
            CurrentMark += 0;
            ComboCount = 0;
        };

        return _Drag;
    }
    public IEnumerator CreateDrag(double From, double To, int Count, double Length, float Offset = 1)
    {
        for (int i = 0; i < Count; i++)
        {
            double Pos = From + (To - From) / Count * i;
            float cy = (float)GetJudgeY((Music.time / SecondPerBeat + 1) + HeadPending);
            Vector3 wp = new Vector3();

            if (!UseWorldCoord)
            {
                wp = Camera.main.ScreenToWorldPoint(new Vector3((float)(Accuracy + Accuracy * Pos), 0, 0));
                wp.y = cy;
            }
            else
                wp = new Vector3((float)Pos, cy, 0);

            var ccp = CreateDrag_Single(wp, Offset);

            wp.z = -9f;
            var dc = CreateVecLine(wp, KeyLayer.transform, 1f / (HeadPending * SecondPerBeat));
            dc.Key = ccp.BAnimation;
            yield return new WaitForSeconds((float)(Length * SecondPerBeat / (float)Count));
        }
        yield break;
    }

    public DragController CreateDragAngle(Vector2 Origin, float rou, float seta, float Offset, GameObject Parent = null)
    {
        return CreateDrag_Single(Origin.Offset(rou, seta), Offset, Parent);
    }

    public DropController CreateVecLine(Vector3 Pos,Transform Parent,float SpeedScale)
    {
        var Obj = Instantiate(Drop, Pos, Parent.rotation, Parent);
        Obj.GetComponent<Animator>().speed = SpeedScale;
        return Obj.GetComponent<DropController>();
    }
}
