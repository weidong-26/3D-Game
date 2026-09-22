using System.Collections;
using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    [RequireComponent(typeof(Interactable))]
    public sealed class InteractableCabinet : MonoBehaviour
    {
        [SerializeField] private Transform movingPart;
        [SerializeField] private Vector3 openOffset;
        [SerializeField] private Vector3 openAngles;
        [SerializeField] private float duration = 0.45f;
        [SerializeField] private float cooldown = 0.2f;
        private Interactable interactable;
        private Vector3 closedPosition;
        private Quaternion closedRotation;
        private Coroutine animation;
        private float nextUseTime;
        public bool IsOpen { get; private set; }

        public void Configure(Transform part, Vector3 offset, Vector3 angles)
        {
            movingPart = part;
            openOffset = offset;
            openAngles = angles;
        }

        private void Awake()
        {
            if (movingPart == null) movingPart = transform;
            closedPosition = movingPart.localPosition;
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
            interactable.Configure(IsOpen ? "关闭柜子" : "打开柜子", Interact);
        }

        private IEnumerator Animate()
        {
            Vector3 startPosition = movingPart.localPosition;
            Quaternion startRotation = movingPart.localRotation;
            Vector3 targetPosition = closedPosition + (IsOpen ? openOffset : Vector3.zero);
            Quaternion targetRotation = closedRotation * Quaternion.Euler(IsOpen ? openAngles : Vector3.zero);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                movingPart.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                movingPart.localRotation = Quaternion.Slerp(startRotation, targetRotation, progress);
                yield return null;
            }
            movingPart.localPosition = targetPosition;
            movingPart.localRotation = targetRotation;
            animation = null;
        }
    }
}
