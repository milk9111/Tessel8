using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

namespace EnemyControllers
{
    public class LaserWallController : MonoBehaviour
    {
        public float speed;
        public float secondsToSelfDestruct;
        
        private int _direction = 1;
        private int _damage;

        private bool _firstCreated = true;

        private Coroutine _coroutine;

        void Update()
        {
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

        private IEnumerator SelfDestructClock()
        {
            yield return new WaitForSeconds(secondsToSelfDestruct);
            Destroy(gameObject);
        }
    }
}