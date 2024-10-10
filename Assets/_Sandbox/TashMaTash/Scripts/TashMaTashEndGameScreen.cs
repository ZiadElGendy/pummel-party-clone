using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PummelPartyClone
{
    public class TashMaTashEndGameScreen : MonoBehaviour
    {
        public VisualTreeAsset uxmlTemplate;
        private List<PlayerController> _resultsPlayers;
        private List<int> _resultsCoins;
        
        public void CreateGUI()
        {
            // Load and instantiate the UXML template
            var root = uxmlTemplate.CloneTree();
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.rootVisualElement.Add(root);

            // Get data from TashMaTashGameManager instance
            _resultsPlayers = TashMaTashGameManager.Instance.GetResultsPlayer();
            _resultsCoins = TashMaTashGameManager.Instance.GetResultsCoins();

            // Access the Results container
            var resultsContainer = root.Q<VisualElement>("Results");

            // Loop through the players and dynamically create PlayerInfo elements
            for (int i = 0; i < _resultsPlayers.Count; i++)
            {
                var player = _resultsPlayers[i];

                // Create a new PlayerInfo element
                var playerInfoTemplate = new VisualElement();
                playerInfoTemplate.AddToClassList("PlayerInfo");

                // Set the player position
                var positionLabel = new Label { text = (i+1).ToString() };
                positionLabel.AddToClassList("Position");
                playerInfoTemplate.Add(positionLabel);

                // Set the player name
                var nameLabel = new Label { text = $"Player: {player.PlayerId}" };
                nameLabel.AddToClassList("Name");
                playerInfoTemplate.Add(nameLabel);

                // Set the player coins
                var coinsContainer = new VisualElement();
                coinsContainer.AddToClassList("Coins");

                var coinsLabel = new Label { text = _resultsCoins[i].ToString() };
                coinsLabel.AddToClassList("CoinsNum");
                coinsContainer.Add(coinsLabel);

                var coinsImg = new VisualElement();
                coinsImg.AddToClassList("CoinsImg");
                coinsContainer.Add(coinsImg);

                playerInfoTemplate.Add(coinsContainer);

                // Add the PlayerInfo element to the Results container
                resultsContainer.Add(playerInfoTemplate);
            }
            
        }
    }
}