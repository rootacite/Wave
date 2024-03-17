using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sprite))]
public class CircleMask : MonoBehaviour
{ 
    private Material currentMaterial;

    [Range(0,1)]
    public float Front = 0;
    [Range(0,1)]
    public float Back = 0;
    
    private void Awake()
    {
        currentMaterial = GetComponent<SpriteRenderer>().material;
        currentMaterial.SetFloat("_Front", 0);
        currentMaterial.SetFloat("_Back", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentMaterial.SetFloat("_Front", Front);
        currentMaterial.SetFloat("_Back", Back);
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
