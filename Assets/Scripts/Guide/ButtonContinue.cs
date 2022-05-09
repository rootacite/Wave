using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonContinue : MonoBehaviour
{
    public GameObject GuideLayer;
    public GameObject Song;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            Song.GetComponent<AudioSource>().UnPause();
            GuideLayer.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
