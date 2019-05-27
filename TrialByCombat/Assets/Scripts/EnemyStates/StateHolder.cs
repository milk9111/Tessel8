using System;
using UnityEngine;

namespace EnemyStates
{
    [Serializable]
    public class StateHolder
    {
        [Tooltip("The state enum type")]
        public States stateType;

        [Tooltip("The name of the state's class")]
        public string className;
    }
}