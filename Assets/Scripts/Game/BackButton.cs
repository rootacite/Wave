using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject Audio;
    public GameObject PauseCanvas;
    // Start is called before the first frame update
    IEnumerator Back()
    {
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        Audio.GetComponent<AudioSource>().Play();
        PauseCanvas.SetActive(false);
    }
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {

            PauseCanvas.GetComponent<Animator>().SetBool("Showed", false);
            StartCoroutine(Back());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
