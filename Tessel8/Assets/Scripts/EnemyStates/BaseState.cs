using EnemyControllers;
using UnityEngine;

namespace EnemyStates
{
    public class BaseState : MonoBehaviour, IState
    {
        protected EnemyController _controller;
        protected Animator _animator;

        protected bool _isPaused;

        public virtual void Init() { }
        
        public virtual void DoAction() { }

        public virtual void OnPause()
        {
            _isPaused = true;
        }

        public virtual void OnPlay()
        {
            _isPaused = false;
        }

        protected bool IsPaused()
        {
            return _isPaused;
        }

        public void SetupFields(EnemyController controller, Animator animator)
        {
            _controller = controller;

            if (animator == null)
            {
                animator = new Animator();
            }

            _animator = animator;
        }
    }
}