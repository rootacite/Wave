using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OriginButton : MonoBehaviour
{
    IEnumerator ExternalMenu()
    {
        StartInit.Show();
        yield return new WaitForSecondsRealtime(1);
        var Async = SceneManager.LoadSceneAsync(9);
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
            StartCoroutine(ExternalMenu());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
