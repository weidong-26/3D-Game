using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class Object
    {
        public string name;
        public static void Destroy(Object value) { }
        public static T FindFirstObjectByType<T>() where T : Object { return null; }
        public static T[] FindObjectsByType<T>(FindObjectsSortMode mode) where T : Object { return new T[0]; }
    }
    public enum FindObjectsSortMode { None }

    public class Component : Object
    {
        public GameObject gameObject = new GameObject();
        public Transform transform { get { return gameObject.transform; } }
        public T GetComponent<T>() where T : Component, new() { return new T(); }
        public Component GetComponent(Type type) { return null; }
        public T GetComponentInChildren<T>() where T : Component, new() { return new T(); }
        public T GetComponentInParent<T>() where T : Component, new() { return new T(); }
        public T[] GetComponentsInChildren<T>() where T : Component, new() { return new T[0]; }
        public T[] GetComponentsInChildren<T>(bool includeInactive) where T : Component, new() { return new T[0]; }
    }

    public class Behaviour : Component { public bool enabled; }
    public class MonoBehaviour : Behaviour { }
    public class Font : Object { public static Font CreateDynamicFontFromOSFont(string name, int size) { return new Font(); } }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SerializeField : Attribute { }
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HeaderAttribute : Attribute { public HeaderAttribute(string text) { } }
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RequireComponent : Attribute { public RequireComponent(Type type) { } }
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RuntimeInitializeOnLoadMethodAttribute : Attribute
    {
        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType type) { }
    }
    public enum RuntimeInitializeLoadType { BeforeSceneLoad, AfterSceneLoad }

    public class GameObject : Object
    {
        public Transform transform;
        public string tag;
        public bool activeSelf;
        public GameObject() { transform = new Transform(this); }
        public GameObject(string value, params Type[] types) { name = value; transform = new Transform(this); }
        public static GameObject CreatePrimitive(PrimitiveType type) { return new GameObject(); }
        public static GameObject Find(string name) { return null; }
        public T AddComponent<T>() where T : Component, new() { return new T(); }
        public Component AddComponent(Type type) { return new Component(); }
        public T GetComponent<T>() where T : Component, new() { return new T(); }
        public Component GetComponent(Type type) { return null; }
        public T GetComponentInChildren<T>() where T : Component, new() { return new T(); }
        public void SetActive(bool value) { }
    }

    public class Transform : Component
    {
        public Vector3 position;
        public Vector3 localPosition;
        public Vector3 localScale;
        public Vector3 lossyScale;
        public Transform parent;
        public Quaternion rotation;
        public Quaternion localRotation;
        public Vector3 forward;
        public Vector3 right;
        public Transform(GameObject owner) { gameObject = owner; }
        public Transform() { }
        public void SetParent(Transform parent, bool worldPositionStays) { }
        public void LookAt(Vector3 value) { }
        public void SetPositionAndRotation(Vector3 value, Quaternion rotationValue) { }
        public bool IsChildOf(Transform parent) { return false; }
        public Transform GetChild(int index) { return new Transform(); }
        public Transform Find(string name) { return new Transform(); }
    }

    public struct Vector2
    {
        public float x, y;
        public Vector2(float xValue, float yValue) { x = xValue; y = yValue; }
        public static Vector2 zero { get { return new Vector2(); } }
        public static Vector2 one { get { return new Vector2(1f, 1f); } }
        public static Vector2 ClampMagnitude(Vector2 value, float maxLength) { return value; }
    }

    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float xValue, float yValue, float zValue) { x = xValue; y = yValue; z = zValue; }
        public static Vector3 zero { get { return new Vector3(); } }
        public static Vector3 one { get { return new Vector3(1f, 1f, 1f); } }
        public static Vector3 up { get { return new Vector3(0f, 1f, 0f); } }
        public static Vector3 forward { get { return new Vector3(0f, 0f, 1f); } }
        public float sqrMagnitude { get { return 0f; } }
        public float magnitude { get { return 0f; } }
        public void Normalize() { }
        public static Vector3 ClampMagnitude(Vector3 value, float maxLength) { return value; }
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float distance) { return target; }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) { return b; }
        public static float Distance(Vector3 a, Vector3 b) { return 0f; }
        public static Vector3 operator +(Vector3 a, Vector3 b) { return a; }
        public static Vector3 operator -(Vector3 a, Vector3 b) { return a; }
        public static Vector3 operator *(Vector3 a, float b) { return a; }
        public static Vector3 operator *(float a, Vector3 b) { return b; }
        public static Vector3 operator -(Vector3 a) { return a; }
    }

    public struct Quaternion
    {
        public static Quaternion identity { get { return new Quaternion(); } }
        public static Quaternion Euler(float x, float y, float z) { return new Quaternion(); }
        public static Quaternion LookRotation(Vector3 direction) { return new Quaternion(); }
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t) { return a; }
        public static Vector3 operator *(Quaternion a, Vector3 b) { return b; }
    }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float red, float green, float blue, float alpha = 1f) { r = red; g = green; b = blue; a = alpha; }
        public static Color white { get { return new Color(1f, 1f, 1f); } }
        public static Color Lerp(Color a, Color b, float t) { return a; }
    }

    public static class Mathf
    {
        public const float PI = 3.1415927f;
        public static float Sin(float value) { return 0f; }
        public static float Cos(float value) { return 0f; }
        public static float Abs(float value) { return 0f; }
        public static float Sign(float value) { return 0f; }
        public static float Clamp01(float value) { return value; }
        public static float Clamp(float value, float min, float max) { return value; }
        public static float Lerp(float a, float b, float t) { return a; }
        public static float Min(float a, float b) { return a; }
        public static float Max(float a, float b) { return a; }
        public static int RoundToInt(float value) { return 0; }
    }

    public enum PrimitiveType { Sphere, Capsule, Cube, Cylinder }
    public class Shader : Object { public static Shader Find(string value) { return new Shader(); } }
    public class Material : Object
    {
        public Color color;
        public Material(Shader shader) { }
    }
    public class Renderer : Component { public Material sharedMaterial; public Material material = new Material(null); }
    public class MeshFilter : Component { public Mesh sharedMesh; }
    public class MeshRenderer : Renderer { }
    public class SkinnedMeshRenderer : Renderer
    {
        public Mesh sharedMesh;
        public bool updateWhenOffscreen;
        public void SetBlendShapeWeight(int index, float value) { }
    }
    public class Mesh : Object
    {
        public void SetVertices(List<Vector3> value) { }
        public void SetUVs(int channel, List<Vector2> value) { }
        public void SetTriangles(List<int> value, int submesh) { }
        public void RecalculateNormals() { }
        public void RecalculateBounds() { }
        public void AddBlendShapeFrame(string shapeName, float weight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents) { }
        public int GetBlendShapeIndex(string value) { return 0; }
    }

    public class Collider : Component { public bool enabled; public Rigidbody attachedRigidbody; }
    public class Rigidbody : Component
    {
        public bool isKinematic;
        public bool useGravity;
        public float mass;
        public float linearDamping;
        public float angularDamping;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
        public Vector3 worldCenterOfMass;
        public RigidbodyInterpolation interpolation;
        public void AddForce(Vector3 value, ForceMode mode) { }
    }
    public enum ForceMode { VelocityChange }
    public enum RigidbodyInterpolation { Interpolate }
    public class CharacterController : Collider
    {
        public float height;
        public float radius;
        public float slopeLimit, stepOffset, skinWidth;
        public Vector3 center;
        public bool isGrounded;
        public CollisionFlags Move(Vector3 value) { return CollisionFlags.None; }
    }
    [Flags]
    public enum CollisionFlags { None = 0, Sides = 1, Above = 2, Below = 4 }
    public class ControllerColliderHit
    {
        public Collider collider;
        public Vector3 moveDirection;
    }

    public class Camera : Behaviour
    {
        public static Camera main { get { return new Camera(); } }
        public float fieldOfView;
        public CameraClearFlags clearFlags;
        public Ray ViewportPointToRay(Vector3 value) { return new Ray(); }
    }
    public enum CameraClearFlags { Skybox }
    public class AudioListener : Behaviour { }
    public class Light : Behaviour { public LightType type; public float intensity, range, spotAngle; public Color color; public LightShadows shadows; }
    public enum LightType { Directional, Point, Spot }
    public enum LightShadows { Soft }
    public class RuntimeAnimatorController : Object { }
    public class Animator : Behaviour
    {
        public RuntimeAnimatorController runtimeAnimatorController;
        public bool applyRootMotion;
        public void CrossFade(string state, float duration) { }
    }
    public class Canvas : Behaviour { public RenderMode renderMode; public int sortingOrder; }
    public enum RenderMode { ScreenSpaceOverlay }
    public class RectTransform : Transform
    {
        public Vector2 anchorMin, anchorMax, pivot, sizeDelta, anchoredPosition, offsetMin, offsetMax;
    }
    public enum TextAnchor { MiddleLeft, MiddleCenter, UpperLeft }

    public struct Ray { }
    public struct RaycastHit { public Rigidbody rigidbody; public Collider collider; public float distance; }
    public static class Physics
    {
        public static void SyncTransforms() { }
        public static bool Raycast(Ray ray, out RaycastHit hit, float distance) { hit = new RaycastHit(); return false; }
        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance) { return new RaycastHit[0]; }
        public static Collider[] OverlapSphere(Vector3 origin, float radius) { return new Collider[0]; }
    }
    public static class Time { public static float deltaTime, time; }
    public class WaitForEndOfFrame { }
    public class WaitForSeconds { public WaitForSeconds(float seconds) { } }
    public static class ScreenCapture { public static void CaptureScreenshot(string path) { } }
    public static class Input
    {
        public static float GetAxis(string value) { return 0f; }
        public static float GetAxisRaw(string value) { return 0f; }
        public static bool GetKey(KeyCode key) { return false; }
        public static bool GetKeyDown(KeyCode key) { return false; }
        public static bool GetButtonDown(string value) { return false; }
        public static bool GetMouseButtonDown(int button) { return false; }
        public static bool GetMouseButton(int button) { return false; }
    }
    public enum KeyCode { LeftShift, E, F, R, C, Escape }
    public static class Cursor { public static CursorLockMode lockState; public static bool visible; }
    public enum CursorLockMode { None, Locked }
    public static class Application { public static string persistentDataPath; public static string dataPath; public static int targetFrameRate; public static void Quit(int code) { } }
    public static class Debug { public static void Log(string value) { } public static void LogWarning(string value) { } public static void LogError(string value) { } }
    public static class JsonUtility { public static T FromJson<T>(string value) { return default(T); } public static string ToJson(object value, bool prettyPrint) { return ""; } }
    public static class Resources
    {
        public static T GetBuiltinResource<T>(string name) where T : Object, new() { return new T(); }
        public static T Load<T>(string name) where T : Object, new() { return new T(); }
    }
}

namespace UnityEngine.Events
{
    public delegate void UnityAction();
    public delegate void UnityAction<T>(T value);
    public class UnityEvent { public void AddListener(UnityAction action) { } public void Invoke() { } }
    public class UnityEvent<T> { public void AddListener(UnityAction<T> action) { } public void Invoke(T value) { } }
}

namespace UnityEngine.UI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class Graphic : Behaviour { public Color color; }
    public class Image : Graphic { }
    public class CanvasScaler : Behaviour
    {
        public ScaleMode uiScaleMode;
        public Vector2 referenceResolution;
        public float matchWidthOrHeight;
        public enum ScaleMode { ScaleWithScreenSize }
    }
    public class GraphicRaycaster : Behaviour { }
    public class Slider : Behaviour
    {
        public RectTransform fillRect;
        public RectTransform handleRect;
        public Graphic targetGraphic;
        public float minValue, maxValue;
        public bool wholeNumbers;
        public UnityEvent<float> onValueChanged = new UnityEvent<float>();
        public void SetValueWithoutNotify(float value) { }
    }
    public class Button : Behaviour
    {
        public Graphic targetGraphic;
        public UnityEvent onClick = new UnityEvent();
    }
    public class Text : Graphic
    {
        public Font font;
        public string text;
        public int fontSize;
        public TextAnchor alignment;
        public RectTransform rectTransform = new RectTransform();
    }
}

namespace UnityEngine.EventSystems
{
    public class EventSystem : UnityEngine.Behaviour { }
    public class StandaloneInputModule : UnityEngine.Behaviour { }
}
