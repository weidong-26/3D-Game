using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivityX = 3.2f;
        [SerializeField] private float mouseSensitivityY = 2.4f;
        [SerializeField] private float minimumPitch = -65f;
        [SerializeField] private float maximumPitch = 75f;
        [SerializeField] private float minimumDistance = 1.8f;
        [SerializeField] private float maximumDistance = 6.5f;
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private float followSmoothTime = 0.1f;
        [SerializeField] private float rotationSmoothing = 14f;
        private readonly RaycastHit[] cameraHits = new RaycastHit[12];
        private Transform target;
        private float yaw;
        private float pitch = 18f;
        private float distance = 4.5f;
        private Vector3 focus;
        private Vector3 focusVelocity;
        private Vector3 positionVelocity;
        private Quaternion orbitRotation;
        private bool initialized;

        public void SetTarget(Transform value)
        {
            target = value;
            initialized = false;
            if (target != null) yaw = target.eulerAngles.y;
        }

        private void LateUpdate()
        {
            UpdateCamera(Time.deltaTime);
        }

        private void UpdateCamera(float deltaTime)
        {
            if (target == null) return;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (Input.GetMouseButton(1))
                ApplyMouseDelta(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 2f, minimumDistance, maximumDistance);

            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 targetFocus = target.position;
            if (!initialized)
            {
                focus = targetFocus;
                orbitRotation = targetRotation;
                transform.SetPositionAndRotation(focus - orbitRotation * Vector3.forward * distance, orbitRotation);
                positionVelocity = Vector3.zero;
                initialized = true;
            }

            deltaTime = Mathf.Max(0f, deltaTime);
            if (deltaTime > 0f)
            {
                focus = Vector3.SmoothDamp(focus, targetFocus, ref focusVelocity, followSmoothTime,
                    Mathf.Infinity, deltaTime);
                float rotationStep = 1f - Mathf.Exp(-rotationSmoothing * deltaTime);
                orbitRotation = Quaternion.Slerp(orbitRotation, targetRotation, rotationStep);
            }

            Vector3 backward = -(orbitRotation * Vector3.forward);
            float actualDistance = distance;
            int hitCount = Physics.SphereCastNonAlloc(focus, collisionRadius, backward, cameraHits, distance,
                ~0, QueryTriggerInteraction.Ignore);
            bool obstructed = false;
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = cameraHits[i];
                cameraHits[i] = new RaycastHit();
                if (hit.collider == null || hit.collider.transform == target.root || hit.collider.transform.IsChildOf(target.root)) continue;
                actualDistance = Mathf.Min(actualDistance, Mathf.Max(0.4f, hit.distance - 0.1f));
                obstructed = true;
            }

            Vector3 desiredPosition = focus + backward * actualDistance;
            float currentDistance = Vector3.Distance(transform.position, focus);
            if (obstructed && currentDistance > actualDistance)
            {
                transform.position = desiredPosition;
                positionVelocity = Vector3.zero;
            }
            else if (deltaTime > 0f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity,
                    followSmoothTime, Mathf.Infinity, deltaTime);
            }
            transform.rotation = orbitRotation;
        }

        private void ApplyMouseDelta(float x, float y)
        {
            yaw += x * mouseSensitivityX;
            pitch = Mathf.Clamp(pitch - y * mouseSensitivityY, minimumPitch, maximumPitch);
        }
    }
}
