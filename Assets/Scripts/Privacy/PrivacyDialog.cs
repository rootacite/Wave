using UnityEngine;
using UnityEngine.UI;

public class PrivacyDialog : MonoBehaviour
{
    private void Start()
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        GameObject panelObject = new GameObject("Panel");
        RectTransform panelTransform = panelObject.AddComponent<RectTransform>();
        panelTransform.SetParent(canvas.transform, false);
        panelTransform.sizeDelta = new Vector2(400, 200);
        panelTransform.anchoredPosition = Vector2.zero;
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = Color.white;
        GameObject titleObject = new GameObject("Title");
        RectTransform titleTransform = titleObject.AddComponent<RectTransform>();
        titleTransform.SetParent(panelTransform, false);
        titleTransform.sizeDelta = new Vector2(0, 30);
        titleTransform.anchoredPosition = new Vector2(0, 70);
        Text titleText = titleObject.AddComponent<Text>();
        titleText.text = "ER";
       
        titleText.fontSize = 20;
        titleText.alignment = TextAnchor.MiddleCenter;
        GameObject contentObject = new GameObject("Content");
        RectTransform contentTransform = contentObject.AddComponent<RectTransform>();
        contentTransform.SetParent(panelTransform, false);
        contentTransform.sizeDelta = new Vector2(0, 60);
        contentTransform.anchoredPosition = new Vector2(0, 20);
        Text contentText = contentObject.AddComponent<Text>();
        contentText.text = "XXX";
        
        contentText.fontSize = 16;
        contentText.alignment = TextAnchor.MiddleCenter;
        GameObject agreeButtonObject = new GameObject("AgreeButton");
        RectTransform agreeButtonTransform = agreeButtonObject.AddComponent<RectTransform>();
        agreeButtonTransform.SetParent(panelTransform, false);
        agreeButtonTransform.sizeDelta = new Vector2(100, 30);
        agreeButtonTransform.anchoredPosition = new Vector2(-70, -40);
        Button agreeButton = agreeButtonObject.AddComponent<Button>();
        agreeButton.onClick.AddListener(AgreeButtonClicked);
        Text agreeButtonText = agreeButtonObject.AddComponent<Text>();
        agreeButtonText.text = "Agree";
        
        agreeButtonText.fontSize = 16;
        agreeButtonText.alignment = TextAnchor.MiddleCenter;
        GameObject rejectButtonObject = new GameObject("RejectButton");
        RectTransform rejectButtonTransform = rejectButtonObject.AddComponent<RectTransform>();
        rejectButtonTransform.SetParent(panelTransform, false);
        rejectButtonTransform.sizeDelta = new Vector2(100, 30);
        rejectButtonTransform.anchoredPosition = new Vector2(70, -40);
        Button rejectButton = rejectButtonObject.AddComponent<Button>();
        rejectButton.onClick.AddListener(RejectButtonClicked);
        Text rejectButtonText = rejectButtonObject.AddComponent<Text>();
        rejectButtonText.text = "Deny";
        
        rejectButtonText.fontSize = 16;
        rejectButtonText.alignment = TextAnchor.MiddleCenter;
    }
    private void AgreeButtonClicked()
    {
        
    }
    private void RejectButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
