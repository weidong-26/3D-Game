using UnityEngine;
using UnityEngine.UI;

namespace LightweightGame.Runtime
{
    public sealed class InteractionController : MonoBehaviour
    {
        private PhysicsGrabber grabber;
        private Button button;
        private Text buttonText;
        private Button useButton;
        private Button throwButton;
        private PoseController pose;
        private Interactable highlighted;

        public void Initialize(PhysicsGrabber pickup, PoseController poseController, Button primary, Text label, Button secondary, Button throwAction)
        {
            grabber = pickup;
            pose = poseController;
            button = primary;
            buttonText = label;
            useButton = secondary;
            throwButton = throwAction;
            button.onClick.AddListener(Click);
            useButton.onClick.AddListener(grabber.UseHeld);
            throwButton.onClick.AddListener(grabber.ThrowHeld);
            Update();
        }

        private void Update()
        {
            if (button == null) return;
            Interactable nearest = grabber.IsHolding || pose.IsPosed ? null : FindNearest();
            if (nearest != highlighted)
            {
                if (highlighted != null) highlighted.Highlight(false);
                highlighted = nearest;
                if (highlighted != null) highlighted.Highlight(true);
            }
            buttonText.text = pose.IsPosed ? "起身" : grabber.IsHolding ? "放下" : nearest != null ? nearest.Label : "靠近物品或家具";
            useButton.gameObject.SetActive(grabber.CanUseHeld);
            throwButton.gameObject.SetActive(grabber.IsHolding);
        }

        private void Click()
        {
            if (pose.IsPosed) pose.Stand();
            else if (grabber.IsHolding) grabber.ReleaseHeld();
            else
            {
                Interactable nearest = FindNearest();
                if (nearest != null) nearest.Activate(transform);
            }
            Update();
        }

        private Interactable FindNearest()
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position + Vector3.up, 2.1f);
            Interactable nearest = null;
            float distance = float.MaxValue;
            foreach (Collider collider in nearby)
            {
                Interactable item = collider.GetComponentInParent<Interactable>();
                if (item == null) continue;
                float candidateDistance = Vector3.Distance(transform.position, item.transform.position);
                if (candidateDistance >= distance) continue;
                nearest = item;
                distance = candidateDistance;
            }
            return nearest;
        }

        private void OnDisable()
        {
            if (highlighted != null) highlighted.Highlight(false);
            highlighted = null;
            if (button != null) button.gameObject.SetActive(false);
            if (useButton != null) useButton.gameObject.SetActive(false);
            if (throwButton != null) throwButton.gameObject.SetActive(false);
        }

        private void OnEnable() { if (button != null) button.gameObject.SetActive(true); }
    }
}
