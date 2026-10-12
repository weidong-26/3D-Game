using UnityEngine;
using UnityEngine.UI;

namespace LightweightGame.Interaction
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        private Button button;
        private Text label;
        private Text dialogue;
        private float messageUntil;

        public void Initialize(Button promptButton, Text promptLabel)
        {
            button = promptButton;
            label = promptLabel;
            GameObject dialogueObject = new GameObject("DialogueMessage", typeof(RectTransform), typeof(Text));
            dialogueObject.transform.SetParent(button.transform.root, false);
            RectTransform rect = dialogueObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 188f);
            rect.sizeDelta = new Vector2(620f, 48f);
            dialogue = dialogueObject.GetComponent<Text>();
            dialogue.font = label.font;
            dialogue.fontSize = 20;
            dialogue.alignment = TextAnchor.MiddleCenter;
            dialogue.color = Color.white;
            dialogue.horizontalOverflow = HorizontalWrapMode.Wrap;
            dialogue.verticalOverflow = VerticalWrapMode.Truncate;
            dialogue.raycastTarget = false;
            dialogueObject.SetActive(false);
            SetPrompt(string.Empty, false);
        }

        public void SetPrompt(string value, bool visible)
        {
            if (label != null) label.text = value;
            if (button != null && button.gameObject.activeSelf != visible)
                button.gameObject.SetActive(visible);
        }

        public void ShowMessage(string value)
        {
            if (dialogue == null) return;
            dialogue.text = value;
            dialogue.gameObject.SetActive(true);
            messageUntil = Time.unscaledTime + 3.5f;
        }

        public void UpdateMessage()
        {
            if (dialogue != null && dialogue.gameObject.activeSelf && Time.unscaledTime >= messageUntil)
                ClearMessage();
        }

        public void ClearMessage()
        {
            if (dialogue == null) return;
            dialogue.text = string.Empty;
            dialogue.gameObject.SetActive(false);
            messageUntil = 0f;
        }
    }
}
