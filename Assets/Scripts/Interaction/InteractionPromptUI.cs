using UnityEngine;
using UnityEngine.UI;

namespace LightweightGame.Interaction
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        private Button button;
        private Text label;

        public void Initialize(Button promptButton, Text promptLabel)
        {
            button = promptButton;
            label = promptLabel;
            SetPrompt(string.Empty, false);
        }

        public void SetPrompt(string value, bool visible)
        {
            if (label != null) label.text = value;
            if (button != null && button.gameObject.activeSelf != visible)
                button.gameObject.SetActive(visible);
        }
    }
}
