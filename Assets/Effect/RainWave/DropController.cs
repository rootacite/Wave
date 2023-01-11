using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    public Animator? Key = null; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBest()
    {
        if (Key?.gameObject.GetComponent<DragController>() == null)
            Key?.SetTrigger("OnBest");
    }

    public void AutoDestroy()
    {
        Destroy(gameObject);
    }
}
