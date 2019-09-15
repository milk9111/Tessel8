using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class HighScore : MonoBehaviour
    {
        public int currScore;

        private string _currArena;

        private IDictionary<string, ArenaScore> _arenaScores;

        void Awake()
        {
            _currArena = SceneManager.GetActiveScene().name;
            _arenaScores = new Dictionary<string, ArenaScore>();

            if (!File.Exists("highscore.json"))
            {
                return;
            }
            
            using (var r = new StreamReader("highscore.json"))
            {
                var json = r.ReadToEnd();
                _arenaScores = JsonUtility.FromJson<ArenaScores>(json).scores.ToDictionary(k => k.arenaName, v => v);
            }
        }

        public int GetScoreForArena(string arenaName)
        {
            var arenaScore = GetScore(arenaName);

            return arenaScore == null ? 0 : arenaScore.highScore;
        }

        public int GetCurrentScore()
        {
            return currScore;
        }

        private ArenaScore GetScore(string arenaName)
        {
            return !_arenaScores.ContainsKey(arenaName)
                ? new ArenaScore
                {
                    arenaName = arenaName,
                    highScore = 0
                }
                : new ArenaScore
                {
                    arenaName = _arenaScores[arenaName].arenaName,
                    highScore = _arenaScores[arenaName].highScore
                };
        }

        public void AddToScore(int amount)
        {
            currScore += amount;
        }

        public void ResetScore()
        {
            currScore = 0;
        }

        public void GameOver()
        {
            var matchingArenaScore = GetScore(_currArena);

            matchingArenaScore.highScore =
                matchingArenaScore.highScore < currScore ? currScore : matchingArenaScore.highScore;

            if (!_arenaScores.ContainsKey(_currArena))
            {
                _arenaScores.Add(_currArena, matchingArenaScore);
            }
            else
            {
                _arenaScores[_currArena] = matchingArenaScore;
            }

            using (var s = new StreamWriter("highscore.json"))
            {                
                s.Write(JsonUtility.ToJson(new ArenaScores{scores = _arenaScores.Values.ToArray()}));
            }
        }
        
        [Serializable]
        internal class ArenaScore
        {
            public int highScore;
            public string arenaName;
            
            public override string ToString() {
                return "(" + arenaName + ", " + highScore + ")";
            }
        }
        
        [Serializable]
        internal class ArenaScores 
        {
            public ArenaScore[] scores;
        }
    }
}