using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarSystem : MonoBehaviour
{
    public delegate void EnumPolarRouteHandler(Polar2 p, int Index);

    static public void EnumPolarRoute(EnumPolarRouteHandler Proc, Polar2 Origin, Polar2[] TowardKeys, int StepCount)
    {
        Polar2 OriginCopy = new Polar2(Origin.��, Origin.��);
        Proc(OriginCopy, 0);
        for (int i = 1; i < TowardKeys.Length; i++)
        {
            var p1 = TowardKeys[i - 1];
            var p2 = TowardKeys[i];


            for (int j = 1; j <= StepCount; j++)
            {
                double Step�� = (p2 - p1).�� / StepCount;
                double StepToward = (p2 - p1).�� / StepCount;
                double Toward = p1.�� + StepToward * j;

                OriginCopy.�� += Step��;
                OriginCopy.�� += Toward / StepCount;
                Proc(OriginCopy, (i - 1) * StepCount + j);
            }
        }
    }
    static public void EnumPolarRoute(EnumPolarRouteHandler Proc, Polar2 p1, Polar2 p2, int StepCount)
    {

        double Step�� = (p2 - p1).�� / StepCount;
        double Step�� = (p2 - p1).�� / StepCount;

        for (int i = 0; i <= StepCount; i++)
        {
            Proc(new Polar2()
            {
                �� = p1.�� + Step�� * i,
                �� = p1.�� + Step�� * i
            }, i);
        }
    }

}


public struct Polar2
{
    public double ��;
    public double ��; //����

    public Vector2 ToVector()
    {
        return new Vector2(Mathf.Cos((float)��) * (float)��, Mathf.Sin((float)��) * (float)��);
    }

    static public Polar2 FromVector(Vector2 Origin)
    {
        double d�� = 0;

        if (Origin.y > 0)
            d�� += Math.Acos(Origin.x / Origin.magnitude);
        else
            d�� -= Math.Acos(Origin.x / Origin.magnitude);

        return new Polar2()
        {
            �� = Origin.magnitude,
            �� = d��
        };
    }
    public static Polar2 operator +(Polar2 a, Polar2 b)
    {
        return new Polar2()
        {
            �� = b.�� + a.��,
            �� = b.�� + a.��
        };
    }
    public static Polar2 operator -(Polar2 a, Polar2 b)
    {
        return new Polar2()
        {
            �� = a.�� - b.��,
            �� = a.�� - b.��
        };
    }

    public Polar2(double ��, double ��)
    {
        this.�� = ��;
        this.�� = ��;
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