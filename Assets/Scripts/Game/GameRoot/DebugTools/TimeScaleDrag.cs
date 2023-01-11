using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIDragable))]
public class TimeScaleDrag : MonoBehaviour
{
    public GameScripting rootConfig;
    public UIDragable dragable;
    // Start is called before the first frame update
    void Start()
    {
        dragable.ValueChanged += (v) =>
        {
            rootConfig.Music.pitch = v;
            Time.timeScale = v;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
