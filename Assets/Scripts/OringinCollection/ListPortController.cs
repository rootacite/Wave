using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPortController : MonoBehaviour
{
    public Transform horList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var lp = transform.localPosition;
        lp.y = horList.localPosition.x / 60f * 80f;
        transform.localPosition = lp;
    }
}
