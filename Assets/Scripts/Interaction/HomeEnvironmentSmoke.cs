using System;
using System.Collections;
using System.IO;
using System.Reflection;
using LightweightGame.Runtime;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LightweightGame.Interaction
{
    public sealed class HomeEnvironmentSmoke : MonoBehaviour
    {
        private static bool Has(string value) { return Array.IndexOf(Environment.GetCommandLineArgs(), value) >= 0; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (Has("-home-smoke") || Has("-home-capture"))
                new GameObject("HomeEnvironmentSmoke").AddComponent<HomeEnvironmentSmoke>();
        }

        private IEnumerator Start()
        {
            yield return null;
            if (SceneManager.GetActiveScene().name != "Home_Environment_V2")
            {
                Debug.LogError("HOME_SMOKE_FAIL wrong startup scene");
                Application.Quit(1);
                yield break;
            }
            GameBootstrap bootstrap = FindFirstObjectByType<GameBootstrap>();
            typeof(GameBootstrap).GetMethod("SaveAndEnterWorld", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(bootstrap, new object[] { AppearanceSaveService.LoadOrDefault() });
            yield return null;
            if (Has("-home-capture"))
            {
                yield return CaptureViews();
                Application.Quit(0);
                yield break;
            }
            string error = null;
            try { BasicChecks(); }
            catch (Exception exception) { error = exception.ToString(); }
            if (error == null)
            {
                yield return StairCheck();
                if (!stairPassed) error = "Stair traversal failed: " + stairDetails;
            }
            if (error != null) Debug.LogError("HOME_SMOKE_FAIL " + error);
            else Debug.Log("HOME_SMOKE_PASS");
            Application.Quit(error == null ? 0 : 1);
        }

        private void BasicChecks()
        {
            if (GraphicsSettings.currentRenderPipeline == null) throw new Exception("URP not active");
            if (GameObject.Find("NeighborhoodPrototype") != null) throw new Exception("Old prototype spawned in new scene");
            foreach (string name in new[] { "GroundFloor", "SecondFloorMain", "SecondFloorRearLanding", "ContinuousStairCollider", "FlatRoofAndCeiling" })
            {
                GameObject part = GameObject.Find(name);
                if (part == null || part.GetComponent<Collider>() == null) throw new Exception("Missing structure collider: " + name);
            }
            for (int i = 1; i <= 14; i++)
                if (GameObject.Find("OakStair_" + i) == null) throw new Exception("Missing stair " + i);
            if (!Cursor.visible || Cursor.lockState != CursorLockMode.None) throw new Exception("Mouse cursor unavailable");
            if (FindButton("InteractionButton") == null) throw new Exception("Mouse interaction button missing");

            ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
            PoseController pose = player.GetComponent<PoseController>();
            PhysicsGrabber grabber = player.GetComponent<PhysicsGrabber>();
            Button action = FindButton("InteractionButton");
            InteractableDoor door = GameObject.Find("FrontDoor").GetComponent<InteractableDoor>();
            Teleport(player, new Vector3(0, 0, 0.65f));
            door.GetComponent<Interactable>().Activate(player.transform);
            if (!door.IsOpen) throw new Exception("Mouse button failed to open door");
            typeof(InteractableDoor).GetField("nextUseTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(door, 0f);
            door.GetComponent<Interactable>().Activate(player.transform);
            if (door.IsOpen) throw new Exception("Mouse button failed to close door");
            Debug.Log("HOME_DOOR_INTERACTION_PASS");

            InteractableCabinet drawer = GameObject.Find("DrawerCabinet").GetComponent<InteractableCabinet>();
            Teleport(player, new Vector3(-4.1f, 0, 12.4f));
            drawer.GetComponent<Interactable>().Activate(player.transform);
            if (!drawer.IsOpen) throw new Exception("Mouse button failed to open drawer");
            typeof(InteractableCabinet).GetField("nextUseTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(drawer, 0f);
            drawer.GetComponent<Interactable>().Activate(player.transform);
            if (drawer.IsOpen) throw new Exception("Mouse button failed to close drawer");
            Debug.Log("HOME_DRAWER_INTERACTION_PASS");

            Teleport(player, new Vector3(4.5f, 0, 7.3f));
            action.onClick.Invoke();
            if (!pose.IsPosed || player.enabled) throw new Exception("Chair interaction failed");
            action.onClick.Invoke();
            if (pose.IsPosed || !player.enabled) throw new Exception("Chair stand-up failed");
            Teleport(player, new Vector3(-2.55f, 0, 10f));
            action.onClick.Invoke();
            if (!pose.IsPosed || player.enabled) throw new Exception("Bed interaction failed");
            action.onClick.Invoke();
            if (pose.IsPosed || !player.enabled) throw new Exception("Bed stand-up failed");
            Debug.Log("HOME_SEAT_BED_INTERACTION_PASS");

            PickupItem item = GameObject.Find("CarryBox").GetComponent<PickupItem>();
            Vector3 scale = item.transform.lossyScale;
            Transform parent = item.transform.parent;
            Rigidbody body = item.GetComponent<Rigidbody>();
            Collider boxCollider = item.GetComponent<Collider>();
            Teleport(player, new Vector3(0.9f, 0, 3.25f));
            for (int cycle = 0; cycle < 10; cycle++)
            {
                grabber.Pickup(item);
                if (!grabber.IsHolding || Vector3.Distance(item.transform.lossyScale, scale) > 0.0001f)
                    throw new Exception("Crate size changed while held at cycle " + cycle);
                grabber.ReleaseHeld();
                if (grabber.IsHolding || item.transform.parent != parent || Vector3.Distance(item.transform.lossyScale, scale) > 0.0001f
                    || body.isKinematic || !body.useGravity || !boxCollider.enabled)
                    throw new Exception("Crate state changed after drop at cycle " + cycle);
            }
            Debug.Log("HOME_BOX_10_CYCLES_PASS");
            if (Camera.main == null || !Camera.main.GetComponent<ThirdPersonCamera>().enabled)
                throw new Exception("Third-person camera unavailable");
            ThirdPersonCamera orbit = Camera.main.GetComponent<ThirdPersonCamera>();
            MethodInfo applyMouse = typeof(ThirdPersonCamera).GetMethod("ApplyMouseDelta", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo pitch = typeof(ThirdPersonCamera).GetField("pitch", BindingFlags.Instance | BindingFlags.NonPublic);
            applyMouse.Invoke(orbit, new object[] { 0f, 100f });
            float up = (float)pitch.GetValue(orbit);
            applyMouse.Invoke(orbit, new object[] { 0f, -100f });
            float down = (float)pitch.GetValue(orbit);
            if (up > -55f || down < 65f) throw new Exception("Camera cannot look up and down");
            Teleport(player, new Vector3(5.1f, 0, 6f));
            player.enabled = false;
            player.GetComponent<CharacterController>().Move(Vector3.right * 2f);
            if (player.transform.position.x > 5.85f) throw new Exception("Character passed through exterior wall");
            Debug.Log("HOME_WALL_CAMERA_PASS");
            Debug.Log("HOME_BASIC_CHECKS_PASS");
        }

        private bool stairPassed;
        private string stairDetails;
        private IEnumerator StairCheck()
        {
            ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
            player.enabled = false;
            CharacterController capsule = player.GetComponent<CharacterController>();
            Teleport(player, new Vector3(-4.6f, 0.08f, 3.22f));
            for (int i = 0; i < 135; i++)
            {
                capsule.Move(new Vector3(0, -0.035f, 0.045f));
                yield return null;
            }
            float topY = player.transform.position.y;
            float topZ = player.transform.position.z;
            for (int i = 0; i < 135; i++)
            {
                capsule.Move(new Vector3(0, -0.035f, -0.045f));
                yield return null;
            }
            float bottomY = player.transform.position.y;
            stairPassed = topY > 2.45f && topZ > 8.75f && bottomY < 0.45f;
            stairDetails = "topY=" + topY.ToString("F2") + " topZ=" + topZ.ToString("F2") + " bottomY=" + bottomY.ToString("F2");
            Debug.Log(stairPassed ? "HOME_STAIR_UP_DOWN_PASS " + stairDetails : "HOME_STAIR_UP_DOWN_FAIL " + stairDetails);
        }

        private static void Teleport(ThirdPersonController player, Vector3 point)
        {
            CharacterController capsule = player.GetComponent<CharacterController>();
            capsule.enabled = false;
            player.transform.position = point;
            capsule.enabled = true;
            Physics.SyncTransforms();
        }

        private static Button FindButton(string name)
        {
            foreach (Button candidate in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (candidate.name == name) return candidate;
            return null;
        }

        private IEnumerator CaptureViews()
        {
            Screen.SetResolution(1280, 720, false);
            ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
            player.enabled = false;
            CharacterAppearance avatar = player.GetComponentInChildren<CharacterAppearance>();
            if (avatar != null)
                foreach (Renderer renderer in avatar.GetComponentsInChildren<Renderer>()) renderer.enabled = false;
            GameObject hud = GameObject.Find("GameplayHUD");
            if (hud != null) hud.SetActive(false);
            Camera camera = Camera.main;
            camera.GetComponent<ThirdPersonCamera>().enabled = false;
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "Deliverables", "EnvironmentPhase1", "Screenshots");
            Directory.CreateDirectory(directory);
            yield return Shot(camera, directory, "01_Exterior.png", new Vector3(13, 7.6f, -8.5f), new Vector3(0, 2.5f, 7));
            yield return Shot(camera, directory, "02_LivingRoom.png", new Vector3(0.1f, 2.0f, 3.4f), new Vector3(3.3f, 1.0f, 6.5f));
            yield return Shot(camera, directory, "03_Stairs.png", new Vector3(-1.5f, 2.25f, 4.1f), new Vector3(-4.6f, 1.5f, 6.4f));
            yield return Shot(camera, directory, "04_SecondFloor.png", new Vector3(4.4f, 4.7f, 4.9f), new Vector3(0.8f, 3.5f, 10.5f));
            yield return Shot(camera, directory, "05_BedAndFurniture.png", new Vector3(-0.7f, 1.8f, 9.7f), new Vector3(-2.6f, 0.7f, 11.9f));
            Teleport(player, new Vector3(0, 0, 0.6f));
            if (hud != null) hud.SetActive(true);
            yield return null;
            yield return Shot(camera, directory, "06_DoorInteraction.png", new Vector3(1.5f, 1.8f, -0.2f), new Vector3(0, 1.2f, 2));
            Teleport(player, new Vector3(-4.1f, 0, 12.4f));
            GameObject.Find("DrawerCabinet").GetComponent<Interactable>().Activate(player.transform);
            yield return new WaitForSeconds(0.4f);
            yield return Shot(camera, directory, "07_DrawerInteraction.png", new Vector3(-2.3f, 1.8f, 10.4f), new Vector3(-4.9f, 1.1f, 12.8f));
            if (avatar != null)
                foreach (Renderer renderer in avatar.GetComponentsInChildren<Renderer>()) renderer.enabled = true;
            yield return null;
            ProfilerRecorder drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 15);
            ProfilerRecorder triangles = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 15);
            float elapsed = 0f;
            for (int i = 0; i < 120; i++)
            {
                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
            string report = "Scene: Home_Environment_V2\nResolution: " + Screen.width + "x" + Screen.height
                + "\nGPU: " + SystemInfo.graphicsDeviceName
                + "\nAverage FPS over 120 Game frames: " + (120f / elapsed).ToString("F1")
                + "\nDraw Calls: " + (drawCalls.Valid && drawCalls.Count > 0 ? drawCalls.LastValue.ToString() : "unavailable")
                + "\nTriangles: " + (triangles.Valid && triangles.Count > 0 ? triangles.LastValue.ToString() : "unavailable") + "\n";
            File.WriteAllText(Path.Combine(Directory.GetParent(directory).FullName, "performance.txt"), report);
            Debug.Log("HOME_GAME_VIEW_STATS " + report.Replace('\n', ' '));
            drawCalls.Dispose();
            triangles.Dispose();
            Debug.Log("HOME_CAPTURE_DONE " + directory);
        }

        private static IEnumerator Shot(Camera camera, string folder, string name, Vector3 position, Vector3 target)
        {
            camera.transform.position = position;
            camera.transform.LookAt(target);
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(folder, name));
            yield return new WaitForSeconds(0.45f);
        }
    }
}
