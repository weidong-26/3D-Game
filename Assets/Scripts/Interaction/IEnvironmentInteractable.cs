using UnityEngine;

namespace LightweightGame.Interaction
{
    public interface IEnvironmentInteractable
    {
        string InteractionLabel { get; }
        bool CanInteract(Transform actor);
        void Interact(Transform actor);
        void SetHighlighted(bool enabled);
    }
}
