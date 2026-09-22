using System;
using LightweightGame.Interaction;
using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class Interactable : MonoBehaviour, IEnvironmentInteractable
    {
        private Action<Transform> action;
        private Renderer[] renderers;
        private Color[] colors;
        private MaterialPropertyBlock propertyBlock;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        public string Label { get; private set; }
        public string InteractionLabel { get { return Label; } }
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

        public bool CanInteract(Transform actor) { return enabled && gameObject.activeInHierarchy && action != null; }
        public void Interact(Transform actor) { Activate(actor); }
        public void SetHighlighted(bool enabled) { Highlight(enabled); }

        public void Highlight(bool enabled)
        {
            if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
                colors = new Color[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    Material material = renderers[i].sharedMaterial;
                    if (material == null) colors[i] = Color.white;
                    else if (material.HasProperty(BaseColor)) colors[i] = material.GetColor(BaseColor);
                    else if (material.HasProperty(ColorProperty)) colors[i] = material.GetColor(ColorProperty);
                    else colors[i] = Color.white;
                }
            }
            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = colors[i];
                Color result = enabled
                    ? new Color(Mathf.Min(1f, color.r + 0.15f), Mathf.Min(1f, color.g + 0.15f), Mathf.Min(1f, color.b + 0.15f), color.a)
                    : color;
                Material material = renderers[i].sharedMaterial;
                if (material == null) continue;
                renderers[i].GetPropertyBlock(propertyBlock);
                if (material.HasProperty(BaseColor)) propertyBlock.SetColor(BaseColor, result);
                else if (material.HasProperty(ColorProperty)) propertyBlock.SetColor(ColorProperty, result);
                else continue;
                renderers[i].SetPropertyBlock(propertyBlock);
            }
        }

        private void OnDisable() { if (renderers != null) Highlight(false); }
    }
}
