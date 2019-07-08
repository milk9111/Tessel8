using System.Collections.Generic;
using System.Linq;
using Spawn.Domain;
using Spawn.Domain.Round;
using UnityEngine;
using UserInterface;

namespace Spawn
{
    public class SpawnUIController : MonoBehaviour
    {
        private const string ROUND_TEXT_PREFIX = "Round ";
        private const string WAVE_TEXT_PREFIX = "Wave ";
        private const string VICTORY_TEXT = "Victory!";
        
        public FadingText roundText;
        public FadingText waveText;
        public FadingText timerText;

        private Queue<SpawnQueueMember> _uiQueue;
        private SpawnQueueMember _currMember;

        private bool _isPaused;
        private bool _isFinished;

        private SpawnQueueMembers _lastType;

        void Awake()
        {
            _uiQueue = new Queue<SpawnQueueMember>();
        }

        void Update()
        {
            if (_uiQueue == null) _uiQueue = new Queue<SpawnQueueMember>();
            CheckCurrMemberIsDone();
            
            if (_isPaused || _isFinished || _currMember != null || !_uiQueue.Any()) return;
            
            _currMember = _uiQueue.Dequeue();
            switch (_currMember.type)
            {
                case SpawnQueueMembers.Round:
                    timerText.TurnOff();
                    roundText.SetText(ROUND_TEXT_PREFIX + _currMember.index);
                    roundText.TurnOn();
                    roundText.FadeIn();
                    timerText.SetRound(_currMember.round);
                    TurnOnTimer();
                    break;
                case SpawnQueueMembers.Wave:
                    waveText.SetText(WAVE_TEXT_PREFIX + _currMember.index);
                    waveText.TurnOn();
                    waveText.FadeIn();
                    TurnOnTimer();
                    break;
                case SpawnQueueMembers.Victory:
                    _isFinished = true;
                    roundText.SetText(VICTORY_TEXT);
                    roundText.TurnOn();
                    roundText.FadeIn();
                    break;
                default:
                    Debug.Log("Invalid spawn type in SpawnUiController: " + _currMember.type);
                    break;
            }
        }

        public void NewRound(RoundTypes type, IRound round, int roundNumber)
        {
            if (_uiQueue == null) _uiQueue = new Queue<SpawnQueueMember>();
            _uiQueue.Enqueue(new SpawnQueueMember(SpawnQueueMembers.Round, type, round, roundNumber));
        }

        public void NewWave(int waveNumber)
        {
            _uiQueue.Enqueue(new SpawnQueueMember(SpawnQueueMembers.Wave, waveNumber));
        }

        public void NewVictory()
        {
            _uiQueue.Enqueue(new SpawnQueueMember(SpawnQueueMembers.Victory));
        }

        public bool IsFinished()
        {
            return _isFinished;
        }

        public void OnPause()
        {
            _isPaused = true;
        }

        public void OnPlay()
        {
            _isPaused = false;
        }

        private void CheckCurrMemberIsDone()
        {
            // if _currMember is null then nothing is displaying
            if (_currMember == null) return;

            // get the current member ui text status
            bool isOn;
            switch (_currMember.type)
            {
                case SpawnQueueMembers.Round:
                    isOn = roundText.IsOn();
                    break;
                case SpawnQueueMembers.Wave:
                    isOn = waveText.IsOn();
                    break;
                default:
                    Debug.Log("Invalid spawn type in SpawnUiController: " + _currMember.type);
                    return;
            }

            // if it's still on then return cause it isn't done animating
            if (isOn) return;

            // set to null when it isn't on because the animation is finished
            _currMember = null;
        }

        private void TurnOnTimer()
        {
            switch (_currMember.subType)
            {
                case RoundTypes.TimedWave:
                case RoundTypes.Survival:
                    if (!timerText.IsOn())
                    {
                        timerText.TurnOn();
                    }

                    return;
                default:
                    return;
            }
        }

        private class SpawnQueueMember
        {
            public SpawnQueueMembers type;
            public RoundTypes subType;
            public IRound round;
            public int index;
            
            public SpawnQueueMember(SpawnQueueMembers theType, RoundTypes theSubType, IRound theRound, int theIndex)
            {
                type = theType;
                subType = theSubType;
                round = theRound;
                index = theIndex;
            }

            public SpawnQueueMember(SpawnQueueMembers theType, int theIndex)
            {
                type = theType;
                subType = RoundTypes.Normal;
                index = theIndex;
            }
            
            public SpawnQueueMember(SpawnQueueMembers theType)
            {
                type = theType;
                subType = RoundTypes.Normal;
                index = -1;
            }
        }

        private enum SpawnQueueMembers
        {
            Round = 1,
            Wave = 2,
            Victory = 3
        }
    }
}