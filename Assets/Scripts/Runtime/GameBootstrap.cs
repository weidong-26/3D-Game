using LightweightGame.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LightweightGame.Interaction;

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
        private bool environmentHome;
        private bool authoredEnvironment;
        private CharacterCreatorUI creatorUi;
        private GameObject gameplayHud;

        private void Update()
        {
            if (gameplayHud != null && gameplayHud.activeSelf && wake.State == WakeState.FreeControl && Input.GetKeyDown(KeyCode.C))
                OpenCreator();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Environment_Select_V2") return;
            if (FindFirstObjectByType<GameBootstrap>() == null)
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            environmentHome = sceneName == "Home_Environment_V2";
            authoredEnvironment = environmentHome || sceneName == "School_Environment_V1"
                || sceneName == "Park_Environment_V1" || sceneName == "Urban_Environment_V1";
            if (!authoredEnvironment)
            {
                BuildLighting();
                TestWorldFactory.Create();
            }

            GameObject player = new GameObject("Player");
            Transform homeSpawn = authoredEnvironment
                ? GameObject.Find(environmentHome ? "HomeSpawn" : "PlayerSpawn")?.transform
                : null;
            if (homeSpawn != null && homeSpawn.GetComponent<DefaultSpawnPoint>() == null)
                homeSpawn.gameObject.AddComponent<DefaultSpawnPoint>();
            player.transform.position = homeSpawn != null ? homeSpawn.position : new Vector3(0f, 0f, -5f);
            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.34f;
            characterController.center = new Vector3(0f, 1f, 0f);
            characterController.slopeLimit = 45f;
            characterController.stepOffset = 0.3f;
            characterController.skinWidth = 0.04f;
            playerController = player.AddComponent<ThirdPersonController>();
            PlayerVisualProfile visualProfile = Resources.Load<PlayerVisualProfile>("CharacterModels/PlayerVisualProfile");
            CharacterAppearance appearance = ProceduralAvatarFactory.Create(player, visualProfile != null ? visualProfile.VisualPrefab : null);
            CharacterVisualRig characterVisual = player.GetComponent<CharacterVisualRig>();

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(ThirdPersonCamera), typeof(CharacterPreviewCamera));
            cameraObject.tag = "MainCamera";
            Camera mainCamera = cameraObject.GetComponent<Camera>();
            mainCamera.fieldOfView = 55f;
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            cameraController = cameraObject.GetComponent<ThirdPersonCamera>();
            cameraController.SetTarget(characterVisual.CameraTarget);
            previewCamera = cameraObject.GetComponent<CharacterPreviewCamera>();
            previewCamera.SetTarget(characterVisual.CameraTarget);
            playerController.SetCamera(cameraObject.transform);
            grabber = player.AddComponent<PhysicsGrabber>();
            grabber.SetCamera(mainCamera);
            grabber.SetHandAnchor(characterVisual.RightHandAnchor);
            pose = player.AddComponent<PoseController>();
            pose.Initialize(playerController, characterVisual);
            foreach (PickupItem item in FindObjectsByType<PickupItem>(FindObjectsSortMode.None))
            {
                if (item.GetComponent<Interactable>() == null) item.gameObject.AddComponent<Interactable>();
                PickupInteractable pickup = item.GetComponent<PickupInteractable>();
                if (pickup == null) pickup = item.gameObject.AddComponent<PickupInteractable>();
                pickup.Bind(grabber);
            }
            interaction = player.AddComponent<InteractionController>();
            PlayerRespawnSystem respawn = player.AddComponent<PlayerRespawnSystem>();
            respawn.Initialize(playerController, homeSpawn);
            player.AddComponent<CharacterMotionAnimator>().Initialize(playerController, grabber, characterVisual.Animator);

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
            if (authoredEnvironment) EnvironmentNavigation.Attach(gameplayHud);
            interaction.Initialize(grabber, pose, mainCamera, actionButton, actionLabel, useButton, throwButton);
            wake = player.AddComponent<WakeFlow>();
            wake.Initialize(pose, grabber, interaction, wakeButton, wakeLabel, skipWakeButton, GameObject.Find("Blanket"));
            gameplayHud.SetActive(false);
            EnterCreatorView(cameraObject.transform, player.transform);
            if (authoredEnvironment && PlayerPrefs.GetInt(EnvironmentSelectionUI.SkipCreatorKey, 0) == 1)
            {
                PlayerPrefs.DeleteKey(EnvironmentSelectionUI.SkipCreatorKey);
                SaveAndEnterWorld(data);
            }
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
                if (authoredEnvironment) SpawnEnvironmentNpcs(SceneManager.GetActiveScene().name);
                else wake.Begin();
            }
        }

        private static void SpawnEnvironmentNpcs(string sceneName)
        {
            if (sceneName == "School_Environment_V1")
            {
                ConfigureDoorPrompt("ClassroomDoor", "打开教学楼入口", "关闭教学楼入口");
                SpawnNpcNear("StudentChair_1", new Vector3(1.1f, 0f, 0.7f), "学生", "下节课去操场吧。", 2);
                SpawnNpcNear("TeacherDesk", new Vector3(1.5f, 0f, 0.8f), "老师", "教学楼入口在前面。", 1);
            }
            else if (sceneName == "Park_Environment_V1")
            {
                SpawnNpcNear("ParkBench_1", new Vector3(0.9f, 0f, 1.1f), "晨练居民", "沿着步道走一圈吧。", 0);
                SpawnNpcNear("CentralPlaza", new Vector3(2f, 0f, -1.5f), "公园游客", "这里可以坐下休息。", 1);
            }
            else if (sceneName == "Home_Environment_V2")
            {
                SpawnNpcNear("FrontDoor", new Vector3(1.6f, 0f, 1.4f), "邻居", "今天的天气不错。", 0);
            }
            else if (sceneName == "Urban_Environment_V1")
            {
                ConfigureDoorPrompt("UrbanExampleDoor", "打开商店门", "关闭商店门");
                SpawnNpcNear("UrbanExampleDoor", new Vector3(1.6f, 0f, 2f), "店员", "欢迎光临，需要什么？", 1);
            }
        }

        private static void ConfigureDoorPrompt(string objectName, string openPrompt, string closePrompt)
        {
            GameObject doorObject = GameObject.Find(objectName);
            if (doorObject == null) return;
            InteractableDoor door = doorObject.GetComponent<InteractableDoor>();
            if (door != null) door.ConfigurePromptLabels(openPrompt, closePrompt);
        }

        private static void SpawnNpcNear(string anchorName, Vector3 localOffset, string displayName, string greeting, int outfit)
        {
            GameObject anchor = GameObject.Find(anchorName);
            if (anchor == null || GameObject.Find("NPC_" + displayName) != null) return;

            GameObject npc = new GameObject("NPC_" + displayName);
            npc.transform.SetPositionAndRotation(anchor.transform.TransformPoint(localOffset), anchor.transform.rotation);
            CharacterController capsule = npc.AddComponent<CharacterController>();
            capsule.height = 2f;
            capsule.radius = 0.34f;
            capsule.center = new Vector3(0f, 1f, 0f);
            CharacterAppearance appearance = ProceduralAvatarFactory.Create(npc);
            AppearanceData appearanceData = AppearanceData.CreateDefault();
            appearanceData.Outfit = outfit;
            appearance.Apply(appearanceData);
            npc.AddComponent<Interactable>();
            npc.AddComponent<NpcInteractable>().Configure(displayName, greeting);
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
