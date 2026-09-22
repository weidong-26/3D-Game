using UnityEngine;

namespace LightweightGame.Interaction
{
    public sealed class PlacementSurface : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float minimumUpDot = 0.65f;
        private readonly Collider[] overlaps = new Collider[32];
        public bool Accepts(Vector3 normal) { return Vector3.Dot(normal.normalized, Vector3.up) >= minimumUpDot; }

        public bool CanPlace(Bounds bounds, Transform itemRoot, Transform actor)
        {
            Vector3 extents = Vector3.Max(bounds.extents * 0.88f, Vector3.one * 0.02f);
            int count = Physics.OverlapBoxNonAlloc(bounds.center, extents, overlaps, Quaternion.identity, ~0,
                QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                Collider candidate = overlaps[i];
                overlaps[i] = null;
                if (candidate == null) continue;
                Transform candidateTransform = candidate.transform;
                if (candidateTransform.IsChildOf(itemRoot) || candidateTransform.IsChildOf(actor)) continue;
                PlacementSurface owner = candidate.GetComponentInParent<PlacementSurface>();
                if (owner == this) continue;
                return false;
            }
            return true;
        }
    }
}
