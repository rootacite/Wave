using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AvatarPic : MonoBehaviour
{
    private static RawImage _avatarInstance = null;
    

    private static IEnumerator DownloadImage(string url)
    {
        if (url == "")
        {
            _avatarInstance.texture = null;
            yield break;
        }
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // 成功下载图像，将其设置为RawImage的纹理
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            _avatarInstance.texture = texture;
        }
        else
        {
            // 下载失败，输出错误信息
            Debug.LogError("Image download failed: " + www.error);
        }
    }
    public static void SetAvatar(string uri)
    {
        _avatarInstance.StartCoroutine(DownloadImage(uri));
    }
    // Start is called before the first frame update
    void Start()
    {
        _avatarInstance = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
