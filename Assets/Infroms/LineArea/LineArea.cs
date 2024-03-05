using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LineArea : MonoBehaviour
{
    public float ExpandPoint { get; set; } = 0.5f;
    private readonly List<Vector3> _positions = new List<Vector3>();
    private LineRenderer _line;

    private float Rt { get; set; } = 0f;

    public static LineArea Create(GameObject Origin, GameObject Parent, Vector3 p1, Vector3 p2, float ExistTime,float disappearTime = -1)
    {
        GameObject obj = Instantiate(Origin, Parent.transform);
        var r = obj.GetComponent<LineArea>();
        var l = obj.GetComponent<LineRenderer>();

        r._time = ExistTime;
        r._positions.Add(p1);
        r._positions.Add(p2);
        r._disappearTime = disappearTime;

        return r;
    }

    public static LineArea Create(GameObject Origin, GameObject Parent, Vector3[] ps, float ExistTime)
    {
        GameObject obj = Instantiate(Origin, Parent.transform);
        var r = obj.GetComponent<LineArea>();
        var l = obj.GetComponent<LineRenderer>();

        r._time = ExistTime;
        r._positions.AddRange(ps);

        return r;
    }

    private float _time;
    private float _disappearTime;

    IEnumerator DelayExit()
    {
        yield return new  WaitForSeconds(_time);
        if (_disappearTime > 0f)
        {
            yield return StartCoroutine(Shrink());
        }
        Destroy(gameObject);
    }

    IEnumerator Expland()
    {
        Stopwatch stw = new Stopwatch();
        stw.Reset();
        stw.Start();
        
        while (true)
        {
            yield return null;
            
            double r = stw.ElapsedMilliseconds / 450.0d;
            Rt = (float)r;
            if (r >= 1f)
            {
                stw.Stop();
                yield break;
            }
        }
    }

    IEnumerator Shrink()
    {
        Stopwatch stw = new Stopwatch();
        stw.Reset();
        stw.Start();
        ExpandPoint = 1f;
        while (true)
        {
            yield return null;
            
            double r = stw.ElapsedMilliseconds / (_disappearTime * 1000.0f);
            Rt = 1.0f - (float)r;
            if (r <= 0f)
            {
                stw.Stop();
                yield break;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(_time >= 0)
            StartCoroutine(DelayExit());
        
        _line = GetComponent<LineRenderer>();
        StartCoroutine(Expland());
    }

    // Update is called once per frame
    void Update()
    {
        var pl = GetSubLine();
        _line.positionCount = pl.Length;
        _line.SetPositions(pl);
    }

    public static List<Vector3> GenerateHalfCircle(float radius, float resolution)
    {
        if (radius <= 0 || resolution <= 0)
        {
            return new List<Vector3>();
        }

        List<Vector3> points = new List<Vector3>();
        for (float t = 0; t < Mathf.PI; t += resolution)
        {
            float x = radius * Mathf.Cos(t);
            float y = radius * Mathf.Sin(t);
            points.Add(new Vector3(x, y, 0));
        }
        return points;
    }
    private Vector3[] GetSubLine()
    {
        if (_positions.Count < 2 || Mathf.Abs(Rt - 0f) < 0.001f)
        {
            return new Vector3[0];
        }

        if (Mathf.Abs(Rt - 1.0f) < 0.001f)
        {
            return _positions.ToArray();
        }

        
        float start = ExpandPoint * (1f - Rt);    //Start Point Rate at Total Line
        float end = ExpandPoint + (1f - ExpandPoint) * Rt; // ...
        float totalLength = TotalCurveLength(_positions.ToArray()); // Total Length

        List<Vector3> r = new List<Vector3>();
        int i = 1;
        Vector3? Fpoint = null, Spoint = null; 
        Vector3? n_Fpoint = null, o_Spoint = null; // The Nearest Point to start Point, even start Point not exists

        float? rFpoint = null, rSpoint = null;
        float? rn_Fpoint = null, ro_Spoint = null;
        
        for (; i <= _positions.Count; i++)
        {
            var ll = TotalCurveLength(_positions.GetRange(0, i).ToArray());
            if (ll >= start * totalLength && n_Fpoint == null)
            {
                n_Fpoint =  _positions[(i - 2 < 0) ? 0 : i - 2];
                rn_Fpoint = TotalCurveLength(_positions.GetRange(0, i - 1).ToArray()) / totalLength;
            }

            if (ll > end * totalLength && o_Spoint == null)
            {
                o_Spoint = _positions[i - 1];
                ro_Spoint = ll / totalLength;
            }
            if (ll >= start * totalLength && ll <= end * totalLength) // The Start Point is in between of start position and end position
            {
                Fpoint = _positions[i - 1];
                rFpoint = ll / totalLength;
                break;
            }
        }
        
        for (; i <= _positions.Count; i++)
        {
            var ll = TotalCurveLength(_positions.GetRange(0, i).ToArray());
            if (ll > end * totalLength)
            {
                if (o_Spoint == null)
                {
                    o_Spoint = _positions[i - 1];
                    ro_Spoint = ll / totalLength;
                }

                break;
            }

            r.Add(_positions[i - 1]);
            Spoint = _positions[i - 1];
            rSpoint = ll / totalLength;
            
            if (i == _positions.Count && o_Spoint == null)
            {
                o_Spoint = _positions[i - 1];
                ro_Spoint = ll / totalLength;
            }
        }

        if (Fpoint == null) // Can not find two points in between of start and end
        {
            // left interrupt
            if(ro_Spoint != rn_Fpoint)
                r.Insert(0, Vector3.Lerp(n_Fpoint.Value, o_Spoint.Value, ((start - rn_Fpoint) / (ro_Spoint - rn_Fpoint) ).Value));
            
            // right interrupt
            
            if(ro_Spoint != rn_Fpoint) 
                r.Add(Vector3.Lerp(n_Fpoint.Value, o_Spoint.Value, ((end - rn_Fpoint) / (ro_Spoint - rn_Fpoint) ).Value));
            return r.ToArray();
        }

        if (Spoint == null) // only right interrupt is needed
        {
            if(ro_Spoint != rFpoint)
                r.Add(Vector3.Lerp(Fpoint.Value, o_Spoint.Value, ( (end - rFpoint) / (ro_Spoint - rFpoint)).Value));
            return r.ToArray();
        }
        if(rFpoint != rn_Fpoint)
            r.Insert(0, Vector3.Lerp(n_Fpoint.Value, Fpoint.Value, ((start - rn_Fpoint) / (rFpoint - rn_Fpoint)).Value));
        if(ro_Spoint != rSpoint)
            r.Add(Vector3.Lerp(Spoint.Value, o_Spoint.Value, ((end - rSpoint) / (ro_Spoint - rSpoint)).Value));
        
        return r.ToArray();
    }
    
    private float TotalCurveLength(Vector3[] Curve)
    {
        float totalLength = 0;

        for (int i = 0; i < Curve.Length - 1; i++)
        {
            totalLength += (Curve[i + 1] - Curve[i]).magnitude;
        }

        return totalLength;
    }
}
