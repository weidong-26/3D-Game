using LightweightGame.Core;
using UnityEngine;

namespace LightweightGame.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class ThirdPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float acceleration = 16f;
        [SerializeField] private float turnSpeed = 14f;
        [Header("Jump and recovery")]
        [SerializeField] private float jumpHeight = 1.35f;
        [SerializeField] private int maximumAirJumps = 1;
        [SerializeField] private float gravity = -18f;
        [SerializeField] private float respawnHeight = -8f;

        private CharacterController controller;
        private Transform cameraTransform;
        private Vector3 planarVelocity;
        private Vector3 safePosition;
        private Vector3 spawnPosition;
        private float verticalVelocity;
        private int airJumpsUsed;
        private bool grounded;

        public float PlanarSpeed { get { return planarVelocity.magnitude; } }
        public float VerticalSpeed { get { return verticalVelocity; } }
        public bool IsGrounded { get { return grounded; } }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            spawnPosition = transform.position;
            safePosition = spawnPosition;
        }

        public void SetCamera(Transform value) { cameraTransform = value; }

        public void Respawn()
        {
            controller.enabled = false;
            transform.position = safePosition;
            controller.enabled = true;
            planarVelocity = Vector3.zero;
            verticalVelocity = 0f;
            airJumpsUsed = 0;
            grounded = false;
        }

        private void Update()
        {
            if (cameraTransform == null) return;
            if (Input.GetKeyDown(KeyCode.R)) Respawn();
            if (MovementRules.NeedsRespawn(transform.position.y, respawnHeight)) Respawn();

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            Vector2 input = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
            Vector3 direction = right * input.x + forward * input.y;
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            planarVelocity = Vector3.MoveTowards(planarVelocity, direction * speed, acceleration * Time.deltaTime);
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);

            grounded = controller.isGrounded;
            if (grounded)
            {
                airJumpsUsed = 0;
                if (verticalVelocity < 0f) verticalVelocity = -2f;
                if (transform.position.y > spawnPosition.y - 0.5f) safePosition = transform.position;
            }
            if (Input.GetButtonDown("Jump") && MovementRules.CanJump(grounded, airJumpsUsed, maximumAirJumps))
            {
                if (!grounded) airJumpsUsed++;
                verticalVelocity = MovementRules.JumpVelocity(jumpHeight, gravity);
                grounded = false;
            }
            verticalVelocity += gravity * Time.deltaTime;
            CollisionFlags flags = controller.Move((planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
            if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f) verticalVelocity = 0f;
            grounded = (flags & CollisionFlags.Below) != 0;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic || hit.moveDirection.y < -0.3f) return;
            body.AddForce(new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z) * 3f, ForceMode.VelocityChange);
        }
    }
}
