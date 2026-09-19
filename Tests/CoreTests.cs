using System;
using LightweightGame.Core;

internal static class CoreTests
{
    private static int failures;

    private static void Main()
    {
        Run("defaults are centered", DefaultsAreCentered);
        Run("normalization clamps every parameter", NormalizationClampsEveryParameter);
        Run("random appearance is deterministic and valid", RandomAppearanceIsDeterministicAndValid);
        Run("second round body defaults are centered", SecondRoundDefaults);
        Run("second round body parameters clamp", SecondRoundClamp);
        Run("legacy appearance receives new defaults", LegacyAppearanceUpgrade);

        if (failures > 0)
        {
            Console.Error.WriteLine(failures + " core test(s) failed.");
            Environment.Exit(1);
        }

        Console.WriteLine("6/6 core tests passed.");
    }

    private static void DefaultsAreCentered()
    {
        AppearanceData data = AppearanceData.CreateDefault();
        Equal(0.5f, data.FaceWidth, "FaceWidth");
        Equal(0.5f, data.FaceLength, "FaceLength");
        Equal(0.5f, data.ChinWidth, "ChinWidth");
        Equal(0.5f, data.ChinLength, "ChinLength");
        Equal(0.5f, data.EyeSize, "EyeSize");
        Equal(0.5f, data.EyeSpacing, "EyeSpacing");
        Equal(0.5f, data.NoseSize, "NoseSize");
        Equal(0.5f, data.NoseWidth, "NoseWidth");
        Equal(0.5f, data.MouthSize, "MouthSize");
        Equal(0.5f, data.LipThickness, "LipThickness");
        Equal(0.5f, data.SkinTone, "SkinTone");
        Equal(0, data.HairStyle, "HairStyle");
    }

    private static void NormalizationClampsEveryParameter()
    {
        AppearanceData data = new AppearanceData
        {
            FaceWidth = -1f,
            FaceLength = 2f,
            ChinWidth = -2f,
            ChinLength = 3f,
            EyeSize = -3f,
            EyeSpacing = 4f,
            NoseSize = -4f,
            NoseWidth = 5f,
            MouthSize = -5f,
            LipThickness = 6f,
            SkinTone = 7f,
            HairStyle = 99
        };

        data.Normalize();

        Equal(0f, data.FaceWidth, "FaceWidth");
        Equal(1f, data.FaceLength, "FaceLength");
        Equal(0f, data.ChinWidth, "ChinWidth");
        Equal(1f, data.ChinLength, "ChinLength");
        Equal(0f, data.EyeSize, "EyeSize");
        Equal(1f, data.EyeSpacing, "EyeSpacing");
        Equal(0f, data.NoseSize, "NoseSize");
        Equal(1f, data.NoseWidth, "NoseWidth");
        Equal(0f, data.MouthSize, "MouthSize");
        Equal(1f, data.LipThickness, "LipThickness");
        Equal(1f, data.SkinTone, "SkinTone");
        Equal(2, data.HairStyle, "HairStyle");
    }

    private static void RandomAppearanceIsDeterministicAndValid()
    {
        AppearanceData first = AppearanceRandomizer.Create(new Random(1234));
        AppearanceData second = AppearanceRandomizer.Create(new Random(1234));

        Equal(first.FaceWidth, second.FaceWidth, "deterministic face width");
        Equal(first.LipThickness, second.LipThickness, "deterministic lip thickness");
        Equal(first.HairStyle, second.HairStyle, "deterministic hair style");

        foreach (float value in first.AllNormalizedValues())
        {
            True(value >= 0f && value <= 1f, "random value outside [0, 1]");
        }

        True(first.HairStyle >= 0 && first.HairStyle <= 2, "random hair style outside [0, 2]");
    }

    private static void SecondRoundDefaults()
    {
        AppearanceData data = AppearanceData.CreateDefault();
        Equal(2, data.Version, "version");
        Equal(0.5f, data.Cheekbones, "cheekbones");
        Equal(0.5f, data.BrowHeight, "brow height");
        Equal(0.5f, data.Height, "height");
        Equal(0.5f, data.Build, "build");
        Equal(0.5f, data.ShoulderWidth, "shoulder width");
        Equal(0.5f, data.LegLength, "leg length");
        Equal(0.5f, data.HairColor, "hair color");
    }

    private static void SecondRoundClamp()
    {
        AppearanceData data = AppearanceData.CreateDefault();
        data.Cheekbones = -1f;
        data.BrowHeight = 2f;
        data.Height = -2f;
        data.Build = 3f;
        data.ShoulderWidth = -3f;
        data.LegLength = 4f;
        data.HairColor = 5f;
        data.BodyType = 9;
        data.Outfit = 9;
        data.Normalize();
        Equal(0f, data.Cheekbones, "cheekbones");
        Equal(1f, data.BrowHeight, "brow height");
        Equal(0f, data.Height, "height");
        Equal(1f, data.Build, "build");
        Equal(0f, data.ShoulderWidth, "shoulder width");
        Equal(1f, data.LegLength, "leg length");
        Equal(1f, data.HairColor, "hair color");
        Equal(1, data.BodyType, "body type");
        Equal(2, data.Outfit, "outfit");
    }

    private static void LegacyAppearanceUpgrade()
    {
        AppearanceData data = new AppearanceData { FaceWidth = 0.72f, HairStyle = 2 };
        data.UpgradeLegacy();
        Equal(2, data.Version, "version");
        Equal(0.72f, data.FaceWidth, "old face kept");
        Equal(2, data.HairStyle, "old hair kept");
        Equal(0.5f, data.Height, "new height centered");
        Equal(0.5f, data.Cheekbones, "new cheekbones centered");
    }

    private static void Run(string name, Action test)
    {
        try
        {
            test();
            Console.WriteLine("PASS " + name);
        }
        catch (Exception error)
        {
            failures++;
            Console.Error.WriteLine("FAIL " + name + ": " + error.Message);
        }
    }

    private static void Equal(float expected, float actual, string label)
    {
        if (Math.Abs(expected - actual) > 0.0001f)
            throw new Exception(label + ": expected " + expected + ", got " + actual);
    }

    private static void Equal(int expected, int actual, string label)
    {
        if (expected != actual)
            throw new Exception(label + ": expected " + expected + ", got " + actual);
    }

    private static void True(bool condition, string message)
    {
        if (!condition)
            throw new Exception(message);
    }
}
