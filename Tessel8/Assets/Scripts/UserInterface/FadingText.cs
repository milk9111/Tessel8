using System;
using Spawn.Domain.Round;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class FadingText : MonoBehaviour
    {
        private Animation _fullFade;
        private Text _text;
        private bool _isOn;
        private IRound _round;

        void Awake()
        {
            _fullFade = GetComponent<Animation>();
            _text = GetComponent<Text>();
            _round = null;
            
            _text.enabled = false;
        }

        void Update()
        {
            if (_round == null || !_isOn)
            {
                return;
            }
            
            SetText(FormatTime(_round.GetRemainingTime()));
        }

        public void FadeIn()
        {
            _fullFade.Play();
        }

        public void TurnOff()
        {
            _text.enabled = false;
            _isOn = false;
        }

        public void TurnOn()
        {
            _text.enabled = true;
            _isOn = true;
        }

        public void SetRound(IRound round)
        {
            _round = round;
        }

        public bool IsOn()
        {
            return _isOn;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        private string FormatTime(float secondsRemaining)
        {
            var minutes = Mathf.FloorToInt((secondsRemaining - secondsRemaining % 60) / 60);
            var seconds = Mathf.FloorToInt(Math.Abs(minutes * 60 - secondsRemaining));
            return string.Format("{0:00} : {1:00}", minutes, seconds);
        }
    }
}