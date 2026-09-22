using System;
using System.Collections;
using System.IO;
using System.Reflection;
using LightweightGame.Runtime;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace LightweightGame.Interaction
{
    public sealed class EnvironmentPhase3Capture : MonoBehaviour
    {
        private static bool started;
        private string folder;
        private string performance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (started || Array.IndexOf(Environment.GetCommandLineArgs(), "-phase3-capture") < 0) return;
            started = true;
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            GameObject runner = new GameObject("EnvironmentPhase3Capture");
            DontDestroyOnLoad(runner);
            runner.AddComponent<EnvironmentPhase3Capture>();
        }

        private IEnumerator Start()
        {
            Screen.SetResolution(1280, 720, false);
            folder = Path.Combine(Directory.GetCurrentDirectory(), "Deliverables", "EnvironmentPhase3", "Screenshots");
            Directory.CreateDirectory(folder);
            yield return new WaitForSeconds(0.5f);
            yield return Shot("01_MapSelection.png");

            yield return EnterWorld("School_Environment_V1");
            ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
            InteractableDoor schoolDoor = GameObject.Find("ClassroomDoor").GetComponent<InteractableDoor>();
            schoolDoor.GetComponent<Interactable>().Activate(player.transform);
            yield return new WaitForSeconds(0.65f);
            SetCamera(new Vector3(7.2f, 2.1f, -2.8f), new Vector3(4.4f, 1.2f, 0.5f));
            yield return Shot("02_SchoolDoorOpen.png");
            SeatInteractable chair = GameObject.Find("StudentChair_1").GetComponent<SeatInteractable>();
            chair.GetComponent<Interactable>().Activate(player.transform);
            yield return new WaitForSeconds(0.55f);
            SetCamera(new Vector3(-1.7f, 1.65f, 4.5f), new Vector3(-3.7f, 0.9f, 7.0f));
            yield return Shot("03_ClassroomSitting.png");
            player.GetComponent<PoseController>().Stand();
            PickupItem schoolItem = GameObject.Find("Phase3SchoolPickup").GetComponent<PickupItem>();
            PhysicsGrabber schoolGrabber = player.GetComponent<PhysicsGrabber>();
            schoolGrabber.Pickup(schoolItem);
            yield return null;
            SetCamera(new Vector3(-1.0f, 1.7f, 4.6f), new Vector3(-3.7f, 1.0f, 6.3f));
            yield return Shot("04_ClassroomPickup.png");
            PlaceOn("StudentDesk_1", schoolGrabber);
            SetCamera(new Vector3(-1.2f, 1.75f, 8.9f), new Vector3(-3.7f, 0.8f, 7.8f));
            yield return Shot("05_ObjectPlacedOnDesk.png");
            yield return Measure("School_Environment_V1");

            yield return EnterWorld("Home_Environment_V2");
            player = FindFirstObjectByType<ThirdPersonController>();
            InteractableCabinet cabinet = GameObject.Find("DrawerCabinet").GetComponent<InteractableCabinet>();
            cabinet.GetComponent<Interactable>().Activate(player.transform);
            yield return new WaitForSeconds(0.55f);
            SetCamera(new Vector3(-2.5f, 1.7f, 10.5f), new Vector3(-4.3f, 0.9f, 12.3f));
            yield return Shot("06_HomeCabinetOpen.png");
            LieInteractable bed = GameObject.Find("ModernBed").GetComponent<LieInteractable>();
            bed.GetComponent<Interactable>().Activate(player.transform);
            yield return new WaitForSeconds(0.55f);
            SetCamera(new Vector3(-0.2f, 2.8f, 12.8f), new Vector3(-2.6f, 0.9f, 11.8f));
            yield return Shot("07_HomeLyingOnBed.png");
            player.GetComponent<PoseController>().Stand();
            yield return null;
            SetCamera(new Vector3(-1.3f, 2.3f, 4.0f), new Vector3(-4.6f, 1.5f, 6.5f));
            yield return Shot("08_HomeStairs.png");
            yield return Measure("Home_Environment_V2");

            yield return EnterWorld("Park_Environment_V1");
            player = FindFirstObjectByType<ThirdPersonController>();
            GameObject.Find("ParkBench_1").GetComponent<SeatInteractable>().GetComponent<Interactable>().Activate(player.transform);
            yield return new WaitForSeconds(0.55f);
            SetCamera(new Vector3(7.0f, 1.8f, -15f), new Vector3(3.8f, 0.8f, -15f));
            yield return Shot("09_ParkBenchSitting.png");
            player.GetComponent<PoseController>().Stand();
            CharacterController capsule = player.GetComponent<CharacterController>();
            capsule.enabled = false;
            player.transform.position = new Vector3(0f, -20f, 0f);
            capsule.enabled = true;
            yield return null;
            SetCamera(new Vector3(3.5f, 2.2f, -24f), new Vector3(0f, 1.0f, -22f));
            yield return Shot("10_RespawnRecovered.png");
            yield return Measure("Park_Environment_V1");

            yield return EnterWorld("Urban_Environment_V1");
            yield return Measure("Urban_Environment_V1");
            File.WriteAllText(Path.Combine(Directory.GetParent(folder).FullName, "performance.txt"), performance);
            Debug.Log("PHASE3_CAPTURE_PASS " + folder);
            Application.Quit(0);
        }

        private static IEnumerator EnterWorld(string scene)
        {
            PlayerPrefs.DeleteKey(EnvironmentSelectionUI.SkipCreatorKey);
            SceneManager.LoadScene(scene);
            yield return null;
            yield return null;
            GameBootstrap bootstrap = FindFirstObjectByType<GameBootstrap>();
            typeof(GameBootstrap).GetMethod("SaveAndEnterWorld", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(bootstrap, new object[] { AppearanceSaveService.LoadOrDefault() });
            yield return null;
        }

        private static void SetCamera(Vector3 position, Vector3 target)
        {
            Camera camera = Camera.main;
            ThirdPersonCamera orbit = camera.GetComponent<ThirdPersonCamera>();
            if (orbit != null) orbit.enabled = false;
            camera.transform.position = position;
            camera.transform.LookAt(target);
        }

        private static void PlaceOn(string surfaceName, PhysicsGrabber grabber)
        {
            Collider collider = GameObject.Find(surfaceName).GetComponent<Collider>();
            Bounds bounds = collider.bounds;
            Vector3 target = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            SetCamera(target + new Vector3(0f, 2.2f, -2.2f), target);
            Physics.SyncTransforms();
            if (!grabber.PlaceHeld()) throw new InvalidOperationException("Capture placement failed on " + surfaceName);
        }

        private IEnumerator Shot(string name)
        {
            Camera camera = Camera.main;
            RenderTexture target = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            RenderMode[] modes = new RenderMode[canvases.Length];
            for (int i = 0; i < canvases.Length; i++)
            {
                modes[i] = canvases[i].renderMode;
                if (modes[i] == RenderMode.ScreenSpaceOverlay)
                {
                    canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[i].worldCamera = camera;
                    canvases[i].planeDistance = 1f;
                }
            }
            target.Create();
            RenderPipeline.StandardRequest request = new RenderPipeline.StandardRequest
            {
                destination = target,
                mipLevel = 0,
                slice = 0,
                face = CubemapFace.Unknown
            };
            RenderPipeline.SubmitRenderRequest(camera, request);
            yield return null;
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = target;
            Texture2D image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply(false);
            File.WriteAllBytes(Path.Combine(folder, name), image.EncodeToPNG());
            Destroy(image);
            RenderTexture.active = previous;
            target.Release();
            Destroy(target);
            for (int i = 0; i < canvases.Length; i++)
                if (modes[i] == RenderMode.ScreenSpaceOverlay) canvases[i].renderMode = modes[i];
            yield return null;
        }

        private IEnumerator Measure(string scene)
        {
            ProfilerRecorder drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 15);
            ProfilerRecorder triangles = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 15);
            Camera camera = Camera.main;
            const int width = 1280;
            const int height = 720;
            RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            target.Create();
            RenderPipeline.StandardRequest request = new RenderPipeline.StandardRequest
            {
                destination = target,
                mipLevel = 0,
                slice = 0,
                face = CubemapFace.Unknown
            };
            float elapsed = 0f;
            for (int i = 0; i < 120; i++)
            {
                RenderPipeline.SubmitRenderRequest(camera, request);
                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
            performance += scene + " | ExplicitRender1280x720FPS=" + (120f / elapsed).ToString("F1")
                + " | DrawCalls=" + (drawCalls.Valid && drawCalls.Count > 0 ? drawCalls.LastValue.ToString() : "unavailable")
                + " | Triangles=" + (triangles.Valid && triangles.Count > 0 ? triangles.LastValue.ToString() : "unavailable") + "\n";
            drawCalls.Dispose();
            triangles.Dispose();
            target.Release();
            Destroy(target);
        }
    }
}
