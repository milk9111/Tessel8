using System;
using UnityEngine;

namespace EnemyControllers
{
    public class AntiTeleportRadius : MonoBehaviour
    {
        private GameObject _player;
        private PlayerTeleportController _playerTeleport;

        private float _radiusSize = 3;
        private Vector3 _centerPoint;
        private bool _playerIsInRadius;

        void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _playerTeleport = _player.GetComponent<PlayerTeleportController>();
            _playerIsInRadius = false;
        }

        void Update()
        {
            if (IsPlayerWithinRange())
            {
                _playerTeleport.DisableTeleport();
                _playerIsInRadius = true;
                return;
            } 
            
            _playerTeleport.EnableTeleport();
            _playerIsInRadius = false;
        }

        public void ReleasePlayer()
        {
            if (_playerTeleport == null) return;
            _playerTeleport.EnableTeleport();
            _playerIsInRadius = false;
        }

        public void SetCenterPoint(Vector3 center)
        {
            _centerPoint = center;
            transform.SetPositionAndRotation(center, Quaternion.identity);
        }

        public void SetRadiusSize(float size)
        {
            _radiusSize = size;
        }
        
        public bool IsPlayerWithinRange()
        {
            if (_player == null || _centerPoint == null)
            {
                return false;
            }
            
            var xDiff = Math.Pow(_player.transform.position.x - _centerPoint.x, 2);
            var yDiff = Math.Pow(_player.transform.position.y - _centerPoint.y, 2);
		
            var distance = Math.Sqrt(xDiff + yDiff);
		
            return distance <= _radiusSize;
        }
    }
}