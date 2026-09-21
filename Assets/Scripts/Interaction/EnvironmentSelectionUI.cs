using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LightweightGame.Interaction
{
    public sealed class EnvironmentSelectionUI : MonoBehaviour
    {
        public const string SkipCreatorKey = "environment.skipCreatorOnce";

        private void Awake()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                cameraObject.tag = "MainCamera";
                camera = cameraObject.GetComponent<Camera>();
            }
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.055f, 0.075f, 0.095f);
            CreateUi();
        }

        private void CreateUi()
        {
            GameObject canvasObject = new GameObject("EnvironmentSelectionCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            Text title = Text(canvasObject.transform, "第二阶段环境地图", 42, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -56f);
            titleRect.sizeDelta = new Vector2(900f, 70f);

            string[] names = { "家庭住宅", "学校 / 教室", "社区公园", "城市街区" };
            string[] details = { "两层现代住宅", "教室、走廊与教学设施", "步道、绿化与休息区", "道路、商住立面与城市设施" };
            string[] scenes = { "Home_Environment_V2", "School_Environment_V1", "Park_Environment_V1", "Urban_Environment_V1" };
            Color[] colors =
            {
                new Color(0.23f, 0.36f, 0.42f), new Color(0.30f, 0.32f, 0.42f),
                new Color(0.20f, 0.38f, 0.28f), new Color(0.36f, 0.28f, 0.24f)
            };
            for (int i = 0; i < names.Length; i++)
            {
                int index = i;
                GameObject card = new GameObject("MapCard_" + i, typeof(RectTransform), typeof(Image));
                card.transform.SetParent(canvasObject.transform, false);
                RectTransform rect = card.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(270f, 360f);
                rect.anchoredPosition = new Vector2(-450f + i * 300f, -15f);
                card.GetComponent<Image>().color = colors[i];

                Text name = Text(card.transform, names[i], 28, TextAnchor.MiddleCenter);
                SetRect(name.rectTransform, new Vector2(0.08f, 0.55f), new Vector2(0.92f, 0.9f));
                Text detail = Text(card.transform, details[i], 17, TextAnchor.UpperCenter);
                SetRect(detail.rectTransform, new Vector2(0.08f, 0.30f), new Vector2(0.92f, 0.56f));
                Button button = Button(card.transform, "进入", new Vector2(0.12f, 0.08f), new Vector2(0.88f, 0.24f));
                button.onClick.AddListener(delegate { Enter(scenes[index]); });
            }

            Text note = Text(canvasObject.transform, "选择场景后保留已保存的角色外观；游戏中可点击右上角返回。", 17, TextAnchor.MiddleCenter);
            RectTransform noteRect = note.rectTransform;
            noteRect.anchorMin = new Vector2(0.5f, 0f);
            noteRect.anchorMax = new Vector2(0.5f, 0f);
            noteRect.pivot = new Vector2(0.5f, 0f);
            noteRect.anchoredPosition = new Vector2(0, 28f);
            noteRect.sizeDelta = new Vector2(900f, 38f);
        }

        private static void Enter(string scene)
        {
            PlayerPrefs.SetInt(SkipCreatorKey, 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(scene);
        }

        private static Text Text(Transform parent, string value, int size, TextAnchor alignment)
        {
            GameObject item = new GameObject("Text", typeof(RectTransform), typeof(Text));
            item.transform.SetParent(parent, false);
            Text text = item.GetComponent<Text>();
            text.font = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", size);
            text.fontSize = size;
            text.color = Color.white;
            text.alignment = alignment;
            text.text = value;
            return text;
        }

        private static Button Button(Transform parent, string caption, Vector2 min, Vector2 max)
        {
            GameObject item = new GameObject("EnterButton", typeof(RectTransform), typeof(Image), typeof(Button));
            item.transform.SetParent(parent, false);
            SetRect(item.GetComponent<RectTransform>(), min, max);
            item.GetComponent<Image>().color = new Color(0.07f, 0.12f, 0.15f, 0.95f);
            Button button = item.GetComponent<Button>();
            Text label = Text(item.transform, caption, 21, TextAnchor.MiddleCenter);
            SetRect(label.rectTransform, Vector2.zero, Vector2.one);
            return button;
        }

        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}

