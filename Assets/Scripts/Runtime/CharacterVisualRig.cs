using UnityEngine;

namespace LightweightGame.Runtime
{
    public sealed class CharacterVisualRig : MonoBehaviour
    {
        private static readonly HumanBodyBones[] RequiredHumanoidBones =
        {
            HumanBodyBones.Hips,
            HumanBodyBones.Spine,
            HumanBodyBones.Head,
            HumanBodyBones.LeftHand,
            HumanBodyBones.RightHand,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.RightFoot
        };

        public Transform VisualRoot { get; private set; }
        public Animator Animator { get; private set; }
        public Transform CameraTarget { get; private set; }
        public Transform RightHandAnchor { get; private set; }

        public void Initialize(Transform visualRoot, Animator animator, Transform cameraFallback, Transform handFallback)
        {
            VisualRoot = visualRoot;
            Animator = animator;
            CameraTarget = GetHumanoidBone(HumanBodyBones.Head)
                ?? GetHumanoidBone(HumanBodyBones.Chest)
                ?? GetHumanoidBone(HumanBodyBones.UpperChest)
                ?? GetHumanoidBone(HumanBodyBones.Spine)
                ?? cameraFallback;
            RightHandAnchor = GetHumanoidBone(HumanBodyBones.RightHand) ?? handFallback;
            ValidateHumanoidRig();
        }

        public Transform GetHumanoidBone(HumanBodyBones bone)
        {
            if (Animator == null || Animator.avatar == null || !Animator.avatar.isHuman)
                return null;
            return Animator.GetBoneTransform(bone);
        }

        private void ValidateHumanoidRig()
        {
            if (Animator == null || Animator.avatar == null || !Animator.avatar.isHuman)
                return;

            foreach (HumanBodyBones bone in RequiredHumanoidBones)
                if (Animator.GetBoneTransform(bone) == null)
                    Debug.LogWarning("Humanoid Avatar is missing the " + bone + " bone.", this);

            SkinnedMeshRenderer[] meshes = VisualRoot.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer mesh in meshes)
                if (mesh.rootBone == null)
                    Debug.LogWarning("Skinned mesh " + mesh.name + " has no Root Bone.", mesh);
        }
    }
}
