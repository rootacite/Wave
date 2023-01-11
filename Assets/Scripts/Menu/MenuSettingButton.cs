using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSettingButton : MonoBehaviour
{
    IEnumerator Setting()
    {
        StartInit.Show();
        yield return new WaitForSecondsRealtime(1);
        var Async = SceneManager.LoadSceneAsync(5);
        Async.completed += (v) =>
        {
            StartInit.Hide();
        };
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            StartCoroutine(Setting());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
