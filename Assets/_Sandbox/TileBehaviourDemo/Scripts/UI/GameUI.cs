using System.Collections.Generic;
using PummelPartyClone;
using TMPro;
using UnityEngine;

namespace Sandbox.TileBehaviourDemo.Scripts.UI
{
    public class GameUI : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        void OnEnable()
        {
            OnChange();
        }

        // Update is called once per frame
        public void OnChange()
        {
            PlayerController currentPlayer = TurnManager.Instance.CurrentPlayer;
            List<PlayerController> Players = BoardManager.Instance.GetPlayers();
            Inventory inventory = currentPlayer.Inventory;

            _text.text = $"Player: {currentPlayer.PlayerId + 1}\n" +
                         $"Coins: {inventory.Coins}\n " +
                         $"Items: {string.Join(", ", inventory.GetItems())}" +
                         "\n\n";

            foreach (PlayerController player in Players)
            {
                _text.text += $"Player {player.PlayerId + 1} Health: {player.Health}\n";
            }

        }
    }
}
