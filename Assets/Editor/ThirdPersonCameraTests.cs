using System;
using System.Reflection;
using LightweightGame.Runtime;
using UnityEngine;

namespace LightweightGame.Editor
{
    public static class ThirdPersonCameraTests
    {
        public static void Run()
        {
            GameObject targetObject = new GameObject("CameraTestTarget");
            GameObject cameraObject = new GameObject("CameraTestCamera");
            GameObject wall = null;
            try
            {
                targetObject.transform.position = new Vector3(10000f, 0f, 10000f);
                ThirdPersonCamera camera = cameraObject.AddComponent<ThirdPersonCamera>();
                camera.SetTarget(targetObject.transform);

                MethodInfo update = typeof(ThirdPersonCamera).GetMethod("UpdateCamera", BindingFlags.Instance | BindingFlags.NonPublic);
                if (update == null) throw new InvalidOperationException("Camera update method missing");
                update.Invoke(camera, new object[] { 0f });

                Vector3 cameraBeforeMove = camera.transform.position;
                targetObject.transform.position += Vector3.right * 3f;
                update.Invoke(camera, new object[] { 1f / 60f });
                float followDistance = Vector3.Distance(camera.transform.position, cameraBeforeMove);
                if (followDistance <= 0.001f || followDistance >= 1f)
                    throw new InvalidOperationException("Camera follow was not smooth: " + followDistance);

                Quaternion rotationBefore = camera.transform.rotation;
                typeof(ThirdPersonCamera).GetMethod("ApplyMouseDelta", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(camera, new object[] { 90f, 0f });
                update.Invoke(camera, new object[] { 1f / 60f });
                float rotationDelta = Quaternion.Angle(rotationBefore, camera.transform.rotation);
                if (rotationDelta <= 0.01f || rotationDelta >= 90f)
                    throw new InvalidOperationException("Camera rotation was not smooth: " + rotationDelta);

                typeof(ThirdPersonCamera).GetMethod("ApplyMouseDelta", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(camera, new object[] { -90f, 0f });
                update.Invoke(camera, new object[] { 1f });
                targetObject.transform.position = new Vector3(10000f, 0f, 10000f);
                update.Invoke(camera, new object[] { 1f });
                wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.transform.position = targetObject.transform.position + Vector3.back * 2f;
                wall.transform.localScale = new Vector3(4f, 4f, 0.2f);
                Physics.SyncTransforms();
                update.Invoke(camera, new object[] { 1f / 60f });
                float collisionDistance = Vector3.Distance(camera.transform.position,
                    targetObject.transform.position);
                if (collisionDistance >= 2f)
                    throw new InvalidOperationException("Camera did not shorten its distance near a wall: " + collisionDistance);

                Debug.Log("THIRD_PERSON_CAMERA_PASS follow rotation collision");
            }
            finally
            {
                if (wall != null) UnityEngine.Object.DestroyImmediate(wall);
                UnityEngine.Object.DestroyImmediate(cameraObject);
                UnityEngine.Object.DestroyImmediate(targetObject);
            }
        }
    }
}
