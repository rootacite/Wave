using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ProcessBar : MonoBehaviour
{
    public RectTransform rectTransform;

    public AudioSource Music;
    public RootConfig RootConfig;

    // Start is called before the first frame update
    void Start()
    {
        if (!RootConfig.isDebugMode) Destroy(GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {

        if (!Draging && Music.clip != null)
        {
            double rate = Music.time / Music.clip.length;
            var oldpos = rectTransform.anchoredPosition;
            oldpos.x = (float)(rate * 2300d - 1150d);

            if (oldpos.x > -1150 & oldpos.x < 1150)
                rectTransform.anchoredPosition = oldpos;

        }
    }

    private void OnMouseDrag()
    {
        var oldpos = rectTransform.anchoredPosition;
        oldpos.x = Input.mousePosition.x - 1200;

        if (oldpos.x > -1150 & oldpos.x < 1150)
            rectTransform.anchoredPosition = oldpos;
    }

    private void OnMouseUp()
    {
        Draging = false;
        float ract = (rectTransform.anchoredPosition.x + 1150f) / 2300f;
        Debug.Log("Redirect to " + ract.ToString() + ".");

        Music.time = Music.clip.length * ract;


        RootConfig.ReloadSongData();
    }

    private void OnMouseDown()
    {
        Draging = true;
    }
    bool Draging = false;
}
