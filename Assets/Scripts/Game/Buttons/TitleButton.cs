using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    IEnumerator Title()
    {
        StartInit.Show();
        yield return new WaitForSecondsRealtime(1);
        var Async = SceneManager.LoadSceneAsync(8);
        Async.completed += (v) =>
        {
            Time.timeScale = 1;
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
