using UnityEngine;

namespace EMR.Medal
{
    public static class GoldMedalJudge
    {
        public static bool IsGoldMedal(float chance)
        {
            return Random.value < chance;
        }
    }
}