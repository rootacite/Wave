using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTrigger : MonoBehaviour
{
    public void OnTrigger()
    {
        if (GameScripting.Instance?.gameObject != null)
            GameScripting.Instance?.OnPrepared();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
