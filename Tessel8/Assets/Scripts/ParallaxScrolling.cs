using UnityEngine;

namespace DefaultNamespace
{
    public class ParallaxScrolling : MonoBehaviour
    {
        public float speed = 1f;
        public PlayerPlatformerController player;

        private bool _isPaused = false;

        void Awake()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPlatformerController>();
            }
        }
        
        void Update()
        {
            if (_isPaused) return;
            
            var movement = new Vector3(
                speed * (player.Velocity().x * -1),
                0,
                0);

            movement *= Time.deltaTime;
            transform.Translate(movement);
        }

        public void OnPause()
        {
            _isPaused = true;
        }

        public void OnPlay()
        {
            _isPaused = false;
        }
    }
}