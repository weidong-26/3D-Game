using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    public enum HomeAction { Door, Drawer, Seat, Bed }

    [RequireComponent(typeof(Interactable))]
    public sealed class HomeInteractable : MonoBehaviour
    {
        [SerializeField] private HomeAction action;
        [SerializeField] private Transform movingPart;
        [SerializeField] private Transform posePoint;
        [SerializeField] private Transform standPoint;
        [SerializeField] private Vector3 openOffset;
        [SerializeField] private Vector3 openAngles;

        private Interactable interactable;
        private Vector3 closedPosition;
        private Quaternion closedRotation;
        private bool isOpen;

        public bool IsOpen { get { return isOpen; } }

        public void Configure(HomeAction kind, Transform part, Transform pose, Transform stand, Vector3 offset, Vector3 angles)
        {
            action = kind;
            movingPart = part;
            posePoint = pose;
            standPoint = stand;
            openOffset = offset;
            openAngles = angles;
        }

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            if (movingPart != null)
            {
                closedPosition = movingPart.localPosition;
                closedRotation = movingPart.localRotation;
            }
            interactable.Configure(ClosedLabel(), Activate);
        }

        private void Activate(Transform actor)
        {
            if (action == HomeAction.Door || action == HomeAction.Drawer)
            {
                isOpen = !isOpen;
                interactable.SetLabel(isOpen ? OpenLabel() : ClosedLabel());
                return;
            }
            PoseController pose = actor.GetComponent<PoseController>();
            if (pose == null || posePoint == null || standPoint == null) return;
            pose.Enter(action == HomeAction.Seat ? PoseKind.Sitting : PoseKind.Lying,
                posePoint.position, posePoint.rotation, standPoint.position);
        }

        private void Update()
        {
            if (movingPart == null) return;
            float t = 1f - Mathf.Exp(-8f * Time.deltaTime);
            movingPart.localPosition = Vector3.Lerp(movingPart.localPosition,
                closedPosition + (isOpen ? openOffset : Vector3.zero), t);
            movingPart.localRotation = Quaternion.Slerp(movingPart.localRotation,
                closedRotation * Quaternion.Euler(isOpen ? openAngles : Vector3.zero), t);
        }

        private string ClosedLabel()
        {
            switch (action)
            {
                case HomeAction.Door: return "打开门";
                case HomeAction.Drawer: return "打开抽屉";
                case HomeAction.Seat: return "坐下";
                default: return "躺下休息";
            }
        }

        private string OpenLabel() { return action == HomeAction.Door ? "关门" : "关抽屉"; }
    }
}
