using UnityEngine;

namespace EMR.Medal
{
    public static class MedalBuffData
    {
        // ID11
        public static float GoldMedalChance { get; set; } = 0f;

        // ID12
        public static float RoundMedalBonus { get; set; } = 0f;

        public static float FrictionReduction = 0f;

        public static void Reset()
        {
            GoldMedalChance = 0f;
            RoundMedalBonus = 0f;
        }
    }
}