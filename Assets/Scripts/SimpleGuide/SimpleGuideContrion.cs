using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGuideContrion : MonoBehaviour
{
    IEnumerator GuideCoroutine(GameScripting Root)
    {
        Root.DisablePause(true);
        Root.Metronome.Music.Pause();
        yield return new WaitForSeconds(4);
        StartInit.ShowText("Tap键:蓝色，在边缘收缩至最小，或下落的雨滴恰好命中Tap键时单击即可");
        yield return new WaitForSeconds(2);
        Root.Metronome.Music.Play();
        yield return new WaitForSeconds(10.5f);
        StartInit.ShowText("Hold键:绿色，按住Hold键，直到扩大的边缘完全舒展");
        yield return new WaitForSeconds(11.5f);
        StartInit.ShowText("需要同时按下的两个键之间，会有直线连接");

        yield return new WaitUntil(() => { return Root.Metronome.Music.time >= 28; });
        StartInit.ShowText("Slide键:金色的星形键，在点击的同时，轻轻向任意一个方向滑动", 4f);
        yield return new WaitUntil(() => { return Root.Metronome.Music.time >= 36; });
        StartInit.ShowText("Drag键:紫色，通常成串出现，在其上滑过即可", 2f);
        
        yield return new WaitUntil(() => { return Root.Metronome.Music.time >= 41; });
        StartInit.ShowText("Wave键:会在周围展开星盘，命中后在星盘上对应的位置，生成四种基本键\n不类型的键位，在星盘中的样式略有不同。", 8f);
        
        yield return new WaitUntil(() => { return Root.Metronome.Music.time >= 50; });
        StartInit.ShowText("让我们把所有元素结合起来!", 4);
        
        yield return new WaitUntil(() => { return Root.Metronome.Music.time >= 82; });
        StartInit.ShowText("祝您游玩愉快!", 5);
        
        
        yield break;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public Coroutine StartGuide(GameScripting Root)
    {
        return StartCoroutine(GuideCoroutine(Root));
    }
}
