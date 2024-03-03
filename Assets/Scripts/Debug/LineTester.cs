using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTester : MonoBehaviour
{
    public GameObject LineAreaObj;
    // Start is called before the first frame update
    void Start()
    {
        LineArea.Create(LineAreaObj, gameObject, LineArea.GenerateHalfCircle(5f,2.5f).ToArray(), -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
