using System.Collections;
using EnemyControllers;
using UnityEngine;

namespace EnemyStates.AntiTeleportStates
{
    public class AntiTeleportAttacking : BaseState
    {
        [Tooltip("The anti teleport radius prefab game object to instantiate on attack")]
        public GameObject antiTeleportRadiusPrefab;
        
        [Tooltip("The enemy's radius size")]
        public float radiusSize = 3;

        [Tooltip("The time in seconds between each attack")]
        public float secondsBetweenAttacks = 1f;

        private float _remainingSecondsOnTimer;

        private bool _isReadyToAttack;
        private AntiTeleportRadius _radius;

        private bool _useOverride;

        private Coroutine _lastCoroutine;

        void Awake()
        {
            _isReadyToAttack = true;
            _useOverride = false;
            _radius = antiTeleportRadiusPrefab.GetComponent<AntiTeleportRadius>();
        }
                
        public override void DoAction()
        {
            if (IsPaused())
            {
                return;
            }
            
            if (_isReadyToAttack && _controller.IsPlayerWithinStoppingDistance() || _useOverride)
            {
                _animator.SetBool("Attacking", true);
            }
        }

        public void AttackPlayer()
        {
            PlaySoundFx();
            _lastCoroutine = StartCoroutine(AttackCooldown());
            antiTeleportRadiusPrefab.SetActive(true);
            _radius.SetRadiusSize(radiusSize);
            _radius.SetCenterPoint(transform.position);
        }
        
        public override void OnPause()
        {
            base.OnPause();
            
            if (_lastCoroutine != null)
            {
                StopCoroutine(_lastCoroutine);
            }
        }

        public override void OnPlay()
        {
            base.OnPlay();
            _lastCoroutine = StartCoroutine(AttackCooldown());
        }

        IEnumerator AttackCooldown()
        {
            _isReadyToAttack = false;
            var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : secondsBetweenAttacks;
            for(_remainingSecondsOnTimer = timerLength; _remainingSecondsOnTimer > 0; _remainingSecondsOnTimer -= Time.deltaTime)
                yield return null;
            
            _isReadyToAttack = true;
            if (!_radius.IsPlayerWithinRange())
            {
                _useOverride = false;
                _radius.ReleasePlayer();
                antiTeleportRadiusPrefab.SetActive(false);
            }
            else
            {
                _useOverride = true;
            }
            _animator.SetBool("Attacking", false);
            _controller.ChangeState(States.Idle);
        }
    }
}