using LightweightGame.Interaction;
using LightweightGame.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LightweightGame.Editor
{
    public static class EnvironmentPhase3Builder
    {
        public static void Apply()
        {
            UpgradeSharedPrefabs();
            PatchSchool();
            PatchHome();
            PatchPark();
            PatchUrban();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("PHASE3_SCENE_PATCH_PASS");
        }

        private static void UpgradeSharedPrefabs()
        {
            EditPrefab("Assets/Prefabs/Interactables/FrontDoor.prefab", root =>
            {
                RemoveLegacy(root);
                Ensure<Interactable>(root);
                InteractableDoor door = Ensure<InteractableDoor>(root);
                door.Configure(root.transform, new Vector3(0f, -100f, 0f));
            });
            EditPrefab("Assets/Prefabs/Interactables/DrawerCabinet.prefab", root =>
            {
                RemoveLegacy(root);
                Ensure<Interactable>(root);
                Transform drawer = FindChild(root.transform, "drawer_cabinet_drawer_01");
                Ensure<InteractableCabinet>(root).Configure(drawer, new Vector3(0f, 0f, -0.35f), Vector3.zero);
            });
            UpgradeSeatPrefab("Assets/Prefabs/Interactables/DiningSeat.prefab");
            UpgradeSeatPrefab("Assets/Prefabs/Interactables/LoungeSeat.prefab");
            EditPrefab("Assets/Prefabs/Interactables/ModernBed.prefab", root =>
            {
                RemoveLegacy(root);
                Ensure<Interactable>(root);
                Ensure<LieInteractable>(root).Configure(root.transform.Find("BedPosePoint"), root.transform.Find("BedExitPoint"));
            });
            EditPrefab("Assets/Prefabs/Interactables/CarryBox.prefab", root =>
            {
                Ensure<Interactable>(root);
                Ensure<PickupInteractable>(root);
            });
        }

        private static void UpgradeSeatPrefab(string path)
        {
            EditPrefab(path, root =>
            {
                RemoveLegacy(root);
                Ensure<Interactable>(root);
                Ensure<SeatInteractable>(root).Configure(root.transform.Find("SeatPoint"), root.transform.Find("StandPoint"));
            });
        }

        private static void PatchSchool()
        {
            Scene scene = Open("Assets/Scenes/School_Environment_V1.unity");
            ReplaceSeat(Require("StudentChair_1"));
            Ensure<PlacementSurface>(Require("StudentDesk_1"));
            Ensure<PlacementSurface>(Require("TeacherDesk"));
            Ensure<DefaultSpawnPoint>(Require("PlayerSpawn"));
            if (GameObject.Find("Phase3SchoolPickup") == null)
                AddPickup("Phase3SchoolPickup", new Vector3(0.45f, 0.82f, 10.2f), "练习册");
            Save(scene);
        }

        private static void PatchHome()
        {
            Scene scene = Open("Assets/Scenes/Home_Environment_V2.unity");
            Ensure<PlacementSurface>(Require("GroundFloor"));
            Ensure<DefaultSpawnPoint>(Require("HomeSpawn"));
            Save(scene);
        }

        private static void PatchPark()
        {
            Scene scene = Open("Assets/Scenes/Park_Environment_V1.unity");
            ReplaceSeat(Require("ParkBench_1"));
            Ensure<PlacementSurface>(Require("ParkSafeGround"));
            Ensure<PlacementSurface>(Require("CentralPlaza"));
            Ensure<PlacementSurface>(Require("PicnicTable"));
            Ensure<DefaultSpawnPoint>(Require("PlayerSpawn"));
            if (GameObject.Find("Phase3ParkPickup") == null)
                AddPickup("Phase3ParkPickup", new Vector3(16f, 0.82f, 13f), "公园读物");
            Save(scene);
        }

        private static void PatchUrban()
        {
            Scene scene = Open("Assets/Scenes/Urban_Environment_V1.unity");
            Ensure<PlacementSurface>(Require("UrbanSafeGround"));
            Ensure<DefaultSpawnPoint>(Require("PlayerSpawn"));
            if (GameObject.Find("UrbanExampleDoor") == null)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Interactables/FrontDoor.prefab");
                GameObject door = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                door.name = "UrbanExampleDoor";
                door.transform.SetPositionAndRotation(new Vector3(25f, 0.12f, -13.85f), Quaternion.Euler(0f, 180f, 0f));
                door.transform.SetParent(Require("UrbanEnvironment").transform, true);
            }
            Save(scene);
        }

        private static void ReplaceSeat(GameObject target)
        {
            RemoveLegacy(target);
            Ensure<Interactable>(target);
            Transform seat = target.transform.Find("SeatPose");
            Transform exit = target.transform.Find("StandPoint");
            if (seat == null || exit == null) throw new System.InvalidOperationException("Seat points missing on " + target.name);
            Ensure<SeatInteractable>(target).Configure(seat, exit);
        }

        private static void AddPickup(string name, Vector3 position, string label)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnvironmentPhase2/binder_notebook.prefab");
            GameObject item = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            item.name = name;
            item.transform.position = position;
            if (item.GetComponent<Collider>() == null) item.AddComponent<BoxCollider>();
            Rigidbody body = Ensure<Rigidbody>(item);
            body.mass = 0.35f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            PickupItem pickup = Ensure<PickupItem>(item);
            pickup.Initialize(label);
            Ensure<Interactable>(item);
            Ensure<PickupInteractable>(item);
        }

        private static Scene Open(string path)
        {
            return EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }

        private static void Save(Scene scene)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static GameObject Require(string name)
        {
            GameObject target = GameObject.Find(name);
            if (target == null) throw new System.InvalidOperationException("Required object missing: " + name);
            return target;
        }

        private static T Ensure<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            return component != null ? component : target.AddComponent<T>();
        }

        private static void RemoveLegacy(GameObject target)
        {
            HomeInteractable legacy = target.GetComponent<HomeInteractable>();
            if (legacy != null) Object.DestroyImmediate(legacy, true);
        }

        private static Transform FindChild(Transform root, string name)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
            throw new System.InvalidOperationException("Child missing: " + name);
        }

        private static void EditPrefab(string path, System.Action<GameObject> edit)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            edit(root);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
