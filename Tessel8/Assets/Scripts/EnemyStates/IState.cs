using System;
using EnemyControllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyStates
{
    public interface IState
    {
        void Init();
        
        void DoAction();

        void OnPause();

        void OnPlay();

        void SetupFields(EnemyController controller, Animator animator);
    }

}