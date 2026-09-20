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
        private InteractionController interaction;
        private PoseController pose;
        private WakeFlow wake;
        private bool enteredWorld;
        private CharacterCreatorUI creatorUi;
        private GameObject gameplayHud;

        private void Update()
        {
            if (gameplayHud != null && gameplayHud.activeSelf && wake.State == WakeState.FreeControl && Input.GetKeyDown(KeyCode.C))
                OpenCreator();
        }

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
            pose = player.AddComponent<PoseController>();
            pose.Initialize(playerController, appearance.GetComponentInChildren<Animator>());
            foreach (PickupItem item in FindObjectsByType<PickupItem>(FindObjectsSortMode.None))
            {
                PickupItem picked = item;
                item.gameObject.AddComponent<Interactable>().Configure("拾取 " + item.DisplayName, delegate { grabber.Pickup(picked); });
            }
            interaction = player.AddComponent<InteractionController>();
            player.AddComponent<CharacterMotionAnimator>().Initialize(playerController, grabber, appearance.GetComponentInChildren<Animator>());

            AppearanceData data = AppearanceSaveService.LoadOrDefault();
            appearance.Apply(data);
            creatorUi = CharacterCreatorUI.Create(appearance, data, SaveAndEnterWorld);
            Button actionButton;
            Text actionLabel;
            Button useButton;
            Button throwButton;
            Button wakeButton;
            Text wakeLabel;
            Button skipWakeButton;
            gameplayHud = CreateGameplayHud(out actionButton, out actionLabel, out useButton, out throwButton, out wakeButton, out wakeLabel, out skipWakeButton);
            interaction.Initialize(grabber, pose, actionButton, actionLabel, useButton, throwButton);
            wake = player.AddComponent<WakeFlow>();
            wake.Initialize(pose, grabber, interaction, wakeButton, wakeLabel, skipWakeButton, GameObject.Find("Blanket"));
            gameplayHud.SetActive(false);
            EnterCreatorView(cameraObject.transform, player.transform);
        }

        private void EnterCreatorView(Transform cameraTransform, Transform player)
        {
            playerController.enabled = false;
            grabber.enabled = false;
            interaction.enabled = false;
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
            interaction.enabled = true;
            cameraController.enabled = true;
            previewCamera.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (!enteredWorld)
            {
                enteredWorld = true;
                wake.Begin();
            }
        }

        private void OpenCreator()
        {
            gameplayHud.SetActive(false);
            creatorUi.Show();
            EnterCreatorView(Camera.main.transform, playerController.transform);
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

        private static GameObject CreateGameplayHud(out Button actionButton, out Text actionLabel, out Button useButton, out Button throwButton,
            out Button wakeButton, out Text wakeLabel, out Button skipWakeButton)
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
            text.text = "WASD 移动  |  Shift 奔跑  |  Space 跳跃  |  R 重生  |  C 捏脸\n按住鼠标右键拖动镜头  |  点击屏幕按钮交互";
            text.font = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 18);

            actionButton = CreateHudButton(canvasObject.transform, "InteractionButton", "靠近物品或家具", 38f, out actionLabel);
            Text useLabel;
            useButton = CreateHudButton(canvasObject.transform, "UseButton", "开关手电", 88f, out useLabel);
            Text throwLabel;
            throwButton = CreateHudButton(canvasObject.transform, "ThrowButton", "投掷", 138f, out throwLabel);
            wakeButton = CreateHudButton(canvasObject.transform, "WakeButton", "掀开被子", 150f, out wakeLabel);
            Text skipLabel;
            skipWakeButton = CreateHudButton(canvasObject.transform, "SkipWakeButton", "跳过起床", 96f, out skipLabel);

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

        private static Button CreateHudButton(Transform parent, string name, string caption, float bottom, out Text label)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, bottom);
            rect.sizeDelta = new Vector2(220f, 42f);
            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.12f, 0.28f, 0.35f, 0.94f);
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            GameObject captionObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            captionObject.transform.SetParent(buttonObject.transform, false);
            RectTransform captionRect = captionObject.GetComponent<RectTransform>();
            captionRect.anchorMin = Vector2.zero;
            captionRect.anchorMax = Vector2.one;
            captionRect.offsetMin = Vector2.zero;
            captionRect.offsetMax = Vector2.zero;
            label = captionObject.GetComponent<Text>();
            label.font = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 20);
            label.fontSize = 20;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = caption;
            return button;
        }
    }
}
