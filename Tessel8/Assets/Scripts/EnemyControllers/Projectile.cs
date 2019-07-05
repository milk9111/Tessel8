using System.Collections;
using DefaultNamespace;
using UnityEngine;

namespace EnemyControllers
{
    public class Projectile : MonoBehaviour
    {
        public float speed;
        public float secondsToSelfDestruct;
        
        private int _direction = 1;
        private int _damage;

        private float _remainingSecondsOnTimer;

        private bool _firstCreated = true;
        private bool _isPaused;

        private Coroutine _coroutine;

        void Update()
        {
            if (_isPaused)
            {
                return;
            }
            
            if (_firstCreated)
            {
                _coroutine = StartCoroutine(SelfDestructClock());
                _firstCreated = false;
            }
            
            transform.Translate(transform.right * _direction * speed * Time.deltaTime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!string.Equals(other.gameObject.tag, "Player")) return;
            
            other.gameObject.GetComponent<PlayerCombatController>().DealDamage(_damage);
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            Destroy(gameObject);
        }

        public void SetDirection(int direction)
        {
            _direction = direction;
        }

        public void SetDamage(int damage)
        {
            _damage = damage;
        }

        public void OnPause()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _isPaused = true;
        }

        public void OnPlay()
        {
            _coroutine = StartCoroutine(SelfDestructClock());
            _isPaused = false;
        }

        private IEnumerator SelfDestructClock()
        {
            var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : secondsToSelfDestruct;
            for (_remainingSecondsOnTimer = timerLength;
                _remainingSecondsOnTimer > 0;
                _remainingSecondsOnTimer -= Time.deltaTime)
            {
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}