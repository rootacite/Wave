using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KeyRoute = System.Collections.Generic.List<System.Collections.Generic.List<CircularKey>>;
public partial class Creator : MonoBehaviour
{
    public void Connect(Vector2 p1, Vector2 p2, float timeWithBeat, GameObject parent, float disappearTimeWithBeat = -1)
    {
        var ln = LineArea.Create(LineAreaObj, parent, p1, p2, timeWithBeat * Metronome.BeatSpeed, disappearTimeWithBeat * Metronome.BeatSpeed);
        ln.ExpandPoint = 0f;
    }
    public HoldController CreateHold(Vector2 Pos, float Length, float Offset = 1, GameObject Parent = null)
    {
        float max_z = 0;
        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invalided == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _hold = HoldController.Creat(new Vector3(Pos.x, Pos.y, max_z + 0.01f), Hold, Parent == null ? gameObject : Parent, Length, Metronome.BeatSpeed, Offset);
        
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

                if (i?.Invalided == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _tap = TapController.Creat( new Vector3(Pos.x, Pos.y, max_z + 0.01f), Tap, Parent == null ? gameObject : Parent, Metronome.BeatSpeed, Offset);

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

                if (i?.Invalided == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _slide = SlideController.Creat(new Vector3(Pos.x, Pos.y, max_z + 0.01f), Slide, Parent == null ? gameObject : Parent, Metronome.BeatSpeed, Offset);
        
        return _slide;
    }
    public WaveController CreateWave(Vector2 Pos, List<CircularKey> Childrens, float Length, float LastTime, float Offset = 1, float Scale = 1, GameObject Parent = null)
    {
        float max_z = 0;

        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invalided == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _wave = WaveController.Creat(this, new Vector3(Pos.x, Pos.y, max_z + 0.02f), Wave, Parent == null ? gameObject : Parent, Metronome.BeatSpeed, Childrens, LastTime, Offset, Scale);
        #region Behavior
        _wave.Length = Length;
        #endregion

        var OriginPoint = new Vector3(Pos.x, Pos.y, max_z);
        (Vector3 Head, Vector3 End) DrawDragRoute(CircularKey i)
        {
            Vector2 OriginPos = new Vector3(Pos.x, Pos.y, max_z + 0.01f);
            List<Vector3> Points = new List<Vector3>();

            if (i.DragData.DragRoute.Count == 0)
            {
                double initR = _wave.RealRod * (i.WaveOffset / Length);
                double endR = _wave.RealRod * ((i.Length + i.WaveOffset) / Length);
                for (int p = 0; p <= i.DragData.Count; p++)
                {
                    double CurrentAngle = i.DragData.From + (i.DragData.To - i.DragData.From) / i.DragData.Count * p;
                    double CurrentR = initR + (endR - initR) / i.DragData.Count * p;

                    Vector3 ccp = OriginPos.Offset((float)CurrentAngle, (float)CurrentR);
                    ccp.z = max_z;
                    Points.Add(ccp);

                }
            }
            else
            {
                var StartPos = OriginPos.Offset((float)i.DragData.From, (float)(_wave.RealRod * (i.WaveOffset / Length)));
                List<Polar2> KFrameList = new List<Polar2>();


                foreach (var z in i.DragData.DragRoute)
                {
                    KFrameList.Add(new Polar2(Polar2.d2r(z.dsita), _wave.RealRod * ((i.WaveOffset + z.rou) / Length)));
                }
                PolarSystem.EnumPolarRoute((p, ii) =>
                {
                    Vector3 ccp = p.ToVector();
                    ccp = (Vector2)ccp + StartPos;
                    ccp.z = max_z;
                    Points.Add(ccp);

                }, new Polar2(0, 0), KFrameList.ToArray(), 35);
            }
            double towardHead = Polar2.FromVector(Points[1] - Points[0]).sita;
            double towardEnd= Polar2.FromVector(Points[Points.Count - 1] - Points[Points.Count - 2]).sita;

            PrePoint.Create(Points[0] - new Vector3(Pos.x, Pos.y, 0), PrePoint_Obj, _wave.gameObject, Offset * Metronome.BeatSpeed, 1 / (Metronome.BeatSpeed * LevelBasicInformation.HeadPending), KeyType.Drag).Angle = (float)Polar2.r2d(towardHead);
            PrePoint.Create(Points[Points.Count - 1] - new Vector3(Pos.x, Pos.y, 0), PrePoint_Obj, _wave.gameObject, Offset * Metronome.BeatSpeed, 1 / (Metronome.BeatSpeed * LevelBasicInformation.HeadPending), KeyType.Drag).Angle = (float)Polar2.r2d(towardEnd);
            var la = LineArea.Create(LineAreaObj, _wave.gameObject, Points.ToArray(), Offset * Metronome.BeatSpeed);
            la.ExpandPoint = 0f;

            return (Points[0], Points[Points.Count - 1]);
        }

        void DrawSingleRoute(KeyRoute Route/**/)
        {
            for (int v = 0; v < Route.Count; v++)
            {
                if (v == 0)
                {
                    foreach (var Key in Route[v])
                    {
                        if (Key.Type == KeyType.Drag)
                        {
                            var np = DrawDragRoute(Key);
                            if((OriginPoint - np.Head).magnitude > 0.01)
                                Connect(OriginPoint, np.Head, (float)Offset, _wave.gameObject,
                                    (float)Key.WaveOffset
                                    );  // Connect origin point to first node if it's Drag Key

                            continue;
                        }

                        
                        PrePoint.Create(Pos.Offset(Key.Angle, _wave.RealRod * ((float)Key.WaveOffset / Length)) - Pos,
                                PrePoint_Obj, _wave.gameObject, (float)(Key.WaveOffset + Offset) * Metronome.BeatSpeed,
                                1 / (Metronome.BeatSpeed * LevelBasicInformation.HeadPending), Key.Type);
                        
                        if(Key.WaveOffset != 0)  // If it is Root Node Connection is not Needed and Will led to error
                            Connect(OriginPoint,
                            ((Vector2)OriginPoint).Offset(Key.Angle,
                                _wave.RealRod * ((float)Key.WaveOffset / _wave.Length)), (float)Offset, _wave.gameObject,
                            (float)Key.WaveOffset
                            );
                    }
                    continue;
                }
                
                GameScripting.ForEachPoint(Route[v - 1].ToArray(), Route[v].ToArray(), (p1, p2) =>
                {
                    Connect(
                        p1.GetPositionInDicar(OriginPoint,_wave),
                        p2.GetPositionInDicar(OriginPoint, _wave), 
                        (float)(p1.WaveOffset + Offset),
                        _wave.gameObject,
                        Mathf.Abs((float)(p2.WaveOffset - p1.WaveOffset))
                        );
                });

                foreach (var i in Route[v])
                {
                    if (i.Type == KeyType.Drag)
                    {
                        DrawDragRoute(i);
                        continue;
                    }

                    if (i.Type != KeyType.Drag)
                        PrePoint.Create(Pos.Offset(i.Angle, _wave.RealRod * ((float)i.WaveOffset / Length)) - Pos,
                            PrePoint_Obj, _wave.gameObject, (float)(i.WaveOffset + Offset) * Metronome.BeatSpeed,
                            1 / (Metronome.BeatSpeed * LevelBasicInformation.HeadPending), i.Type);
                }

            }
        }

        var Routes = GameScripting.GetRoutes(Childrens);

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

                if (i?.Invalided == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _Drag = DragController.Creat(new Vector3(Pos.x, Pos.y, max_z + 0.01f), Drag, Parent == null ? gameObject : Parent, Metronome.BeatSpeed, Offset);
        
        return _Drag;
    }
    public IEnumerator CreateDrag(double From, double To, int Count, double Length, float Offset = 1)
    {
        for (int i = 0; i < Count; i++)
        {
            double Pos = From + (To - From) / Count * i;
            float cy = (float)Metronome.GetJudgeY((Metronome.Music.time / Metronome.BeatSpeed + 1) + LevelBasicInformation.HeadPending);
            Vector3 wp = new Vector3();

            if (!LevelBasicInformation.UseWorldCoordinate)
            {
                wp = Camera.main.ScreenToWorldPoint(new Vector3((float)(LevelBasicInformation.Accuracy + LevelBasicInformation.Accuracy * Pos), 0, 0));
                wp.y = cy;
            }
            else
                wp = new Vector3((float)Pos, cy, 0);

            var ccp = CreateDrag_Single(wp, Offset);

            wp.z = -9f;
            var dc = CreateVecLine(wp, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
            //dc.Key = ccp.BAnimation;
            yield return new WaitForSeconds((float)(Length * Metronome.BeatSpeed / (float)Count));
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
    public HWaveController CreateHWave(Vector2 Pos, List<CircularKey> Childrens, float Length, float LastTime, float Offset = 1, float Scale = 1, GameObject Parent = null)
    {
        float max_z = 0;

        try
        {
            foreach (Keys i in Keys.Instances)
            {
                if (i == null) continue;

                if (i?.Invalided == true) continue;

                if (i.Z > max_z)
                {
                    max_z = i.Z;
                }
            }
        }
        catch (Exception) { }
        var _hwave = HWaveController.Creat( this, new Vector3(Pos.x, Pos.y, max_z + 0.01f),  HWave, Parent == null ?  gameObject : Parent,Length,  Metronome.BeatSpeed, Childrens, LastTime, Offset, Scale);
        #region Behavior
        _hwave.Length = Length;
        #endregion

        var OriginPoint = new Vector3(Pos.x, Pos.y, max_z);
        (Vector3 Head, Vector3 End) DrawDragRoute(CircularKey i)
        {
            Vector2 OriginPos = new Vector3(Pos.x, Pos.y, max_z + 0.01f);
            List<Vector3> Points = new List<Vector3>();

            if (i.DragData.DragRoute.Count == 0)
            {
                double initR = _hwave.RealRod * (i.WaveOffset / Length);
                double endR = _hwave.RealRod * ((i.Length + i.WaveOffset) / Length);
                for (int p = 0; p <= i.DragData.Count; p++)
                {
                    double CurrentAngle = i.DragData.From + (i.DragData.To - i.DragData.From) / i.DragData.Count * p;
                    double CurrentR = initR + (endR - initR) / i.DragData.Count * p;

                    Vector3 ccp = OriginPos.Offset((float)CurrentAngle, (float)CurrentR);
                    ccp.z = max_z;
                    Points.Add(ccp);

                    if (p == 0)
                    {
                        var pcc = PrePoint.Create(ccp - new Vector3(Pos.x, Pos.y, 0),  PrePoint_Obj,  gameObject, Offset *  Metronome.BeatSpeed, 1 / ( Metronome.BeatSpeed * LevelBasicInformation.HeadPending), KeyType.Drag);
                        if (i.DragData.DragRoute.Count == 0)
                            pcc.Angle = (float)i.DragData.From;


                    }

                    if (p == i.DragData.Count)
                    {
                        var pcc = PrePoint.Create(ccp - new Vector3(Pos.x, Pos.y, 0),  PrePoint_Obj,  gameObject, Offset *  Metronome.BeatSpeed, 1 / ( Metronome.BeatSpeed * LevelBasicInformation.HeadPending), KeyType.Drag);
                        if (i.DragData.DragRoute.Count == 0)
                            pcc.Angle = (float)i.DragData.To;
                    }

                }
            }
            else
            {
                var StartPos = OriginPos.Offset((float)i.DragData.From, (float)(_hwave.RealRod * (i.WaveOffset / Length)));
                List<Polar2> KFrameList = new List<Polar2>();


                foreach (var z in i.DragData.DragRoute)
                {
                    KFrameList.Add(new Polar2(Polar2.d2r(z.dsita), _hwave.RealRod * ((i.WaveOffset + z.rou) / Length)));
                }
                PolarSystem.EnumPolarRoute((p, ii) =>
                {
                    Vector3 ccp = p.ToVector();
                    ccp = (Vector2)ccp + StartPos;
                    ccp.z = max_z;
                    Points.Add(ccp);

                }, new Polar2(0, 0), KFrameList.ToArray(), 35);
            }
            double towardHead = Polar2.FromVector(Points[1] - Points[0]).sita;
            double towardEnd = Polar2.FromVector(Points[Points.Count - 1] - Points[Points.Count - 2]).sita;

            PrePoint.Create(Points[0] - new Vector3(Pos.x, Pos.y, 0),  PrePoint_Obj,  _hwave.gameObject, Offset *  Metronome.BeatSpeed, 1 / ( Metronome.BeatSpeed * LevelBasicInformation.HeadPending), KeyType.Drag).Angle = (float)Polar2.r2d(towardHead);
            PrePoint.Create(Points[Points.Count - 1] - new Vector3(Pos.x, Pos.y, 0),  PrePoint_Obj,  _hwave.gameObject, Offset *  Metronome.BeatSpeed, 1 / ( Metronome.BeatSpeed * LevelBasicInformation.HeadPending), KeyType.Drag).Angle = (float)Polar2.r2d(towardEnd);
            var la = LineArea.Create( LineAreaObj,  _hwave.gameObject, Points.ToArray(), Offset *  Metronome.BeatSpeed);
            la.ExpandPoint = 0f;
            return (Points[0], Points[Points.Count - 1]);
        }

        void DrawSingleRoute(KeyRoute Route/**/)
        {
            for (int v = 0; v < Route.Count; v++)
            {
                if (v == 0)
                {
                    foreach (var Key in Route[0])
                    {
                        if (Key.Type == KeyType.Drag)
                        {
                            var np = DrawDragRoute(Key);
                            Connect(OriginPoint, np.Head, (float)Offset, _hwave.gameObject,(float)Key.WaveOffset);

                            continue;
                        }

                        PrePoint.Create(Pos.Offset(Key.Angle, _hwave.RealRod * ((float)Key.WaveOffset / Length)) - Pos,  PrePoint_Obj,  _hwave.gameObject, (float)(Key.WaveOffset + Offset) *  Metronome.BeatSpeed, 1 / ( Metronome.BeatSpeed * LevelBasicInformation.HeadPending), Key.Type);
                        Connect(OriginPoint, 
                            ((Vector2)OriginPoint).Offset(Key.Angle,
                                _hwave.RealRod * ((float)Key.WaveOffset / _hwave.Length)),
                            (float)Offset, _hwave.gameObject,
                            (float)Key.WaveOffset
                            );
                    }
                    continue;
                }


                GameScripting.ForEachPoint(Route[v - 1].ToArray(), Route[v].ToArray(), (p1, p2) =>
                {
                    Connect(
                        p1.GetPositionInDicar(OriginPoint, _hwave), 
                        p2.GetPositionInDicar(OriginPoint, _hwave),
                        (float)(p1.WaveOffset + Offset),
                        _hwave.gameObject,
                        Mathf.Abs((float)(p2.WaveOffset - p1.WaveOffset)));
                });

                foreach (var i in Route[v])
                {
                    if (i.Type == KeyType.Drag)
                    {
                        DrawDragRoute(i);
                        continue;
                    }

                    PrePoint.Create(Pos.Offset(i.Angle, _hwave.RealRod * ((float)i.WaveOffset / Length)) - Pos,  PrePoint_Obj,  _hwave.gameObject, (float)(i.WaveOffset + Offset) *  Metronome.BeatSpeed, 1 / ( Metronome.BeatSpeed * LevelBasicInformation.HeadPending), i.Type);
                }

            }
        }

        var Routes =  GameScripting.GetRoutes(Childrens);

        foreach (var Route in Routes)
        {
            DrawSingleRoute(Route);
        }



        return _hwave;
    }
}
