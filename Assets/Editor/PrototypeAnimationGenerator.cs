using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace LightweightGame.Editor
{
    public static class PrototypeAnimationGenerator
    {
        public static void Generate()
        {
            const string folder = "Assets/Resources/PrototypeAnimations";
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets/Resources", "PrototypeAnimations");
            string controllerPath = "Assets/Resources/PrototypeAnimator.controller";
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null) AssetDatabase.DeleteAsset(controllerPath);
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            Add(controller, folder, "Idle", 1f, 2f, 0f, 0f, true);
            Add(controller, folder, "Walk", 0.8f, 25f, 30f, 0f, true);
            Add(controller, folder, "Run", 0.55f, 42f, 46f, 0f, true);
            Add(controller, folder, "Jump", 0.4f, 0f, 0f, -28f, false);
            Add(controller, folder, "Fall", 0.4f, 0f, 0f, 25f, false);
            Add(controller, folder, "Land", 0.2f, 0f, 0f, -10f, false);
            Add(controller, folder, "Hold", 0.8f, 7f, 22f, -45f, true);
            controller.layers[0].stateMachine.defaultState = controller.layers[0].stateMachine.states[0].state;
            AssetDatabase.SaveAssets();
            AnimationAssetChecks.Check();
        }

        private static void Add(AnimatorController controller, string folder, string name, float length, float armSwing, float legSwing, float rightArmOffset, bool loop)
        {
            string path = folder + "/" + name + ".anim";
            if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null) AssetDatabase.DeleteAsset(path);
            AnimationClip clip = new AnimationClip { name = name, frameRate = 30f };
            clip.SetCurve("LeftArm", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, armSwing, 0f));
            clip.SetCurve("RightArm", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, -armSwing, rightArmOffset));
            clip.SetCurve("LeftLeg", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, -legSwing, 0f));
            clip.SetCurve("RightLeg", typeof(Transform), "localEulerAnglesRaw.x", Wave(length, legSwing, 0f));
            AssetDatabase.CreateAsset(clip, path);
            SerializedObject serialized = new SerializedObject(clip);
            SerializedProperty loopTime = serialized.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (loopTime != null) { loopTime.boolValue = loop; serialized.ApplyModifiedProperties(); }
            AnimatorState state = controller.layers[0].stateMachine.AddState(name);
            state.motion = clip;
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
