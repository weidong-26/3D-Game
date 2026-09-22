using System;
using System.Collections;
using System.Reflection;
using LightweightGame.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LightweightGame.Interaction
{
    public sealed class EnvironmentPhase3Smoke : MonoBehaviour
    {
        private static bool started;
        private static bool HasFlag(string value) { return Array.IndexOf(Environment.GetCommandLineArgs(), value) >= 0; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (started || !HasFlag("-phase3-smoke")) return;
            started = true;
            Application.runInBackground = true;
            GameObject runner = new GameObject("EnvironmentPhase3Smoke");
            DontDestroyOnLoad(runner);
            runner.AddComponent<EnvironmentPhase3Smoke>();
        }

        private IEnumerator Start()
        {
            string failure = null;
            PlayerPrefs.DeleteKey(EnvironmentSelectionUI.SkipCreatorKey);
            yield return null;
            try
            {
                if (SceneManager.GetActiveScene().name != "Environment_Select_V2")
                    throw new Exception("Smoke must start from Environment_Select_V2");
                if (FindObjectsByType<Button>(FindObjectsSortMode.None).Length < 4)
                    throw new Exception("Map selection cards missing");
                if (EventSystem.current == null)
                    throw new Exception("Map selection EventSystem missing");
            }
            catch (Exception exception) { failure = exception.ToString(); }

            if (failure == null) yield return CheckSchool(value => failure = value);
            if (failure == null) yield return CheckHome(value => failure = value);
            if (failure == null) yield return CheckPark(value => failure = value);
            if (failure == null) yield return CheckUrban(value => failure = value);
            if (failure == null)
            {
                SceneManager.LoadScene("Environment_Select_V2");
                yield return null;
                if (SceneManager.GetActiveScene().name != "Environment_Select_V2") failure = "Return to map failed";
                else Debug.Log("PHASE3_SCENE_SWITCH_PASS");
            }

            if (failure == null) Debug.Log("PHASE3_INTERACTION_PASS");
            else Debug.LogError("PHASE3_INTERACTION_FAIL " + failure);
            Application.Quit(failure == null ? 0 : 1);
        }

        private IEnumerator CheckSchool(Action<string> fail)
        {
            string error = null;
            yield return EnterWorld("School_Environment_V1", value => error = value);
            if (error != null) { fail(error); yield break; }
            try
            {
                ThirdPersonController player = Player();
                InteractableDoor door = Require<InteractableDoor>("ClassroomDoor");
                door.GetComponent<Interactable>().Activate(player.transform);
            }
            catch (Exception exception) { error = exception.ToString(); }
            yield return new WaitForSeconds(0.65f);
            if (error == null)
            {
                try
                {
                    ThirdPersonController player = Player();
                    InteractableDoor door = Require<InteractableDoor>("ClassroomDoor");
                    if (!door.IsOpen) throw new Exception("School door did not open");
                    door.GetComponent<Interactable>().Activate(player.transform);
                    SeatCycle("StudentChair_1", player);
                    PickupItem item = Require<PickupItem>("Phase3SchoolPickup");
                    PhysicsGrabber grabber = player.GetComponent<PhysicsGrabber>();
                    PlaceOn("StudentDesk_1", item, grabber);
                    Debug.Log("PHASE3_SCHOOL_PASS");
                }
                catch (Exception exception) { error = exception.ToString(); }
            }
            fail(error);
        }

        private IEnumerator CheckHome(Action<string> fail)
        {
            string error = null;
            yield return EnterWorld("Home_Environment_V2", value => error = value);
            if (error != null) { fail(error); yield break; }
            try
            {
                ThirdPersonController player = Player();
                ToggleOpen<InteractableDoor>("FrontDoor", player);
                ToggleOpen<InteractableCabinet>("DrawerCabinet", player);
                SeatCycle("DiningSeat", player);
                LieCycle("ModernBed", player);
                PhysicsGrabber grabber = player.GetComponent<PhysicsGrabber>();
                PickupItem item = Require<PickupItem>("CarryBox");
                Vector3 scale = item.transform.lossyScale;
                Transform parent = item.transform.parent;
                Rigidbody body = item.GetComponent<Rigidbody>();
                Collider collider = item.GetComponent<Collider>();
                for (int i = 0; i < 10; i++)
                {
                    grabber.Pickup(item);
                    if (!grabber.IsHolding || Vector3.Distance(scale, item.transform.lossyScale) > 0.0001f)
                        throw new Exception("Pickup scale changed while held at cycle " + i);
                    grabber.ReleaseHeld();
                    if (grabber.IsHolding || item.transform.parent != parent || Vector3.Distance(scale, item.transform.lossyScale) > 0.0001f
                        || body.isKinematic || !body.useGravity || !collider.enabled)
                        throw new Exception("Pickup state changed after cycle " + i);
                }
                Debug.Log("PHASE3_PICKUP_10_CYCLES_PASS");
                PlaceOn("GroundFloor", item, grabber);
            }
            catch (Exception exception) { error = exception.ToString(); }
            yield return null;
            if (error == null) yield return RespawnByFall(Player(), "HomeSpawn", value => error = value);
            if (error == null) Debug.Log("PHASE3_HOME_PASS");
            fail(error);
        }

        private IEnumerator CheckPark(Action<string> fail)
        {
            string error = null;
            yield return EnterWorld("Park_Environment_V1", value => error = value);
            if (error != null) { fail(error); yield break; }
            try
            {
                ThirdPersonController player = Player();
                SeatCycle("ParkBench_1", player);
                PickupItem item = Require<PickupItem>("Phase3ParkPickup");
                PlaceOn("PicnicTable", item, player.GetComponent<PhysicsGrabber>());
            }
            catch (Exception exception) { error = exception.ToString(); }
            yield return null;
            if (error == null) yield return RespawnByFall(Player(), "PlayerSpawn", value => error = value);
            if (error == null) Debug.Log("PHASE3_PARK_PASS");
            fail(error);
        }

        private IEnumerator CheckUrban(Action<string> fail)
        {
            string error = null;
            yield return EnterWorld("Urban_Environment_V1", value => error = value);
            if (error != null) { fail(error); yield break; }
            try
            {
                ThirdPersonController player = Player();
                ToggleOpen<InteractableDoor>("UrbanExampleDoor", player);
            }
            catch (Exception exception) { error = exception.ToString(); }
            yield return null;
            if (error == null) yield return RespawnByFall(Player(), "PlayerSpawn", value => error = value);
            if (error == null) Debug.Log("PHASE3_URBAN_PASS");
            fail(error);
        }

        private static IEnumerator EnterWorld(string scene, Action<string> fail)
        {
            SceneManager.LoadScene(scene);
            yield return null;
            yield return null;
            string error = null;
            try
            {
                GameBootstrap bootstrap = FindFirstObjectByType<GameBootstrap>();
                if (bootstrap == null) throw new Exception("GameBootstrap missing in " + scene);
                typeof(GameBootstrap).GetMethod("SaveAndEnterWorld", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(bootstrap, new object[] { AppearanceSaveService.LoadOrDefault() });
            }
            catch (Exception exception) { error = exception.ToString(); }
            yield return null;
            if (error == null && FindObjectsByType<ThirdPersonController>(FindObjectsSortMode.None).Length != 1)
                error = "Player count invalid in " + scene;
            fail(error);
        }

        private static ThirdPersonController Player()
        {
            ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
            if (player == null) throw new Exception("Player missing");
            return player;
        }

        private static void SeatCycle(string name, ThirdPersonController player)
        {
            Require<SeatInteractable>(name).GetComponent<Interactable>().Activate(player.transform);
            PoseController pose = player.GetComponent<PoseController>();
            if (!pose.IsPosed || player.enabled) throw new Exception("Seat enter failed: " + name);
            pose.Stand();
            if (pose.IsPosed || !player.enabled) throw new Exception("Seat exit failed: " + name);
        }

        private static void LieCycle(string name, ThirdPersonController player)
        {
            Require<LieInteractable>(name).GetComponent<Interactable>().Activate(player.transform);
            PoseController pose = player.GetComponent<PoseController>();
            if (!pose.IsPosed || player.enabled) throw new Exception("Lie enter failed: " + name);
            pose.Stand();
            if (pose.IsPosed || !player.enabled) throw new Exception("Lie exit failed: " + name);
        }

        private static void ToggleOpen<T>(string name, ThirdPersonController player) where T : Component
        {
            T component = Require<T>(name);
            component.GetComponent<Interactable>().Activate(player.transform);
            PropertyInfo state = typeof(T).GetProperty("IsOpen");
            if (state == null || !(bool)state.GetValue(component)) throw new Exception("Open failed: " + name);
        }

        private static void PlaceOn(string surfaceName, PickupItem item, PhysicsGrabber grabber)
        {
            GameObject surface = GameObject.Find(surfaceName);
            if (surface == null || surface.GetComponent<PlacementSurface>() == null)
                throw new Exception("Placement surface missing: " + surfaceName);
            Collider surfaceCollider = surface.GetComponent<Collider>();
            if (surfaceCollider == null) throw new Exception("Placement collider missing: " + surfaceName);
            Vector3 scale = item.transform.lossyScale;
            grabber.Pickup(item);
            Camera camera = Camera.main;
            ThirdPersonCamera orbit = camera.GetComponent<ThirdPersonCamera>();
            if (orbit != null) orbit.enabled = false;
            Bounds bounds = surfaceCollider.bounds;
            Vector3 target = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            camera.transform.position = target + new Vector3(0f, 2.2f, -2.2f);
            camera.transform.LookAt(target);
            Physics.SyncTransforms();
            if (!grabber.PlaceHeld()) throw new Exception("Placement failed: " + surfaceName);
            if (Vector3.Distance(scale, item.transform.lossyScale) > 0.0001f)
                throw new Exception("Placement changed scale: " + surfaceName);
            if (item.GetComponent<Rigidbody>().isKinematic || !item.GetComponent<Collider>().enabled)
                throw new Exception("Placement did not restore physics: " + surfaceName);
            if (orbit != null) orbit.enabled = true;
            Debug.Log("PHASE3_PLACEMENT_PASS " + surfaceName);
        }

        private static IEnumerator RespawnByFall(ThirdPersonController player, string spawnName, Action<string> fail)
        {
            PlayerRespawnSystem respawn = player.GetComponent<PlayerRespawnSystem>();
            if (respawn == null) { fail("Respawn system missing"); yield break; }
            Transform spawn = GameObject.Find(spawnName).transform;
            CharacterController capsule = player.GetComponent<CharacterController>();
            capsule.enabled = false;
            player.transform.position = new Vector3(spawn.position.x, -20f, spawn.position.z);
            capsule.enabled = true;
            yield return null;
            if (Vector3.Distance(player.transform.position, spawn.position) > 0.1f || respawn.RespawnCount < 1)
            {
                fail("Automatic respawn failed in " + SceneManager.GetActiveScene().name);
                yield break;
            }
            Debug.Log("PHASE3_RESPAWN_PASS " + SceneManager.GetActiveScene().name);
            fail(null);
        }

        private static T Require<T>(string name) where T : Component
        {
            GameObject target = GameObject.Find(name);
            if (target == null) throw new Exception("Object missing: " + name);
            T component = target.GetComponent<T>();
            if (component == null) throw new Exception("Component missing: " + name + " " + typeof(T).Name);
            return component;
        }
    }
}
