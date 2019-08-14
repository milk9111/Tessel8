using UnityEngine;

namespace DefaultNamespace
{
    public class ParallaxScrolling : MonoBehaviour
    {
        public float speed = 1f;
        public PlayerPlatformerController player;

        void Awake()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPlatformerController>();
            }
        }
        
        void Update()
        {
            var movement = new Vector3(
                speed * (player.velocity.x * -1),
                0,
                0);

            movement *= Time.deltaTime;
            transform.Translate(movement);
        }
    }
}