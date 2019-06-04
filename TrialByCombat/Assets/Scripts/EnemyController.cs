using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EnemyStates;
using NUnit.Framework;
using UnityEngine;
using Hellmade.Sound;

public class EnemyController : PhysicsObject
{
	[Tooltip("The list of states for this enemy. It MUST have the names of the state classes")]
	public StateHolder[] states;

	private SpriteRenderer _spriteRenderer;
	
	private Animator _animator;
	
	private int _direction;
	
	private IDictionary<States, IState> _stateObjects;
	
	private States _currState;

	private float _speed;
	
	void Awake ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer> ();

		if (gameObject.HasComponent<Animator>())
		{
			_animator = GetComponent<Animator>();
		}

		GatherStates();
		_currState = States.Idle;
	}
	
	protected override void ChildUpdate()
	{
		switch (_currState)
		{
			case States.Idle:
				((EnemyIdle)_stateObjects[_currState]).SetInStoppingDistance(
					((EnemyWalking) _stateObjects[States.Walking]).IsTargetWithinStoppingDistance());
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

	public bool IsPlayerWithinStoppingDistance()
	{
		return ((EnemyWalking) _stateObjects[States.Walking]).IsTargetWithinStoppingDistance();
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
