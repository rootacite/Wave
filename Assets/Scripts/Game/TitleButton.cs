using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    IEnumerator Title()
    {
        Time.timeScale = 1;
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
        var btn = GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() =>
        {
            StartCoroutine(Title());
        });
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
