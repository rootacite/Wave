using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public delegate bool TouchHandler(TouchPhase Type, Vector2 Position);
    public List<TouchHandler> OnTouch = new List<TouchHandler>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touch_pos = Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));

                foreach(var e in OnTouch)
                {
                    bool Handled = e.Invoke(touch.phase, touch_pos);
                    if (Handled) break;
                }
                
            }
        }
    }
}
