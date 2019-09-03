using System;
using System.Collections.Generic;
using EnemyStates;
using EnemyStates.SlimeStates;
using UnityEngine;

namespace EnemyControllers
{
    public class SlimeController : EnemyController
    {
	    private Hit _enemyHealth;
	    
	    protected override void ChildAwake()
	    {
		    GatherStates();
		    _currState = States.Idle;
	    }

	    protected override void ChildUpdate()
		{
			if (_isPaused) return;

			if (_enemyHealth != null && _enemyHealth.GetCurrentHealth() <= 0)
			{
				_currState = States.Dead;
			}
			
			switch (_currState)
			{
				case States.Idle:
					((Idle)_stateObjects[_currState]).SetInStoppingDistance(IsPlayerWithinStoppingDistance());
					_stateObjects[_currState].DoAction();
					break;
				case States.Walking:
					_stateObjects[_currState].DoAction();
					break;
				case States.Attacking:
					
					_stateObjects[_currState].DoAction();
					break;
				case States.Hit:
					_enemyHealth = (Hit) _stateObjects[_currState];
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

		    bool flipSprite;
		    if (hasAnimationBones)
		    {
			    flipSprite = _isFlipped ? move.x < 0.0f : move.x > 0.0f;
		    }
		    else
		    {
			    flipSprite = _spriteRenderer.flipX ? move.x > 0.0f : move.x < 0.0f;
		    }

		    if (flipSprite)
		    {
			    if (hasAnimationBones)
			    {
				    FlipSprite(!_isFlipped);
			    }
			    else
			    {
				    _spriteRenderer.flipX = !_spriteRenderer.flipX;
			    }
		    }

		    move *= _movementStop;
		    targetVelocity = move;        
	    }
	
		public override bool IsPlayerWithinStoppingDistance()
		{
			return ((Walking) _stateObjects[States.Walking]).IsTargetWithinXStoppingDistance() 
			       && ((Walking) _stateObjects[States.Walking]).IsTargetWithinYStoppingDistance();
		}
	
		public override void DealDamage(int damage)
		{
			((Hit)_stateObjects[States.Hit]).DealDamage(damage);
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