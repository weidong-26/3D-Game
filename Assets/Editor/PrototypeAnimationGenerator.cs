using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using LightweightGame.Runtime;

namespace LightweightGame.Editor
{
    public static class PrototypeAnimationGenerator
    {
        public static void Generate()
        {
            GenerateController();
            AnimationAssetChecks.Check();
        }

        public static void GenerateController()
        {
            const string folder = "Assets/Resources/PrototypeAnimations";
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets/Resources", "PrototypeAnimations");
            string controllerPath = "Assets/Resources/PrototypeAnimator.controller";
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            AnimationClip idle = EnsureClip(folder, "Idle", 1f, 2f, 0f, 0f, true);
            AnimationClip walk = EnsureClip(folder, "Walk", 0.8f, 25f, 30f, 0f, true);
            AnimationClip run = EnsureClip(folder, "Run", 0.55f, 42f, 46f, 0f, true);
            AnimationClip jump = EnsureClip(folder, "Jump", 0.4f, 0f, 0f, -28f, false);
            AnimationClip fall = EnsureClip(folder, "Fall", 0.4f, 0f, 0f, 25f, false);
            AnimationClip land = EnsureClip(folder, "Land", 0.2f, 0f, 0f, -10f, false);
            AnimationClip hold = EnsureClip(folder, "Hold", 0.8f, 7f, 22f, -45f, true);
            AnimationClip interact = EnsureClip(folder, "Interact", 0.7f, 24f, 3f, -28f, false);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            SetStateMotion(controller, "Jump", jump);
            SetStateMotion(controller, "Fall", fall);
            SetStateMotion(controller, "Land", land);
            SetStateMotion(controller, "Hold", hold);
            SetStateMotion(controller, "Interact", interact);
            RemoveState(controller, "Idle");
            RemoveState(controller, "Walk");
            RemoveState(controller, "Run");

            bool hasSpeed = false;
            foreach (AnimatorControllerParameter parameter in controller.parameters)
            {
                if (parameter.name != "Speed") continue;
                if (parameter.type != AnimatorControllerParameterType.Float)
                    throw new System.InvalidOperationException("Animator parameter Speed must be a Float.");
                hasSpeed = true;
            }
            if (!hasSpeed) controller.AddParameter("Speed", AnimatorControllerParameterType.Float);

            AnimatorState locomotion = FindState(controller, "Locomotion");
            if (locomotion == null) locomotion = stateMachine.AddState("Locomotion");
            BlendTree blendTree = locomotion.motion as BlendTree;
            if (blendTree == null)
            {
                blendTree = new BlendTree { name = "Locomotion" };
                AssetDatabase.AddObjectToAsset(blendTree, controller);
            }
            blendTree.blendType = BlendTreeType.Simple1D;
            blendTree.blendParameter = "Speed";
            blendTree.useAutomaticThresholds = false;
            blendTree.children = new ChildMotion[0];
            blendTree.AddChild(idle, 0f);
            blendTree.AddChild(walk, 3.5f);
            blendTree.AddChild(run, 6f);
            locomotion.motion = blendTree;
            stateMachine.defaultState = locomotion;

            const string profileFolder = "Assets/Resources/CharacterModels";
            if (!AssetDatabase.IsValidFolder(profileFolder))
                AssetDatabase.CreateFolder("Assets/Resources", "CharacterModels");
            const string profilePath = profileFolder + "/PlayerVisualProfile.asset";
            if (AssetDatabase.LoadAssetAtPath<PlayerVisualProfile>(profilePath) == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PlayerVisualProfile>(), profilePath);

            AssetDatabase.SaveAssets();
        }

        private static AnimationClip EnsureClip(string folder, string name, float length, float armSwing, float legSwing, float rightArmOffset, bool loop)
        {
            string path = folder + "/" + name + ".anim";
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            bool newClip = clip == null;
            if (!newClip) return clip;

            clip = new AnimationClip { name = name, frameRate = 30f };
            clip.SetCurve("LeftArm", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, armSwing, 0f));
            clip.SetCurve("RightArm", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, -armSwing, rightArmOffset));
            clip.SetCurve("LeftLeg", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, -legSwing, 0f));
            clip.SetCurve("RightLeg", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, legSwing, 0f));
            SerializedObject serialized = new SerializedObject(clip);
            SerializedProperty loopTime = serialized.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (loopTime != null) { loopTime.boolValue = loop; serialized.ApplyModifiedProperties(); }
            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }

        private static AnimatorState FindState(AnimatorController controller, string name)
        {
            foreach (ChildAnimatorState child in controller.layers[0].stateMachine.states)
                if (child.state.name == name) return child.state;
            return null;
        }

        private static void SetStateMotion(AnimatorController controller, string name, Motion motion)
        {
            AnimatorState state = FindState(controller, name);
            if (state == null) state = controller.layers[0].stateMachine.AddState(name);
            state.motion = motion;
        }

        private static void RemoveState(AnimatorController controller, string name)
        {
            AnimatorState state = FindState(controller, name);
            if (state != null) controller.layers[0].stateMachine.RemoveState(state);
        }

        private static AnimationCurve Wave(float length, float swing, float offset)
        {
            return new AnimationCurve(
                new Keyframe(0f, offset),
                new Keyframe(length * 0.25f, offset + swing),
                new Keyframe(length * 0.5f, offset),
                new Keyframe(length * 0.75f, offset - swing),
                new Keyframe(length, offset));
        }
    }
}
