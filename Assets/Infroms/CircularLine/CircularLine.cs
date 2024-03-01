using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularLine : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    [Range(0f, 1f)]
    public float transparency = 1f;

    public float radius = 1f;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Flush()
    {
        _lineRenderer.positionCount = 630;
        Vector3[] points = new Vector3[630];
        int i = 0;
        for (float a = 0; a <= 6.30f; a += 0.01f, i++)
        {
            var p = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
            p *= radius;
            p += gameObject.transform.position;
            points[i] = p;
        }
        _lineRenderer.SetPositions(points);

        //_lineRenderer.startColor = new Color(1, 1, 1, transparency);
        //_lineRenderer.endColor   = new Color(1, 1, 1, transparency);
    }
    
    // Update is called once per frame
    void Update()
    {
        Flush();
    }
}
