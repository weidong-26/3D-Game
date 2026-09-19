using System;
using System.Collections.Generic;

namespace LightweightGame.Core
{
    [Serializable]
    public sealed class AppearanceData
    {
        public int Version;
        public float FaceWidth;
        public float FaceLength;
        public float ChinWidth;
        public float ChinLength;
        public float Cheekbones;
        public float EyeSize;
        public float EyeSpacing;
        public float BrowHeight;
        public float BrowAngle;
        public float NoseSize;
        public float NoseWidth;
        public float MouthSize;
        public float LipThickness;
        public float SkinTone;
        public int HairStyle;
        public float HairColor;
        public int BodyType;
        public float Height;
        public float Build;
        public float ShoulderWidth;
        public float LegLength;
        public int Outfit;

        public static AppearanceData CreateDefault()
        {
            return new AppearanceData
            {
                Version = 2,
                FaceWidth = 0.5f,
                FaceLength = 0.5f,
                ChinWidth = 0.5f,
                ChinLength = 0.5f,
                Cheekbones = 0.5f,
                EyeSize = 0.5f,
                EyeSpacing = 0.5f,
                BrowHeight = 0.5f,
                BrowAngle = 0.5f,
                NoseSize = 0.5f,
                NoseWidth = 0.5f,
                MouthSize = 0.5f,
                LipThickness = 0.5f,
                SkinTone = 0.5f,
                HairStyle = 0,
                HairColor = 0.5f,
                BodyType = 0,
                Height = 0.5f,
                Build = 0.5f,
                ShoulderWidth = 0.5f,
                LegLength = 0.5f,
                Outfit = 0
            };
        }

        public void Normalize()
        {
            FaceWidth = Clamp01(FaceWidth);
            FaceLength = Clamp01(FaceLength);
            ChinWidth = Clamp01(ChinWidth);
            ChinLength = Clamp01(ChinLength);
            Cheekbones = Clamp01(Cheekbones);
            EyeSize = Clamp01(EyeSize);
            EyeSpacing = Clamp01(EyeSpacing);
            BrowHeight = Clamp01(BrowHeight);
            BrowAngle = Clamp01(BrowAngle);
            NoseSize = Clamp01(NoseSize);
            NoseWidth = Clamp01(NoseWidth);
            MouthSize = Clamp01(MouthSize);
            LipThickness = Clamp01(LipThickness);
            SkinTone = Clamp01(SkinTone);
            HairStyle = HairStyle < 0 ? 0 : HairStyle > 2 ? 2 : HairStyle;
            HairColor = Clamp01(HairColor);
            BodyType = BodyType < 0 ? 0 : BodyType > 1 ? 1 : BodyType;
            Height = Clamp01(Height);
            Build = Clamp01(Build);
            ShoulderWidth = Clamp01(ShoulderWidth);
            LegLength = Clamp01(LegLength);
            Outfit = Outfit < 0 ? 0 : Outfit > 2 ? 2 : Outfit;
        }

        public void UpgradeLegacy()
        {
            if (Version >= 2) return;
            Cheekbones = 0.5f;
            BrowHeight = 0.5f;
            BrowAngle = 0.5f;
            HairColor = 0.5f;
            Height = 0.5f;
            Build = 0.5f;
            ShoulderWidth = 0.5f;
            LegLength = 0.5f;
            Version = 2;
        }

        public IEnumerable<float> AllNormalizedValues()
        {
            yield return FaceWidth;
            yield return FaceLength;
            yield return ChinWidth;
            yield return ChinLength;
            yield return Cheekbones;
            yield return EyeSize;
            yield return EyeSpacing;
            yield return BrowHeight;
            yield return BrowAngle;
            yield return NoseSize;
            yield return NoseWidth;
            yield return MouthSize;
            yield return LipThickness;
            yield return SkinTone;
            yield return HairColor;
            yield return Height;
            yield return Build;
            yield return ShoulderWidth;
            yield return LegLength;
        }

        private static float Clamp01(float value)
        {
            return value < 0f ? 0f : value > 1f ? 1f : value;
        }
    }

    public static class AppearanceRandomizer
    {
        public static AppearanceData Create(Random random)
        {
            if (random == null)
                throw new ArgumentNullException("random");

            return new AppearanceData
            {
                Version = 2,
                FaceWidth = Next(random),
                FaceLength = Next(random),
                ChinWidth = Next(random),
                ChinLength = Next(random),
                Cheekbones = Next(random),
                EyeSize = Next(random),
                EyeSpacing = Next(random),
                BrowHeight = Next(random),
                BrowAngle = Next(random),
                NoseSize = Next(random),
                NoseWidth = Next(random),
                MouthSize = Next(random),
                LipThickness = Next(random),
                SkinTone = Next(random),
                HairStyle = random.Next(0, 3),
                HairColor = Next(random),
                BodyType = random.Next(0, 2),
                Height = Next(random),
                Build = Next(random),
                ShoulderWidth = Next(random),
                LegLength = Next(random),
                Outfit = random.Next(0, 3)
            };
        }

        private static float Next(Random random)
        {
            return (float)random.NextDouble();
        }
    }
}
