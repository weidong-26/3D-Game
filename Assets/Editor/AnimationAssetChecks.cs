using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace LightweightGame.Editor
{
    public static class AnimationAssetChecks
    {
        public static void Check()
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Resources/PrototypeAnimator.controller");
            if (controller == null) throw new Exception("PrototypeAnimator is missing");
            string[] required = { "Jump", "Fall", "Land", "Hold", "Interact" };
            foreach (string name in required)
            {
                bool found = false;
                foreach (ChildAnimatorState state in controller.layers[0].stateMachine.states)
                    found |= state.state.name == name && state.state.motion != null;
                if (!found) throw new Exception("Missing animated state: " + name);
            }

            bool hasSpeed = false;
            foreach (AnimatorControllerParameter parameter in controller.parameters)
                hasSpeed |= parameter.name == "Speed" && parameter.type == AnimatorControllerParameterType.Float;
            if (!hasSpeed) throw new Exception("Missing Float parameter: Speed");

            AnimatorState locomotion = null;
            foreach (ChildAnimatorState state in controller.layers[0].stateMachine.states)
                if (state.state.name == "Locomotion") locomotion = state.state;
            BlendTree tree = locomotion != null ? locomotion.motion as BlendTree : null;
            if (tree == null || tree.blendType != BlendTreeType.Simple1D || tree.blendParameter != "Speed")
                throw new Exception("Locomotion must be a 1D Blend Tree driven by Speed");

            string[] locomotionClips = { "Idle", "Walk", "Run" };
            float[] thresholds = { 0f, 3.5f, 6f };
            foreach (ChildMotion child in tree.children)
            {
                AnimationClip clip = child.motion as AnimationClip;
                if (clip == null) continue;
                for (int i = 0; i < locomotionClips.Length; i++)
                    if (clip.name == locomotionClips[i] && Mathf.Approximately(child.threshold, thresholds[i]))
                        locomotionClips[i] = null;
            }
            foreach (string clipName in locomotionClips)
                if (clipName != null) throw new Exception("Missing locomotion Blend Tree clip: " + clipName);

            Debug.Log("ANIMATION_STATES_PASS locomotion + 5 actions");
        }
    }
}
