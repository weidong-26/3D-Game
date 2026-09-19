using UnityEngine;

namespace LightweightGame.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PickupItem : MonoBehaviour
    {
        [SerializeField] private string displayName = "Item";
        [SerializeField] private Light flashlight;
        public string DisplayName { get { return displayName; } }
        public bool CanUse { get { return flashlight != null; } }

        public void Initialize(string label, Light light = null)
        {
            displayName = label;
            flashlight = light;
        }

        public void Use()
        {
            if (flashlight != null) flashlight.enabled = !flashlight.enabled;
        }
    }
}
