using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ExamCamera : MonoBehaviour
{
    public GameObject PrePoint_Obj;
    public GameObject Drop;
    public GameObject LineAreaObj;
    public GameObject HWave;
    public Camera main;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool Flag = true;
    // Update is called once per frame
    void Update()
    {
        if (Flag && Input.GetMouseButton(0))
        {
           /*
            
             Vector3 vv = new Vector3(0, 0, 0);
            vv.z = 10;

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

            var Children = new List<CirculKey>();
            var _hwave = HWaveController.Creat(this, new Vector3(vv.x, vv.y, max_z + 0.01f), HWave, gameObject, 1,1, Children, 1, 1, 1);
            #region Behavior
            _hwave.Length = 2;
          
            #endregion

            var OriginPoint = new Vector3(vv.x, vv.y, max_z);
            (Vector3 Head, Vector3 End) DrawDragRoute(CirculKey i)
            {
                Vector2 OriginPos = new Vector3(vv.x, vv.y, max_z + 0.01f);
                List<Vector3> Points = new List<Vector3>();

                if (i.IDragData.DragRoute.Count == 0)
                {
                    double initR = _hwave.RealRod * (i.WaveOffset / 1);
                    double endR = _hwave.RealRod * ((i.Length + i.WaveOffset) / 1);
                    for (int p = 0; p <= i.IDragData.Count; p++)
                    {
                        double CurrentAngle = i.IDragData.From + (i.IDragData.To - i.IDragData.From) / i.IDragData.Count * p;
                        double CurrentR = initR + (endR - initR) / i.IDragData.Count * p;

                        Vector3 ccp = OriginPos.Offset((float)CurrentAngle, (float)CurrentR);
                        ccp.z = max_z;
                        Points.Add(ccp);

                        if (p == 0)
                        {
                            var pcc = PrePoint.Create(ccp, PrePoint_Obj, gameObject, 1, 1 / (1 * 1), KeyType.Drag);
                            if (i.IDragData.DragRoute.Count == 0)
                                pcc.Angle = (float)i.IDragData.From;


                        }

                        if (p == i.IDragData.Count)
                        {
                            var pcc = PrePoint.Create(ccp, PrePoint_Obj, gameObject, 1, 1, KeyType.Drag);
                            if (i.IDragData.DragRoute.Count == 0)
                                pcc.Angle = (float)i.IDragData.To;
                        }

                    }
                }
                else
                {
                    var StartPos = OriginPos.Offset((float)i.IDragData.From, (float)(_hwave.RealRod * (i.WaveOffset / 1)));
                    List<Polar2> KFrameList = new List<Polar2>();


                    foreach (var z in i.IDragData.DragRoute)
                    {
                        KFrameList.Add(new Polar2(Polar2.d2r(z.d¦È), _hwave.RealRod * ((i.WaveOffset + z.¦Ñ) / 1)));
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
                double towardEnd = Polar2.FromVector(Points[Points.Count - 1] - Points[Points.Count - 2]).¦È;

                PrePoint.Create(Points[0], PrePoint_Obj, gameObject, 1, 1, KeyType.Drag).Angle = (float)Polar2.r2d(towardHead);
                PrePoint.Create(Points[Points.Count - 1], PrePoint_Obj, gameObject, 1, 1, KeyType.Drag).Angle = (float)Polar2.r2d(towardEnd);
                LineArea.Create(LineAreaObj, gameObject, Points.ToArray(), 1);

                return (Points[0], Points[Points.Count - 1]);
            }

            void DrawSingleRoute(KeyRoute Route)
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
                                Connect(OriginPoint, np.Head, (float)1);
                                continue;
                            }

                            PrePoint.Create(Pos.Offset(Key.Angle, _hwave.RealRod * ((float)Key.WaveOffset / Length)), PrePoint_Obj, KeyLayer, (float)(Key.WaveOffset + Offset) * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), Key.Type);
                            Connect(OriginPoint, ((Vector2)OriginPoint).Offset(Key.Angle, _hwave.RealRod * ((float)Key.WaveOffset / _hwave.Length)), (float)Offset);
                        }
                        continue;
                    }


                    ForEachPoint(Route[v - 1].ToArray(), Route[v].ToArray(), (p1, p2) =>
                    {
                        Connect(p1.GetPositionInDicar(OriginPoint, _hwave), p2.GetPositionInDicar(OriginPoint, _hwave), (float)(p1.WaveOffset + Offset));
                    });

                    foreach (var i in Route[v])
                    {
                        if (i.Type == KeyType.Drag)
                        {
                            DrawDragRoute(i);
                            continue;
                        }

                        PrePoint.Create(vv.Offset(i.Angle, _hwave.RealRod * ((float)i.WaveOffset / Length)), PrePoint_Obj, KeyLayer, (float)(i.WaveOffset + Offset) * SecondPerBeat, 1 / (SecondPerBeat * HeadPending), i.Type);
                    }

                }
            }

            var Routes = GetRoutes(Childrens);

            foreach (var Route in Routes)
            {
                DrawSingleRoute(Route);
            }

            */

            Flag = false;
        }
        if(!Input.GetMouseButton(0))
        {
            Flag = true;
        }
    }
}
