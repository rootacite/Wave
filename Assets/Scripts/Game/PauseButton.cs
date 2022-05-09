using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Audio;
    public GameObject PauseCanvas;
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
            Time.timeScale = 0; 
            Audio.GetComponent<AudioSource>().Pause();

            PauseCanvas.SetActive(true);
            PauseCanvas.GetComponent<Animator>().SetBool("Showed", true);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
