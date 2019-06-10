using System;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;

namespace EnemyControllers
{
	public class EnemyController : PhysicsObject
	{
		[Tooltip("The list of states for this enemy. It MUST have the names of the state classes")]
		public StateHolder[] states;

		protected SpriteRenderer _spriteRenderer;
	
		protected Animator _animator;

		protected Collider2D _collider;
	
		protected int _direction;
	
		protected IDictionary<States, IState> _stateObjects;
	
		protected States _currState;

		protected float _speed;

		protected bool _isPaused;

		protected bool _isDead;
	
		void Awake ()
		{
			_spriteRenderer = GetComponent<SpriteRenderer> ();

			if (gameObject.HasComponent<Animator>())
			{
				_animator = GetComponent<Animator>();
			}

			_collider = GetComponent<CapsuleCollider2D>();

			ChildAwake();
		}
		
		protected virtual void ChildAwake() {}

		public void OnPause()
		{
			_isPaused = true;
		
			foreach (var state in _stateObjects.Values)
			{
				((BaseState) state).enabled = false;
			}

			_animator.enabled = false;
			_collider.enabled = false;
		}

		public void OnPlay()
		{
			_isPaused = false;
		
			foreach (var state in _stateObjects.Values)
			{
				((BaseState) state).enabled = true;
			}
		
			_animator.enabled = true;
			_collider.enabled = true;
		}

		public virtual bool IsPlayerWithinStoppingDistance()
		{
			return false;
		}

		public void ChangeState(States state)
		{
			_currState = state;
		}

		public void SetDirection(int direction)
		{
			_direction = direction;
		}

		public void SetSpeed(float speed)
		{
			_speed = speed;
		}

		public virtual void DealDamage(int damage)
		{
		}

		public bool HasDied()
		{
			return _isDead;
		}

		public void MarkAsDead()
		{
			_isDead = true;
		}
	}
}
