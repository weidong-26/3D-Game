using System;
using System.Collections;
using System.IO;
using System.Reflection;
using LightweightGame.Core;
using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class PrototypeSmoke : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void UseIsolatedSave()
        {
            if (HasArgument("-prototype-persist-write") || HasArgument("-prototype-persist-read"))
                AppearanceSaveService.TestPath = Path.Combine(Application.persistentDataPath, "appearance-round2-smoke.json");
            else
                AppearanceSaveService.TestPath = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (HasArgument("-prototype-smoke") || HasArgument("-prototype-persist-write") || HasArgument("-prototype-persist-read")
                || HasArgument("-round3-capture"))
                new GameObject("PrototypeSmoke").AddComponent<PrototypeSmoke>();
        }

        private static bool HasArgument(string value)
        {
            return Array.IndexOf(Environment.GetCommandLineArgs(), value) >= 0;
        }

        private IEnumerator Start()
        {
            yield return null;
            if (HasArgument("-round3-capture"))
            {
                GameBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<GameBootstrap>();
                typeof(GameBootstrap).GetMethod("SaveAndEnterWorld", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(bootstrap, new object[] { AppearanceSaveService.LoadOrDefault() });
                yield return new WaitForEndOfFrame();
                ScreenCapture.CaptureScreenshot(Path.Combine(Directory.GetCurrentDirectory(), "Logs", "round3-wake.png"));
                yield return new WaitForSeconds(0.8f);
                GameObject.Find("SkipWakeButton").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                ThirdPersonController player = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>();
                CharacterController capsule = player.GetComponent<CharacterController>();
                capsule.enabled = false;
                player.transform.position = new Vector3(0.2f, 2.9f, 4.4f);
                capsule.enabled = true;
                Physics.SyncTransforms();
                ThirdPersonCamera orbit = UnityEngine.Object.FindFirstObjectByType<ThirdPersonCamera>();
                orbit.enabled = false;
                Camera.main.transform.position = new Vector3(1.5f, 4.7f, 2.3f);
                Camera.main.transform.LookAt(new Vector3(0f, 3.2f, 6.7f));
                yield return new WaitForEndOfFrame();
                ScreenCapture.CaptureScreenshot(Path.Combine(Directory.GetCurrentDirectory(), "Logs", "round3-second-floor.png"));
                yield return new WaitForSeconds(0.8f);
                Debug.Log("ROUND3_CAPTURE_DONE");
                Application.Quit(0);
                yield break;
            }
            if (HasArgument("-prototype-persist-write"))
            {
                AppearanceData sample = AppearanceData.CreateDefault();
                sample.FaceWidth = 0.83f;
                sample.Height = 0.71f;
                bool saved = AppearanceSaveService.Save(sample);
                Debug.Log(saved ? "PERSIST_WRITE_PASS" : "PERSIST_WRITE_FAIL");
                Application.Quit(saved ? 0 : 1);
                yield break;
            }
            if (HasArgument("-prototype-persist-read"))
            {
                AppearanceData loaded = AppearanceSaveService.LoadOrDefault();
                CharacterCreatorUI creator = UnityEngine.Object.FindFirstObjectByType<CharacterCreatorUI>();
                AppearanceData inCreator = (AppearanceData)typeof(CharacterCreatorUI).GetField("data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(creator);
                bool restored = Mathf.Abs(loaded.FaceWidth - 0.83f) < 0.001f && Mathf.Abs(loaded.Height - 0.71f) < 0.001f
                    && Mathf.Abs(inCreator.FaceWidth - 0.83f) < 0.001f;
                if (restored) File.Delete(AppearanceSaveService.SavePath);
                Debug.Log(restored ? "PERSIST_RESTART_PASS" : "PERSIST_RESTART_FAIL");
                Application.Quit(restored ? 0 : 1);
                yield break;
            }
            bool passed = RunChecks();
            Debug.Log(passed ? "PROTOTYPE_SMOKE_PASS" : "PROTOTYPE_SMOKE_FAIL");
            Application.Quit(passed ? 0 : 1);
        }

        private static bool RunChecks()
        {
            string savePath = AppearanceSaveService.SavePath;
            byte[] original = File.Exists(savePath) ? File.ReadAllBytes(savePath) : null;
            try
            {
                Require("Ground", typeof(Collider));
                Require("HouseFloor", typeof(Collider));
                Require("BedFrame", typeof(Collider));
                Require("SofaSeat", typeof(Collider));
                Require("DiningTable", typeof(Collider));
                Require("InteriorStair3", typeof(Collider));
                Require("Street", typeof(Collider));
                Require("Cup", typeof(PickupItem));
                Require("Box", typeof(PickupItem));
                Require("Flashlight", typeof(PickupItem));
                CharacterCreatorUI creator = UnityEngine.Object.FindFirstObjectByType<CharacterCreatorUI>();
                if (creator == null || creator.GetComponentsInChildren<UnityEngine.UI.Slider>(true).Length != 22)
                    throw new Exception("Character creator controls missing");
                if (Resources.Load<RuntimeAnimatorController>("PrototypeAnimator") == null) throw new Exception("Animator controller missing");

                AppearanceData data = AppearanceData.CreateDefault();
                data.FaceWidth = 0.8f;
                data.Height = 0.7f;
                data.HairStyle = 2;
                if (!AppearanceSaveService.Save(data)) throw new Exception("Appearance save failed");
                AppearanceData loaded = AppearanceSaveService.LoadOrDefault();
                if (Mathf.Abs(loaded.FaceWidth - 0.8f) > 0.001f || Mathf.Abs(loaded.Height - 0.7f) > 0.001f || loaded.HairStyle != 2)
                    throw new Exception("Appearance reload mismatch");

                GameBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<GameBootstrap>();
                MethodInfo enter = typeof(GameBootstrap).GetMethod("SaveAndEnterWorld", BindingFlags.Instance | BindingFlags.NonPublic);
                enter.Invoke(bootstrap, new object[] { loaded });
                ThirdPersonController movement = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>();
                PhysicsGrabber grabber = UnityEngine.Object.FindFirstObjectByType<PhysicsGrabber>();
                ThirdPersonCamera camera = UnityEngine.Object.FindFirstObjectByType<ThirdPersonCamera>();
                GameObject wakeButton = GameObject.Find("WakeButton");
                GameObject skipButton = GameObject.Find("SkipWakeButton");
                if (wakeButton == null || skipButton == null || movement.enabled || !camera.enabled)
                    throw new Exception("Bed wake flow did not start");
                if (wakeButton.GetComponentInChildren<UnityEngine.UI.Text>().text != "掀开被子")
                    throw new Exception("Wake flow began in the wrong state");
                wakeButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                wakeButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (wakeButton.GetComponentInChildren<UnityEngine.UI.Text>().text != "坐起来")
                    throw new Exception("Rapid wake clicks skipped a state");
                skipButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (!movement.enabled || !grabber.enabled) throw new Exception("Skip did not restore control");
                WakeFlow wake = UnityEngine.Object.FindFirstObjectByType<WakeFlow>();
                wake.Begin();
                wakeButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                typeof(WakeFlow).GetField("nextActionTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(wake, 0f);
                wakeButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                typeof(WakeFlow).GetField("nextActionTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(wake, 0f);
                wakeButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (wake.State != WakeState.Standing || movement.enabled)
                    throw new Exception("Movement resumed before wake sequence reached FreeControl");
                typeof(WakeFlow).GetField("nextActionTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(wake, 0f);
                Invoke(wake, "Update");
                if (wake.State != WakeState.FreeControl || !movement.enabled)
                    throw new Exception("Wake sequence did not restore movement at FreeControl");
                if (!Cursor.visible || Cursor.lockState != CursorLockMode.None)
                    throw new Exception("Gameplay must start with a visible mouse cursor");

                FieldInfo grounded = typeof(ThirdPersonController).GetField("grounded", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo vertical = typeof(ThirdPersonController).GetField("verticalVelocity", BindingFlags.Instance | BindingFlags.NonPublic);
                grounded.SetValue(movement, true);
                Invoke(movement, "TryJump", true);
                if (movement.VerticalSpeed <= 0f) throw new Exception("First jump failed");
                vertical.SetValue(movement, 0f);
                Invoke(movement, "TryJump", true);
                if (movement.VerticalSpeed <= 0f) throw new Exception("Second jump failed");
                vertical.SetValue(movement, 0f);
                Invoke(movement, "TryJump", true);
                if (movement.VerticalSpeed != 0f) throw new Exception("Extra air jump was allowed");
                Invoke(camera, "ApplyMouseDelta", 0f, 100f);
                FieldInfo pitchField = typeof(ThirdPersonCamera).GetField("pitch", BindingFlags.Instance | BindingFlags.NonPublic);
                float pitchUp = (float)pitchField.GetValue(camera);
                Invoke(camera, "ApplyMouseDelta", 0f, -100f);
                float pitchDown = (float)pitchField.GetValue(camera);
                if (pitchUp > -60f || pitchDown < 70f) throw new Exception("Camera vertical look limits failed");
                Invoke(camera, "ApplyMouseDelta", 120f, 0f);
                FieldInfo yawField = typeof(ThirdPersonCamera).GetField("yaw", BindingFlags.Instance | BindingFlags.NonPublic);
                if ((float)yawField.GetValue(camera) < 360f) throw new Exception("Camera orbit failed");
                pitchField.SetValue(camera, 18f);
                yawField.SetValue(camera, 0f);

                GameObject cup = GameObject.Find("Cup");
                movement.transform.position = new Vector3(2.3f, 0f, 2.5f);
                Invoke(grabber, "TryPickup");
                if (!grabber.IsHolding || !cup.GetComponent<Rigidbody>().isKinematic || cup.GetComponentInChildren<Collider>().enabled)
                    throw new Exception("Cup pickup failed");
                Invoke(grabber, "Release");
                if (grabber.IsHolding || cup.GetComponent<Rigidbody>().isKinematic || !cup.GetComponentInChildren<Collider>().enabled)
                    throw new Exception("Cup release failed");

                GameObject box = GameObject.Find("Box");
                Vector3 boxScale = box.transform.GetChild(0).lossyScale;
                Transform boxParent = box.transform.parent;
                Rigidbody boxBody = box.GetComponent<Rigidbody>();
                Collider boxCollider = box.GetComponentInChildren<Collider>();
                movement.transform.position = new Vector3(3f, 0f, -4f);
                for (int cycle = 0; cycle < 20; cycle++)
                {
                    Invoke(grabber, "TryPickup");
                    if (!grabber.IsHolding) throw new Exception("Box pickup failed at cycle " + cycle);
                    if (Vector3.Distance(box.transform.GetChild(0).lossyScale, boxScale) > 0.001f)
                        throw new Exception("Box shrank while held at cycle " + cycle);
                    Invoke(grabber, "Release");
                    if (box.transform.parent != boxParent || boxBody.isKinematic || !boxBody.useGravity || !boxCollider.enabled
                        || Vector3.Distance(box.transform.GetChild(0).lossyScale, boxScale) > 0.001f)
                        throw new Exception("Box state or scale changed after cycle " + cycle);
                }
                Debug.Log("BOX_20_CYCLES_PASS");

                GameObject interactionButton = GameObject.Find("InteractionButton");
                if (interactionButton == null) throw new Exception("Mouse interaction button missing");
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                GameObject throwButton = GameObject.Find("ThrowButton");
                if (!grabber.IsHolding || throwButton == null) throw new Exception("Throw button missing while holding box");
                throwButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (grabber.IsHolding || boxBody.linearVelocity.magnitude < 2f || !boxCollider.enabled
                    || Vector3.Distance(box.transform.GetChild(0).lossyScale, boxScale) > 0.001f)
                    throw new Exception("Throw did not restore box size, collision, and forward motion");
                Debug.Log("BOX_THROW_PASS");

                movement.transform.position = new Vector3(cup.transform.position.x, 0f, cup.transform.position.z);
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (!grabber.IsHolding) throw new Exception("Click did not pick up nearby item");
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (grabber.IsHolding) throw new Exception("Click did not put down held item");
                Debug.Log("MOUSE_INTERACTION_PASS");

                movement.transform.position = new Vector3(2.3f, 0f, 1.6f);
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (movement.enabled) throw new Exception("Chair click did not enter seated state");
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (!movement.enabled) throw new Exception("Chair stand button did not restore control");
                movement.transform.position = new Vector3(-2.25f, 0f, 6.3f);
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (movement.enabled) throw new Exception("Bed click did not enter lying state");
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (!movement.enabled) throw new Exception("Bed stand button did not restore control");
                Debug.Log("FURNITURE_INTERACTION_PASS");

                GameObject door = GameObject.Find("DoorLeafOpen");
                Vector3 closedDirection = door.transform.forward;
                movement.transform.position = new Vector3(0f, 0f, 0.15f);
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (Vector3.Distance(door.transform.forward, closedDirection) < 0.2f)
                    throw new Exception("Mouse click did not open the door");
                interactionButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                if (Vector3.Distance(door.transform.forward, closedDirection) > 0.01f)
                    throw new Exception("Mouse click did not close the door");
                Debug.Log("DOOR_INTERACTION_PASS");

                Require("SecondFloorMain", typeof(Collider));
                CharacterController stairController = movement.GetComponent<CharacterController>();
                movement.enabled = false;
                stairController.enabled = false;
                movement.transform.position = new Vector3(-3.1f, 0f, 1.55f);
                stairController.enabled = true;
                Physics.SyncTransforms();
                for (int step = 0; step < 55; step++) stairController.Move(new Vector3(0f, -0.04f, 0.09f));
                if (movement.transform.position.y < 2.5f)
                    throw new Exception("Player did not reach second floor: " + movement.transform.position);
                for (int step = 0; step < 55; step++) stairController.Move(new Vector3(0f, -0.12f, -0.09f));
                if (movement.transform.position.y > 0.4f)
                    throw new Exception("Player could not safely return downstairs: " + movement.transform.position);
                movement.enabled = true;
                Debug.Log("SECOND_FLOOR_TRAVERSAL_PASS");

                GameObject flashlight = GameObject.Find("Flashlight");
                movement.transform.position = new Vector3(-2.2f, 0f, 6.1f);
                Invoke(grabber, "TryPickup");
                if (!grabber.IsHolding) throw new Exception("Flashlight pickup failed");
                PickupItem item = flashlight.GetComponent<PickupItem>();
                item.Use();
                if (!flashlight.GetComponentInChildren<Light>().enabled) throw new Exception("Flashlight toggle failed");
                Invoke(grabber, "Release");

                movement.transform.position = new Vector3(0f, -10f, 0f);
                Invoke(movement, "Update");
                if (movement.transform.position.y < -1f) throw new Exception("Respawn failed");
                movement.transform.position = new Vector3(3f, 0f, -4f);
                Invoke(movement, "ReturnToSpawn");
                if (Vector3.Distance(movement.transform.position, new Vector3(0f, 0f, -5f)) > 0.1f)
                    throw new Exception("Return to spawn failed");
                Invoke(bootstrap, "OpenCreator");
                if (movement.enabled || grabber.enabled || camera.enabled) throw new Exception("Reopen creator failed");
                enter.Invoke(bootstrap, new object[] { loaded });
                if (!movement.enabled || !grabber.enabled || !camera.enabled) throw new Exception("Reenter gameplay failed");
                Invoke(camera, "LateUpdate");
                Debug.Log("WORLD_INTERACTION_SAVE_RESPAWN_PASS");
                return true;
            }
            catch (Exception error)
            {
                Debug.LogError("PROTOTYPE_SMOKE_FAIL: " + error);
                return false;
            }
            finally
            {
                if (original == null) File.Delete(savePath);
                else File.WriteAllBytes(savePath, original);
            }
        }

        private static void Require(string name, Type component)
        {
            GameObject found = GameObject.Find(name);
            if (found == null || (component != null && found.GetComponent(component) == null))
                throw new Exception("Missing scene object or component: " + name);
        }

        private static void Invoke(object instance, string method)
        {
            instance.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(instance, null);
        }

        private static void Invoke(object instance, string method, params object[] arguments)
        {
            instance.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(instance, arguments);
        }
    }
}
