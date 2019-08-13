
using UnityEngine;

namespace EnemyStates.SkeletonStates
{
    public class SkeletonIdle : BaseState
    {
        private bool _inStoppingDistance;

        private Transform _playerTransform;

        public override void Init()
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
        }

        public override void DoAction()
        {
            _controller.SetMovementStop(0);
            _animator.SetBool("Walking", !_inStoppingDistance);
            
            if (!_inStoppingDistance)
            {
                _controller.SetMovementStop(1);
                _controller.ChangeState(States.Walking);
            }
            else
            {
                _controller.ChangeState(States.Attacking);
            }
            
            _controller.SetDirection(_playerTransform.position.x > _controller.GetPosition().x ? 1 : -1);
        }

        public void SetInStoppingDistance(bool inStoppingDistance)
        {
            _inStoppingDistance = inStoppingDistance;
        }
    }
}