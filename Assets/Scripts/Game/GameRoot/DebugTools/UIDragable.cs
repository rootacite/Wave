using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDragable : MonoBehaviour
{
    private float SavedY;

    public float MaxX;
    public float MinX;

    public RectTransform Rt;
    public float Value
    {
        get
        {
            return (Rt.localPosition.x - MinX) / (MaxX - MinX);
        }
    }
    public delegate void ValueChangedHandler(float Value);
    public event ValueChangedHandler ValueChanged;

    // Start is called before the first frame update
    void Start()
    {
        SavedY = Rt.anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        float a;
        if ((a = Input.GetAxis("Mouse ScrollWheel")) != 0)
        {
            if (a > 0)
            {
                var oldpos = Rt.anchoredPosition;
                oldpos.y = SavedY;


                oldpos.x -= 1;

                if (oldpos.x > MinX & oldpos.x < MaxX)
                    Rt.anchoredPosition = oldpos;

                float ract = (Rt.anchoredPosition.x + (MaxX - MinX) / 2f) / (MaxX - MinX);
                ValueChanged?.Invoke(Value);
            }
            else
            {
                var oldpos = Rt.anchoredPosition;
                oldpos.y = SavedY;


                oldpos.x += 1;

                if (oldpos.x > MinX & oldpos.x < MaxX)
                    Rt.anchoredPosition = oldpos;

                float ract = (Rt.anchoredPosition.x + (MaxX - MinX) / 2f) / (MaxX - MinX);
                ValueChanged?.Invoke(Value);
            }
        }
    }

    private void OnMouseDrag()
    {
        var oldpos = Rt.anchoredPosition;
        oldpos.y = SavedY;

        var ScreenMousePos = Input.mousePosition;
        var ParentScreenPos = Camera.main.WorldToScreenPoint(transform.parent.position);

        ScreenMousePos.x -= ParentScreenPos.x;
        ScreenMousePos.y -= ParentScreenPos.y;


        oldpos.x = ScreenMousePos.x;

        if (oldpos.x > MinX & oldpos.x < MaxX)
            Rt.anchoredPosition = oldpos;
    }

    private void OnMouseUp()
    {
        float ract = (Rt.anchoredPosition.x + (MaxX - MinX) / 2f) / (MaxX - MinX);
        ValueChanged?.Invoke(Value);
    }
}
