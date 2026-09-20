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
        private bool[] colliderStates;
        private Transform originalParent;
        private Vector3 originalLocalScale;
        private Vector3 originalWorldScale;
        private bool originalKinematic;
        private bool originalGravity;
        private Vector3 originalVelocity;
        private Vector3 originalAngularVelocity;
        public bool IsHolding { get { return heldItem != null; } }
        public bool CanUseHeld { get { return heldItem != null && heldItem.CanUse; } }

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
            Pickup(FindCandidate());
        }

        public void Pickup(PickupItem candidate)
        {
            if (heldItem != null) return;
            if (candidate == null) return;
            if ((handAnchor.lossyScale - Vector3.one).sqrMagnitude > 0.000001f)
            {
                Debug.LogError("Hand anchor and its parents must have unit scale.");
                return;
            }
            heldItem = candidate;
            originalParent = heldItem.transform.parent;
            originalLocalScale = heldItem.transform.localScale;
            originalWorldScale = heldItem.transform.lossyScale;
            Rigidbody body = heldItem.GetComponent<Rigidbody>();
            originalKinematic = body.isKinematic;
            originalGravity = body.useGravity;
            originalVelocity = body.linearVelocity;
            originalAngularVelocity = body.angularVelocity;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            body.useGravity = false;
            heldColliders = heldItem.GetComponentsInChildren<Collider>();
            colliderStates = new bool[heldColliders.Length];
            for (int i = 0; i < heldColliders.Length; i++)
            {
                colliderStates[i] = heldColliders[i].enabled;
                heldColliders[i].enabled = false;
            }
            heldItem.transform.SetParent(handAnchor, true);
            heldItem.transform.localScale = originalWorldScale;
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
        }

        private void Release()
        {
            if (heldItem == null) return;
            heldItem.transform.SetParent(originalParent, false);
            heldItem.transform.localScale = originalLocalScale;
            heldItem.transform.position = transform.position + transform.forward * 1.4f + Vector3.up * 0.9f;
            Rigidbody body = heldItem.GetComponent<Rigidbody>();
            body.isKinematic = originalKinematic;
            body.useGravity = originalGravity;
            body.linearVelocity = originalVelocity;
            body.angularVelocity = originalAngularVelocity;
            for (int i = 0; i < heldColliders.Length; i++) heldColliders[i].enabled = colliderStates[i];
            heldColliders = null;
            colliderStates = null;
            heldItem = null;
        }

        public void ReleaseHeld() { Release(); }
        public void UseHeld() { if (heldItem != null) heldItem.Use(); }
        public void ThrowHeld()
        {
            if (heldItem == null) return;
            Rigidbody body = heldItem.GetComponent<Rigidbody>();
            Release();
            body.isKinematic = false;
            body.useGravity = true;
            body.linearVelocity = transform.forward * 6f + Vector3.up * 2f;
        }

        private void OnDisable()
        {
            if (heldItem != null) Release();
            if (prompt != null) prompt.text = "";
        }
    }
}
