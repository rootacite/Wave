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

    public List<(double dsita, double rou)> DragRoute;
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
    public float TimeOfLastChild
    {
        get
        {
            var i = Children[Children.Count - 1];
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

    public List<CircularKey> Children { get; private set; } = new List<CircularKey>();
    public DragData DragData;
    public FlatKey(XElement source)
    {
        if (source.Attribute("Type").Value == "Tap")
            Type = KeyType.Tap;
        else 
        if (source.Attribute("Type").Value == "Hold")
        {
            Type = KeyType.Hold;
            Length = Convert.ToDouble(source.Attribute("Length").Value);
        }
        else
        if (source.Attribute("Type").Value == "Slide")
        {
            Type = KeyType.Slide;
        }
        else
        if (source.Attribute("Type").Value == "Wave")
        {
            Type = KeyType.Wave;
            Length = Convert.ToDouble(source.Attribute("Length").Value);
            if (source.Attribute("WaveScale")?.Value != null)
            {
                WaveScale = Convert.ToDouble(source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in source.Nodes())
            {
                Children.Add(new CircularKey(i));
            }
        }
        else
        if (source.Attribute("Type").Value == "HWave")
        {
            Type = KeyType.HWave;
            Length = Convert.ToDouble(source.Attribute("Length").Value);
            if (source.Attribute("WaveScale")?.Value != null)
            {
                WaveScale = Convert.ToDouble(source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in source.Nodes())
            {
                Children.Add(new CircularKey(i));
            }
        }
        else
        if (source.Attribute("Type").Value == "Drag")
        {
            Type = KeyType.Drag;
            Length = Convert.ToDouble(source.Attribute("Length").Value);
            DragData = new DragData
            {
                From = Convert.ToDouble(source.Attribute("From").Value),
                To = Convert.ToDouble(source.Attribute("To").Value),
                Count = Convert.ToInt32(source.Attribute("Count").Value)
            };

            Time = Convert.ToDouble(source.Attribute("Time").Value);
            return;
        }

        Time = Convert.ToDouble(source.Attribute("Time").Value);
        Pos = Convert.ToDouble(source.Attribute("Pos").Value);

        XAttribute Y = source.Attribute("ForceY");
        if (Y != null)
        {
            ForceY = Convert.ToDouble(Y.Value);
        }

        XAttribute Next = source.Attribute("NextToward");
        if (Next != null)
        {
            NextToward = Convert.ToDouble(Next.Value);
        }
    }
}

public class CircularKey
{
    public float TimeOfLastChildren
    {
        get
        {
            var i = Children[Children.Count - 1];
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
    public List<CircularKey> Children { get; private set; } = new List<CircularKey>();
    public DragData DragData;
    public CircularKey(XElement source)
    {
        if (source.Attribute("Type").Value == "Tap")
            Type = KeyType.Tap;
        else
        if (source.Attribute("Type").Value == "Hold")
        {
            Type = KeyType.Hold;
            Length = Convert.ToDouble(source.Attribute("Length").Value);

        }
        else
        if (source.Attribute("Type").Value == "Slide")
        {
            Type = KeyType.Slide;
        }
        else
        if (source.Attribute("Type").Value == "Wave")
        {
            Type = KeyType.Wave;
            Length = Convert.ToDouble(source.Attribute("Length").Value);
            if(source.Attribute("WaveScale")?.Value!=null)
            {
                WaveScale = Convert.ToDouble(source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in source.Nodes())
            {
                Children.Add(new CircularKey(i));
            }
        }
        else
        if (source.Attribute("Type").Value == "HWave")
        {
            Type = KeyType.HWave;
            Length = Convert.ToDouble(source.Attribute("Length").Value);
            if (source.Attribute("WaveScale")?.Value != null)
            {
                WaveScale = Convert.ToDouble(source.Attribute("WaveScale").Value);
            }
            foreach (XElement i in source.Nodes())
            {
                Children.Add(new CircularKey(i));
            }
        }
        else
        if (source.Attribute("Type").Value == "Drag")
        {
            Type = KeyType.Drag;
            Length = Convert.ToDouble(source.Attribute("Length").Value);
            DragData = new DragData
            {
                Count = Convert.ToInt32(source.Attribute("Count").Value),
                From = Convert.ToDouble(source.Attribute("From").Value),
                DragRoute = new List<(double dsita, double rou)>()
            };

            bool HasNode = false;
            foreach(XElement KeyFrame in source.Nodes())
            {
                HasNode = true;
                DragData.DragRoute.Add((Convert.ToDouble(KeyFrame.Attribute("Xita").Value), Convert.ToDouble(KeyFrame.Attribute("Rou").Value)));
            }

            if(!HasNode)
            {
                DragData.To = Convert.ToDouble(source.Attribute("To").Value);
            }
            WaveOffset = Convert.ToDouble(source.Attribute("Offset").Value);
            return;
        }

        WaveOffset = Convert.ToDouble(source.Attribute("Offset").Value);
        Angle = Convert.ToInt32(source.Attribute("Angle").Value);
    }

    public Vector2 GetPositionInDicar(Vector2 Zero, WaveController ctrl)
    {
        if(Type==KeyType.Drag)
        {
            var p = new Polar2(Polar2.d2r(DragData.From), ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
            return p.ToVector() + Zero;
        }
        return Zero.Offset(Angle, ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
    }

    public Vector2 GetPositionInDicar(Vector2 Zero, HWaveController ctrl)
    {
        if (Type == KeyType.Drag)
        {
            var p = new Polar2(Polar2.d2r(DragData.From), ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
            return p.ToVector() + Zero;
        }
        return Zero.Offset(Angle, ctrl.RealRod * ((float)WaveOffset / ctrl.Length));
    }
}
