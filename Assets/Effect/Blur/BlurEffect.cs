using UnityEngine;

public class BlurEffect : MonoBehaviour
{
    public bool isOpen = true; // �Ƿ���ģ��Ч��
    private RenderTexture finalRT; // ������ģ��Ч�����rt
    private RenderTexture tempRT; // ���ڴ���ģ��Ч����rt
    public int blurCount = 4; // ģ�����Ӵ���
    public Material blurMat; // ģ���Ĳ���(shader)
    const int BLUR_HOR_PASS = 0; // shader�еĺ���ģ��Pass����0
    const int BLUR_VER_PASS = 1; // shader�е�����ģ��Pass����1
    [Range(0, 1.0f)]
    public float blurSize; // ģ���̶�

    private void BlurTexture(Texture source, RenderTexture destination)
    {
        Debug.Log("Called");

        if (isOpen)
        {
            int width = source.width;
            int height = source.height;
            finalRT = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(source, finalRT);
            for (int i = 0; i < blurCount; i++)
            {
                blurMat.SetFloat("_BlurSize", (1.0f + i) * blurSize);
                tempRT = RenderTexture.GetTemporary(width, height, 0);
                Graphics.Blit(finalRT, tempRT, blurMat, BLUR_HOR_PASS);
                Graphics.Blit(tempRT, finalRT, blurMat, BLUR_VER_PASS);
                RenderTexture.ReleaseTemporary(tempRT);
            }
            Graphics.Blit(finalRT, destination);
            RenderTexture.ReleaseTemporary(finalRT);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void Start()
    {
        var rawimage = GetComponent<UnityEngine.UI.RawImage>();
        RenderTexture Tx = new RenderTexture(2400, 1350, 32, RenderTextureFormat.ARGB32);

        BlurTexture(rawimage.texture, Tx);

        rawimage.texture = Tx;
    }
}
