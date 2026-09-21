using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LightweightGame.Interaction
{
    public sealed class EnvironmentNavigation : MonoBehaviour
    {
        public static void Attach(GameObject gameplayHud)
        {
            EnvironmentNavigation navigation = gameplayHud.AddComponent<EnvironmentNavigation>();
            GameObject buttonObject = new GameObject("ReturnToMapButton", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(gameplayHud.transform, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-18f, -18f);
            rect.sizeDelta = new Vector2(170f, 42f);
            buttonObject.GetComponent<Image>().color = new Color(0.12f, 0.28f, 0.35f, 0.94f);
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(navigation.ReturnToMap);
            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(buttonObject.transform, false);
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
            Text label = labelObject.GetComponent<Text>();
            label.font = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 18);
            label.fontSize = 18;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = "返回地图选择";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ReturnToMap();
        }

        private void ReturnToMap()
        {
            SceneManager.LoadScene("Environment_Select_V2");
        }
    }
}

