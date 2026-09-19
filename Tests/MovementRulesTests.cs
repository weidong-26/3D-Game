using System;
using LightweightGame.Core;

internal static class MovementRulesTests
{
    private static void Main()
    {
        Check(Math.Abs(MovementRules.JumpVelocity(1.5f, -18f) - (float)Math.Sqrt(54f)) < 0.001f, "jump height");
        Check(MovementRules.CanJump(true, 1, 1), "ground jump");
        Check(MovementRules.CanJump(false, 0, 1), "second jump");
        Check(!MovementRules.CanJump(false, 1, 1), "third jump blocked");
        Check(MovementRules.NeedsRespawn(-9f, -8f), "fall respawns");
        Check(!MovementRules.NeedsRespawn(-7f, -8f), "normal height stays");
        Console.WriteLine("6/6 movement rules passed.");
    }

    private static void Check(bool ok, string name)
    {
        if (!ok) throw new Exception(name);
    }
}
