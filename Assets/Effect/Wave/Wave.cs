using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Wave : MonoBehaviour
{ 
    float _speed = 2.5f; 
    float forward_speed = 0.0045f;
    float back_speed = 0.0051f;
    float max_dis = 1;
    public float width = 0.002f;

    /// <summary>
    /// 当前的材质
    /// </summary>
    private Material currentMaterial;

    public const int max_click_count = 100;

//shader最大同时水波的数量是10，要修改请到Wave.shader里面相关代码一起修改
    public Vector4[] uis = new Vector4[max_click_count];
    static public Wave Instance = null;
    void Awake()
    {
        Instance = this;
        
        var Renderer = transform.GetComponent<RawImage>();
        currentMaterial = Renderer.material;
        currentMaterial.SetVectorArray("_ArrayParams", uis);
    }

    public  void SetPoint(Vector2 pos)
    {
        for (int i = 0; i < uis.Length; i++)
        {
            if (uis[i].x == 0 && uis[i].y == 0 && uis[i].w == 0 && uis[i].z == 0)
            {
                uis[i].x = pos.x;
                uis[i].y = pos.y;
                uis[i].z = width;
                uis[i].w = 0;
                break;
            }
        }
    }

    private void FixedUpdate()
    {

        for (int i = 0; i < uis.Length; i++)
        {
            if (uis[i].z > max_dis || uis[i].w > uis[i].z)
                uis[i].Set(0, 0, 0, 0);
            if (uis[i].x != 0 || uis[i].y != 0)
            {
                //波纹行进
                uis[i].z += forward_speed * _speed;
                uis[i].w += back_speed * _speed;
            }

        }

        currentMaterial.SetVectorArray("_ArrayParams", uis);
    }

    private void Update()
    {
       
    }
}