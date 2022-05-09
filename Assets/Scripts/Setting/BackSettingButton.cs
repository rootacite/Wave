using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackSettingButton : MonoBehaviour
{

    IEnumerator Title()
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
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
            () =>
            {
                StartCoroutine(Title());
                PlayerPrefs.Save();
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
