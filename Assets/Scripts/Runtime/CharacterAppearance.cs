using System.Collections.Generic;
using LightweightGame.Core;
using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class CharacterAppearance : MonoBehaviour
    {
        public Transform HandAnchor { get; internal set; }
        private SkinnedMeshRenderer head;
        private Transform leftEye;
        private Transform rightEye;
        private Transform nose;
        private Transform upperLip;
        private Transform lowerLip;
        private Renderer[] skinRenderers;
        private GameObject[] hairStyles;
        private Transform visual;
        private Transform torso;
        private Transform leftArm;
        private Transform rightArm;
        private Transform leftLeg;
        private Transform rightLeg;
        private Transform leftBrow;
        private Transform rightBrow;
        private Material hairMaterial;
        private Material shirtMaterial;

        internal void InitializeBody(Transform root, Transform body, Transform armL, Transform armR, Transform legL, Transform legR, Transform browL, Transform browR, Material hair, Material shirt)
        {
            visual = root;
            torso = body;
            leftArm = armL;
            rightArm = armR;
            leftLeg = legL;
            rightLeg = legR;
            leftBrow = browL;
            rightBrow = browR;
            hairMaterial = hair;
            shirtMaterial = shirt;
        }

        internal void Initialize(
            SkinnedMeshRenderer headRenderer,
            Transform leftEyeTransform,
            Transform rightEyeTransform,
            Transform noseTransform,
            Transform upperLipTransform,
            Transform lowerLipTransform,
            List<Renderer> skin,
            GameObject[] hair)
        {
            head = headRenderer;
            leftEye = leftEyeTransform;
            rightEye = rightEyeTransform;
            nose = noseTransform;
            upperLip = upperLipTransform;
            lowerLip = lowerLipTransform;
            skinRenderers = skin.ToArray();
            hairStyles = hair;
        }

        public void Apply(AppearanceData data)
        {
            if (data == null || head == null)
                return;

            data.Normalize();
            SetMorph("FaceWidth", data.FaceWidth);
            SetMorph("FaceLength", data.FaceLength);
            SetMorph("ChinWidth", data.ChinWidth);
            SetMorph("ChinLength", data.ChinLength);
            SetMorph("Cheekbones", data.Cheekbones);
            SetMorph("EyeSize", data.EyeSize);
            SetMorph("EyeSpacing", data.EyeSpacing);
            SetMorph("NoseSize", data.NoseSize);
            SetMorph("NoseWidth", data.NoseWidth);
            SetMorph("MouthSize", data.MouthSize);
            SetMorph("LipThickness", data.LipThickness);

            float eyeSize = Mathf.Lerp(0.78f, 1.28f, data.EyeSize);
            float eyeSpacing = Mathf.Lerp(0.105f, 0.165f, data.EyeSpacing);
            SetEye(leftEye, -eyeSpacing, eyeSize);
            SetEye(rightEye, eyeSpacing, eyeSize);
            float browHeight = Mathf.Lerp(0.16f, 0.25f, data.BrowHeight);
            leftBrow.localPosition = new Vector3(-eyeSpacing, browHeight, 0.245f);
            rightBrow.localPosition = new Vector3(eyeSpacing, browHeight, 0.245f);
            float browAngle = Mathf.Lerp(-17f, 17f, data.BrowAngle);
            leftBrow.localRotation = Quaternion.Euler(0f, 0f, browAngle);
            rightBrow.localRotation = Quaternion.Euler(0f, 0f, -browAngle);

            nose.localScale = new Vector3(
                Mathf.Lerp(0.72f, 1.35f, data.NoseWidth),
                Mathf.Lerp(0.8f, 1.25f, data.NoseSize),
                Mathf.Lerp(0.78f, 1.35f, data.NoseSize));

            float mouthWidth = Mathf.Lerp(0.72f, 1.35f, data.MouthSize);
            float lipHeight = Mathf.Lerp(0.65f, 1.5f, data.LipThickness);
            upperLip.localScale = new Vector3(mouthWidth, lipHeight, 1f);
            lowerLip.localScale = new Vector3(mouthWidth, lipHeight, 1f);

            Color skin = Color.Lerp(new Color(0.34f, 0.17f, 0.09f), new Color(1f, 0.78f, 0.61f), data.SkinTone);
            for (int i = 0; i < skinRenderers.Length; i++)
                skinRenderers[i].material.color = skin;

            for (int i = 0; i < hairStyles.Length; i++)
                hairStyles[i].SetActive(i == data.HairStyle);
            hairMaterial.color = Color.Lerp(new Color(0.025f, 0.02f, 0.018f), new Color(0.75f, 0.53f, 0.27f), data.HairColor);
            shirtMaterial.color = data.Outfit == 1 ? new Color(0.24f, 0.42f, 0.30f) : data.Outfit == 2 ? new Color(0.57f, 0.23f, 0.25f) : new Color(0.13f, 0.35f, 0.62f);
            float width = Mathf.Lerp(0.86f, 1.14f, data.Build);
            visual.localScale = new Vector3(width, Mathf.Lerp(0.88f, 1.12f, data.Height), width);
            torso.localScale = new Vector3(data.BodyType == 0 ? 0.31f : 0.36f, 0.4f, 0.24f);
            float shoulder = Mathf.Lerp(0.37f, 0.52f, data.ShoulderWidth);
            leftArm.localPosition = new Vector3(-shoulder, 1.1f, 0f);
            rightArm.localPosition = new Vector3(shoulder, 1.1f, 0f);
            float legScale = Mathf.Lerp(0.36f, 0.44f, data.LegLength);
            leftLeg.localScale = new Vector3(0.13f, legScale, 0.13f);
            rightLeg.localScale = new Vector3(0.13f, legScale, 0.13f);
            CharacterController capsule = GetComponent<CharacterController>();
            capsule.height = 2f * visual.localScale.y;
            capsule.center = new Vector3(0f, capsule.height * 0.5f, 0f);
        }

        private void SetMorph(string name, float normalizedValue)
        {
            int index = head.sharedMesh.GetBlendShapeIndex(name);
            if (index >= 0)
                head.SetBlendShapeWeight(index, (normalizedValue - 0.5f) * 100f);
        }

        private static void SetEye(Transform eye, float x, float size)
        {
            eye.localPosition = new Vector3(x, 0.09f, 0.245f);
            eye.localScale = new Vector3(size, size, size);
        }
    }
}
