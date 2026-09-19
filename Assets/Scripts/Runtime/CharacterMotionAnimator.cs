using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class CharacterMotionAnimator : MonoBehaviour
    {
        private ThirdPersonController controller;
        private PhysicsGrabber grabber;
        private Animator animator;
        private string currentState = "Idle";
        private bool wasGrounded;
        private float landUntil;

        public void Initialize(ThirdPersonController movement, PhysicsGrabber pickup, Animator visualAnimator)
        {
            controller = movement;
            grabber = pickup;
            animator = visualAnimator;
        }

        private void Update()
        {
            if (controller == null || animator == null || !controller.enabled) return;
            if (controller.IsGrounded && !wasGrounded && controller.VerticalSpeed < 0f)
                landUntil = Time.time + 0.18f;
            wasGrounded = controller.IsGrounded;
            string next;
            if (!controller.IsGrounded) next = controller.VerticalSpeed > 0.1f ? "Jump" : "Fall";
            else if (Time.time < landUntil) next = "Land";
            else if (grabber != null && grabber.IsHolding) next = "Hold";
            else if (controller.PlanarSpeed > 4.5f) next = "Run";
            else if (controller.PlanarSpeed > 0.2f) next = "Walk";
            else next = "Idle";
            if (next == currentState) return;
            currentState = next;
            animator.CrossFade(next, 0.12f);
        }
    }
}
