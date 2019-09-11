using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    public class Credits : MonoBehaviour
    {
        public MainMenu startScreen;
        
        private Animation _animation;

        void Start()
        {
            _animation = GetComponent<Animation>();
        }

        public void StartCredits()
        {
            if (_animation == null)
            {
                _animation = GetComponent<Animation>();
            }
            
            _animation.Rewind();
            _animation.Play();
        }
        
        public void EndCredits()
        {
            startScreen.OnBack();
        }
    }
}

