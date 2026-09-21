using System.IO;
using LightweightGame.Interaction;
using LightweightGame.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LightweightGame.Editor
{
    public static class HomeEnvironmentBuilder
    {
        private const string ScenePath = "Assets/Scenes/Home_Environment_V2.unity";
        private const string Interactables = "Assets/Prefabs/Interactables/";
        private static Material wall, trim, floor, concrete, metal, fabric, bedding, glass, darkWood, garden;
        private static Transform house;

        public static void Build()
        {
            EnvironmentSetup.ConfigureUrp();
            HomeAssetBuilder.Prepare();
            Materials();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            house = new GameObject("ModernHome").transform;
            Structure();
            Furnish();
            Lighting();
            Transform spawn = new GameObject("HomeSpawn").transform;
            spawn.SetParent(house, false);
            spawn.localPosition = new Vector3(0f, 0.02f, -1.2f);
            PrefabUtility.SaveAsPrefabAssetAndConnect(house.gameObject, "Assets/Prefabs/Buildings/ModernHome.prefab", InteractionMode.AutomatedAction);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true),
                new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true)
            };
            AssetDatabase.SaveAssets();
            Debug.Log("HOME_SCENE_BUILT " + ScenePath);
        }

        private static void Materials()
        {
            floor = HomeAssetBuilder.Surface("OakFloor", "Assets/Art/Materials/ambientCG/WoodFloor040", "WoodFloor040", Color.white);
            concrete = HomeAssetBuilder.Surface("WarmConcrete", "Assets/Art/Materials/ambientCG/Concrete045", "Concrete045", new Color(0.86f, 0.84f, 0.8f));
            metal = HomeAssetBuilder.Surface("DarkMetal", "Assets/Art/Materials/ambientCG/Metal029", "Metal029", new Color(0.3f, 0.34f, 0.36f), 0.85f);
            wall = HomeAssetBuilder.Solid("IvoryWall", new Color(0.83f, 0.82f, 0.78f), 0f, 0.2f);
            trim = HomeAssetBuilder.Solid("PaleTrim", new Color(0.92f, 0.91f, 0.87f), 0f, 0.3f);
            darkWood = HomeAssetBuilder.Solid("Walnut", new Color(0.29f, 0.2f, 0.14f), 0f, 0.42f);
            fabric = HomeAssetBuilder.Solid("SlateUpholstery", new Color(0.19f, 0.28f, 0.29f), 0f, 0.17f);
            bedding = HomeAssetBuilder.Solid("Linen", new Color(0.8f, 0.77f, 0.69f), 0f, 0.16f);
            glass = HomeAssetBuilder.Solid("WindowGlass", new Color(0.26f, 0.38f, 0.42f), 0.25f, 0.9f);
            garden = HomeAssetBuilder.Solid("Lawn", new Color(0.2f, 0.32f, 0.21f), 0f, 0.08f);
        }

        private static GameObject Block(string name, Vector3 center, Vector3 size, Material material, bool collider = true, float rounded = 0f, Transform parent = null)
        {
            GameObject instance = HomeAssetBuilder.Block(name, parent != null ? parent : house, center, size, material, collider, rounded);
            if (parent == null) instance.isStatic = true;
            return instance;
        }

        private static void Structure()
        {
            Block("LawnAndSafeGround", new Vector3(0, -0.2f, 5), new Vector3(38, 0.35f, 38), garden);
            Block("EntryWalk", new Vector3(0, -0.005f, 0.2f), new Vector3(2.2f, 0.06f, 3.6f), concrete);
            Block("GroundFloor", new Vector3(0, -0.12f, 8), new Vector3(12, 0.24f, 12), floor);
            Block("Patio", new Vector3(0, -0.08f, 1.1f), new Vector3(3.3f, 0.16f, 1.8f), concrete);

            Front(0f, true);
            Front(2.8f, false);
            Back(0f); Back(2.8f);
            for (int level = 0; level < 2; level++)
            {
                Side(-6f, level * 2.8f);
                Side(6f, level * 2.8f);
                Block("WestSkirting_" + level, new Vector3(-5.86f, level * 2.8f + 0.07f, 8), new Vector3(0.05f, 0.14f, 11.5f), trim);
                Block("EastSkirting_" + level, new Vector3(5.86f, level * 2.8f + 0.07f, 8), new Vector3(0.05f, 0.14f, 11.5f), trim);
            }

            // The west opening is the full stairwell, not a floor painted above the stairs.
            Block("SecondFloorMain", new Vector3(1.4f, 2.68f, 8), new Vector3(9.2f, 0.24f, 12), floor);
            Block("SecondFloorRearLanding", new Vector3(-4.6f, 2.68f, 11.4f), new Vector3(2.8f, 0.24f, 5.2f), floor);
            Block("SecondFloorFrontLanding", new Vector3(-4.6f, 2.68f, 2.6f), new Vector3(2.8f, 0.24f, 1.2f), floor);
            Stairs();
            Rooms();
            Block("FlatRoofAndCeiling", new Vector3(0, 5.72f, 8), new Vector3(12.55f, 0.24f, 12.55f), concrete);
            Block("EntranceCanopy", new Vector3(0, 2.4f, 1.15f), new Vector3(2.7f, 0.13f, 1.5f), metal);
            for (int i = 0; i < 9; i++)
                Block("EastFacadeTimberSlat", new Vector3(6.16f, 2.8f, 2.35f + i * 0.2f), new Vector3(0.05f, 5.3f, 0.075f), darkWood);
            for (int x = -1; x <= 1; x += 2)
                Block("RoofEdgeEastWest_" + x, new Vector3(x * 6.17f, 5.85f, 8), new Vector3(0.25f, 0.28f, 12.55f), metal);
            for (int z = -1; z <= 1; z += 2)
                Block("RoofEdgeNorthSouth_" + z, new Vector3(0, 5.85f, 8 + z * 6.17f), new Vector3(12.55f, 0.28f, 0.25f), metal);
        }

        private static void Front(float baseY, bool entrance)
        {
            float centerY = baseY + 1.4f;
            Block("FacadeFarLeft", new Vector3(-5.4f, centerY, 2), new Vector3(1.2f, 2.8f, 0.24f), wall);
            Block("FacadeLeftMiddle", new Vector3(-1.75f, centerY, 2), new Vector3(2.1f, 2.8f, 0.24f), wall);
            Block("FacadeRightMiddle", new Vector3(1.75f, centerY, 2), new Vector3(2.1f, 2.8f, 0.24f), wall);
            Block("FacadeFarRight", new Vector3(5.4f, centerY, 2), new Vector3(1.2f, 2.8f, 0.24f), wall);
            Window(new Vector3(-3.8f, baseY, 2), false);
            Window(new Vector3(3.8f, baseY, 2), false);
            if (entrance)
            {
                Block("DoorLintel", new Vector3(0, baseY + 2.5f, 2), new Vector3(1.4f, 0.6f, 0.24f), wall);
                Block("DoorLeftJamb", new Vector3(-0.74f, baseY + 1.1f, 1.88f), new Vector3(0.08f, 2.2f, 0.28f), trim);
                Block("DoorRightJamb", new Vector3(0.74f, baseY + 1.1f, 1.88f), new Vector3(0.08f, 2.2f, 0.28f), trim);
                Place(DoorPrefab(), house, new Vector3(-0.67f, 0, 1.99f), Quaternion.identity);
            }
            else Block("UpperFacadeCenter", new Vector3(0, centerY, 2), new Vector3(1.4f, 2.8f, 0.24f), wall);
            Block("FacadeBeam", new Vector3(0, baseY + 2.75f, 1.85f), new Vector3(12.2f, 0.1f, 0.35f), trim);
        }

        private static void Back(float baseY)
        {
            float y = baseY + 1.4f;
            Block("RearLeft", new Vector3(-5.7f, y, 14), new Vector3(0.6f, 2.8f, 0.24f), wall);
            Block("RearCenter", new Vector3(0, y, 14), new Vector3(8.4f, 2.8f, 0.24f), wall);
            Block("RearRight", new Vector3(5.7f, y, 14), new Vector3(0.6f, 2.8f, 0.24f), wall);
            Window(new Vector3(-4.8f, baseY, 14), false, 1.2f);
            Window(new Vector3(4.8f, baseY, 14), false, 1.2f);
        }

        private static void Side(float x, float baseY)
        {
            float y = baseY + 1.4f;
            Block("SideFrontWall", new Vector3(x, y, 3.1f), new Vector3(0.24f, 2.8f, 2.2f), wall);
            Block("SideMiddleWall", new Vector3(x, y, 7.7f), new Vector3(0.24f, 2.8f, 3f), wall);
            Block("SideRearWall", new Vector3(x, y, 12.6f), new Vector3(0.24f, 2.8f, 2.8f), wall);
            SideWindow(x, baseY, 5.2f);
            SideWindow(x, baseY, 10.2f);
        }

        private static void SideWindow(float x, float baseY, float z)
        {
            float outside = x + Mathf.Sign(x) * 0.04f;
            Block("SideWindowSill", new Vector3(x, baseY + 0.42f, z), new Vector3(0.24f, 0.84f, 2f), wall);
            Block("SideWindowHeader", new Vector3(x, baseY + 2.48f, z), new Vector3(0.24f, 0.64f, 2f), wall);
            Block("SideWindowGlass", new Vector3(x, baseY + 1.52f, z), new Vector3(0.06f, 1.27f, 1.88f), glass);
            Block("SideWindowFrontFrame", new Vector3(outside, baseY + 1.52f, z - 0.94f), new Vector3(0.1f, 1.35f, 0.1f), metal);
            Block("SideWindowBackFrame", new Vector3(outside, baseY + 1.52f, z + 0.94f), new Vector3(0.1f, 1.35f, 0.1f), metal);
            Block("SideWindowBottomFrame", new Vector3(outside, baseY + 0.84f, z), new Vector3(0.1f, 0.09f, 2f), metal);
            Block("SideWindowTopFrame", new Vector3(outside, baseY + 2.2f, z), new Vector3(0.1f, 0.09f, 2f), metal);
        }

        private static void Window(Vector3 basePoint, bool side, float width = 2f)
        {
            float z = basePoint.z;
            float x = basePoint.x;
            float y = basePoint.y;
            Block("WindowSillWall", new Vector3(x, y + 0.42f, z), new Vector3(width, 0.84f, 0.24f), wall);
            Block("WindowHeaderWall", new Vector3(x, y + 2.48f, z), new Vector3(width, 0.64f, 0.24f), wall);
            Block("InsulatedWindowPane", new Vector3(x, y + 1.52f, z), new Vector3(width - 0.12f, 1.27f, 0.06f), glass);
            Block("WindowLeftFrame", new Vector3(x - width * 0.5f + 0.05f, y + 1.52f, z - 0.04f), new Vector3(0.1f, 1.35f, 0.1f), metal);
            Block("WindowRightFrame", new Vector3(x + width * 0.5f - 0.05f, y + 1.52f, z - 0.04f), new Vector3(0.1f, 1.35f, 0.1f), metal);
            Block("WindowTopFrame", new Vector3(x, y + 2.2f, z - 0.04f), new Vector3(width, 0.09f, 0.1f), metal);
            Block("WindowBottomFrame", new Vector3(x, y + 0.84f, z - 0.04f), new Vector3(width, 0.09f, 0.1f), metal);
            Block("WindowMullion", new Vector3(x, y + 1.52f, z - 0.08f), new Vector3(0.045f, 1.27f, 0.045f), metal);
        }

        private static void Stairs()
        {
            for (int i = 0; i < 14; i++)
            {
                float height = (i + 1) * 0.2f;
                Block("OakStair_" + (i + 1), new Vector3(-4.6f, height * 0.5f, 3.2f + (i + 0.5f) * 0.4f),
                    new Vector3(1.7f, height, 0.4f), floor, false);
                Block("StairNosing_" + (i + 1), new Vector3(-4.6f, height - 0.025f, 3.2f + i * 0.4f + 0.03f),
                    new Vector3(1.72f, 0.05f, 0.06f), darkWood, false);
            }
            // A single ramp collider gives CharacterController a continuous walkable surface.
            GameObject ramp = new GameObject("ContinuousStairCollider", typeof(BoxCollider));
            ramp.transform.SetParent(house, false);
            ramp.transform.localPosition = new Vector3(-4.6f, 1.5f, 6.0f);
            ramp.transform.localRotation = Quaternion.Euler(-26.565f, 0f, 0f);
            ramp.GetComponent<BoxCollider>().size = new Vector3(1.7f, 0.12f, 6.3f);
            for (int i = 0; i < 13; i++)
            {
                float z = 3.3f + i * 0.45f;
                float y = 0.3f + i * 0.2f;
                Block("StairHandrailPost", new Vector3(-3.66f, y + 0.46f, z), new Vector3(0.07f, 0.92f, 0.07f), metal);
            }
            Block("ContinuousStairHandrail", new Vector3(-3.66f, 2.38f, 6f), new Vector3(0.1f, 0.1f, 6.3f), darkWood)
                .transform.localRotation = Quaternion.Euler(-26.565f, 0, 0);
            for (int i = 0; i < 13; i++)
                Block("LandingGuardPost", new Vector3(-3.16f, 3.3f, 3.4f + i * 0.45f), new Vector3(0.055f, 1f, 0.055f), metal);
            Block("LandingGuardTop", new Vector3(-3.16f, 3.8f, 6.05f), new Vector3(0.09f, 0.08f, 5.8f), darkWood);
        }

        private static void Rooms()
        {
            // Ground floor: front living area, rear bedroom, and kitchen on the east.
            Block("BedroomPartitionLeft", new Vector3(-4.1f, 1.4f, 9.4f), new Vector3(3.8f, 2.8f, 0.18f), wall);
            Block("BedroomPartitionRight", new Vector3(-0.25f, 1.4f, 9.4f), new Vector3(2.1f, 2.8f, 0.18f), wall);
            Block("BedroomDoorHeader", new Vector3(-1.45f, 2.5f, 9.4f), new Vector3(1.2f, 0.6f, 0.18f), wall);
            Block("KitchenPartition", new Vector3(1.5f, 1.4f, 11.8f), new Vector3(0.18f, 2.8f, 4.4f), wall);
            Block("KitchenHalfWall", new Vector3(1.5f, 0.55f, 8.6f), new Vector3(0.18f, 1.1f, 2f), wall);
            Block("UpperBedroomWallLeft", new Vector3(-1.3f, 4.2f, 9), new Vector3(3.8f, 2.8f, 0.18f), wall);
            Block("UpperBedroomWallRight", new Vector3(3.2f, 4.2f, 9), new Vector3(2.1f, 2.8f, 0.18f), wall);
            Block("UpperBedroomDoorHeader", new Vector3(0.85f, 5.3f, 9), new Vector3(1.3f, 0.6f, 0.18f), wall);
            Block("UpperBedroomSideWall", new Vector3(0, 4.2f, 11.7f), new Vector3(0.18f, 2.8f, 4.6f), wall);
            Block("UpperHallRail", new Vector3(-3.2f, 3.45f, 11.6f), new Vector3(0.08f, 1.3f, 3.7f), metal);
        }

        private static void Furnish()
        {
            GameObject sofa = Place(SofaPrefab(), house, new Vector3(3.6f, 0, 6.9f), Quaternion.identity);
            Place("modern_coffee_table_01", house, new Vector3(3.6f, 0, 5.25f), Quaternion.identity);
            Place(SeatPrefab("modern_arm_chair_01", "LoungeSeat", new Vector3(0, 0, 0), new Vector3(0, 0, -1.15f)),
                house, new Vector3(1.2f, 0, 5.6f), Quaternion.Euler(0, 55, 0));
            Place(SeatPrefab("dining_chair_02", "DiningSeat", Vector3.zero, new Vector3(0, 0, -0.95f)),
                house, new Vector3(4.5f, 0, 8.1f), Quaternion.Euler(0, 180, 0));
            Place("modern_wooden_cabinet", house, new Vector3(3.9f, 0, 12.8f), Quaternion.identity);
            Place("side_table_01", house, new Vector3(1.8f, 0, 6.8f), Quaternion.identity);
            Place(BedPrefab(), house, new Vector3(-2.55f, 0, 11.85f), Quaternion.identity);
            Place(DrawerPrefab(), house, new Vector3(-4.9f, 0, 12.8f), Quaternion.Euler(0, 90, 0));
            Place(BedPrefab(), house, new Vector3(2.4f, 2.8f, 11.6f), Quaternion.identity);
            Place("side_table_01", house, new Vector3(4.35f, 2.8f, 11.3f), Quaternion.identity);
            Place("modern_wooden_cabinet", house, new Vector3(2.8f, 2.8f, 4), Quaternion.Euler(0, 180, 0));
            Place(CratePrefab(), house, new Vector3(0.9f, 0.25f, 4.1f), Quaternion.identity);
            Kitchen();
            Place("modern_ceiling_lamp_01", house, new Vector3(3.5f, 2.65f, 5.6f), Quaternion.Euler(0, 0, 180));
            Place("modern_ceiling_lamp_01", house, new Vector3(2.7f, 5.45f, 11.5f), Quaternion.Euler(0, 0, 180));
        }

        private static void Kitchen()
        {
            Block("KitchenBaseCabinet", new Vector3(4.9f, 0.44f, 10.9f), new Vector3(1.8f, 0.88f, 0.62f), darkWood);
            Block("KitchenCountertop", new Vector3(4.9f, 0.91f, 10.9f), new Vector3(1.9f, 0.07f, 0.68f), concrete);
            Block("KitchenIslandBase", new Vector3(3.25f, 0.4f, 8.5f), new Vector3(1.7f, 0.8f, 0.74f), trim);
            Block("KitchenIslandTop", new Vector3(3.25f, 0.84f, 8.5f), new Vector3(1.85f, 0.08f, 0.86f), concrete);
            Block("Sink", new Vector3(4.9f, 0.95f, 10.9f), new Vector3(0.62f, 0.06f, 0.38f), metal);
            for (int i = 0; i < 4; i++)
                Block("KitchenHandle", new Vector3(4.15f + i * 0.5f, 0.52f, 10.55f), new Vector3(0.25f, 0.025f, 0.03f), metal);
        }

        private static GameObject DoorPrefab()
        {
            string path = Interactables + "FrontDoor.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;
            GameObject root = new GameObject("FrontDoor", typeof(Interactable), typeof(HomeInteractable));
            Block("DoorLeaf", new Vector3(0.67f, 1.08f, 0), new Vector3(1.3f, 2.15f, 0.08f), darkWood, true, 0f, root.transform);
            Block("DoorVerticalInlay", new Vector3(0.27f, 1.1f, -0.052f), new Vector3(0.05f, 1.8f, 0.02f), metal, false, 0f, root.transform);
            Block("DoorHandle", new Vector3(1.14f, 1.05f, -0.09f), new Vector3(0.19f, 0.04f, 0.1f), metal, false, 0f, root.transform);
            root.GetComponent<HomeInteractable>().Configure(HomeAction.Door, root.transform, null, null, Vector3.zero, new Vector3(0, -100, 0));
            prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject DrawerPrefab()
        {
            string path = Interactables + "DrawerCabinet.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;
            GameObject root = new GameObject("DrawerCabinet", typeof(Interactable), typeof(HomeInteractable));
            HomeAssetBuilder.Model("drawer_cabinet", root.transform, Vector3.zero, Quaternion.identity);
            Transform drawer = null;
            foreach (Transform part in root.GetComponentsInChildren<Transform>())
                if (part.name == "drawer_cabinet_drawer_01") { drawer = part; break; }
            if (drawer == null) throw new System.InvalidOperationException("Drawer mesh is not independently movable.");
            root.GetComponent<HomeInteractable>().Configure(HomeAction.Drawer, drawer, null, null, new Vector3(0, 0, -0.35f), Vector3.zero);
            prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject SeatPrefab(string modelId, string name, Vector3 pose, Vector3 stand)
        {
            string path = Interactables + name + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;
            GameObject root = new GameObject(name, typeof(Interactable), typeof(HomeInteractable));
            HomeAssetBuilder.Model(modelId, root.transform, Vector3.zero, Quaternion.identity);
            Transform posePoint = Point("SeatPoint", root.transform, pose);
            Transform standPoint = Point("StandPoint", root.transform, stand);
            root.GetComponent<HomeInteractable>().Configure(HomeAction.Seat, null, posePoint, standPoint, Vector3.zero, Vector3.zero);
            prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject BedPrefab()
        {
            string path = Interactables + "ModernBed.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;
            GameObject root = new GameObject("ModernBed", typeof(Interactable), typeof(HomeInteractable));
            Block("LowOakFrame", new Vector3(0, 0.27f, 0), new Vector3(2.2f, 0.3f, 2.25f), darkWood, true, 0.04f, root.transform);
            Block("UpholsteredHeadboard", new Vector3(0, 0.78f, 1.01f), new Vector3(2.24f, 1.15f, 0.18f), fabric, true, 0.07f, root.transform);
            Block("Mattress", new Vector3(0, 0.54f, -0.06f), new Vector3(2.07f, 0.28f, 2.05f), bedding, true, 0.12f, root.transform);
            Block("Duvet", new Vector3(0, 0.72f, -0.4f), new Vector3(1.95f, 0.15f, 1.25f), fabric, false, 0.07f, root.transform);
            for (int side = -1; side <= 1; side += 2)
                Block("Pillow", new Vector3(side * 0.53f, 0.77f, 0.62f), new Vector3(0.84f, 0.14f, 0.43f), bedding, false, 0.06f, root.transform);
            Transform posePoint = Point("BedPosePoint", root.transform, new Vector3(0, 0, -0.1f));
            Transform standPoint = Point("BedExitPoint", root.transform, new Vector3(1.7f, 0, -0.4f));
            root.GetComponent<HomeInteractable>().Configure(HomeAction.Bed, null, posePoint, standPoint, Vector3.zero, Vector3.zero);
            prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject SofaPrefab()
        {
            string path = "Assets/Prefabs/Furniture/ContemporarySofa.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;
            GameObject root = new GameObject("ContemporarySofa");
            Block("OakPlinth", new Vector3(0, 0.15f, 0), new Vector3(2.45f, 0.21f, 0.94f), darkWood, true, 0.025f, root.transform);
            Block("SofaBase", new Vector3(0, 0.39f, 0), new Vector3(2.55f, 0.34f, 1.08f), fabric, true, 0.13f, root.transform);
            Block("SofaBack", new Vector3(0, 0.78f, 0.43f), new Vector3(2.55f, 0.76f, 0.27f), fabric, true, 0.12f, root.transform);
            for (int side = -1; side <= 1; side += 2)
            {
                Block("SofaArm", new Vector3(side * 1.16f, 0.62f, -0.02f), new Vector3(0.29f, 0.54f, 1.06f), fabric, true, 0.12f, root.transform);
                Block("BackCushion", new Vector3(side * 0.57f, 0.69f, 0.21f), new Vector3(1.02f, 0.55f, 0.19f), bedding, false, 0.08f, root.transform);
                Block("SeatCushion", new Vector3(side * 0.57f, 0.6f, -0.22f), new Vector3(1.03f, 0.16f, 0.68f), bedding, false, 0.07f, root.transform);
            }
            prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject CratePrefab()
        {
            string path = Interactables + "CarryBox.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) return prefab;
            GameObject root = new GameObject("CarryBox", typeof(Rigidbody), typeof(BoxCollider), typeof(PickupItem));
            root.GetComponent<BoxCollider>().size = new Vector3(0.48f, 0.42f, 0.48f);
            root.GetComponent<Rigidbody>().mass = 0.9f;
            root.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            root.GetComponent<PickupItem>().Initialize("储物箱");
            Block("CrateBody", Vector3.zero, new Vector3(0.48f, 0.42f, 0.48f), floor, false, 0.02f, root.transform);
            for (int side = -1; side <= 1; side += 2)
            {
                Block("MetalBand", new Vector3(side * 0.16f, 0, -0.245f), new Vector3(0.035f, 0.43f, 0.012f), metal, false, 0f, root.transform);
                Block("SideHandle", new Vector3(side * 0.245f, 0.06f, 0), new Vector3(0.02f, 0.055f, 0.15f), metal, false, 0f, root.transform);
            }
            prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static Transform Point(string name, Transform parent, Vector3 position)
        {
            Transform point = new GameObject(name).transform;
            point.SetParent(parent, false);
            point.localPosition = position;
            return point;
        }

        private static GameObject Place(string id, Transform parent, Vector3 position, Quaternion rotation)
        {
            return HomeAssetBuilder.Model(id, parent, position, rotation);
        }

        private static GameObject Place(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            return instance;
        }

        private static void Lighting()
        {
            const string skyPath = "Assets/Art/Materials/Generated/DaylightSky.mat";
            Material sky = AssetDatabase.LoadAssetAtPath<Material>(skyPath);
            if (sky == null)
            {
                sky = new Material(Shader.Find("Skybox/Panoramic"));
                sky.name = "DaylightSky";
                sky.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Assets/Art/Environment/HDRI/kloofendal_48d_partly_cloudy_2k.hdr"));
                sky.SetFloat("_Exposure", 0.55f);
                AssetDatabase.CreateAsset(sky, skyPath);
            }
            RenderSettings.skybox = sky;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 0.55f;
            GameObject sun = new GameObject("Daylight", typeof(Light));
            sun.transform.SetParent(house, false);
            sun.transform.rotation = Quaternion.Euler(48, -36, 0);
            Light day = sun.GetComponent<Light>();
            day.type = LightType.Directional;
            day.intensity = 0.8f;
            day.color = new Color(1f, 0.96f, 0.89f);
            day.shadows = LightShadows.Soft;
            RenderSettings.sun = day;
            WarmLight(new Vector3(3.5f, 2.45f, 5.8f), 8f, 2.2f);
            WarmLight(new Vector3(-2.3f, 2.4f, 11.5f), 6f, 1.7f);
            WarmLight(new Vector3(3.9f, 2.4f, 10.8f), 6f, 1.6f);
            WarmLight(new Vector3(2.6f, 5.25f, 11.2f), 8f, 2.2f);
            WarmLight(new Vector3(-4f, 5.2f, 5.5f), 6f, 1.6f);
        }

        private static void WarmLight(Vector3 position, float range, float intensity)
        {
            GameObject source = new GameObject("WarmCeilingLight", typeof(Light));
            source.transform.SetParent(house, false);
            source.transform.position = position;
            Light light = source.GetComponent<Light>();
            light.type = LightType.Point;
            light.range = range;
            light.intensity = intensity;
            light.color = new Color(1f, 0.82f, 0.65f);
        }
    }
}
