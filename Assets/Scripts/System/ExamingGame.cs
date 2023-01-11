using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamingGame : MonoBehaviour
{
    GameScripting gameScripting;
    bool Flag = true;
    // Start is called before the first frame update
    void Start()
    {
        gameScripting = GetComponent<GameScripting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Flag && Input.GetMouseButton(0))
        {
            Debug.Log("Created");

            Vector3 vv = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vv.z = 10;

            var ct  = gameScripting.CreateHWave(vv,new List<CirculKey>() { }, (float)2, 1, 1, (float)1);
            vv.z = -9f;

            var dc = gameScripting.CreateVecLine(vv, gameScripting.KeyLayer.transform, 1f);
            dc.Key = ct.BAnimation;

            Flag = false;
        }
        if (!Input.GetMouseButton(0))
        {
            Flag = true;
        }
    }
}
