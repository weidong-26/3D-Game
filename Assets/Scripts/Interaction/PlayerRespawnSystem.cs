using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Interaction
{
    public sealed class PlayerRespawnSystem : MonoBehaviour
    {
        [SerializeField] private float killHeight = -8f;
        private ThirdPersonController controller;
        private Transform defaultSpawn;
        public int RespawnCount { get; private set; }

        public void Initialize(ThirdPersonController playerController, Transform spawn)
        {
            controller = playerController;
            defaultSpawn = spawn;
            if (controller != null) killHeight = controller.RespawnHeight;
        }

        private void Update()
        {
            if (controller != null && controller.transform.position.y < killHeight) ForceRespawn();
        }

        public void ForceRespawn()
        {
            if (controller == null) return;
            controller.RespawnAt(defaultSpawn != null ? defaultSpawn.position : Vector3.zero);
            RespawnCount++;
        }
    }
}
