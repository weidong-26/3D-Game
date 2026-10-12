using System.Collections;
using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    [RequireComponent(typeof(Interactable))]
    public sealed class InteractableDoor : MonoBehaviour
    {
        [SerializeField] private Transform movingPart;
        [SerializeField] private Vector3 openAngles = new Vector3(0f, -100f, 0f);
        [SerializeField] private float duration = 0.55f;
        [SerializeField] private float cooldown = 0.2f;
        private string openLabel = "打开门";
        private string closeLabel = "关闭门";
        private Interactable interactable;
        private Quaternion closedRotation;
        private Coroutine animation;
        private float nextUseTime;
        public bool IsOpen { get; private set; }

        public void Configure(Transform part, Vector3 angles)
        {
            movingPart = part;
            openAngles = angles;
        }

        public void ConfigurePromptLabels(string openPrompt, string closePrompt)
        {
            if (!string.IsNullOrEmpty(openPrompt)) openLabel = openPrompt;
            if (!string.IsNullOrEmpty(closePrompt)) closeLabel = closePrompt;
            if (interactable != null) Refresh();
        }

        private void Awake()
        {
            if (movingPart == null) movingPart = transform;
            closedRotation = movingPart.localRotation;
            interactable = GetComponent<Interactable>();
            Refresh();
        }

        private void Interact(Transform actor)
        {
            if (Time.unscaledTime < nextUseTime) return;
            nextUseTime = Time.unscaledTime + cooldown;
            IsOpen = !IsOpen;
            Refresh();
            if (animation != null) StopCoroutine(animation);
            animation = StartCoroutine(Animate());
        }

        private void Refresh()
        {
            interactable.Configure(IsOpen ? closeLabel : openLabel, Interact);
        }

        private IEnumerator Animate()
        {
            Quaternion start = movingPart.localRotation;
            Quaternion target = closedRotation * Quaternion.Euler(IsOpen ? openAngles : Vector3.zero);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                movingPart.localRotation = Quaternion.Slerp(start, target, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            movingPart.localRotation = target;
            animation = null;
        }
    }
}
