using System.Collections.Generic;
using UnityEngine;

namespace LightweightGame.Runtime
{
    internal static class ProceduralAvatarFactory
    {
        public static CharacterAppearance Create(GameObject player)
        {
            Transform visual = new GameObject("UnifiedCharacterVisual").transform;
            visual.SetParent(player.transform, false);

            Material skin = RuntimeFactory.Material("Skin", new Color(0.75f, 0.48f, 0.32f));
            Material shirt = RuntimeFactory.Material("Shirt", new Color(0.13f, 0.35f, 0.62f));
            Material trousers = RuntimeFactory.Material("Trousers", new Color(0.08f, 0.11f, 0.18f));
            Material shoe = RuntimeFactory.Material("Shoes", new Color(0.035f, 0.04f, 0.05f));
            Material white = RuntimeFactory.Material("EyeWhite", Color.white);
            Material dark = RuntimeFactory.Material("Pupil", new Color(0.025f, 0.02f, 0.018f));
            Material lips = RuntimeFactory.Material("Lips", new Color(0.48f, 0.09f, 0.09f));
            Material hair = RuntimeFactory.Material("Hair", new Color(0.055f, 0.025f, 0.012f));

            List<Renderer> skinRenderers = new List<Renderer>();
            GameObject torso = new GameObject("Torso", typeof(MeshFilter), typeof(MeshRenderer));
            torso.transform.SetParent(visual, false);
            torso.transform.localPosition = new Vector3(0f, 1.12f, 0f);
            torso.transform.localScale = new Vector3(0.34f, 0.4f, 0.24f);
            torso.GetComponent<MeshFilter>().sharedMesh = ProceduralTorsoMesh.Create();
            torso.GetComponent<MeshRenderer>().sharedMaterial = shirt;
            GameObject leftLeg = RuntimeFactory.Primitive(PrimitiveType.Capsule, "LeftLeg", visual, new Vector3(-0.16f, 0.48f, 0f), new Vector3(0.13f, 0.40f, 0.13f), trousers);
            GameObject rightLeg = RuntimeFactory.Primitive(PrimitiveType.Capsule, "RightLeg", visual, new Vector3(0.16f, 0.48f, 0f), new Vector3(0.13f, 0.40f, 0.13f), trousers);
            GameObject leftShoe = RuntimeFactory.Primitive(PrimitiveType.Capsule, "LeftShoe", visual, new Vector3(-0.16f, 0.13f, 0.11f), new Vector3(0.14f, 0.24f, 0.12f), shoe);
            GameObject rightShoe = RuntimeFactory.Primitive(PrimitiveType.Capsule, "RightShoe", visual, new Vector3(0.16f, 0.13f, 0.11f), new Vector3(0.14f, 0.24f, 0.12f), shoe);
            leftShoe.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            rightShoe.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            GameObject leftArm = RuntimeFactory.Primitive(PrimitiveType.Capsule, "LeftArm", visual, new Vector3(-0.42f, 1.1f, 0f), new Vector3(0.11f, 0.38f, 0.11f), skin);
            GameObject rightArm = RuntimeFactory.Primitive(PrimitiveType.Capsule, "RightArm", visual, new Vector3(0.42f, 1.1f, 0f), new Vector3(0.11f, 0.38f, 0.11f), skin);
            leftArm.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
            rightArm.transform.localRotation = Quaternion.Euler(0f, 0f, 8f);
            skinRenderers.Add(RuntimeFactory.Primitive(PrimitiveType.Sphere, "LeftHand", leftArm.transform, new Vector3(0f, -0.96f, 0f), Vector3.one, skin).GetComponent<Renderer>());
            skinRenderers.Add(RuntimeFactory.Primitive(PrimitiveType.Sphere, "RightHand", rightArm.transform, new Vector3(0f, -0.96f, 0f), Vector3.one, skin).GetComponent<Renderer>());
            Transform handAnchor = new GameObject("RightHandAnchor").transform;
            handAnchor.SetParent(player.transform, false);
            handAnchor.localPosition = new Vector3(0.52f, 1.05f, 0.55f);
            skinRenderers.Add(leftArm.GetComponent<Renderer>());
            skinRenderers.Add(rightArm.GetComponent<Renderer>());

            Transform headRoot = new GameObject("HeadRoot").transform;
            headRoot.SetParent(visual, false);
            headRoot.localPosition = new Vector3(0f, 1.72f, 0f);

            GameObject headObject = new GameObject("MorphableHead");
            headObject.transform.SetParent(headRoot, false);
            SkinnedMeshRenderer head = headObject.AddComponent<SkinnedMeshRenderer>();
            head.sharedMesh = ProceduralHeadMesh.Create();
            head.sharedMaterial = skin;
            head.updateWhenOffscreen = true;
            skinRenderers.Add(head);

            Transform leftEye = CreateEye("LeftEye", headRoot, white, dark);
            Transform rightEye = CreateEye("RightEye", headRoot, white, dark);
            Transform leftBrow = RuntimeFactory.Primitive(PrimitiveType.Cube, "LeftBrow", headRoot, new Vector3(-0.13f, 0.2f, 0.245f), new Vector3(0.11f, 0.025f, 0.02f), hair).transform;
            Transform rightBrow = RuntimeFactory.Primitive(PrimitiveType.Cube, "RightBrow", headRoot, new Vector3(0.13f, 0.2f, 0.245f), new Vector3(0.11f, 0.025f, 0.02f), hair).transform;
            Transform nose = CreateContainer("Nose", headRoot);
            GameObject noseShape = RuntimeFactory.Primitive(PrimitiveType.Sphere, "NoseShape", nose, Vector3.zero, new Vector3(0.055f, 0.08f, 0.06f), skin);
            nose.localPosition = new Vector3(0f, -0.015f, 0.27f);
            skinRenderers.Add(noseShape.GetComponent<Renderer>());

            Transform upperLip = CreateContainer("UpperLip", headRoot);
            upperLip.localPosition = new Vector3(0f, -0.145f, 0.267f);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "UpperLipShape", upperLip, Vector3.zero, new Vector3(0.105f, 0.022f, 0.024f), lips);
            Transform lowerLip = CreateContainer("LowerLip", headRoot);
            lowerLip.localPosition = new Vector3(0f, -0.175f, 0.262f);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "LowerLipShape", lowerLip, Vector3.zero, new Vector3(0.105f, 0.024f, 0.025f), lips);

            GameObject[] hairStyles = CreateHairStyles(headRoot, hair);

            CharacterAppearance appearance = player.AddComponent<CharacterAppearance>();
            appearance.Initialize(head, leftEye, rightEye, nose, upperLip, lowerLip, skinRenderers, hairStyles);
            appearance.InitializeBody(visual, torso.transform, leftArm.transform, rightArm.transform, leftLeg.transform, rightLeg.transform, leftBrow, rightBrow, hair, shirt);
            appearance.HandAnchor = handAnchor;
            Animator animator = visual.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PrototypeAnimator");
            animator.applyRootMotion = false;
            return appearance;
        }

        private static Transform CreateEye(string name, Transform parent, Material white, Material dark)
        {
            Transform eye = CreateContainer(name, parent);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "White", eye, Vector3.zero, new Vector3(0.075f, 0.05f, 0.035f), white);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "Pupil", eye, new Vector3(0f, 0f, 0.032f), new Vector3(0.024f, 0.028f, 0.012f), dark);
            return eye;
        }

        private static GameObject[] CreateHairStyles(Transform head, Material material)
        {
            GameObject[] styles = new GameObject[3];

            styles[0] = new GameObject("Hair_Short");
            styles[0].transform.SetParent(head, false);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "ShortCap", styles[0].transform, new Vector3(0f, 0.38f, -0.01f), new Vector3(0.38f, 0.24f, 0.34f), material);

            styles[1] = new GameObject("Hair_Long");
            styles[1].transform.SetParent(head, false);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "LongCap", styles[1].transform, new Vector3(0f, 0.38f, -0.01f), new Vector3(0.38f, 0.24f, 0.34f), material);
            RuntimeFactory.Primitive(PrimitiveType.Capsule, "LongBack", styles[1].transform, new Vector3(0f, -0.05f, -0.26f), new Vector3(0.29f, 0.42f, 0.08f), material);

            styles[2] = new GameObject("Hair_Bun");
            styles[2].transform.SetParent(head, false);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "BunCap", styles[2].transform, new Vector3(0f, 0.38f, -0.01f), new Vector3(0.38f, 0.24f, 0.34f), material);
            RuntimeFactory.Primitive(PrimitiveType.Sphere, "Bun", styles[2].transform, new Vector3(0f, 0.43f, -0.14f), new Vector3(0.16f, 0.16f, 0.16f), material);
            return styles;
        }

        private static Transform CreateContainer(string name, Transform parent)
        {
            Transform transform = new GameObject(name).transform;
            transform.SetParent(parent, false);
            return transform;
        }
    }
}
