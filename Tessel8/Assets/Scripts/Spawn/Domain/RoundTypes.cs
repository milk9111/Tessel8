using UnityEngine;

namespace Spawn.Domain
{
    public enum RoundTypes
    {
        Wave,
        TimedWave,
        Normal,
        Once,
        Survival
    }

    public static class RoundTypesHelper
    {
        public static RoundTypes GetTypeFromName(string name)
        {
            switch (name)
            {
                case "WaveRound":
                    return RoundTypes.Wave;
                case "TimedWaveRound":
                    return RoundTypes.TimedWave;
                case "NormalRound":
                    return RoundTypes.Normal;
                case "OnceRound":
                    return RoundTypes.Once;
                case "SurvivalRound":
                    return RoundTypes.Survival;
                default:
                    Debug.Log("Invalid type name in GetTypeFromName: " + name);
                    return RoundTypes.Normal;
            }
        }
    }
}