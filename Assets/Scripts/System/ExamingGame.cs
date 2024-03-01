using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamingGame : MonoBehaviour
{
    public Creator creator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            creator.CreateTap(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
