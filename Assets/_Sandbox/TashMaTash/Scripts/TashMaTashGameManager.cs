using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PummelPartyClone
{
    public class TashMaTashGameManager : Singleton<TashMaTashGameManager>
    {
        
        private int _playerCount = 4;
        private List<PlayerController> _activePlayers = new();
        private  Stack<PlayerController> _playerResults = new();
        [SerializeField] private List<int> _baseCoinRewards= new List<int>
            {
                20, 10, 5, 0
            };
        [SerializeField] private int _maxCoinBonus = 10;
        [SerializeField] private float _timerBonus;
        [SerializeField] private GameEvent _onGameOver;
        private float _timer;

        public override void Awake()
        {
            base.Awake();
            InitPlayers();
        }
        
        public void PlayerHit(PlayerController player)
        {
            _activePlayers.Remove(player);
            _playerResults.Push(player);
            if (_playerResults.Count == _playerCount - 1)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            //Add remaining player to the results
            _playerResults.Push(_activePlayers[0]);
            RewardPlayers();
            RemoveTashMaTashComponents();
            _onGameOver.Raise();
        }
        
        private void RewardPlayers()
        {
            List<int> rewards = GetResultsCoins();
            List<PlayerController> results = GetResultsPlayer();
            for (int i = 0; i < _playerCount; i++)
            {
                results[i].Inventory.AddSubCoins(rewards[i]);
            }
        }
        
        private void RemoveTashMaTashComponents()
        {
            foreach (var player in _activePlayers)
            {
                Destroy(player.GetComponent<TashMaTashPlayer>());
            }
        }

        private void InitPlayers()
        {
            PlayerController[] scenePlayers = FindObjectsOfType<PlayerController>();
            _playerCount = scenePlayers.Length;
            
            // Add TashMaTashPlayer script to all players and add them to the list
            foreach (var player in scenePlayers)
            {
                player.gameObject.AddComponent<TashMaTashPlayer>();
                _activePlayers.Add(player);
            }
        }
        
        public List<PlayerController> GetResultsPlayer()
        {
            return _playerResults.ToList();
        }

        public List<int> GetResultsCoins()
        {
            List<int> rewards = new List<int>();
            foreach (var reward in _baseCoinRewards)
            {
                // Add the bonus coins to the reward
                float newRewardFloat = Mathf.Min(reward + _maxCoinBonus, reward + _timer);
                int newReward = (int) newRewardFloat;
                rewards.Add(newReward);
            }

            return rewards;
        }
        
        void FixedUpdate()
        {
            _timer += _timerBonus;
        }
    }
}
