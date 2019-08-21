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

        [Tooltip("The y amount to offset the origin of the raycast from")]
        public float raycastOriginYOffset = 0.08f;
	
        [Tooltip("The distance the enemy stops from the target on the X axis")]
        public float stoppingXDistance = 0.1f;

        [Tooltip("The distance the enemy stops from the target on the Y axis")]
        public float stoppingYDistance = 1f;
        
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
            base.Init();
            _controller.SetSpeed(speed);
        }

        public override void DoAction()
        {
            PlaySoundFx();
            _controller.SetSpeed(speed);

            var direction = transform.right;
            if (_controller.GetDirection() < 0)
            {
                direction *= -1;
            }
            
            _foundHit = Physics2D.Raycast(transform.position + new Vector3(0, raycastOriginYOffset, 0), direction, raycastDistance, 1<<LayerMask.NameToLayer("Ground"));
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
            return Math.Abs(newX - target.position.x) <= stoppingXDistance;
        }

        public bool IsTargetWithinYStoppingDistance()
        {
            return Math.Abs(transform.position.y - target.position.y) <= stoppingYDistance;
        }
    }
}