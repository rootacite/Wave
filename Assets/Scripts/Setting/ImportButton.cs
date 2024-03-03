using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;

public class ImportButton : MonoBehaviour
{
    private string[] _preSetUris =
    {
        "package://wave.extern.res/default.pkg",
        "package://wave.extern.res/disable-default.pkg"
    };
    
    public Button Import;

    public TMP_InputField IUri;

    IEnumerator AddMusic()
    {
        StartInit.Show();

        if (IUri.text == _preSetUris[0])
        {
            PlayerPrefs.SetInt("Enable-Default", 1);
            yield return new WaitForSeconds(3f);
            StartInit.ShowText("操作成功",2f);
            StartInit.Hide();
            yield break;
        }
        else if (IUri.text == _preSetUris[1])
        {
            PlayerPrefs.SetInt("Enable-Default", 0);
            yield return new WaitForSeconds(3f);
            StartInit.ShowText("操作成功",2f);
            StartInit.Hide();
            yield break;
        }
        
        var src = new WWW(IUri.text);

        while (true)
        {
            if (src.isDone || src.error != null) break;
            
            StartInit.SetTipText("Loading...  " + (src.progress * 100f).ToString("00.00") + "%");
            yield return null;
        }
        
        if (src.error != null)
        {
            StartInit.ShowText("无法加载包，请检查您输入的路径是否有效，或网络是否通畅。:" + src.error,2f);
            StartInit.Hide();
            yield break;
        }

        MemoryStream Ms = new MemoryStream(src.bytes, false);
        var Fz = new FastZip();

         Fz.ExtractZip(
                       Ms, 
                       StartInit.CombinPath("Songs/"),
                       FastZip.Overwrite.Always, 
                       null,
                       "", 
                       "", 
                       true, 
                       true
                   );
        StartInit.Hide();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Import.onClick.AddListener(() =>
        {
            StartCoroutine(AddMusic());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
