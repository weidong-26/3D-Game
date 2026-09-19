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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-prototype-smoke") >= 0)
                new GameObject("PrototypeSmoke").AddComponent<PrototypeSmoke>();
        }

        private IEnumerator Start()
        {
            string logs = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "..", "Logs"));
            Directory.CreateDirectory(logs);
            yield return new WaitForEndOfFrame();
            Debug.Log("CREATOR_CAPTURE_BEGIN");
            CaptureWorld(Path.Combine(logs, "creator-smoke.png"));
            Debug.Log("CREATOR_CAPTURE_END");
            bool passed = RunChecks();
            Debug.Log("GAMEPLAY_CAPTURE_BEGIN");
            CaptureWorld(Path.Combine(logs, "gameplay-smoke.png"));
            Debug.Log("GAMEPLAY_CAPTURE_END");
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
                Require("Street", typeof(Collider));
                Require("Cup", typeof(PickupItem));
                Require("Box", typeof(PickupItem));
                Require("Flashlight", typeof(PickupItem));
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
                if (!movement.enabled || !grabber.enabled || !camera.enabled) throw new Exception("Creator did not enter gameplay");

                GameObject cup = GameObject.Find("Cup");
                movement.transform.position = new Vector3(2.3f, 0f, 2.5f);
                Invoke(grabber, "TryPickup");
                if (!grabber.IsHolding || cup.GetComponent<Rigidbody>().isKinematic == false) throw new Exception("Cup pickup failed");
                Invoke(grabber, "Release");
                if (grabber.IsHolding || cup.GetComponent<Rigidbody>().isKinematic) throw new Exception("Cup release failed");

                GameObject flashlight = GameObject.Find("Flashlight");
                movement.transform.position = new Vector3(-2.2f, 0f, 6.1f);
                Invoke(grabber, "TryPickup");
                if (!grabber.IsHolding) throw new Exception("Flashlight pickup failed");
                PickupItem item = flashlight.GetComponent<PickupItem>();
                item.Use();
                if (!flashlight.GetComponentInChildren<Light>().enabled) throw new Exception("Flashlight toggle failed");
                Invoke(grabber, "Release");

                movement.transform.position = new Vector3(0f, -10f, 0f);
                movement.Respawn();
                if (movement.transform.position.y < -1f) throw new Exception("Respawn failed");
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

        private static void CaptureWorld(string path)
        {
            Camera camera = Camera.main;
            RenderTexture texture = new RenderTexture(1280, 720, 24);
            RenderTexture previous = RenderTexture.active;
            camera.targetTexture = texture;
            RenderTexture.active = texture;
            camera.Render();
            Texture2D image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = previous;
            UnityEngine.Object.Destroy(texture);
            UnityEngine.Object.Destroy(image);
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
    }
}
