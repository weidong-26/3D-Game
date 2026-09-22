using System;
using LightweightGame.Interaction;
using LightweightGame.Runtime;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LightweightGame.Editor
{
    public static class EnvironmentPhase3SceneTests
    {
        public static void Run()
        {
            CheckSchool();
            CheckHome();
            CheckPark();
            CheckUrban();
            Debug.Log("PHASE3_SCENE_CONTRACT_PASS");
        }

        private static void CheckSchool()
        {
            Open("Assets/Scenes/School_Environment_V1.unity");
            Require<InteractableDoor>("ClassroomDoor");
            Require<SeatInteractable>("StudentChair_1");
            Require<PickupInteractable>("Phase3SchoolPickup");
            Require<PlacementSurface>("StudentDesk_1");
            Require<DefaultSpawnPoint>("PlayerSpawn");
        }

        private static void CheckHome()
        {
            Open("Assets/Scenes/Home_Environment_V2.unity");
            Require<InteractableDoor>("FrontDoor");
            Require<InteractableCabinet>("DrawerCabinet");
            Require<SeatInteractable>("DiningSeat");
            Require<LieInteractable>("ModernBed");
            Require<PickupInteractable>("CarryBox");
            Require<PlacementSurface>("GroundFloor");
            Require<DefaultSpawnPoint>("HomeSpawn");
        }

        private static void CheckPark()
        {
            Open("Assets/Scenes/Park_Environment_V1.unity");
            Require<SeatInteractable>("ParkBench_1");
            Require<PickupInteractable>("Phase3ParkPickup");
            Require<PlacementSurface>("PicnicTable");
            Require<DefaultSpawnPoint>("PlayerSpawn");
        }

        private static void CheckUrban()
        {
            Open("Assets/Scenes/Urban_Environment_V1.unity");
            Require<InteractableDoor>("UrbanExampleDoor");
            Require<DefaultSpawnPoint>("PlayerSpawn");
        }

        private static void Open(string path)
        {
            EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Single);
        }

        private static T Require<T>(string name) where T : Component
        {
            GameObject target = GameObject.Find(name);
            if (target == null) throw new InvalidOperationException("PHASE3_SCENE_MISSING_OBJECT " + name);
            T component = target.GetComponent<T>();
            if (component == null)
                throw new InvalidOperationException("PHASE3_SCENE_MISSING_COMPONENT " + name + " " + typeof(T).Name);
            return component;
        }
    }
}
