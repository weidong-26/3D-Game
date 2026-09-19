using UnityEngine;
using UnityEngine.UI;

namespace LightweightGame.Runtime
{
    public sealed class PhysicsGrabber : MonoBehaviour
    {
        private Camera viewCamera;
        private Transform handAnchor;
        private Text prompt;
        private PickupItem heldItem;
        private Collider[] heldColliders;
        public bool IsHolding { get { return heldItem != null; } }

        public void SetCamera(Camera value) { viewCamera = value; }
        public void SetHandAnchor(Transform value) { handAnchor = value; }
        public void SetPrompt(Text value) { prompt = value; }

        private void Update()
        {
            if (viewCamera == null || handAnchor == null) return;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (heldItem == null) TryPickup();
                else Release();
            }
            if (heldItem != null && Input.GetKeyDown(KeyCode.F)) heldItem.Use();
            if (prompt != null)
            {
                PickupItem available = heldItem == null ? FindCandidate() : null;
                prompt.text = heldItem != null
                    ? "E Put down " + heldItem.DisplayName + (heldItem.CanUse ? "  |  F Toggle" : "")
                    : available != null ? "E Pick up " + available.DisplayName : "";
            }
        }

        private PickupItem FindCandidate()
        {
            RaycastHit hit;
            Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out hit, 4f))
            {
                PickupItem aimed = hit.collider.GetComponentInParent<PickupItem>();
                if (aimed != null && Vector3.Distance(transform.position, aimed.transform.position) < 3f) return aimed;
            }
            Collider[] nearby = Physics.OverlapSphere(transform.position + Vector3.up, 1.6f);
            PickupItem nearest = null;
            float nearestDistance = float.MaxValue;
            foreach (Collider collider in nearby)
            {
                PickupItem item = collider.GetComponentInParent<PickupItem>();
                if (item == null) continue;
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance >= nearestDistance) continue;
                nearest = item;
                nearestDistance = distance;
            }
            return nearest;
        }

        private void TryPickup()
        {
            heldItem = FindCandidate();
            if (heldItem == null) return;
            Rigidbody body = heldItem.GetComponent<Rigidbody>();
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            body.useGravity = false;
            heldColliders = heldItem.GetComponentsInChildren<Collider>();
            foreach (Collider collider in heldColliders) collider.enabled = false;
            heldItem.transform.SetParent(handAnchor, false);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
        }

        private void Release()
        {
            if (heldItem == null) return;
            heldItem.transform.SetParent(null, true);
            heldItem.transform.position = transform.position + transform.forward * 1.15f + Vector3.up * 0.9f;
            Rigidbody body = heldItem.GetComponent<Rigidbody>();
            body.isKinematic = false;
            body.useGravity = true;
            foreach (Collider collider in heldColliders) collider.enabled = true;
            heldColliders = null;
            heldItem = null;
        }

        private void OnDisable()
        {
            if (heldItem != null) Release();
            if (prompt != null) prompt.text = "";
        }
    }
}
