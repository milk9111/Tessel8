using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerControllerHelper : MonoBehaviour
    {
        public GameController gameController;
        
        private PlayerPlatformerController _platformerController;
        private PlayerCombatController _combatController;
        private PlayerTeleportController _teleportController;
        private Animator _animator;
        private Collider2D _collider;
        private bool _isPaused;

        void Awake()
        {
            _platformerController = GetComponent<PlayerPlatformerController>();
            _combatController = GetComponent<PlayerCombatController>();
            _teleportController = GetComponent<PlayerTeleportController>();
            _animator = GetComponentInChildren<Animator>();
            _collider = GetComponent<CapsuleCollider2D>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                {
                    gameController.ClosePauseMenu();
                }
                else
                {
                    gameController.OpenPauseMenu();
                }
            }
        }

        public void OnPause()
        {
            _isPaused = true;
            _platformerController.enabled = false;
            _combatController.enabled = false;
            _teleportController.enabled = false;
            _animator.enabled = false;
            _collider.enabled = false;
        }
        
        public void OnPlay()
        {
            _isPaused = false;
            _platformerController.enabled = true;
            _combatController.enabled = true;
            _teleportController.enabled = true;
            _animator.enabled = true;
            _collider.enabled = true;
        }

        public void OnStart(Vector3 startingPos, Quaternion startingRot)
        {
            _animator.Rebind();
            _platformerController.EnableMovement();
            _teleportController.EnableTeleport();
            _combatController.ResetHealth();
            gameObject.transform.SetPositionAndRotation(startingPos, startingRot);
        }
    }
}