using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class AngleCalc : MonoBehaviour
{
    public TMP_InputField P1;
    public TMP_InputField P2;
    public TMP_InputField Angle;
    public TMP_InputField Beat;

    Vector2 pp1;
    Vector2 pp2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            var RayHit = Physics2D.Raycast(pos, Vector2.zero);
            if (RayHit.collider != null)
            {
                P2.text = ((Vector2)RayHit.collider.gameObject.transform.position).x.ToString("0.0")+","+ ((Vector2)RayHit.collider.gameObject.transform.position).y.ToString("0.0");
                pp2 = (Vector2)RayHit.collider.gameObject.transform.position;
            }
            else
            {
                P2.text = pos.x.ToString("0.0") +","+ pos.y.ToString("0.0");
                pp2 = pos;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            var RayHit = Physics2D.Raycast(pos, Vector2.zero);
            if (RayHit.collider != null)
            {
                P1.text = ((Vector2)RayHit.collider.gameObject.transform.position).x.ToString("0.0") + "," + ((Vector2)RayHit.collider.gameObject.transform.position).y.ToString("0.0");
                pp1 = (Vector2)RayHit.collider.gameObject.transform.position;
            }
            else
            {
                P1.text = pos.x.ToString("0.0") + "," + pos.y.ToString("0.0");
                pp1 = pos;
            }
        }

        try
        {

            var pv = pp1 - pp2;
            var sita = pv.magnitude;

            double cospv = pv.x / sita;
            double sinpv = pv.y / sita;

            if (sinpv >= 0)
            {
                Angle.text = (360d * (Math.Acos(cospv) / (2 * Math.PI))).ToString("0.00");
            }
            else
            {
                Angle.text = (-360d * (Math.Acos(cospv) / (2 * Math.PI))).ToString("0.00");
            }
        }
        catch (Exception)
        {

        }
    }

    Vector2 I2P(TMP_InputField input)
    {
        float x = (float)Convert.ToDouble(input.text.Split(',')[0]);
        float y = (float)Convert.ToDouble(input.text.Split(',')[1]);

        return new Vector2(x, y);
    }
}
