using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamCamera : MonoBehaviour
{
    public GameObject Drop;
    public Camera main;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool Flag = true;
    // Update is called once per frame
    void Update()
    {
        if (Flag && Input.GetMouseButton(0))
        {
            Vector3 vv = new Vector3(0, 0, 0);
            vv.z = 10;

            Instantiate(Drop, vv, this.transform.rotation, this.transform);

            Flag = false;
        }
        if(!Input.GetMouseButton(0))
        {
            Flag = true;
        }
    }
}
