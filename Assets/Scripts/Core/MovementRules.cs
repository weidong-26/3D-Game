using System;

namespace LightweightGame.Core
{
    public static class MovementRules
    {
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
