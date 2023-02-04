using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTap : MonoBehaviour
{
    public GameObject Effect;

    public void OnEnd()
    {
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var x = Instantiate(Effect);
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0;
            x.transform.localPosition = p;
        }
    }
}
