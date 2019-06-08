using System;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;

public class EnemyController : PhysicsObject
{
	[Tooltip("The list of states for this enemy. It MUST have the names of the state classes")]
	public StateHolder[] states;

	private SpriteRenderer _spriteRenderer;
	
	private Animator _animator;

	private Collider2D _collider;
	
	private int _direction;
	
	private IDictionary<States, IState> _stateObjects;
	
	private States _currState;

	private float _speed;

	private bool _isPaused;

	private bool _isDead;
	
	void Awake ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer> ();

		if (gameObject.HasComponent<Animator>())
		{
			_animator = GetComponent<Animator>();
		}

		_collider = GetComponent<CapsuleCollider2D>();

		GatherStates();
		_currState = States.Idle;
	}
	
	protected override void ChildUpdate()
	{
		if (_isPaused) return;
		
		switch (_currState)
		{
			case States.Idle:
				((EnemyIdle)_stateObjects[_currState]).SetInStoppingDistance(
					((EnemyWalking) _stateObjects[States.Walking]).IsTargetWithinXStoppingDistance());
				_stateObjects[_currState].DoAction();
				break;
			case States.Walking:
				_stateObjects[_currState].DoAction();
				break;
			case States.Attacking:
				_stateObjects[_currState].DoAction();
				break;
			case States.Hit:
				_stateObjects[_currState].DoAction();
				break;
			case States.Dead:
				_stateObjects[_currState].DoAction();
				break;
			default:
				Debug.LogError("Hit default case!");
				break;
		}
	}

	protected override void ComputeVelocity()
	{
		var move = Vector2.zero;

		move.x = _direction * _speed;

		var flipSprite = _spriteRenderer.flipX ? move.x > 0.0f : move.x < 0.0f;
		if (flipSprite)
		{
			_spriteRenderer.flipX = !_spriteRenderer.flipX;
		}
        
		targetVelocity = move;        
	}

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

	public bool IsPlayerWithinStoppingDistance()
	{
		return ((EnemyWalking) _stateObjects[States.Walking]).IsTargetWithinXStoppingDistance();
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

	public void DealDamage(int damage)
	{
		((EnemyHit)_stateObjects[States.Hit]).DealDamage(damage);
	}

	public bool HasDied()
	{
		return _isDead;
	}

	public void MarkAsDead()
	{
		_isDead = true;
	}
	
	private void GatherStates()
	{
		_stateObjects = new Dictionary<States, IState>();
		foreach (var holder in states)
		{
			if (_stateObjects.ContainsKey(holder.stateType))
			{
				continue;
			}
			
			_stateObjects[holder.stateType] = (IState)GetComponent(Type.GetType("EnemyStates." + holder.className));
			_stateObjects[holder.stateType].SetupFields(this, _animator);
			_stateObjects[holder.stateType].Init();
		}
	}

	
}
