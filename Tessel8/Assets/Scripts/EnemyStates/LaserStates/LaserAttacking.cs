using System.Collections;
using DefaultNamespace;
using EnemyControllers;
using UnityEngine;

namespace EnemyStates.LaserStates
{
    public class LaserAttacking : BaseState
    {
        [Tooltip("The player's health component")]
        public PlayerCombatController playerCombat;

        [Tooltip("The laser wall prefab game object to instantiate on attack")]
        public GameObject laserWallPrefab;
        
        [Tooltip("The enemy's damage output")]
        public int damageOutput = 10;

        [Tooltip("The time in seconds between each attack")]
        public float secondsBetweenAttacks = 1f;

        private float _remainingSecondsOnTimer;

        private bool _isReadyToAttack;

        private Coroutine _lastCoroutine;

        void Awake()
        {
            _isReadyToAttack = true;

            if (playerCombat == null)
            {
                playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
            }
        }
                
        public override void DoAction()
        {
            if (IsPaused())
            {
                return;
            }
            
            if (!_isReadyToAttack)
            {
                _animator.SetBool("Attacking", false);
                _controller.ChangeState(States.Idle);
                return;
            }
            
            _animator.SetBool("Attacking", true);
        }

        public void AttackPlayer()
        {
            _lastCoroutine = StartCoroutine(AttackCooldown());
            
            PlaySoundFx();
            var direction = _controller.GetDirection();
            
            var laserWall = Instantiate(laserWallPrefab, 
                new Vector3(direction < 0 ? transform.position.x - 1 : transform.position.x + 1,
                    transform.position.y - 0.5f), laserWallPrefab.transform.rotation);
            var wallController = laserWall.GetComponent<Projectile>();
            wallController.SetDirection(direction);
            wallController.SetDamage(damageOutput);
            

            _animator.SetBool("Attacking", false);
            _controller.ChangeState(States.Idle);
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
        }
    }
}