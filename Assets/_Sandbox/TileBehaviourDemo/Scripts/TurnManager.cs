using PummelPartyClone;
using Sandbox.TileBehaviourDemo.Scripts.UI;
using UnityEngine;

namespace Sandbox.TileBehaviourDemo.Scripts
{
    public class TurnManager : Singleton<TurnManager>
    {
        private BoardManager _boardManager => BoardManager.Instance;
        private UIManager _uiManager;
        private int _turnCount;
        public int CurrentPlayerId;
        public PlayerController CurrentPlayer => _boardManager.GetPlayerById(CurrentPlayerId);
        private Tile _currentTile => _boardManager.GetTileByPlayerId(CurrentPlayerId);
        public bool IsUsedItem = false;
        public GameEvent OnTurnEnd;
        public GameEvent OnEnterableTileStep;
        public GameEvent OnEnterableTileLeave;
        public GameEvent OnPlayerChange;
        public IntGameEvent OnDiceRoll;
        public int DEBUG_ROLL = -1;

        public override void Awake()
        {
            base.Awake();
            _uiManager = FindObjectOfType<UIManager>();
            _turnCount = 0;
            CurrentPlayerId = 0;
        }

        public void Moved()
        {
            int roll = CurrentPlayer.RollDice();
            Debug.Log($"Rolled a {roll}");
            if (DEBUG_ROLL != -1)
            {
                roll = DEBUG_ROLL;
            }
            OnDiceRoll.Raise(roll);
        }

        public void UsedItem()
        {
            IsUsedItem = true;
        }

        public void TurnEnded()
        {
            UpdateOldPlayerState();
            CurrentPlayerId = (CurrentPlayerId + 1) % _boardManager.GetPlayerCount();
            UpdateNewPlayerState();
            _turnCount++;
            OnPlayerChange.Raise();
        }

        private void UpdateOldPlayerState()
        {
            _currentTile.OnStep(CurrentPlayer);
        }

        private void UpdateNewPlayerState()
        {
            if (_currentTile.IsEnterable())
            {
                OnEnterableTileStep.Raise();
            }
            else
            {
                OnEnterableTileLeave.Raise();
            }
            IsUsedItem = false;
        }
    }
}
