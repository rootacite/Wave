using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Dialog;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(
            () =>
            {
                var d = Instantiate(Dialog);
                d.GetComponent<KC_DIALOG>().Content = "确认要退出吗？";
                d.GetComponent<KC_DIALOG>().OnCancel += () =>
                {
                    return 0;
                };
                d.GetComponent<KC_DIALOG>().OnOK += () =>
                {
                    Application.Quit();
                    return 0;
                };
            }
            );

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
