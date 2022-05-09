using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(UnityEngine.UI.Button))]
public class CheckBoxBehavior : MonoBehaviour
{
    public Sprite[] Items;
    private UnityEngine.UI.Button Button;

    bool status = false;
    // Start is called before the first frame update
    private void Awake()
    {
        Button = GetComponent<UnityEngine.UI.Button>();
    }
    void Start()
    {
        Button.onClick.AddListener(() =>
        {
            status = !status;

            if (status)
                Button.GetComponent<UnityEngine.UI.Image>().sprite = Items[1];
            else
                Button.GetComponent<UnityEngine.UI.Image>().sprite = Items[0];

            ValueChanged?.Invoke(status);
        });
    }
    public delegate void ValueChangeHanlder(bool status);
    public event ValueChangeHanlder ValueChanged;

    public void SetValue(bool status)
    {
        this.status = status;

        if (status)
            Button.GetComponent<UnityEngine.UI.Image>().sprite = Items[1];
        else
            Button.GetComponent<UnityEngine.UI.Image>().sprite = Items[0];
    }
}
