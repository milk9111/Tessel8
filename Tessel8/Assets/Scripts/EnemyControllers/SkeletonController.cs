using System;
using System.Collections.Generic;
using EnemyStates;
using EnemyStates.SkeletonStates;
using UnityEngine;

namespace EnemyControllers
{
    public class SkeletonController : EnemyController
    {
	    protected override void ChildAwake()
	    {
		    GatherStates();
		    _currState = States.Idle;
	    }

	    protected override void ChildUpdate()
		{
			if (_isPaused) return;
			
			switch (_currState)
			{
				case States.Idle:
					((SkeletonIdle)_stateObjects[_currState]).SetInStoppingDistance(
						((SkeletonWalking) _stateObjects[States.Walking]).IsTargetWithinXStoppingDistance());
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

		    move.x = _direction * _speed * _movementStop;

		    var flipSprite = _spriteRenderer.flipX ? _direction != -1 : _direction != 1;
		    if (flipSprite && _direction != 0)
		    {
			    _spriteRenderer.flipX = !_spriteRenderer.flipX;
		    }
        
		    targetVelocity = move;        
	    }
	
		public override bool IsPlayerWithinStoppingDistance()
		{
			return ((SkeletonWalking) _stateObjects[States.Walking]).IsTargetWithinXStoppingDistance();
		}
	
		public override void DealDamage(int damage)
		{
			((SkeletonHit)_stateObjects[States.Hit]).DealDamage(damage);
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
}