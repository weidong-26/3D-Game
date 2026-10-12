using UnityEngine;
using UnityEngine.UI;
using LightweightGame.Interaction;

namespace LightweightGame.Runtime
{
    public sealed class InteractionController : MonoBehaviour
    {
        private PhysicsGrabber grabber;
        private Button button;
        private Button useButton;
        private Button throwButton;
        private PoseController pose;
        private CharacterMotionAnimator motionAnimator;
        private Interactable highlighted;
        private InteractionDetector detector;
        private InteractionPromptUI prompt;

        public void Initialize(PhysicsGrabber pickup, PoseController poseController, Camera viewCamera,
            Button primary, Text label, Button secondary, Button throwAction)
        {
            grabber = pickup;
            pose = poseController;
            motionAnimator = GetComponent<CharacterMotionAnimator>();
            button = primary;
            useButton = secondary;
            throwButton = throwAction;
            detector = gameObject.AddComponent<InteractionDetector>();
            detector.Initialize(viewCamera, transform);
            prompt = primary.gameObject.AddComponent<InteractionPromptUI>();
            prompt.Initialize(primary, label);
            button.onClick.AddListener(Click);
            useButton.onClick.AddListener(grabber.UseHeld);
            throwButton.onClick.AddListener(grabber.ThrowHeld);
            Update();
        }

        private void Update()
        {
            if (button == null) return;
            if (prompt != null) prompt.UpdateMessage();
            if (Input.GetKeyDown(KeyCode.E)) Click();
            if (Input.GetKeyDown(KeyCode.F) && grabber.CanUseHeld) grabber.UseHeld();
            Interactable nearest = grabber.IsHolding || pose.IsPosed ? null : detector.Detect();
            if (nearest != highlighted)
            {
                if (highlighted != null) highlighted.Highlight(false);
                highlighted = nearest;
                if (highlighted != null) highlighted.Highlight(true);
            }
            string label = pose.IsPosed ? "按 E 起身" : grabber.IsHolding ? "按 E 放置" : nearest != null ? "按 E " + nearest.Label : string.Empty;
            prompt.SetPrompt(label, pose.IsPosed || grabber.IsHolding || nearest != null);
            useButton.gameObject.SetActive(grabber.CanUseHeld);
            throwButton.gameObject.SetActive(grabber.IsHolding);
        }

        private void Click()
        {
            if (pose.IsPosed) pose.Stand();
            else if (grabber.IsHolding) grabber.PlaceHeld();
            else
            {
                Interactable nearest = detector.Detect();
                if (nearest != null)
                {
                    nearest.Activate(transform);
                    if (motionAnimator != null) motionAnimator.PlayInteraction();
                }
            }
            Update();
        }

        public void ShowMessage(string value)
        {
            if (prompt != null) prompt.ShowMessage(value);
        }

        private void OnDisable()
        {
            if (highlighted != null) highlighted.Highlight(false);
            highlighted = null;
            if (button != null) button.gameObject.SetActive(false);
            if (useButton != null) useButton.gameObject.SetActive(false);
            if (throwButton != null) throwButton.gameObject.SetActive(false);
            if (prompt != null) prompt.SetPrompt(string.Empty, false);
            if (prompt != null) prompt.ClearMessage();
        }

        private void OnEnable() { if (button != null) button.gameObject.SetActive(true); }
    }
}
