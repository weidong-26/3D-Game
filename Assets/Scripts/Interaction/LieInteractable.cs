using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    [RequireComponent(typeof(Interactable))]
    public sealed class LieInteractable : MonoBehaviour
    {
        [SerializeField] private Transform liePoint;
        [SerializeField] private Transform exitPoint;

        public void Configure(Transform lie, Transform exit)
        {
            liePoint = lie;
            exitPoint = exit;
        }

        private void Awake()
        {
            GetComponent<Interactable>().Configure("躺下", Interact);
        }

        private void Interact(Transform actor)
        {
            PoseController pose = actor.GetComponent<PoseController>();
            if (pose == null || liePoint == null || exitPoint == null) return;
            pose.Enter(PoseKind.Lying, liePoint.position, liePoint.rotation, exitPoint.position);
        }
    }
}
