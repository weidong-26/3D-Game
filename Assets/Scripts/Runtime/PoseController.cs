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
        private Quaternion toLeftLegRotation;
        private Quaternion toRightLegRotation;
        private float transitionStart;
        public bool IsPosed { get; private set; }

        public void Initialize(ThirdPersonController controller, CharacterVisualRig visualRig)
        {
            movement = controller;
            animator = visualRig.Animator;
            visual = visualRig.VisualRoot;
            leftLeg = visualRig.GetHumanoidBone(HumanBodyBones.LeftUpperLeg) ?? visual.Find("LeftLeg");
            rightLeg = visualRig.GetHumanoidBone(HumanBodyBones.RightUpperLeg) ?? visual.Find("RightLeg");
            normalPosition = visual.localPosition;
            normalRotation = visual.localRotation;
            leftLegRotation = leftLeg != null ? leftLeg.localRotation : Quaternion.identity;
            rightLegRotation = rightLeg != null ? rightLeg.localRotation : Quaternion.identity;
        }

        public void Enter(PoseKind kind, Vector3 place, Quaternion facing, Vector3 standAt)
        {
            if (IsPosed) return;
            exitPosition = standAt;
            movement.enabled = false;
            if (animator != null) animator.enabled = false;
            transform.SetPositionAndRotation(place, facing);
            IsPosed = true;
            Change(kind);
        }

        public void Change(PoseKind kind)
        {
            if (!IsPosed) return;
            fromPosition = visual.localPosition;
            fromRotation = visual.localRotation;
            fromLeftLeg = leftLeg != null ? leftLeg.localRotation : Quaternion.identity;
            fromRightLeg = rightLeg != null ? rightLeg.localRotation : Quaternion.identity;
            toPosition = kind == PoseKind.Lying ? new Vector3(0f, 0.91f, 0f) : new Vector3(0f, -0.18f, 0f);
            toRotation = kind == PoseKind.Lying ? Quaternion.Euler(90f, 0f, 0f) : normalRotation;
            toLeftLegRotation = kind == PoseKind.Sitting ? leftLegRotation * Quaternion.Euler(-65f, 0f, 0f) : leftLegRotation;
            toRightLegRotation = kind == PoseKind.Sitting ? rightLegRotation * Quaternion.Euler(-65f, 0f, 0f) : rightLegRotation;
            transitionStart = Time.time;
        }

        private void Update()
        {
            if (!IsPosed) return;
            float progress = Mathf.Clamp01((Time.time - transitionStart) / 0.45f);
            visual.localPosition = Vector3.Lerp(fromPosition, toPosition, progress);
            visual.localRotation = Quaternion.Slerp(fromRotation, toRotation, progress);
            if (leftLeg != null) leftLeg.localRotation = Quaternion.Slerp(fromLeftLeg, toLeftLegRotation, progress);
            if (rightLeg != null) rightLeg.localRotation = Quaternion.Slerp(fromRightLeg, toRightLegRotation, progress);
        }

        public void Stand(bool resumeMovement = true)
        {
            if (!IsPosed) return;
            IsPosed = false;
            visual.localPosition = normalPosition;
            visual.localRotation = normalRotation;
            if (leftLeg != null) leftLeg.localRotation = leftLegRotation;
            if (rightLeg != null) rightLeg.localRotation = rightLegRotation;
            transform.position = exitPosition;
            if (animator != null) animator.enabled = true;
            movement.enabled = resumeMovement;
        }

        public void ResumeMovement() { movement.enabled = true; }
    }
}
