using System;
using System.Reflection;
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
        SpeedChoosesLocomotionState();
        Console.WriteLine("9/9 movement rules passed.");
    }

    private static void SpeedChoosesLocomotionState()
    {
        MethodInfo classify = typeof(MovementRules).GetMethod("LocomotionForSpeed", BindingFlags.Public | BindingFlags.Static);
        Check(classify != null, "speed-based locomotion rule exists");
        Equal("Idle", classify.Invoke(null, new object[] { 0f, 0.2f, 4.5f }).ToString(), "zero speed");
        Equal("Walk", classify.Invoke(null, new object[] { 2f, 0.2f, 4.5f }).ToString(), "walking speed");
        Equal("Run", classify.Invoke(null, new object[] { 5f, 0.2f, 4.5f }).ToString(), "running speed");
    }

    private static void Check(bool ok, string name)
    {
        if (!ok) throw new Exception(name);
    }

    private static void Equal(string expected, string actual, string name)
    {
        if (expected != actual) throw new Exception(name + ": expected " + expected + ", got " + actual);
    }
}
