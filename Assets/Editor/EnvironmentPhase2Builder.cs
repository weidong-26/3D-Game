using System.IO;
using LightweightGame.Interaction;
using LightweightGame.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LightweightGame.Editor
{
    public static class EnvironmentPhase2Builder
    {
        private const string SchoolScene = "Assets/Scenes/School_Environment_V1.unity";
        private const string ParkScene = "Assets/Scenes/Park_Environment_V1.unity";
        private const string UrbanScene = "Assets/Scenes/Urban_Environment_V1.unity";
        private const string SelectScene = "Assets/Scenes/Environment_Select_V2.unity";

        private static Transform root;
        private static Material asphalt, paving, grass, tile, plaster, metal, timber, glass, chalk, water, white, yellow, red;

        public static void BuildAll()
        {
            EnvironmentSetup.ConfigureUrp();
            HomeAssetBuilder.Prepare();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Materials();
            PrepareModelPrefabs();
            BuildSchool();
            BuildPark();
            BuildUrban();
            BuildSelection();
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(SelectScene, true),
                new EditorBuildSettingsScene("Assets/Scenes/Home_Environment_V2.unity", true),
                new EditorBuildSettingsScene(SchoolScene, true),
                new EditorBuildSettingsScene(ParkScene, true),
                new EditorBuildSettingsScene(UrbanScene, true),
                new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true)
            };
            AssetDatabase.SaveAssets();
            Debug.Log("PHASE2_SCENES_BUILT school park urban selector");
        }

        private static void Materials()
        {
            const string source = "Assets/Art/EnvironmentPhase2/ambientCG/";
            asphalt = HomeAssetBuilder.Surface("P2_Asphalt", source + "Asphalt033", "Asphalt033", Color.white);
            paving = HomeAssetBuilder.Surface("P2_Paving", source + "PavingStones151", "PavingStones151", Color.white);
            grass = HomeAssetBuilder.Surface("P2_Grass", source + "Grass005", "Grass005", new Color(0.8f, 0.9f, 0.8f));
            tile = HomeAssetBuilder.Surface("P2_Tile", source + "Tiles141", "Tiles141", Color.white);
            plaster = HomeAssetBuilder.Surface("P2_Plaster", source + "PaintedPlaster017", "PaintedPlaster017", new Color(0.92f, 0.92f, 0.9f));
            metal = HomeAssetBuilder.Solid("P2_CharcoalMetal", new Color(0.12f, 0.15f, 0.17f), 0.72f, 0.66f);
            timber = HomeAssetBuilder.Solid("P2_WarmTimber", new Color(0.32f, 0.19f, 0.10f), 0f, 0.32f);
            glass = HomeAssetBuilder.Solid("P2_Glass", new Color(0.20f, 0.34f, 0.42f), 0.18f, 0.92f);
            chalk = HomeAssetBuilder.Solid("P2_ChalkGreen", new Color(0.035f, 0.15f, 0.12f), 0f, 0.16f);
            water = HomeAssetBuilder.Solid("P2_Water", new Color(0.05f, 0.28f, 0.36f), 0.15f, 0.9f);
            white = HomeAssetBuilder.Solid("P2_WhitePaint", new Color(0.88f, 0.89f, 0.87f), 0f, 0.25f);
            yellow = HomeAssetBuilder.Solid("P2_RoadYellow", new Color(0.95f, 0.65f, 0.05f), 0f, 0.3f);
            red = HomeAssetBuilder.Solid("P2_BrickRed", new Color(0.36f, 0.11f, 0.07f), 0f, 0.22f);
        }

        private static void PrepareModelPrefabs()
        {
            Transform holder = new GameObject("Phase2ModelPreparation").transform;
            string[] ids =
            {
                "SchoolChair_01", "SchoolDesk_01", "standing_chalkboard_01", "binder_notebook",
                "book_encyclopedia_set_01", "mounted_fluorescent_lights", "painted_wooden_bench",
                "fir_sapling", "shrub_04", "street_lamp_02", "metal_trash_can", "wooden_picnic_table",
                "modular_urban_apartments_facade", "modular_factory_facade", "fire_hydrant", "water_manhole_cover"
            };
            foreach (string id in ids) Object.DestroyImmediate(HomeAssetBuilder.EnvironmentModel(id, holder, Vector3.zero, Quaternion.identity));
            Object.DestroyImmediate(holder.gameObject);
            PrepareFacadeModule("modular_urban_apartments_facade", "ApartmentWindowModule",
                new[] { "wall_window_centered_large_01", "window_centered_large_01" },
                new Vector3(3f, 3f, 0.45f), new Vector3(1.5f, 1.5f, -0.08f));
            PrepareFacadeModule("modular_urban_apartments_facade", "ApartmentDoorModule",
                new[] { "wall_door_centered_large_01", "door_centered_large_01" },
                new Vector3(3f, 3f, 0.45f), new Vector3(1.5f, 1.5f, -0.08f));
            PrepareFacadeModule("modular_factory_facade", "FactoryWindowModule",
                new[] { "wall_window_centered_large_01", "window_centered_large_01" },
                new Vector3(3f, 3f, 0.45f), new Vector3(1.5f, 1.5f, -0.1f));
            PrepareFacadeModule("modular_factory_facade", "FactoryGarageModule",
                new[] { "wall_door_garage_double_01", "door_garage_double_01" },
                new Vector3(9f, 3f, 0.6f), new Vector3(4.5f, 1.5f, -0.15f));
        }

        private static void PrepareFacadeModule(string sourceId, string outputName, string[] partNames, Vector3 colliderSize, Vector3 colliderCenter)
        {
            string sourcePath = "Assets/Prefabs/EnvironmentPhase2/" + sourceId + ".prefab";
            GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (sourcePrefab == null) throw new FileNotFoundException("Missing approved source prefab", sourcePath);
            GameObject source = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
            GameObject module = new GameObject(outputName);
            foreach (string partName in partNames)
            {
                Transform part = FindChild(source.transform, partName);
                if (part == null) throw new System.Exception(sourceId + " missing module part " + partName);
                GameObject copy = Object.Instantiate(part.gameObject, module.transform);
                copy.name = partName;
                copy.transform.localPosition = Vector3.zero;
                copy.transform.localRotation = part.localRotation;
                copy.transform.localScale = part.localScale;
            }
            BoxCollider collider = module.AddComponent<BoxCollider>();
            collider.size = colliderSize;
            collider.center = colliderCenter;
            string outputPath = "Assets/Prefabs/EnvironmentPhase2/" + outputName + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(module, outputPath);
            Object.DestroyImmediate(source);
            Object.DestroyImmediate(module);
        }

        private static Transform FindChild(Transform parent, string name)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
            return null;
        }

        private static void BuildSchool()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            root = new GameObject("SchoolEnvironment").transform;
            Block("SchoolSafeGround", new Vector3(0, -0.24f, 5), new Vector3(42, 0.4f, 42), paving);
            Block("ClassroomFloor", new Vector3(0, -0.08f, 6), new Vector3(14, 0.16f, 12), tile);
            Block("SchoolCorridorFloor", new Vector3(0, -0.07f, -2.5f), new Vector3(24, 0.16f, 5), tile);
            Block("ClassroomCeiling", new Vector3(0, 3.55f, 6), new Vector3(14.4f, 0.18f, 12.4f), white);
            Block("CorridorCeiling", new Vector3(0, 3.55f, -2.5f), new Vector3(24.4f, 0.18f, 5.4f), white);
            Block("ClassroomBackWall", new Vector3(0, 1.75f, 12), new Vector3(14.4f, 3.5f, 0.22f), plaster);
            Block("ClassroomWestWall", new Vector3(-7f, 1.75f, 6), new Vector3(0.22f, 3.5f, 12), plaster);
            Block("ClassroomEastWall", new Vector3(7f, 1.75f, 6), new Vector3(0.22f, 3.5f, 12), plaster);
            Block("FrontWallLeft", new Vector3(-2.4f, 1.75f, 0), new Vector3(9.2f, 3.5f, 0.22f), plaster);
            Block("FrontWallRight", new Vector3(6.0f, 1.75f, 0), new Vector3(2.0f, 3.5f, 0.22f), plaster);
            Block("DoorHeader", new Vector3(4.6f, 3.05f, 0), new Vector3(0.8f, 0.9f, 0.22f), plaster);
            PlacePrefab("Assets/Prefabs/Interactables/FrontDoor.prefab", new Vector3(4.05f, 0, 0.02f), Quaternion.Euler(0, 180, 0), "ClassroomDoor");

            Block("CorridorBackWallLeft", new Vector3(-8.5f, 1.75f, -5), new Vector3(7f, 3.5f, 0.22f), plaster);
            Block("CorridorBackWallRight", new Vector3(8.5f, 1.75f, -5), new Vector3(7f, 3.5f, 0.22f), plaster);
            Block("CorridorEndWest", new Vector3(-12f, 1.75f, -2.5f), new Vector3(0.22f, 3.5f, 5), plaster);
            Block("CorridorEndEast", new Vector3(12f, 1.75f, -2.5f), new Vector3(0.22f, 3.5f, 5), plaster);
            for (int i = 0; i < 5; i++)
            {
                float x = -8f + i * 4f;
                Block("CorridorWindowGlass_" + i, new Vector3(x, 2.05f, -4.89f), new Vector3(2.8f, 1.5f, 0.05f), glass, false);
                Block("CorridorWindowSill_" + i, new Vector3(x, 1.26f, -4.86f), new Vector3(3f, 0.08f, 0.14f), metal);
                Block("CorridorWindowMullion_" + i, new Vector3(x, 2.05f, -4.83f), new Vector3(0.06f, 1.55f, 0.08f), metal);
            }
            Block("ExteriorEntryLanding", new Vector3(0, -0.04f, -7.5f), new Vector3(7f, 0.1f, 5f), paving);

            GameObject board = Model("standing_chalkboard_01", new Vector3(0, 0, 11.55f), Quaternion.Euler(0, 180, 0));
            board.name = "ClassroomBlackboard";
            Model("SchoolDesk_01", new Vector3(0, 0, 10.2f), Quaternion.identity).name = "TeacherDesk";
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 3; col++)
                {
                    int index = row * 3 + col + 1;
                    float x = -3.7f + col * 3.7f;
                    float z = 7.8f - row * 1.75f;
                    GameObject desk = Model("SchoolDesk_01", new Vector3(x, 0, z), Quaternion.identity);
                    desk.name = "StudentDesk_" + index;
                    GameObject chair = Model("SchoolChair_01", new Vector3(x, 0, z - 0.78f), Quaternion.identity);
                    chair.name = "StudentChair_" + index;
                    if (index == 1) AddSeat(chair, new Vector3(0, 0.46f, 0), new Vector3(0, 0, -0.85f));
                    if (row == 0) Model("binder_notebook", new Vector3(x, 0.77f, z), Quaternion.identity).name = "Notebook_" + index;
                }
            Model("book_encyclopedia_set_01", new Vector3(-5.7f, 0.82f, 10.7f), Quaternion.Euler(0, 90, 0)).name = "ClassroomBooks";
            for (int x = -4; x <= 4; x += 4)
                for (int z = 3; z <= 9; z += 3)
                    Model("mounted_fluorescent_lights", new Vector3(x, 3.34f, z), Quaternion.identity).name = "ClassroomLight";
            for (int x = -8; x <= 8; x += 4)
                Model("mounted_fluorescent_lights", new Vector3(x, 3.34f, -2.5f), Quaternion.identity).name = "CorridorLight";
            PointLight("ClassroomFillA", new Vector3(-3.5f, 2.35f, 4.3f), 1.5f, 8f);
            PointLight("ClassroomFillB", new Vector3(3.5f, 2.35f, 4.3f), 1.5f, 8f);
            PointLight("ClassroomFillC", new Vector3(-3.5f, 2.35f, 9.0f), 1.5f, 8f);
            PointLight("ClassroomFillD", new Vector3(3.5f, 2.35f, 9.0f), 1.5f, 8f);
            PointLight("CorridorFillA", new Vector3(-6f, 2.35f, -2.5f), 1.2f, 7f);
            PointLight("CorridorFillB", new Vector3(6f, 2.35f, -2.5f), 1.2f, 7f);

            Spawn(new Vector3(0, 0.02f, -3.2f));
            Lighting(new Color(0.93f, 0.96f, 1f), 1.1f, 48f, -30f);
            Save(scene, SchoolScene, "Assets/Prefabs/Buildings/SchoolEnvironment.prefab");
        }

        private static void BuildPark()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            root = new GameObject("ParkEnvironment").transform;
            Block("ParkSafeGround", new Vector3(0, -0.22f, 0), new Vector3(64, 0.4f, 64), grass);
            Block("ParkMainPath", new Vector3(0, -0.01f, 0), new Vector3(5, 0.08f, 58), paving);
            Block("ParkCrossPath", new Vector3(0, -0.005f, 0), new Vector3(58, 0.09f, 5), paving);
            Block("CentralPlaza", new Vector3(0, 0.005f, 0), new Vector3(13, 0.1f, 13), paving);
            Block("PondWater", new Vector3(-15f, -0.04f, 14f), new Vector3(12f, 0.08f, 8f), water, false, 0.04f);
            Block("PondNorthEdge", new Vector3(-15f, 0.05f, 18.2f), new Vector3(13f, 0.28f, 0.35f), timber);
            Block("PondSouthEdge", new Vector3(-15f, 0.05f, 9.8f), new Vector3(13f, 0.28f, 0.35f), timber);
            Block("PondWestEdge", new Vector3(-21.3f, 0.05f, 14f), new Vector3(0.35f, 0.28f, 8.8f), timber);
            Block("PondEastEdge", new Vector3(-8.7f, 0.05f, 14f), new Vector3(0.35f, 0.28f, 8.8f), timber);

            Vector3[] treePositions =
            {
                new Vector3(-24,0,-23), new Vector3(-15,0,-24), new Vector3(-7,0,-23), new Vector3(9,0,-24),
                new Vector3(20,0,-20), new Vector3(25,0,-8), new Vector3(24,0,10), new Vector3(20,0,23),
                new Vector3(8,0,25), new Vector3(-5,0,24), new Vector3(-23,0,23), new Vector3(-25,0,2)
            };
            for (int i = 0; i < treePositions.Length; i++)
            {
                GameObject tree = Model("fir_sapling", treePositions[i], Quaternion.Euler(0, i * 31f, 0));
                tree.name = "ParkTree_" + (i + 1);
                SetActive(tree, "fir_sapling_b", false);
                SetActive(tree, "fir_sapling_c", false);
                tree.transform.localScale = Vector3.one * (3.4f + (i % 3) * 0.25f);
                BoxCollider collider = tree.GetComponent<BoxCollider>();
                if (collider != null) { collider.size = new Vector3(0.2f, 1.3f, 0.2f); collider.center = new Vector3(0, 0.65f, 0); }
            }
            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 16f, 0, Mathf.Sin(angle) * 16f);
                GameObject shrub = Model("shrub_04", position, Quaternion.Euler(0, i * 27f, 0));
                shrub.name = "ParkShrub_" + (i + 1);
                shrub.transform.localScale = Vector3.one * (2.6f + (i % 3) * 0.2f);
            }
            for (int i = 0; i < 8; i++)
            {
                bool vertical = i < 4;
                float axis = -15f + (i % 4) * 10f;
                Vector3 position = vertical ? new Vector3(3.8f, 0, axis) : new Vector3(axis, 0, 3.8f);
                Quaternion rotation = vertical ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 90, 0);
                GameObject bench = Model("painted_wooden_bench", position, rotation);
                bench.name = "ParkBench_" + (i + 1);
                if (i == 0) AddSeat(bench, new Vector3(0, 0.48f, 0), new Vector3(0, 0, -1.1f));
            }
            for (int i = 0; i < 10; i++)
            {
                bool alongZ = i < 5;
                float axis = -20f + (i % 5) * 10f;
                Vector3 position = alongZ ? new Vector3(-4.1f, 0, axis) : new Vector3(axis, 0, -4.1f);
                GameObject lamp = Model("street_lamp_02", position, Quaternion.identity);
                lamp.name = "ParkLamp_" + (i + 1);
                lamp.transform.localScale = Vector3.one * 2.2f;
            }
            GameObject parkTrash = Model("metal_trash_can", new Vector3(5.2f, 0, 5.2f), Quaternion.identity);
            parkTrash.name = "ParkTrashCan";
            DisableRenderersContaining(parkTrash, "_rust");
            Model("wooden_picnic_table", new Vector3(16f, 0, 13f), Quaternion.Euler(0, 25, 0)).name = "PicnicTable";
            Pergola(new Vector3(16f, 0, 13f));
            Spawn(new Vector3(0, 0.02f, -22f));
            Lighting(new Color(1f, 0.94f, 0.82f), 1.25f, 42f, -38f);
            Save(scene, ParkScene, "Assets/Prefabs/Buildings/ParkEnvironment.prefab");
        }

        private static void BuildUrban()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            root = new GameObject("UrbanEnvironment").transform;
            Block("UrbanSafeGround", new Vector3(0, -0.3f, 0), new Vector3(90, 0.5f, 90), paving);
            Block("UrbanRoadNorthSouth", new Vector3(0, -0.03f, 0), new Vector3(14, 0.12f, 86), asphalt);
            Block("UrbanRoadEastWest", new Vector3(0, -0.02f, 0), new Vector3(86, 0.13f, 14), asphalt);
            Block("WestSidewalk", new Vector3(-9.5f, 0.02f, 0), new Vector3(5, 0.16f, 86), paving);
            Block("EastSidewalk", new Vector3(9.5f, 0.02f, 0), new Vector3(5, 0.16f, 86), paving);
            Block("NorthSidewalk", new Vector3(0, 0.03f, 9.5f), new Vector3(86, 0.17f, 5), paving);
            Block("SouthSidewalk", new Vector3(0, 0.03f, -9.5f), new Vector3(86, 0.17f, 5), paving);
            for (int i = -3; i <= 3; i++)
            {
                Block("CrosswalkNS_" + i, new Vector3(i * 1.6f, 0.055f, 0), new Vector3(0.8f, 0.025f, 6f), white, false);
                Block("CrosswalkEW_" + i, new Vector3(0, 0.056f, i * 1.6f), new Vector3(6f, 0.025f, 0.8f), white, false);
            }
            for (int z = -34; z <= 34; z += 8)
            {
                Block("CenterLineNS", new Vector3(0, 0.045f, z), new Vector3(0.18f, 0.02f, 4f), yellow, false);
                Block("CenterLineEW", new Vector3(z, 0.046f, 0), new Vector3(4f, 0.02f, 0.18f), yellow, false);
            }
            Building("NorthWestBlock", new Vector3(-25, 0, 25), new Vector3(23, 15, 22), red, 5, 5);
            Building("NorthEastBlock", new Vector3(25, 0, 25), new Vector3(23, 18, 22), plaster, 6, 5, false);
            Building("SouthWestBlock", new Vector3(-25, 0, -25), new Vector3(23, 12, 22), plaster, 4, 5, false);
            Building("SouthEastBlock", new Vector3(25, 0, -25), new Vector3(23, 15, 22), red, 5, 5);

            Transform apartments = new GameObject("UrbanApartmentFacade").transform;
            apartments.SetParent(root, false);
            for (int floor = 0; floor < 6; floor++)
                for (int bay = 0; bay < 7; bay++)
                {
                    bool entrance = floor == 0 && bay == 3;
                    string prefab = "Assets/Prefabs/EnvironmentPhase2/" + (entrance ? "ApartmentDoorModule" : "ApartmentWindowModule") + ".prefab";
                    PlacePrefab(prefab, new Vector3(14.5f + bay * 3f, floor * 3f, 13.88f), Quaternion.identity,
                        "ApartmentFacade_" + floor + "_" + bay, apartments);
                }

            Transform factory = new GameObject("UrbanFactoryFacade").transform;
            factory.SetParent(root, false);
            PlacePrefab("Assets/Prefabs/EnvironmentPhase2/FactoryGarageModule.prefab", new Vector3(-15.5f, 0, -13.88f),
                Quaternion.Euler(0, 180, 0), "FactoryGarage_A", factory);
            PlacePrefab("Assets/Prefabs/EnvironmentPhase2/FactoryGarageModule.prefab", new Vector3(-24.5f, 0, -13.88f),
                Quaternion.Euler(0, 180, 0), "FactoryGarage_B", factory);
            for (int floor = 1; floor < 4; floor++)
                for (int bay = 0; bay < 7; bay++)
                    PlacePrefab("Assets/Prefabs/EnvironmentPhase2/FactoryWindowModule.prefab",
                        new Vector3(-14.5f - bay * 3f, floor * 3f, -13.88f), Quaternion.Euler(0, 180, 0),
                        "FactoryFacade_" + floor + "_" + bay, factory);

            for (int i = 0; i < 12; i++)
            {
                bool east = i % 2 == 0;
                float z = -33f + (i / 2) * 13f;
                GameObject lamp = Model("street_lamp_02", new Vector3(east ? 12.4f : -12.4f, 0.12f, z), Quaternion.identity);
                lamp.name = "UrbanStreetLamp_" + (i + 1);
                lamp.transform.localScale = Vector3.one * 2.2f;
            }
            for (int i = 0; i < 6; i++)
            {
                float x = -34f + i * 13f;
                Model("water_manhole_cover", new Vector3(x, 0.045f, -2.8f), Quaternion.identity).name = "Manhole_" + (i + 1);
            }
            Model("fire_hydrant", new Vector3(12f, 0.12f, 11.5f), Quaternion.identity).name = "FireHydrant";
            GameObject urbanTrash = Model("metal_trash_can", new Vector3(-12f, 0.12f, -11.5f), Quaternion.identity);
            urbanTrash.name = "UrbanTrashCan";
            DisableRenderersContaining(urbanTrash, "_rust");
            Model("painted_wooden_bench", new Vector3(13.4f, 0.12f, -17f), Quaternion.Euler(0, -90, 0)).name = "UrbanBench";
            Spawn(new Vector3(-3.2f, 0.08f, -18f));
            Lighting(new Color(0.92f, 0.94f, 1f), 1.05f, 52f, -24f);
            Save(scene, UrbanScene, "Assets/Prefabs/Buildings/UrbanEnvironment.prefab");
        }

        private static void BuildSelection()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("EnvironmentSelection", typeof(EnvironmentSelectionUI));
            EditorSceneManager.SaveScene(scene, SelectScene);
        }

        private static void Building(string name, Vector3 position, Vector3 size, Material body, int floors, int bays, bool windows = true)
        {
            Transform building = new GameObject(name).transform;
            building.SetParent(root, false);
            Block(name + "_Body", position + Vector3.up * (size.y * 0.5f), size, body, true, 0, building);
            float frontZ = position.z - Mathf.Sign(position.z) * (size.z * 0.5f + 0.02f);
            if (windows)
                for (int floorIndex = 0; floorIndex < floors; floorIndex++)
                    for (int bay = 0; bay < bays; bay++)
                    {
                        float x = position.x - size.x * 0.38f + bay * (size.x * 0.76f / (bays - 1));
                        float y = 1.5f + floorIndex * ((size.y - 1.8f) / floors);
                        Block(name + "_Window", new Vector3(x, y, frontZ), new Vector3(2.1f, 1.45f, 0.08f), glass, false, 0, building);
                        Block(name + "_WindowTop", new Vector3(x, y + 0.78f, frontZ), new Vector3(2.25f, 0.08f, 0.12f), metal, false, 0, building);
                    }
            Block(name + "_RoofTrim", position + new Vector3(0, size.y + 0.18f, 0), new Vector3(size.x + 0.35f, 0.35f, size.z + 0.35f), metal, true, 0, building);
        }

        private static void Pergola(Vector3 center)
        {
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    Block("PergolaPost", center + new Vector3(x * 3f, 1.5f, z * 2.5f), new Vector3(0.22f, 3f, 0.22f), timber);
            for (int i = -4; i <= 4; i++)
                Block("PergolaSlat", center + new Vector3(i * 0.72f, 3.05f, 0), new Vector3(0.18f, 0.18f, 5.5f), timber);
        }

        private static GameObject Block(string name, Vector3 center, Vector3 size, Material material, bool collider = true, float rounded = 0f, Transform parent = null)
        {
            GameObject instance = HomeAssetBuilder.Block(name, parent != null ? parent : root, center, size, material, collider, rounded);
            instance.isStatic = true;
            return instance;
        }

        private static GameObject Model(string id, Vector3 position, Quaternion rotation)
        {
            return HomeAssetBuilder.EnvironmentModel(id, root, position, rotation);
        }

        private static GameObject PlacePrefab(string path, Vector3 position, Quaternion rotation, string name, Transform parent = null)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) throw new FileNotFoundException("Missing prefab", path);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = name;
            instance.transform.SetParent(parent != null ? parent : root, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            return instance;
        }

        private static void SetActive(GameObject parent, string childName, bool active)
        {
            Transform child = FindChild(parent.transform, childName);
            if (child != null) child.gameObject.SetActive(active);
        }

        private static void DisableRenderersContaining(GameObject parent, string text)
        {
            foreach (Renderer renderer in parent.GetComponentsInChildren<Renderer>(true))
                if (renderer.name.Contains(text)) renderer.gameObject.SetActive(false);
        }

        private static void PointLight(string name, Vector3 position, float intensity, float range)
        {
            GameObject lightObject = new GameObject(name, typeof(Light));
            lightObject.transform.SetParent(root, false);
            lightObject.transform.localPosition = position;
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.94f, 0.84f);
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.Soft;
        }

        private static void AddSeat(GameObject chair, Vector3 pose, Vector3 stand)
        {
            if (chair.GetComponent<Interactable>() == null) chair.AddComponent<Interactable>();
            HomeInteractable home = chair.GetComponent<HomeInteractable>();
            if (home == null) home = chair.AddComponent<HomeInteractable>();
            Transform posePoint = new GameObject("SeatPose").transform;
            posePoint.SetParent(chair.transform, false);
            posePoint.localPosition = pose;
            Transform standPoint = new GameObject("StandPoint").transform;
            standPoint.SetParent(chair.transform, false);
            standPoint.localPosition = stand;
            home.Configure(HomeAction.Seat, null, posePoint, standPoint, Vector3.zero, Vector3.zero);
        }

        private static void Spawn(Vector3 position)
        {
            Transform spawn = new GameObject("PlayerSpawn").transform;
            spawn.SetParent(root, false);
            spawn.localPosition = position;
        }

        private static void Lighting(Color color, float intensity, float pitch, float yaw)
        {
            RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Generated/DaylightSky.mat");
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 0.62f;
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.58f, 0.65f, 0.68f);
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 48f;
            RenderSettings.fogEndDistance = 125f;
            GameObject sun = new GameObject("Sun", typeof(Light));
            sun.transform.SetParent(root, false);
            sun.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            Light light = sun.GetComponent<Light>();
            light.type = LightType.Directional;
            light.color = color;
            light.intensity = intensity;
            light.shadows = LightShadows.Soft;
            RenderSettings.sun = light;
        }

        private static void Save(Scene scene, string scenePath, string prefabPath)
        {
            string directory = Path.GetDirectoryName(prefabPath).Replace('\\', '/');
            if (!AssetDatabase.IsValidFolder(directory)) AssetDatabase.CreateFolder("Assets/Prefabs", "Buildings");
            PrefabUtility.SaveAsPrefabAssetAndConnect(root.gameObject, prefabPath, InteractionMode.AutomatedAction);
            EditorSceneManager.SaveScene(scene, scenePath);
        }
    }
}
