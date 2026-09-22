using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    [RequireComponent(typeof(Interactable), typeof(PickupItem))]
    public sealed class PickupInteractable : MonoBehaviour
    {
        private PhysicsGrabber grabber;
        private PickupItem item;

        private void Awake()
        {
            item = GetComponent<PickupItem>();
        }

        public void Bind(PhysicsGrabber value)
        {
            grabber = value;
            GetComponent<Interactable>().Configure("拿起 " + item.DisplayName, Interact);
        }

        private void Interact(Transform actor)
        {
            if (grabber != null) grabber.Pickup(item);
        }
    }
}
