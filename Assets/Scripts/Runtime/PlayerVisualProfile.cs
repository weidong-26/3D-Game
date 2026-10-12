using UnityEngine;

namespace LightweightGame.Runtime
{
    [CreateAssetMenu(fileName = "PlayerVisualProfile", menuName = "Lightweight Game/Player Visual Profile")]
    public sealed class PlayerVisualProfile : ScriptableObject
    {
        [SerializeField] private GameObject visualPrefab;

        public GameObject VisualPrefab { get { return visualPrefab; } }
    }
}
