using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportBtnMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            StartInit.ShowText("请于设置页面中，在导入扩展包一栏导入外部包。", 3.5f);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
