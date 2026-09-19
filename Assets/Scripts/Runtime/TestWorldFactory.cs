using UnityEngine;

namespace LightweightGame.Runtime
{
    internal static class TestWorldFactory
    {
        public static void Create()
        {
            Transform world = new GameObject("NeighborhoodPrototype").transform;
            Material grass = RuntimeFactory.Material("Grass", new Color(0.25f, 0.36f, 0.24f));
            Material road = RuntimeFactory.Material("Road", new Color(0.24f, 0.27f, 0.30f));
            Material wall = RuntimeFactory.Material("HousePlaster", new Color(0.82f, 0.78f, 0.68f));
            Material wood = RuntimeFactory.Material("Wood", new Color(0.43f, 0.27f, 0.16f));
            Material roof = RuntimeFactory.Material("Roof", new Color(0.27f, 0.31f, 0.35f));
            Material glass = RuntimeFactory.Material("WindowGlass", new Color(0.42f, 0.67f, 0.72f));
            Material fabric = RuntimeFactory.Material("Fabric", new Color(0.27f, 0.43f, 0.50f));
            Material leaf = RuntimeFactory.Material("Leaf", new Color(0.16f, 0.37f, 0.21f));

            Box("Ground", world, new Vector3(0f, -0.25f, 0f), new Vector3(32f, 0.5f, 32f), grass);
            Box("Street", world, new Vector3(0f, 0.015f, -11f), new Vector3(30f, 0.04f, 6f), road);
            Box("GardenWalk", world, new Vector3(0f, 0.02f, -3.5f), new Vector3(1.8f, 0.05f, 9f), road);
            Box("HouseFloor", world, new Vector3(0f, 0.05f, 5f), new Vector3(8f, 0.1f, 8f), wood);
            Box("BackWall", world, new Vector3(0f, 2f, 9f), new Vector3(8f, 4f, 0.24f), wall);
            Box("LeftWall", world, new Vector3(-4f, 2f, 5f), new Vector3(0.24f, 4f, 8f), wall);
            Box("RightWall", world, new Vector3(4f, 2f, 5f), new Vector3(0.24f, 4f, 8f), wall);
            Box("FrontWallLeft", world, new Vector3(-2.4f, 2f, 1f), new Vector3(3.2f, 4f, 0.24f), wall);
            Box("FrontWallRight", world, new Vector3(2.4f, 2f, 1f), new Vector3(3.2f, 4f, 0.24f), wall);
            Box("DoorLintel", world, new Vector3(0f, 3.5f, 1f), new Vector3(1.6f, 1f, 0.24f), wall);
            Box("Roof", world, new Vector3(0f, 4.25f, 5f), new Vector3(8.6f, 0.3f, 8.6f), roof);
            Box("LeftWindow", world, new Vector3(-2.5f, 2.1f, 0.85f), new Vector3(1.15f, 1.1f, 0.07f), glass, false);
            Box("RightWindow", world, new Vector3(2.5f, 2.1f, 0.85f), new Vector3(1.15f, 1.1f, 0.07f), glass, false);
            Box("SideWindow", world, new Vector3(4.16f, 2.1f, 5.2f), new Vector3(0.07f, 1.1f, 1.4f), glass, false);
            Box("DoorLeafOpen", world, new Vector3(-0.78f, 1.15f, 1.75f), new Vector3(0.12f, 2.3f, 1.4f), wood);

            Box("BedFrame", world, new Vector3(-2.25f, 0.4f, 7.25f), new Vector3(2.6f, 0.45f, 1.7f), wood);
            Box("Mattress", world, new Vector3(-2.25f, 0.73f, 7.25f), new Vector3(2.45f, 0.22f, 1.55f), fabric);
            Box("Pillow", world, new Vector3(-3f, 0.91f, 7.25f), new Vector3(0.55f, 0.14f, 1.1f), wall);
            Box("SofaSeat", world, new Vector3(2.5f, 0.5f, 7.3f), new Vector3(2.3f, 0.5f, 0.9f), fabric);
            Box("SofaBack", world, new Vector3(2.5f, 0.97f, 7.7f), new Vector3(2.3f, 0.8f, 0.22f), fabric);
            Box("DiningTable", world, new Vector3(2.3f, 0.78f, 3.6f), new Vector3(1.7f, 0.13f, 1.2f), wood);
            Box("TableLegA", world, new Vector3(1.65f, 0.37f, 3.18f), new Vector3(0.13f, 0.74f, 0.13f), wood);
            Box("TableLegB", world, new Vector3(2.95f, 0.37f, 4.02f), new Vector3(0.13f, 0.74f, 0.13f), wood);
            Box("DiningChair", world, new Vector3(2.3f, 0.48f, 2.45f), new Vector3(0.65f, 0.95f, 0.6f), wood);
            Box("Wardrobe", world, new Vector3(0.2f, 1.1f, 8.55f), new Vector3(1.35f, 2.2f, 0.65f), wood);
            Box("KitchenCounter", world, new Vector3(3.2f, 0.48f, 5.4f), new Vector3(1.2f, 0.95f, 1.5f), wood);
            for (int i = 0; i < 4; i++)
                Box("InteriorStair" + i, world, new Vector3(-3.1f, 0.12f + i * 0.24f, 2.45f + i * 0.48f), new Vector3(1.15f, 0.24f + i * 0.48f, 0.48f), wood);
            Box("StairLanding", world, new Vector3(-3.1f, 1.04f, 4.5f), new Vector3(1.15f, 0.15f, 0.8f), wood);

            for (int i = -1; i <= 1; i += 2)
            {
                for (int z = -14; z <= 14; z += 2)
                    Box("SideFence", world, new Vector3(i * 15.5f, 0.7f, z), new Vector3(0.18f, 1.4f, 1.85f), wood);
                for (int x = -14; x <= 14; x += 2)
                    Box("EndFence", world, new Vector3(x, 0.7f, i * 15.5f), new Vector3(1.85f, 1.4f, 0.18f), wood);
            }
            for (int i = -1; i <= 1; i += 2)
            {
                Tree(world, new Vector3(i * 8f, 0f, -4f), wood, leaf);
                Tree(world, new Vector3(i * 10f, 0f, 7f), wood, leaf);
                Lamp(world, new Vector3(i * 10f, 0f, -9f));
            }
            Box("PorchLight", world, new Vector3(0f, 3.2f, 0.65f), new Vector3(0.25f, 0.25f, 0.25f), glass, false);
            PointLight("IndoorWarmLight", world, new Vector3(0f, 3.55f, 5f), 9f, 2.2f);
            PointLight("PorchWarmLight", world, new Vector3(0f, 3.1f, 0.4f), 7f, 1.3f);
            CreateItem("Cup", world, new Vector3(2.3f, 1f, 3.6f), PrimitiveType.Cylinder, new Vector3(0.22f, 0.13f, 0.22f), wall);
            CreateItem("Box", world, new Vector3(3f, 0.32f, -3f), PrimitiveType.Cube, new Vector3(0.65f, 0.65f, 0.65f), wood);
            GameObject flashlight = CreateItem("Flashlight", world, new Vector3(-2.2f, 1.07f, 7.2f), PrimitiveType.Capsule, new Vector3(0.15f, 0.27f, 0.15f), road);
            flashlight.transform.GetChild(0).localRotation = Quaternion.Euler(90f, 0f, 0f);
            GameObject beam = new GameObject("FlashlightBeam", typeof(Light));
            beam.transform.SetParent(flashlight.transform, false);
            beam.transform.localPosition = new Vector3(0f, 0f, 0.25f);
            Light beamLight = beam.GetComponent<Light>();
            beamLight.type = LightType.Spot;
            beamLight.range = 12f;
            beamLight.spotAngle = 42f;
            beamLight.intensity = 4f;
            beamLight.enabled = false;
            flashlight.GetComponent<PickupItem>().Initialize("Flashlight", beamLight);
        }

        private static GameObject CreateItem(string name, Transform parent, Vector3 position, PrimitiveType shape, Vector3 scale, Material material)
        {
            GameObject item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            RuntimeFactory.Primitive(shape, "Mesh", item.transform, Vector3.zero, scale, material, true);
            Rigidbody body = item.AddComponent<Rigidbody>();
            body.mass = 0.5f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            item.AddComponent<PickupItem>().Initialize(name);
            return item;
        }

        private static GameObject Box(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool collider = true)
        {
            return RuntimeFactory.Primitive(PrimitiveType.Cube, name, parent, position, scale, material, collider);
        }

        private static void Tree(Transform parent, Vector3 position, Material trunk, Material leaves)
        {
            Transform tree = new GameObject("Tree").transform;
            tree.SetParent(parent, false);
            tree.localPosition = position;
            Box("Trunk", tree, new Vector3(0f, 1.2f, 0f), new Vector3(0.35f, 2.4f, 0.35f), trunk);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "Canopy", tree, new Vector3(0f, 3f, 0f), new Vector3(2.2f, 2.2f, 2.2f), leaves);
        }

        private static void Lamp(Transform parent, Vector3 position)
        {
            Material metal = RuntimeFactory.Material("LampMetal", new Color(0.12f, 0.14f, 0.16f));
            Box("StreetLamp", parent, position + new Vector3(0f, 1.7f, 0f), new Vector3(0.18f, 3.4f, 0.18f), metal);
            PointLight("StreetLight", parent, position + new Vector3(0f, 3.5f, 0f), 7f, 1.3f);
        }

        private static void PointLight(string name, Transform parent, Vector3 position, float range, float intensity)
        {
            GameObject lightObject = new GameObject(name, typeof(Light));
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.localPosition = position;
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Point;
            light.range = range;
            light.intensity = intensity;
            light.color = new Color(1f, 0.82f, 0.61f);
        }
    }
}
