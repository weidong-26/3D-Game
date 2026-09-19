using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivityX = 3.2f;
        [SerializeField] private float mouseSensitivityY = 2.4f;
        [SerializeField] private float minimumPitch = -65f;
        [SerializeField] private float maximumPitch = 75f;
        [SerializeField] private float minimumDistance = 2.5f;
        [SerializeField] private float maximumDistance = 6.5f;
        [SerializeField] private float collisionRadius = 0.25f;
        private Transform target;
        private float yaw;
        private float pitch = 18f;
        private float distance = 4.5f;

        public void SetTarget(Transform value) { target = value; }

        private void LateUpdate()
        {
            if (target == null) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivityY, minimumPitch, maximumPitch);
            }
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 2f, minimumDistance, maximumDistance);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 focus = target.position + Vector3.up * 1.45f;
            Vector3 backward = -(rotation * Vector3.forward);
            float actualDistance = distance;
            RaycastHit[] hits = Physics.SphereCastAll(focus, collisionRadius, backward, distance);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.transform.IsChildOf(target)) continue;
                actualDistance = Mathf.Min(actualDistance, Mathf.Max(0.4f, hits[i].distance - 0.1f));
            }
            transform.SetPositionAndRotation(focus + backward * actualDistance, rotation);
        }
    }
}
