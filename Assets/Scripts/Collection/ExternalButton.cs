using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExternalButton : MonoBehaviour
{
    IEnumerator ExternalMenu()
    {
        StartInit.Show();
        yield return new WaitForSecondsRealtime(1);
        var Async = SceneManager.LoadSceneAsync(1);
        Async.completed += (v) =>
        {
            StartInit.Hide();
        };
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetInt("Enable-Default", 0) != 1)
            {
                StartInit.ShowText("当前未导入任何外部曲目，或未导入默认包\n请到设置页面内进行相应操作");
                return;
            }
            StartCoroutine(ExternalMenu());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
