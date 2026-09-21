using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LightweightGame.Runtime;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace LightweightGame.Interaction
{
    public sealed class EnvironmentPhase2Smoke : MonoBehaviour
    {
        private static bool Has(string value) { return Array.IndexOf(Environment.GetCommandLineArgs(), value) >= 0; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (!Has("-phase2-smoke") && !Has("-phase2-capture")) return;
            if (FindFirstObjectByType<EnvironmentPhase2Smoke>() != null) return;
            GameObject runner = new GameObject("EnvironmentPhase2Smoke");
            DontDestroyOnLoad(runner);
            runner.AddComponent<EnvironmentPhase2Smoke>();
        }

        private IEnumerator Start()
        {
            yield return null;
            if (Has("-phase2-capture")) yield return CaptureAll();
            else yield return SmokeAll();
        }

        private IEnumerator SmokeAll()
        {
            string failure = null;
            string[] scenes = { "School_Environment_V1", "Park_Environment_V1", "Urban_Environment_V1" };
            foreach (string scene in scenes)
            {
                yield return LoadEnvironment(scene);
                try { CheckScene(scene); }
                catch (Exception exception) { failure = scene + ": " + exception; break; }
                Debug.Log("PHASE2_SCENE_PASS " + scene);
            }
            if (failure == null)
            {
                SceneManager.LoadScene("Environment_Select_V2");
                yield return null;
                yield return null;
                if (GameObject.Find("EnvironmentSelectionCanvas") == null || FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None).Length < 4)
                    failure = "Environment selector did not create four map buttons";
                else Debug.Log("PHASE2_SELECTOR_PASS");
            }
            if (failure == null) Debug.Log("PHASE2_SMOKE_PASS");
            else Debug.LogError("PHASE2_SMOKE_FAIL " + failure);
            Application.Quit(failure == null ? 0 : 1);
        }

        private static void CheckScene(string scene)
        {
            if (GraphicsSettings.currentRenderPipeline == null) throw new Exception("URP inactive");
            ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
            if (player == null || Camera.main == null) throw new Exception("player or camera missing");
            if (GameObject.Find("ReturnToMapButton") == null) throw new Exception("return-to-map button missing");
            if (GameObject.Find("PlayerSpawn") == null) throw new Exception("player spawn missing");
            if (player.transform.position.y < -1f) throw new Exception("player fell below environment");
            if (scene == "School_Environment_V1")
            {
                Required("ClassroomFloor", "ClassroomCeiling", "SchoolCorridorFloor", "ClassroomDoor", "ClassroomBlackboard", "StudentDesk_12", "StudentChair_12");
                if (FindObjectsByType<HomeInteractable>(FindObjectsSortMode.None).Length < 2) throw new Exception("school interaction missing");
                Grounded(new Vector3(0, 0, -3.2f), new Vector3(4.1f, 0, 0.7f), new Vector3(0, 0, 6f));
            }
            else if (scene == "Park_Environment_V1")
            {
                Required("ParkSafeGround", "ParkMainPath", "ParkCrossPath", "CentralPlaza", "PondWater", "PicnicTable", "ParkTree_12", "ParkBench_8", "ParkLamp_10");
                if (FindObjectsByType<HomeInteractable>(FindObjectsSortMode.None).Length < 1) throw new Exception("park seating interaction missing");
                Grounded(new Vector3(0, 0, -22f), new Vector3(0, 0, 0), new Vector3(20f, 0, 0));
            }
            else
            {
                Required("UrbanSafeGround", "UrbanRoadNorthSouth", "UrbanRoadEastWest", "WestSidewalk", "EastSidewalk", "UrbanApartmentFacade", "UrbanFactoryFacade", "UrbanStreetLamp_12", "FireHydrant");
                Grounded(new Vector3(-3.2f, 0, -18f), new Vector3(0, 0, 0), new Vector3(9.5f, 0, 20f));
            }
            foreach (string ground in scene == "School_Environment_V1"
                ? new[] { "ClassroomFloor", "SchoolCorridorFloor", "SchoolSafeGround" }
                : scene == "Park_Environment_V1" ? new[] { "ParkSafeGround", "ParkMainPath", "CentralPlaza" }
                : new[] { "UrbanSafeGround", "UrbanRoadNorthSouth", "WestSidewalk" })
                if (GameObject.Find(ground).GetComponent<Collider>() == null) throw new Exception("missing collider: " + ground);
            Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            if (renderers.Length < 40) throw new Exception("environment is visually under-populated: " + renderers.Length);
        }

        private static void Required(params string[] names)
        {
            foreach (string name in names) if (GameObject.Find(name) == null) throw new Exception("missing object: " + name);
        }

        private static void Grounded(params Vector3[] points)
        {
            foreach (Vector3 point in points)
                if (!Physics.Raycast(point + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 12f, ~0, QueryTriggerInteraction.Ignore)
                    || hit.point.y < -0.6f)
                    throw new Exception("walkable ground missing at " + point);
        }

        private IEnumerator CaptureAll()
        {
            Screen.SetResolution(1280, 720, false);
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Deliverables", "EnvironmentPhase2", "Screenshots");
            Directory.CreateDirectory(folder);
            var reports = new List<string>();
            yield return CaptureScene("School_Environment_V1", folder, reports,
                new View("01_SchoolClassroom.png", new Vector3(6.1f, 2.4f, 1.5f), new Vector3(0, 1.2f, 8.2f)),
                new View("02_SchoolDesks.png", new Vector3(-5.8f, 1.9f, 3.0f), new Vector3(1.2f, 0.8f, 8.0f)),
                new View("03_SchoolCorridor.png", new Vector3(-10f, 1.8f, -2.3f), new Vector3(8f, 1.4f, -2.4f)));
            yield return CaptureScene("Park_Environment_V1", folder, reports,
                new View("04_ParkOverview.png", new Vector3(26f, 14f, -28f), new Vector3(0, 0, 0)),
                new View("05_ParkPlaza.png", new Vector3(10f, 3.2f, -10f), new Vector3(0, 0.8f, 0)),
                new View("06_ParkPond.png", new Vector3(-4f, 3.2f, 5f), new Vector3(-15f, 0.3f, 14f)));
            yield return CaptureScene("Urban_Environment_V1", folder, reports,
                new View("07_UrbanIntersection.png", new Vector3(0f, 17f, -25f), new Vector3(0, 1f, 0)),
                new View("08_UrbanStreet.png", new Vector3(-4.5f, 2.5f, -32f), new Vector3(0, 2f, 18f)),
                new View("09_UrbanFacades.png", new Vector3(25f, 5.5f, -5f), new Vector3(25f, 8f, 14f)));
            SceneManager.LoadScene("Environment_Select_V2");
            yield return null;
            yield return new WaitForSeconds(0.5f);
            yield return Shot(Camera.main, folder, new View("10_MapSelection.png", Vector3.zero, Vector3.forward));
            string reportPath = Path.Combine(Directory.GetParent(folder).FullName, "performance.txt");
            File.WriteAllLines(reportPath, reports);
            Debug.Log("PHASE2_CAPTURE_DONE " + folder);
            Application.Quit(0);
        }

        private IEnumerator CaptureScene(string scene, string folder, List<string> reports, params View[] views)
        {
            yield return LoadEnvironment(scene);
            ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
            player.enabled = false;
            CharacterAppearance avatar = player.GetComponentInChildren<CharacterAppearance>();
            if (avatar != null) foreach (Renderer renderer in avatar.GetComponentsInChildren<Renderer>()) renderer.enabled = false;
            GameObject hud = GameObject.Find("GameplayHUD");
            if (hud != null) hud.SetActive(false);
            Camera camera = Camera.main;
            ThirdPersonCamera orbit = camera.GetComponent<ThirdPersonCamera>();
            if (orbit != null) orbit.enabled = false;
            foreach (View view in views) yield return Shot(camera, folder, view);
            if (avatar != null) foreach (Renderer renderer in avatar.GetComponentsInChildren<Renderer>()) renderer.enabled = true;
            if (hud != null) hud.SetActive(true);
            yield return null;
            ProfilerRecorder draw = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 15);
            ProfilerRecorder triangles = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 15);
            float elapsed = 0;
            for (int i = 0; i < 120; i++) { yield return null; elapsed += Time.unscaledDeltaTime; }
            string line = scene + " | FPS=" + (120f / elapsed).ToString("F1") + " | DrawCalls=" + draw.LastValue + " | Triangles=" + triangles.LastValue;
            reports.Add(line);
            Debug.Log("PHASE2_GAME_VIEW_STATS " + line);
            draw.Dispose(); triangles.Dispose();
        }

        private static IEnumerator LoadEnvironment(string scene)
        {
            PlayerPrefs.SetInt(EnvironmentSelectionUI.SkipCreatorKey, 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(scene);
            yield return null;
            yield return null;
            yield return new WaitForSeconds(0.25f);
        }

        private static IEnumerator Shot(Camera camera, string folder, View view)
        {
            if (view.Position != Vector3.zero || view.Target != Vector3.forward)
            {
                camera.transform.position = view.Position;
                camera.transform.LookAt(view.Target);
            }
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(folder, view.Name));
            yield return new WaitForSeconds(0.45f);
        }

        private readonly struct View
        {
            public readonly string Name;
            public readonly Vector3 Position;
            public readonly Vector3 Target;
            public View(string name, Vector3 position, Vector3 target) { Name = name; Position = position; Target = target; }
        }
    }
}
