using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    public sealed class InteractionDetector : MonoBehaviour
    {
        [SerializeField] private float rayDistance = 3.2f;
        [SerializeField] private float nearbyRadius = 2.1f;
        private readonly Collider[] nearby = new Collider[32];
        private Camera viewCamera;
        private Transform actor;

        public void Initialize(Camera camera, Transform actorTransform)
        {
            viewCamera = camera;
            actor = actorTransform;
        }

        public Interactable Detect()
        {
            if (viewCamera == null || actor == null) return null;
            RaycastHit hit;
            Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out hit, rayDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                Interactable aimed = hit.collider.GetComponentInParent<Interactable>();
                if (Valid(aimed)) return aimed;
            }

            int count = Physics.OverlapSphereNonAlloc(actor.position + Vector3.up, nearbyRadius, nearby, ~0,
                QueryTriggerInteraction.Ignore);
            Interactable nearest = null;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                Interactable candidate = nearby[i].GetComponentInParent<Interactable>();
                nearby[i] = null;
                if (!Valid(candidate)) continue;
                float distance = (candidate.transform.position - actor.position).sqrMagnitude;
                if (distance >= nearestDistance) continue;
                nearest = candidate;
                nearestDistance = distance;
            }
            return nearest;
        }

        private bool Valid(Interactable candidate)
        {
            return candidate != null && candidate.CanInteract(actor)
                && Vector3.Distance(actor.position, candidate.transform.position) <= rayDistance;
        }
    }
}
