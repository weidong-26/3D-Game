using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class CharacterPreviewCamera : MonoBehaviour
    {
        private Transform target;
        private float yaw = 180f;
        private float pitch = 3f;
        private float distance = 4.2f;

        public void SetTarget(Transform value) { target = value; }

        private void LateUpdate()
        {
            if (target == null) return;
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * 3f;
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 2f, -50f, 60f);
            }
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 2f, 2.2f, 6f);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 focus = target.position + Vector3.up * 1.35f;
            transform.SetPositionAndRotation(focus - rotation * Vector3.forward * distance, rotation);
        }
    }
}
