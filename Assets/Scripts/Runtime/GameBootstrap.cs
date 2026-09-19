using LightweightGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LightweightGame.Runtime
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private ThirdPersonController playerController;
        private ThirdPersonCamera cameraController;
        private CharacterPreviewCamera previewCamera;
        private PhysicsGrabber grabber;
        private CharacterCreatorUI creatorUi;
        private GameObject gameplayHud;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (FindFirstObjectByType<GameBootstrap>() == null)
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            BuildLighting();
            TestWorldFactory.Create();

            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0f, 0f, -5f);
            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.34f;
            characterController.center = new Vector3(0f, 1f, 0f);
            characterController.slopeLimit = 45f;
            characterController.stepOffset = 0.3f;
            characterController.skinWidth = 0.04f;
            playerController = player.AddComponent<ThirdPersonController>();
            CharacterAppearance appearance = ProceduralAvatarFactory.Create(player);

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(ThirdPersonCamera), typeof(CharacterPreviewCamera));
            cameraObject.tag = "MainCamera";
            Camera mainCamera = cameraObject.GetComponent<Camera>();
            mainCamera.fieldOfView = 55f;
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            cameraController = cameraObject.GetComponent<ThirdPersonCamera>();
            cameraController.SetTarget(player.transform);
            previewCamera = cameraObject.GetComponent<CharacterPreviewCamera>();
            previewCamera.SetTarget(player.transform);
            playerController.SetCamera(cameraObject.transform);
            grabber = player.AddComponent<PhysicsGrabber>();
            grabber.SetCamera(mainCamera);
            grabber.SetHandAnchor(appearance.HandAnchor);
            player.AddComponent<CharacterMotionAnimator>().Initialize(playerController, grabber, appearance.GetComponentInChildren<Animator>());

            AppearanceData data = AppearanceSaveService.LoadOrDefault();
            appearance.Apply(data);
            creatorUi = CharacterCreatorUI.Create(appearance, data, SaveAndEnterWorld);
            Text interactionPrompt;
            gameplayHud = CreateGameplayHud(out interactionPrompt);
            grabber.SetPrompt(interactionPrompt);
            gameplayHud.SetActive(false);
            EnterCreatorView(cameraObject.transform, player.transform);
        }

        private void EnterCreatorView(Transform cameraTransform, Transform player)
        {
            playerController.enabled = false;
            grabber.enabled = false;
            cameraController.enabled = false;
            previewCamera.enabled = true;
            cameraTransform.position = player.position + new Vector3(0f, 1.55f, 4.2f);
            cameraTransform.LookAt(player.position + Vector3.up * 1.4f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void SaveAndEnterWorld(AppearanceData data)
        {
            if (!AppearanceSaveService.Save(data))
                return;

            creatorUi.Hide();
            gameplayHud.SetActive(true);
            playerController.enabled = true;
            grabber.enabled = true;
            cameraController.enabled = true;
            previewCamera.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private static void BuildLighting()
        {
            GameObject lightObject = new GameObject("Sun", typeof(Light));
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.94f, 0.84f);
            light.shadows = LightShadows.Soft;
            lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private static GameObject CreateGameplayHud(out Text interactionPrompt)
        {
            GameObject canvasObject = new GameObject("GameplayHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            GameObject textObject = new GameObject("Controls", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(canvasObject.transform, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(18f, -18f);
            rect.sizeDelta = new Vector2(560f, 80f);
            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 16;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.text = "WASD Move  |  Shift Run  |  Space Jump x2  |  R Respawn\nMouse Look  |  E Pick Up/Put Down  |  F Use  |  Esc Unlock";

            GameObject promptObject = new GameObject("Prompt", typeof(RectTransform), typeof(Text));
            promptObject.transform.SetParent(canvasObject.transform, false);
            RectTransform promptRect = promptObject.GetComponent<RectTransform>();
            promptRect.anchorMin = new Vector2(0.5f, 0.5f);
            promptRect.anchorMax = new Vector2(0.5f, 0.5f);
            promptRect.anchoredPosition = new Vector2(0f, -52f);
            promptRect.sizeDelta = new Vector2(450f, 36f);
            interactionPrompt = promptObject.GetComponent<Text>();
            interactionPrompt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            interactionPrompt.fontSize = 19;
            interactionPrompt.alignment = TextAnchor.MiddleCenter;
            interactionPrompt.color = Color.white;

            GameObject crosshairObject = new GameObject("Crosshair", typeof(RectTransform), typeof(Text));
            crosshairObject.transform.SetParent(canvasObject.transform, false);
            RectTransform crosshairRect = crosshairObject.GetComponent<RectTransform>();
            crosshairRect.anchorMin = new Vector2(0.5f, 0.5f);
            crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
            crosshairRect.sizeDelta = new Vector2(30f, 30f);
            Text crosshair = crosshairObject.GetComponent<Text>();
            crosshair.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            crosshair.fontSize = 22;
            crosshair.alignment = TextAnchor.MiddleCenter;
            crosshair.color = Color.white;
            crosshair.text = "+";
            return canvasObject;
        }
    }
}
