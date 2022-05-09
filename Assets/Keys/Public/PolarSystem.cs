using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarSystem : MonoBehaviour
{
    public delegate void EnumPolarRouteHandler(Polar2 p, int Index);

    static public void EnumPolarRoute(EnumPolarRouteHandler Proc, Polar2 Origin, Polar2[] TowardKeys, int StepCount)
    {
        Polar2 OriginCopy = new Polar2(Origin.牟, Origin.老);
        Proc(OriginCopy, 0);
        for (int i = 1; i < TowardKeys.Length; i++)
        {
            var p1 = TowardKeys[i - 1];
            var p2 = TowardKeys[i];


            for (int j = 1; j <= StepCount; j++)
            {
                double Step老 = (p2 - p1).老 / StepCount;
                double StepToward = (p2 - p1).牟 / StepCount;
                double Toward = p1.牟 + StepToward * j;

                OriginCopy.老 += Step老;
                OriginCopy.牟 += Toward / StepCount;
                Proc(OriginCopy, (i - 1) * StepCount + j);
            }
        }
    }
    static public void EnumPolarRoute(EnumPolarRouteHandler Proc, Polar2 p1, Polar2 p2, int StepCount)
    {

        double Step成 = (p2 - p1).牟 / StepCount;
        double Step老 = (p2 - p1).老 / StepCount;

        for (int i = 0; i <= StepCount; i++)
        {
            Proc(new Polar2()
            {
                牟 = p1.牟 + Step成 * i,
                老 = p1.老 + Step老 * i
            }, i);
        }
    }

}


public struct Polar2
{
    public double 牟;
    public double 老; //說僅

    public Vector2 ToVector()
    {
        return new Vector2(Mathf.Cos((float)牟) * (float)老, Mathf.Sin((float)牟) * (float)老);
    }

    static public Polar2 FromVector(Vector2 Origin)
    {
        double d老 = 0;

        if (Origin.y > 0)
            d老 += Math.Acos(Origin.x / Origin.magnitude);
        else
            d老 -= Math.Acos(Origin.x / Origin.magnitude);

        return new Polar2()
        {
            老 = Origin.magnitude,
            牟 = d老
        };
    }
    public static Polar2 operator +(Polar2 a, Polar2 b)
    {
        return new Polar2()
        {
            老 = b.老 + a.老,
            牟 = b.牟 + a.牟
        };
    }
    public static Polar2 operator -(Polar2 a, Polar2 b)
    {
        return new Polar2()
        {
            老 = a.老 - b.老,
            牟 = a.牟 - b.牟
        };
    }

    public Polar2(double 牟, double 老)
    {
        this.牟 = 牟;
        this.老 = 老;
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