using System;
using Spawn.Domain.Round;
using UnityEngine;

namespace Spawn.Domain
{
    [Serializable]
    public class RoundHolder : MonoBehaviour
    {
        public RoundTypes type;
        
        public WaveRound waveRound;

        public TimedWaveRound timedWaveRound;

        public NormalRound normalRound;

        public OnceRound onceRound;

        public SurvivalRound survivalRound;

        public IRound GetSelectedRound()
        {
            switch (type)
            {
                case RoundTypes.Wave:
                    return waveRound;
                case RoundTypes.TimedWave:
                    return timedWaveRound;
                case RoundTypes.Normal:
                    return normalRound;
                case RoundTypes.Once:
                    return onceRound;
                case RoundTypes.Survival:
                    return survivalRound;
            }

            return null;
        }
    }
}