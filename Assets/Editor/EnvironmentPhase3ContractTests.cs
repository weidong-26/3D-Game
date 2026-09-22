using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LightweightGame.Editor
{
    public static class EnvironmentPhase3ContractTests
    {
        private const string RuntimeAssembly = "Assembly-CSharp";

        public static void Run()
        {
            Type contract = RequireType("LightweightGame.Interaction.IEnvironmentInteractable");
            Type interactable = RequireType("LightweightGame.Runtime.Interactable");
            if (!contract.IsAssignableFrom(interactable))
                throw new InvalidOperationException("PHASE3_CONTRACT_INTERACTABLE_INTERFACE_MISSING");

            RequireMethod("LightweightGame.Interaction.InteractionDetector", "Detect");
            RequireMethod("LightweightGame.Interaction.InteractionPromptUI", "SetPrompt", typeof(string), typeof(bool));
            RequireProperty("LightweightGame.Interaction.InteractableDoor", "IsOpen");
            RequireMethod("LightweightGame.Interaction.InteractableDoor", "Configure", typeof(Transform), typeof(Vector3));
            RequireProperty("LightweightGame.Interaction.InteractableCabinet", "IsOpen");
            RequireMethod("LightweightGame.Interaction.InteractableCabinet", "Configure", typeof(Transform), typeof(Vector3), typeof(Vector3));
            RequireMethod("LightweightGame.Interaction.SeatInteractable", "Configure", typeof(Transform), typeof(Transform));
            RequireMethod("LightweightGame.Interaction.LieInteractable", "Configure", typeof(Transform), typeof(Transform));
            RequireMethod("LightweightGame.Interaction.PickupInteractable", "Bind", RequireType("LightweightGame.Runtime.PhysicsGrabber"));
            RequireMethod("LightweightGame.Interaction.PlacementSurface", "Accepts", typeof(Vector3));
            RequireMethod("LightweightGame.Interaction.PlacementSurface", "CanPlace", typeof(Bounds), typeof(Transform), typeof(Transform));
            RequireType("LightweightGame.Interaction.DefaultSpawnPoint");
            RequireMethod("LightweightGame.Interaction.PlayerRespawnSystem", "ForceRespawn");

            RequireMethod("LightweightGame.Runtime.PhysicsGrabber", "PlaceHeld");
            RequireMethod("LightweightGame.Runtime.ThirdPersonController", "RespawnAt", typeof(Vector3));

            Debug.Log("PHASE3_CONTRACT_TEST_PASS");
        }

        private static Type RequireType(string fullName)
        {
            Type type = Type.GetType(fullName + ", " + RuntimeAssembly);
            if (type == null)
                throw new InvalidOperationException("PHASE3_CONTRACT_MISSING_TYPE " + fullName);
            return type;
        }

        private static void RequireMethod(string fullTypeName, string methodName, params Type[] parameters)
        {
            Type type = RequireType(fullTypeName);
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, parameters, null);
            if (method == null)
                throw new InvalidOperationException("PHASE3_CONTRACT_MISSING_METHOD " + fullTypeName + "." + methodName);
        }

        private static void RequireProperty(string fullTypeName, string propertyName)
        {
            Type type = RequireType(fullTypeName);
            if (type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) == null)
                throw new InvalidOperationException("PHASE3_CONTRACT_MISSING_PROPERTY " + fullTypeName + "." + propertyName);
        }
    }
}
