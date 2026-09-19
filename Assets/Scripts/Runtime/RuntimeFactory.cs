using UnityEngine;

namespace LightweightGame.Runtime
{
    internal static class RuntimeFactory
    {
        public static Material Material(string name, Color color)
        {
            Shader shader = Resources.Load<Shader>("PrototypeColor");
            if (shader == null)
                throw new System.InvalidOperationException("Required shader resource 'PrototypeColor' is missing.");
            Material material = new Material(shader) { name = name, color = color };
            return material;
        }

        public static GameObject Primitive(
            PrimitiveType type,
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool keepCollider = false)
        {
            GameObject gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localScale = localScale;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            if (!keepCollider)
            {
                Collider collider = gameObject.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false;
                    Object.Destroy(collider);
                }
            }

            return gameObject;
        }
    }
}
