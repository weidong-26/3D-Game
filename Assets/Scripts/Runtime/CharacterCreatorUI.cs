using System;
using System.Collections.Generic;
using LightweightGame.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LightweightGame.Runtime
{
    public sealed class CharacterCreatorUI : MonoBehaviour
    {
        private readonly List<Slider> sliders = new List<Slider>();
        private readonly GameObject[] pages = new GameObject[3];
        private AppearanceData data;
        private CharacterAppearance appearance;
        private Action<AppearanceData> saveAction;
        private Text status;

        public static CharacterCreatorUI Create(CharacterAppearance appearance, AppearanceData data, Action<AppearanceData> saveAction)
        {
            EnsureEventSystem();
            GameObject canvasObject = new GameObject("CharacterCreatorCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            CharacterCreatorUI ui = canvasObject.AddComponent<CharacterCreatorUI>();
            ui.appearance = appearance;
            ui.data = data;
            ui.saveAction = saveAction;
            ui.Build(canvasObject.transform);
            ui.Refresh();
            return ui;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Build(Transform canvas)
        {
            GameObject panel = CreateUiObject("Panel", canvas, typeof(Image));
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 0.5f);
            panelRect.sizeDelta = new Vector2(400f, 0f);
            panel.GetComponent<Image>().color = new Color(0.035f, 0.045f, 0.065f, 0.95f);

            CreateText("Title", panel.transform, "CHARACTER CREATOR", 25, TextAnchor.MiddleLeft, new Vector2(20f, -22f), new Vector2(360f, 38f));
            CreateText("Subtitle", panel.transform, "In-game customization / saved locally", 13, TextAnchor.MiddleLeft, new Vector2(20f, -52f), new Vector2(360f, 24f)).color = new Color(0.65f, 0.72f, 0.82f);

            CreateButton(panel.transform, "Face", new Vector2(20f, -90f), delegate { ShowPage(0); });
            CreateButton(panel.transform, "Body", new Vector2(142f, -90f), delegate { ShowPage(1); });
            CreateButton(panel.transform, "Style", new Vector2(264f, -90f), delegate { ShowPage(2); });
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i] = CreateUiObject("Page" + i, panel.transform);
                Stretch(pages[i].GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            }

            float y = -140f;
            sliders.Add(CreateSlider(pages[0].transform, "Face Width", y, 0, delegate(float value) { data.FaceWidth = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Face Length", y, 0, delegate(float value) { data.FaceLength = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Chin Width", y, 0, delegate(float value) { data.ChinWidth = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Chin Length", y, 0, delegate(float value) { data.ChinLength = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Cheekbones", y, 0, delegate(float value) { data.Cheekbones = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Eye Size", y, 0, delegate(float value) { data.EyeSize = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Eye Spacing", y, 0, delegate(float value) { data.EyeSpacing = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Brow Height", y, 0, delegate(float value) { data.BrowHeight = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Brow Angle", y, 0, delegate(float value) { data.BrowAngle = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Nose Size", y, 0, delegate(float value) { data.NoseSize = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Nose Width", y, 0, delegate(float value) { data.NoseWidth = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Mouth Size", y, 0, delegate(float value) { data.MouthSize = value; Apply(); })); y -= 35f;
            sliders.Add(CreateSlider(pages[0].transform, "Lip Thickness", y, 0, delegate(float value) { data.LipThickness = value; Apply(); }));

            y = -140f;
            sliders.Add(CreateSlider(pages[1].transform, "Body Type", y, 1, delegate(float value) { data.BodyType = Mathf.RoundToInt(value); Apply(); })); y -= 43f;
            sliders.Add(CreateSlider(pages[1].transform, "Height", y, 0, delegate(float value) { data.Height = value; Apply(); })); y -= 43f;
            sliders.Add(CreateSlider(pages[1].transform, "Build", y, 0, delegate(float value) { data.Build = value; Apply(); })); y -= 43f;
            sliders.Add(CreateSlider(pages[1].transform, "Shoulders", y, 0, delegate(float value) { data.ShoulderWidth = value; Apply(); })); y -= 43f;
            sliders.Add(CreateSlider(pages[1].transform, "Leg Length", y, 0, delegate(float value) { data.LegLength = value; Apply(); }));

            y = -140f;
            sliders.Add(CreateSlider(pages[2].transform, "Skin Tone", y, 0, delegate(float value) { data.SkinTone = value; Apply(); })); y -= 43f;
            sliders.Add(CreateSlider(pages[2].transform, "Hair Style", y, 2, delegate(float value) { data.HairStyle = Mathf.RoundToInt(value); Apply(); })); y -= 43f;
            sliders.Add(CreateSlider(pages[2].transform, "Hair Color", y, 0, delegate(float value) { data.HairColor = value; Apply(); })); y -= 43f;
            sliders.Add(CreateSlider(pages[2].transform, "Outfit", y, 2, delegate(float value) { data.Outfit = Mathf.RoundToInt(value); Apply(); }));
            ShowPage(0);

            float buttonY = -625f;
            CreateButton(panel.transform, "Random", new Vector2(20f, buttonY), delegate { data = AppearanceRandomizer.Create(new System.Random()); Refresh(); });
            CreateButton(panel.transform, "Default", new Vector2(142f, buttonY), delegate { data = AppearanceData.CreateDefault(); Refresh(); });
            CreateButton(panel.transform, "Save & Play", new Vector2(264f, buttonY), delegate { saveAction(data); });
            status = CreateText("Status", panel.transform, "Changes preview instantly", 13, TextAnchor.MiddleLeft, new Vector2(20f, -675f), new Vector2(360f, 25f));

            Text hint = CreateText("PreviewHint", canvas, "Right drag: rotate character  |  Wheel: zoom  |  Save & Play", 16, TextAnchor.MiddleCenter, new Vector2(210f, 20f), new Vector2(850f, 35f));
            RectTransform hintRect = hint.rectTransform;
            hintRect.anchorMin = new Vector2(0.5f, 0f);
            hintRect.anchorMax = new Vector2(0.5f, 0f);
            hintRect.pivot = new Vector2(0.5f, 0f);
            hintRect.anchoredPosition = new Vector2(170f, 18f);
        }

        private void Refresh()
        {
            float[] values =
            {
                data.FaceWidth, data.FaceLength, data.ChinWidth, data.ChinLength, data.Cheekbones,
                data.EyeSize, data.EyeSpacing, data.BrowHeight, data.BrowAngle, data.NoseSize, data.NoseWidth,
                data.MouthSize, data.LipThickness, data.BodyType, data.Height, data.Build,
                data.ShoulderWidth, data.LegLength, data.SkinTone, data.HairStyle, data.HairColor, data.Outfit
            };
            for (int i = 0; i < sliders.Count; i++)
                sliders[i].SetValueWithoutNotify(values[i]);
            Apply();
        }

        private void Apply()
        {
            appearance.Apply(data);
            if (status != null)
                status.text = "Changes preview instantly";
        }

        private void ShowPage(int index)
        {
            for (int i = 0; i < pages.Length; i++) pages[i].SetActive(i == index);
        }

        private static Slider CreateSlider(Transform parent, string label, float y, int maxInteger, UnityAction<float> changed)
        {
            CreateText(label + "Label", parent, label, 14, TextAnchor.MiddleLeft, new Vector2(20f, y), new Vector2(145f, 30f));
            GameObject sliderObject = CreateUiObject(label + "Slider", parent, typeof(Slider));
            RectTransform rect = sliderObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(165f, y);
            rect.sizeDelta = new Vector2(210f, 28f);

            GameObject background = CreateUiObject("Background", sliderObject.transform, typeof(Image));
            Stretch(background.GetComponent<RectTransform>(), new Vector2(0f, 0.38f), new Vector2(1f, 0.62f), Vector2.zero, Vector2.zero);
            background.GetComponent<Image>().color = new Color(0.17f, 0.20f, 0.25f);

            GameObject fillArea = CreateUiObject("Fill Area", sliderObject.transform);
            Stretch(fillArea.GetComponent<RectTransform>(), new Vector2(0f, 0.25f), new Vector2(1f, 0.75f), new Vector2(7f, 0f), new Vector2(-7f, 0f));
            GameObject fill = CreateUiObject("Fill", fillArea.transform, typeof(Image));
            Stretch(fill.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            fill.GetComponent<Image>().color = new Color(0.16f, 0.65f, 0.93f);

            GameObject handleArea = CreateUiObject("Handle Slide Area", sliderObject.transform);
            Stretch(handleArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(8f, 0f), new Vector2(-8f, 0f));
            GameObject handle = CreateUiObject("Handle", handleArea.transform, typeof(Image));
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(16f, 24f);
            handle.GetComponent<Image>().color = Color.white;

            Slider slider = sliderObject.GetComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handleRect;
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.minValue = 0f;
            slider.maxValue = maxInteger > 0 ? maxInteger : 1f;
            slider.wholeNumbers = maxInteger > 0;
            slider.onValueChanged.AddListener(changed);
            return slider;
        }

        private static void CreateButton(Transform parent, string label, Vector2 position, UnityAction clicked)
        {
            GameObject buttonObject = CreateUiObject(label + "Button", parent, typeof(Image), typeof(Button));
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(112f, 38f);
            Image image = buttonObject.GetComponent<Image>();
            image.color = label == "Save & Play" ? new Color(0.13f, 0.58f, 0.36f) : new Color(0.18f, 0.23f, 0.31f);
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(clicked);
            CreateText("Text", buttonObject.transform, label, 14, TextAnchor.MiddleCenter, Vector2.zero, Vector2.zero, true);
        }

        private static Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment, Vector2 position, Vector2 size, bool stretch = false)
        {
            GameObject textObject = CreateUiObject(name, parent, typeof(Text));
            RectTransform rect = textObject.GetComponent<RectTransform>();
            if (stretch)
                Stretch(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            else
            {
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 0.5f);
                rect.anchoredPosition = position;
                rect.sizeDelta = size;
            }
            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            return text;
        }

        private static GameObject CreateUiObject(string name, Transform parent, params Type[] components)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            for (int i = 0; i < components.Length; i++)
                gameObject.AddComponent(components[i]);
            return gameObject;
        }

        private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }
}
