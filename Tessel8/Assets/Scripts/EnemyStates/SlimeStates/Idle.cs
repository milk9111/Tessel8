
using UnityEngine;

namespace EnemyStates.SlimeStates
{
    public class Idle : BaseState
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
            _animator.SetBool("Walking", false);
            
            if (!_inStoppingDistance)
            {
                _controller.SetMovementStop(1);
                _controller.ChangeState(States.Walking);
            }
            else
            {
                _controller.ChangeState(States.Attacking);
            }

            var dir = _playerTransform.position.x > _controller.GetPosition().x ? 1 : -1;
            _controller.SetDirection(dir);
        }

        public void SetInStoppingDistance(bool inStoppingDistance)
        {
            _inStoppingDistance = inStoppingDistance;
        }
    }
}