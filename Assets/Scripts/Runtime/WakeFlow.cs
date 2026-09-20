using UnityEngine;
using UnityEngine.UI;

namespace LightweightGame.Runtime
{
    public enum WakeState { Sleeping, BlanketOpened, SittingOnBed, Standing, FreeControl }

    public sealed class WakeFlow : MonoBehaviour
    {
        private PoseController pose;
        private PhysicsGrabber grabber;
        private InteractionController interaction;
        private Button actionButton;
        private Button skipButton;
        private Text actionLabel;
        private GameObject blanket;
        private float nextActionTime;
        public WakeState State { get; private set; }

        public void Initialize(PoseController poseController, PhysicsGrabber pickup, InteractionController interactor,
            Button action, Text label, Button skip, GameObject cover)
        {
            pose = poseController;
            grabber = pickup;
            interaction = interactor;
            actionButton = action;
            actionLabel = label;
            skipButton = skip;
            blanket = cover;
            actionButton.onClick.AddListener(Advance);
            skipButton.onClick.AddListener(Skip);
            actionButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
        }

        public void Begin()
        {
            State = WakeState.Sleeping;
            nextActionTime = 0f;
            blanket.SetActive(true);
            grabber.enabled = false;
            interaction.enabled = false;
            pose.Enter(PoseKind.Lying, new Vector3(-1.45f, 0f, 7.25f), Quaternion.Euler(0f, -90f, 0f), new Vector3(-0.95f, 0f, 6.15f));
            actionLabel.text = "掀开被子";
            actionButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(true);
        }

        private void Advance()
        {
            if (Time.time < nextActionTime) return;
            nextActionTime = Time.time + 0.55f;
            switch (State)
            {
                case WakeState.Sleeping:
                    blanket.SetActive(false);
                    State = WakeState.BlanketOpened;
                    actionLabel.text = "坐起来";
                    break;
                case WakeState.BlanketOpened:
                    State = WakeState.SittingOnBed;
                    pose.transform.position = new Vector3(-1.45f, 0.6f, 7.25f);
                    pose.Change(PoseKind.Sitting);
                    actionLabel.text = "下床";
                    break;
                case WakeState.SittingOnBed:
                    State = WakeState.Standing;
                    pose.Stand(false);
                    actionButton.gameObject.SetActive(false);
                    skipButton.gameObject.SetActive(false);
                    break;
            }
        }

        private void Update()
        {
            if (State == WakeState.Standing && Time.time >= nextActionTime)
                Finish();
        }

        private void Skip()
        {
            if (State == WakeState.FreeControl) return;
            blanket.SetActive(false);
            pose.Stand();
            Finish();
        }

        private void Finish()
        {
            State = WakeState.FreeControl;
            actionButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            grabber.enabled = true;
            interaction.enabled = true;
            pose.ResumeMovement();
        }
    }
}
