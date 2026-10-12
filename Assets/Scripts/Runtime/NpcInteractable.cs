using LightweightGame.Interaction;
using UnityEngine;

namespace LightweightGame.Runtime
{
    [RequireComponent(typeof(Interactable))]
    public sealed class NpcInteractable : MonoBehaviour
    {
        private string displayName;
        private string greeting;

        public void Configure(string name, string message)
        {
            displayName = name;
            greeting = message;
            GetComponent<Interactable>().Configure("交谈", Talk);
        }

        private void Talk(Transform actor)
        {
            InteractionController interaction = actor.GetComponent<InteractionController>();
            if (interaction != null) interaction.ShowMessage(displayName + "：" + greeting);
        }
    }
}
