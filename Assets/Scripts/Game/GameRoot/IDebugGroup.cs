using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public partial class RootConfig
{
    /// <summary>
    /// Debug Tools 这些只用于开发模式
    /// </summary>

    public GameObject DebugPlane;
    public TMP_InputField BeatEditor;

    public AudioClip Debug_Music;
    public TextAsset Debug_SongConfig;
    public TextAsset Debug_SongData;
    public Texture Debug_Image;
    public VideoClip Debug_Video;

    public bool UseDebugResource;
}
