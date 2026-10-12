using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class CharacterMotionAnimator : MonoBehaviour
    {
        private ThirdPersonController controller;
        private PhysicsGrabber grabber;
        private Animator animator;
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private string currentState = "Locomotion";
        private bool wasGrounded;
        private float landUntil;
        private float interactUntil;

        public void Initialize(ThirdPersonController movement, PhysicsGrabber pickup, Animator visualAnimator)
        {
            controller = movement;
            grabber = pickup;
            animator = visualAnimator;
        }

        public void PlayInteraction(float duration = 0.7f)
        {
            if (animator == null || !animator.enabled) return;
            interactUntil = Time.time + duration;
        }

        private void Update()
        {
            if (controller == null || animator == null || !controller.enabled) return;
            animator.SetFloat(SpeedParameter, controller.PlanarSpeed);
            if (controller.IsGrounded && !wasGrounded && controller.VerticalSpeed < 0f)
                landUntil = Time.time + 0.18f;
            wasGrounded = controller.IsGrounded;
            string next;
            if (!controller.IsGrounded) next = controller.VerticalSpeed > 0.1f ? "Jump" : "Fall";
            else if (Time.time < landUntil) next = "Land";
            else if (grabber != null && grabber.IsHolding) next = "Hold";
            else if (Time.time < interactUntil) next = "Interact";
            else next = "Locomotion";
            if (next == currentState) return;
            currentState = next;
            animator.CrossFade(next, 0.12f);
        }
    }
}
