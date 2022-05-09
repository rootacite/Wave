using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlay : MonoBehaviour
{
    static public void PlayAudioOnce(AudioClip Source)
    {
        GameObject obj = new GameObject("AudioOnce");
        var AS = obj.AddComponent<AudioSource>();
        var Ctrl = obj.AddComponent<TempPlay>();

        AS.clip = Source;
        AS.playOnAwake = true;
        AS.loop = false;
        AS.Play();

        Ctrl.StartCoroutine(Ctrl.DelayDestory(Source.length));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DelayDestory(float Time)
    {
        yield return new WaitForSeconds(Time);
        Destroy(gameObject);
    }
}
