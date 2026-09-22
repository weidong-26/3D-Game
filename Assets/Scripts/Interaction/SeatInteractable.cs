using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    [RequireComponent(typeof(Interactable))]
    public sealed class SeatInteractable : MonoBehaviour
    {
        [SerializeField] private Transform seatPoint;
        [SerializeField] private Transform exitPoint;

        public void Configure(Transform seat, Transform exit)
        {
            seatPoint = seat;
            exitPoint = exit;
        }

        private void Awake()
        {
            GetComponent<Interactable>().Configure("坐下", Interact);
        }

        private void Interact(Transform actor)
        {
            PoseController pose = actor.GetComponent<PoseController>();
            if (pose == null || seatPoint == null || exitPoint == null) return;
            pose.Enter(PoseKind.Sitting, seatPoint.position, seatPoint.rotation, exitPoint.position);
        }
    }
}
