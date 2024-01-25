using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarSystem : MonoBehaviour
{
    public delegate void EnumPolarRouteHandler(Polar2 p, int Index);

    static public void EnumPolarRoute(EnumPolarRouteHandler Proc, Polar2 Origin, Polar2[] TowardKeys, int StepCount)
    {
        Polar2 OriginCopy = new Polar2(Origin.sita, Origin.rou);
        Proc(OriginCopy, 0);
        for (int i = 1; i < TowardKeys.Length; i++)
        {
            var p1 = TowardKeys[i - 1];
            var p2 = TowardKeys[i];


            for (int j = 1; j <= StepCount; j++)
            {
                double step_rou = (p2 - p1).rou / StepCount;
                double StepToward = (p2 - p1).sita / StepCount;
                double Toward = p1.sita + StepToward * j;

                Polar2 newPoint = new Polar2(Toward, step_rou);
                OriginCopy.sita = Polar2.FromVector(OriginCopy.ToVector() + newPoint.ToVector()).sita;
                OriginCopy.rou += step_rou;

                Proc(OriginCopy, (i - 1) * StepCount + j);
            }
        }
    }
    static public void EnumPolarRoute(EnumPolarRouteHandler Proc, Polar2 p1, Polar2 p2, int StepCount)
    {

        double step_sita = (p2 - p1).sita / StepCount;
        double step_rou = (p2 - p1).rou / StepCount;

        for (int i = 0; i <= StepCount; i++)
        {
            Proc(new Polar2()
            {
                sita = p1.sita + step_sita * i,
                rou = p1.rou + step_rou * i
            }, i);
        }
    }

}


public struct Polar2
{
    public double sita;
    public double rou; //????

    public Vector2 ToVector()
    {
        return new Vector2(Mathf.Cos((float)sita) * (float)rou, Mathf.Sin((float)sita) * (float)rou);
    }

    static public Polar2 FromVector(Vector2 Origin)
    {
        double d_rou = 0;

        if (Origin.y > 0)
            d_rou += Math.Acos(Origin.x / Origin.magnitude);
        else
            d_rou -= Math.Acos(Origin.x / Origin.magnitude);

        return new Polar2()
        {
            rou = Origin.magnitude,
            sita = d_rou
        };
    }
    public static Polar2 operator +(Polar2 a, Polar2 b)
    {
        return new Polar2()
        {
            rou = b.rou + a.rou,
            sita = b.sita + a.sita
        };
    }
    public static Polar2 operator -(Polar2 a, Polar2 b)
    {
        return new Polar2()
        {
            rou = a.rou - b.rou,
            sita = a.sita - b.sita
        };
    }

    public Polar2(double sita, double rou)
    {
        this.sita = sita;
        this.rou = rou;
    }

    static public double r2d(double r)
    {
        return r / Math.PI * 180d;
    }

    static public double d2r(double d)
    {
        return d / 180d * Math.PI;
    }
}