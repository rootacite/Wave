
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

using CKeyGroup = System.Collections.Generic.List<CircularKey>;
using KeyRoute = System.Collections.Generic.List<System.Collections.Generic.List<CircularKey>>;

public partial class GameScripting : MonoBehaviour
{
    // Field
    public static GameScripting Instance;
    
    // Method
    public static List<KeyRoute> GetRoutes(CKeyGroup keys)
    {
        if(keys.Count == 0) return new List<KeyRoute>() { };
        KeyRoute Routes = new KeyRoute();

        var CurrentTarget = new CKeyGroup();
        Routes.Add(CurrentTarget);
        CurrentTarget.Add(keys[0]);


        for (int i = 1; i < keys.Count; i++)
        {
            var child = keys[i];
            if (child.Type == KeyType.Route)
            {
                CurrentTarget = new CKeyGroup();
                Routes.Add(CurrentTarget);
                continue;
            }

            if (child.WaveOffset >= keys[i - 1].WaveOffset)
            {
                CurrentTarget.Add(child);
            }
            else
            {
                CurrentTarget = new CKeyGroup();
                Routes.Add(CurrentTarget);
                CurrentTarget.Add(child);
            }
        }

        List<KeyRoute> SortedKeys = new List<KeyRoute>();

        for (int i = 0; i < Routes.Count; i++) SortedKeys.Add(new KeyRoute());

        int b = 0;
        foreach (var CurrentLayer in SortedKeys)
        {
            var nChildrens = Routes[b];
            for (int i = 0; i < nChildrens.Count;)
            {
                var ChildLayer = new List<CircularKey>();

                CircularKey SaveLast = null;
                for (; i < nChildrens.Count; i++)
                {
                    if (SaveLast != null)
                        if (SaveLast.WaveOffset != nChildrens[i].WaveOffset) break;

                    ChildLayer.Add(nChildrens[i]);
                    SaveLast = nChildrens[i];
                }
                CurrentLayer.Add(ChildLayer);
            }
            b++;
        }

        return SortedKeys;
    }
    public static void ForEachPoint(CircularKey[] ps1, CircularKey[] ps2, TwoWithTwoProc proc)
    {
        foreach(var i in ps1)
        {
            foreach(var j in ps2)
            {
                proc?.Invoke(i, j);
            }
        }
    }
    public static Vector3[] CircularKeyArrayToPoints(Vector2 zero, WaveController ctrl, CKeyGroup origin,float z)
    {
        List<Vector3> Result = new List<Vector3>();
        foreach (var i in origin)
        {
            Vector3 p = zero.Offset(i.Angle, ctrl.RealRod * ((float)i.WaveOffset / ctrl.Length));
            p.z = z;
            Result.Add(p); 
        }

        return Result.ToArray();
    }
    private static Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }
    private static Texture2D Blur(Texture2D image, int blurSize)
    {
        Texture2D blurred = new Texture2D(image.width, image.height);

        // look at every pixel in the blur rectangle
        for (int xx = 0; xx < image.width; xx++)
        {
            for (int yy = 0; yy < image.height; yy++)
            {
                float avgR = 0, avgG = 0, avgB = 0, avgA = 0;
                int blurPixelCount = 0;

                // average the color of the red, green and blue for each pixel in the
                // blur size while making sure you don't go outside the image bounds
                for (int x = xx; (x < xx + blurSize && x < image.width); x++)
                {
                    for (int y = yy; (y < yy + blurSize && y < image.height); y++)
                    {
                        Color pixel = image.GetPixel(x, y);

                        avgR += pixel.r;
                        avgG += pixel.g;
                        avgB += pixel.b;
                        avgA += pixel.a;

                        blurPixelCount++;
                    }
                }

                avgR = avgR / blurPixelCount;
                avgG = avgG / blurPixelCount;
                avgB = avgB / blurPixelCount;
                avgA = avgA / blurPixelCount;

                // now that we know the average for the blur size, set each pixel to that color
                for (int x = xx; x < xx + blurSize && x < image.width; x++)
                    for (int y = yy; y < yy + blurSize && y < image.height; y++)
                        blurred.SetPixel(x, y, new Color(avgR, avgG, avgB, avgA));
            }
        }
        blurred.Apply();
        return blurred;
    }
    public static string BuildNum(int x)
    {
        string l = x.ToString();
        if (l.Length > 7) throw new System.Exception("The value is too big.");
        else if (l.Length == 7) return l;
        else
        {
            string r = "";
            for (int i = 0; i < 7 - l.Length; i++)
            {
                r += "0";
            }
            r += l;
            return r;
        }
    }
    
    // Definitions
    public delegate TextAsset DataReloading();
    public delegate void TwoWithTwoProc(CircularKey p1, CircularKey p2);
    
    // Events
    public event DataReloading Reloading;
    
}