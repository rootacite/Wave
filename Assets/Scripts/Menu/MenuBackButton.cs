using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBackButton : MonoBehaviour
{
    IEnumerator Title()
    {
        StartInit.Show();
        yield return new WaitForSecondsRealtime(1);
        var Async = SceneManager.LoadSceneAsync(8);
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
            StartCoroutine(Title());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
