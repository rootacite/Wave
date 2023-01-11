using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum KeyType
{
    Tap,
    Hold,
    Slide,
    Wave,
    Drag,
    HWave
}
public struct DragData
{
    public double From;
    public double To;
    public int Count;

    public List<(double d¦È,double ¦Ñ)> DragRoute;
}

public abstract class BScriptable
{
    public double Time { get; protected set; }
    public double Pos { get; protected set; }
    public double Length { get; protected set; }
}

public class FlatKey : BScriptable
{
    public double? NextToward = null;
    public double? ForceY = null;
    public float TimeOfLastChlidren
    {
        get
        {
            var i = Childrens[Childrens.Count - 1];
            switch (i.Type)
            {
                case KeyType.Tap:
                    return 1;
                case KeyType.Hold:
                    return (float)i.Length + 1;
                case KeyType.Slide:
                    return 1;
                case KeyType.Wave:
                    return (float)i.Length + i.TimeOfLastChildren + 1;
                case KeyType.HWave:
                    return (float)i.Length + i.TimeOfLastChildren + 1;
                case KeyType.Drag:
                    return (float)i.Length + 1;
            }

            return 1;
        }
    }
    public KeyType Type { get; private set; }
    public double WaveScale { get; set; } = 1;

    public List<CirculKey> Childrens { get; private set; } = new List<CirculKey>();
    public DragData IDragData;

    public FlatKey(XElement Source)
    {
        if (Source.Attribute("Type").Value == "Tap")
            Type = KeyType.Tap;
        else 
        if (Source.Attribute("Type").Value == "Hold")
        {
            Type = KeyType.Hold;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);
        }
        else
        if (Source.Attribute("Type").Value == "Slide")
        {
            Type = KeyType.Slide;
        }
        else
        if (Source.Attribute("Type").Value == "Wave")
        {
            Type = KeyType.Wave;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);
            if (Source.Attribute("WaveScale")?.Value != null)
            {
                WaveScale = Convert.ToDouble(Source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in Source.Nodes())
            {
                Childrens.Add(new CirculKey(i));
            }
        }
        else
        if (Source.Attribute("Type").Value == "HWave")
        {
            Type = KeyType.HWave;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);
            if (Source.Attribute("WaveScale")?.Value != null)
            {
                WaveScale = Convert.ToDouble(Source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in Source.Nodes())
            {
                Childrens.Add(new CirculKey(i));
            }
        }
        else
        if (Source.Attribute("Type").Value == "Drag")
        {
            Type = KeyType.Drag;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);
            IDragData = new DragData
            {
                From = Convert.ToDouble(Source.Attribute("From").Value),
                To = Convert.ToDouble(Source.Attribute("To").Value),
                Count = Convert.ToInt32(Source.Attribute("Count").Value)
            };

            Time = Convert.ToDouble(Source.Attribute("Time").Value);
            return;
        }

        Time = Convert.ToDouble(Source.Attribute("Time").Value);
        Pos = Convert.ToDouble(Source.Attribute("Pos").Value);

        XAttribute Y = Source.Attribute("ForceY");
        if (Y != null)
        {
            ForceY = Convert.ToDouble(Y.Value);
        }

        XAttribute Next = Source.Attribute("NextToward");
        if (Next != null)
        {
            NextToward = Convert.ToDouble(Next.Value);
        }
    }
}

public class CirculKey
{
    public float TimeOfLastChildren
    {
        get
        {
            var i = Childrens[Childrens.Count - 1];
            switch (i.Type)
            {
                case KeyType.Tap:
                    return 1;
                case KeyType.Hold:
                    return (float)i.Length + 1;
                case KeyType.Slide:
                    return 1;
                case KeyType.Wave:
                    return (float)i.Length + i.TimeOfLastChildren + 1;
                case KeyType.HWave:
                    return (float)i.Length + i.TimeOfLastChildren + 1;
                case KeyType.Drag:
                    return (float)i.Length + 1;
            }

            return 1;
        }
    }
    public double WaveScale { get; set; } = 1;
    public double Length { get; private set; }
    public KeyType Type { get; private set; }
    public double WaveOffset { get; private set; }
    public int Angle { get; private set; }
    public List<CirculKey> Childrens { get; private set; } = new List<CirculKey>();
    public DragData IDragData;
    public CirculKey(XElement Source)
    {
        if (Source.Attribute("Type").Value == "Tap")
            Type = KeyType.Tap;
        else
        if (Source.Attribute("Type").Value == "Hold")
        {
            Type = KeyType.Hold;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);

        }
        else
        if (Source.Attribute("Type").Value == "Slide")
        {
            Type = KeyType.Slide;
        }
        else
        if (Source.Attribute("Type").Value == "Wave")
        {
            Type = KeyType.Wave;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);
            if(Source.Attribute("WaveScale")?.Value!=null)
            {
                WaveScale = Convert.ToDouble(Source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in Source.Nodes())
            {
                Childrens.Add(new CirculKey(i));
            }
        }
        else
        if (Source.Attribute("Type").Value == "HWave")
        {
            Type = KeyType.HWave;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);
            if (Source.Attribute("WaveScale")?.Value != null)
            {
                WaveScale = Convert.ToDouble(Source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in Source.Nodes())
            {
                Childrens.Add(new CirculKey(i));
            }
        }
        else
        if (Source.Attribute("Type").Value == "Drag")
        {
            Type = KeyType.Drag;
            Length = Convert.ToDouble(Source.Attribute("Length").Value);
            IDragData = new DragData
            {
                Count = Convert.ToInt32(Source.Attribute("Count").Value),
                From = Convert.ToDouble(Source.Attribute("From").Value),
                DragRoute = new List<(double d¦È, double ¦Ñ)>()
            };

            bool HasNode = false;
            foreach(XElement KeyFrame in Source.Nodes())
            {
                HasNode = true;
                IDragData.DragRoute.Add((Convert.ToDouble(KeyFrame.Attribute("Xita").Value), Convert.ToDouble(KeyFrame.Attribute("Rou").Value)));
            }

            if(!HasNode)
            {
                IDragData.To = Convert.ToDouble(Source.Attribute("To").Value);
            }
            WaveOffset = Convert.ToDouble(Source.Attribute("Offset").Value);
            return;
        }

        WaveOffset = Convert.ToDouble(Source.Attribute("Offset").Value);
        Angle = Convert.ToInt32(Source.Attribute("Angle").Value);
    }

    public Vector2 GetPositionInDicar(Vector2 Zero, WaveController ctrl)
    {
        if(Type==KeyType.Drag)
        {
            var p = new Polar2(Polar2.d2r(IDragData.From), ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
            return p.ToVector() + Zero;
        }
        return Zero.Offset(Angle, ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
    }

    public Vector2 GetPositionInDicar(Vector2 Zero, HWaveController ctrl)
    {
        if (Type == KeyType.Drag)
        {
            var p = new Polar2(Polar2.d2r(IDragData.From), ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
            return p.ToVector() + Zero;
        }
        return Zero.Offset(Angle, ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
    }
}
