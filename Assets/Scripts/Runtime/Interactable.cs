using System;
using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class Interactable : MonoBehaviour
    {
        private Action<Transform> action;
        private Renderer[] renderers;
        private Color[] colors;
        public string Label { get; private set; }
        public void SetLabel(string value) { Label = value; }

        public void Configure(string label, Action<Transform> callback)
        {
            Label = label;
            action = callback;
        }

        public void Activate(Transform actor)
        {
            if (action != null) action(actor);
        }

        public void Highlight(bool enabled)
        {
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
                colors = new Color[renderers.Length];
                for (int i = 0; i < renderers.Length; i++) colors[i] = renderers[i].material.color;
            }
            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = colors[i];
                renderers[i].material.color = enabled
                    ? new Color(Mathf.Min(1f, color.r + 0.15f), Mathf.Min(1f, color.g + 0.15f), Mathf.Min(1f, color.b + 0.15f), color.a)
                    : color;
            }
        }

        private void OnDisable() { if (renderers != null) Highlight(false); }
    }
}
