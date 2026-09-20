using UnityEngine;

namespace LightweightGame.Runtime
{
    public enum PoseKind { Sitting, Lying }

    public sealed class PoseController : MonoBehaviour
    {
        private ThirdPersonController movement;
        private Animator animator;
        private Transform visual;
        private Transform leftLeg;
        private Transform rightLeg;
        private Vector3 normalPosition;
        private Quaternion normalRotation;
        private Quaternion leftLegRotation;
        private Quaternion rightLegRotation;
        private Vector3 exitPosition;
        private Vector3 fromPosition;
        private Vector3 toPosition;
        private Quaternion fromRotation;
        private Quaternion toRotation;
        private Quaternion fromLeftLeg;
        private Quaternion fromRightLeg;
        private Quaternion toLegRotation;
        private float transitionStart;
        public bool IsPosed { get; private set; }

        public void Initialize(ThirdPersonController controller, Animator visualAnimator)
        {
            movement = controller;
            animator = visualAnimator;
            visual = visualAnimator.transform;
            leftLeg = visual.Find("LeftLeg");
            rightLeg = visual.Find("RightLeg");
            normalPosition = visual.localPosition;
            normalRotation = visual.localRotation;
            leftLegRotation = leftLeg.localRotation;
            rightLegRotation = rightLeg.localRotation;
        }

        public void Enter(PoseKind kind, Vector3 place, Quaternion facing, Vector3 standAt)
        {
            if (IsPosed) return;
            exitPosition = standAt;
            movement.enabled = false;
            animator.enabled = false;
            transform.SetPositionAndRotation(place, facing);
            IsPosed = true;
            Change(kind);
        }

        public void Change(PoseKind kind)
        {
            if (!IsPosed) return;
            fromPosition = visual.localPosition;
            fromRotation = visual.localRotation;
            fromLeftLeg = leftLeg.localRotation;
            fromRightLeg = rightLeg.localRotation;
            toPosition = kind == PoseKind.Lying ? new Vector3(0f, 0.91f, 0f) : new Vector3(0f, -0.18f, 0f);
            toRotation = kind == PoseKind.Lying ? Quaternion.Euler(90f, 0f, 0f) : normalRotation;
            toLegRotation = kind == PoseKind.Sitting ? Quaternion.Euler(-65f, 0f, 0f) : leftLegRotation;
            transitionStart = Time.time;
        }

        private void Update()
        {
            if (!IsPosed) return;
            float progress = Mathf.Clamp01((Time.time - transitionStart) / 0.45f);
            visual.localPosition = Vector3.Lerp(fromPosition, toPosition, progress);
            visual.localRotation = Quaternion.Slerp(fromRotation, toRotation, progress);
            leftLeg.localRotation = Quaternion.Slerp(fromLeftLeg, toLegRotation, progress);
            rightLeg.localRotation = Quaternion.Slerp(fromRightLeg, toLegRotation, progress);
        }

        public void Stand(bool resumeMovement = true)
        {
            if (!IsPosed) return;
            IsPosed = false;
            visual.localPosition = normalPosition;
            visual.localRotation = normalRotation;
            leftLeg.localRotation = leftLegRotation;
            rightLeg.localRotation = rightLegRotation;
            transform.position = exitPosition;
            animator.enabled = true;
            movement.enabled = resumeMovement;
        }

        public void ResumeMovement() { movement.enabled = true; }
    }
}
