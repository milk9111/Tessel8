using System;
using UnityEngine;

namespace EnemyStates.SkeletonStates
{
    public class SkeletonWalking : BaseState
    {
        [Tooltip("The movement speed of the enemy")]
        public float speed = 0.5f;
	
        [Tooltip("The speed of the jump take off")]
        public float jumpTakeOffSpeed = 15;
	
        [Tooltip("The distance of the forward ground detection ray cast")]
        public float raycastDistance = 5f;
	
        [Tooltip("The distance the enemy stops from the target")]
        public float stoppingDistance = 0.1f;
        
        [Tooltip("The target transform to follow. The default target is the GameObject with the tag 'Player'.")]
        public Transform target;

        [Tooltip("Follow target debug. Defaults to right direction.")]
        public bool followTarget;
	        
        private bool _foundHit;
        
        
        void Awake ()
        {
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }
        }

        public override void Init()
        {
            _controller.SetSpeed(speed);
        }

        public override void DoAction()
        {
            _controller.SetSpeed(speed);
            
            _foundHit = Physics2D.Raycast(transform.position, transform.right, raycastDistance, 1<<LayerMask.NameToLayer("Ground"));
            if (_foundHit && _controller.isGrounded())
            {
                _controller.velocity.y = jumpTakeOffSpeed;
            }
            
            _foundHit = Physics2D.Raycast(transform.position, transform.right * -1, raycastDistance, 1<<LayerMask.NameToLayer("Ground"));
            if (_foundHit && _controller.isGrounded())
            {
                _controller.velocity.y = jumpTakeOffSpeed;
            }
            
            if (!followTarget)
            {
                _controller.SetDirection(1);
                return;
            }

            var newX = MoveTowardsX();
            if (IsTargetWithinXStoppingDistance())
            {
                _controller.SetMovementStop(0);
                if (IsTargetWithinYStoppingDistance())
                {
                    _controller.ChangeState(States.Attacking);
                }
                else
                {
                    _controller.ChangeState(States.Idle);
                }
            }
            else
            {
                _controller.SetMovementStop(1);
            }

            _controller.SetDirection(newX > _controller.GetPosition().x ? 1 : -1);
        }

        private float MoveTowardsX()
        {
            return Vector2.MoveTowards(_controller.GetPosition(), 
                target.position,speed * Time.deltaTime).x;
        }
        
        public bool IsTargetWithinXStoppingDistance()
        {
            var newX = MoveTowardsX();
            return Math.Abs(newX - target.position.x) <= stoppingDistance;
        }

        private bool IsTargetWithinYStoppingDistance()
        {
            return Math.Abs(transform.position.y - target.position.y) <= stoppingDistance;
        }
    }
}