using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragLine : MonoBehaviour
{

    static public DragLine Create(GameObject Origin, GameObject Parent, Vector3[] ps)
    {
        GameObject obj = Instantiate(Origin, Parent.transform);
        var r = obj.GetComponent<DragLine>();
        var l = obj.GetComponent<LineRenderer>();


        l.positionCount = ps.Length;
        l.SetPositions(ps);

        r.Points = new List<Vector3>();
        r.Points.AddRange(ps);
        r.lineRenderer = l;

        return r;
    }

    List<Vector3> Points;
    LineRenderer lineRenderer;
    public void Sub(double Rate)
    {

        if (Rate == 0) Destroy(gameObject);
        List<Vector3> SubPoints = new List<Vector3>();
        SubPoints.AddRange(Points);

        for (int i = 0; i < Points.Count * (1d - Rate); i++)
            SubPoints.RemoveAt(0);

        lineRenderer.positionCount = SubPoints.Count;
        lineRenderer.SetPositions(SubPoints.ToArray());

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

}
