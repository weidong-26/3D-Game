using System;
using UnityEditor;
using UnityEditor.Animations;

namespace LightweightGame.Editor
{
    public static class AnimationAssetChecks
    {
        public static void Check()
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Resources/PrototypeAnimator.controller");
            if (controller == null) throw new Exception("PrototypeAnimator is missing");
            string[] required = { "Idle", "Walk", "Run", "Jump", "Fall", "Land", "Hold" };
            foreach (string name in required)
            {
                bool found = false;
                foreach (ChildAnimatorState state in controller.layers[0].stateMachine.states)
                    found |= state.state.name == name && state.state.motion != null;
                if (!found) throw new Exception("Missing animated state: " + name);
            }
            UnityEngine.Debug.Log("ANIMATION_STATES_PASS 7/7");
        }
    }
}
