using System;

namespace LightweightGame.Core
{
    public enum LocomotionState { Idle, Walk, Run }

    public static class MovementRules
    {
        public static LocomotionState LocomotionForSpeed(float planarSpeed, float idleThreshold, float runThreshold)
        {
            if (planarSpeed >= runThreshold) return LocomotionState.Run;
            if (planarSpeed > idleThreshold) return LocomotionState.Walk;
            return LocomotionState.Idle;
        }

        public static float JumpVelocity(float height, float gravity)
        {
            return (float)Math.Sqrt(2f * Math.Abs(gravity) * height);
        }

        public static bool CanJump(bool grounded, int airJumpsUsed, int maximumAirJumps)
        {
            return grounded || airJumpsUsed < maximumAirJumps;
        }

        public static bool NeedsRespawn(float height, float respawnHeight)
        {
            return height < respawnHeight;
        }
    }
}
