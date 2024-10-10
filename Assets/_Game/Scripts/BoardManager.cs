using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox.TileBehaviourDemo.Scripts;
using UnityEngine;

namespace PummelPartyClone
{
    [System.Serializable]
    public class MovementDependency
    {
        public List<int> MovableTileIds;
    }
    public class BoardManager : Singleton<BoardManager>
    {
        private Dictionary<int, PlayerController> _playerDict;
        private Dictionary<int, Tile> _tileDict;
        private Dictionary<int, int> _playerIdTileIdDict;
        [SerializeField] private List<MovementDependency> _tileIDMovementMatrix; // 2D matrix to store the board layout;
        [SerializeField] private GameEvent OnTurnEnd;
        private TaskCompletionSource<int> _branchSelectionTcs;

        public override void Awake()
        {
            base.Awake();

            InitTiles();
            InitPlayers();
        }

        #region Initialization

        private void InitTiles()
        {
            _tileDict = new Dictionary<int, Tile>();
            Tile[] sceneTiles = FindObjectsOfType<Tile>()
                .OrderBy(tile => tile.transform.GetSiblingIndex())
                .ToArray(); //order tiles by their order in the scene object hierarchy

            for (var i = 0; i < sceneTiles.Length; i++)
            {
                sceneTiles[i].Id = i;
                _tileDict[i] = sceneTiles[i];
            }
        }

        private void InitPlayers()
        {
            _playerDict = new Dictionary<int, PlayerController>();
            PlayerController[] scenePlayers = FindObjectsOfType<PlayerController>()
                .OrderBy(player => player.transform.GetSiblingIndex())
                .ToArray(); //order player by their order in the scene object hierarchy

            for (var i = 0; i < scenePlayers.Length; i++)
            {
                scenePlayers[i].PlayerId = i;
                _playerDict[i] = scenePlayers[i];
            }

            _playerIdTileIdDict = new Dictionary<int, int>();
            foreach (var player in scenePlayers)
            {
                _playerIdTileIdDict.Add(player.PlayerId, 0);
            }
        }

        #endregion

        #region Getters and Setters

        public Tile GetTileByTileId(int id)
        {
            _tileDict.TryGetValue(id, out Tile tile);
            return tile;
        }

        public Tile GetTileByPlayerId(int playerId)
        {
            _playerIdTileIdDict.TryGetValue(playerId, out int tileId);
            _tileDict.TryGetValue(tileId, out Tile tile);
            return tile;
        }

        public int GetTileCount()
        {
            return _tileDict.Count;
        }

        public int GetPlayerCount()
        {
            return _playerDict.Count;
        }

        public List<PlayerController> GetPlayers()
        {
            return _playerDict.Values.ToList();
        }

        public PlayerController GetPlayerById(int playerId)
        {
            _playerDict.TryGetValue(playerId, out PlayerController player);
            return player;
        }

        public void SetPlayerTileId(int playerId, int tileId)
        {
            if (tileId >= GetTileCount())
            {
                tileId = tileId % GetTileCount();
            }

            _playerIdTileIdDict[playerId] = tileId;
        }

        public void SetBranchSelection(int selectedBranchTileId)
        {
            _branchSelectionTcs?.SetResult(selectedBranchTileId);
        }
        #endregion

        public async void MoveCurrentPlayer(int steps)
        {
            int currentPlayerId = TurnManager.Instance.CurrentPlayerId;
            PlayerController currentPlayer = GetPlayerById(currentPlayerId);
            int currentTileId = _playerIdTileIdDict[currentPlayerId];

            while (steps > 0)
            {
                List<int> movableTileIds = _tileIDMovementMatrix[currentTileId].MovableTileIds;
                Debug.Log($"Movable tiles from tile {currentTileId}: {string.Join(", ", movableTileIds)}");

                int nextTileId = movableTileIds[0];
                if (movableTileIds.Count > 1)
                {
                    nextTileId = await HandleBranching(movableTileIds);
                }

                Debug.Log($"Moving to tile {nextTileId} from tile {currentTileId}. Roll left: {steps}");
                Vector3 nextTilePosition = GetTileByTileId(nextTileId).transform.position;
                await currentPlayer.MoveToPosition(nextTilePosition); //wait for player to finish moving

                currentTileId = nextTileId;
                steps--;
            }

            _playerIdTileIdDict[currentPlayerId] = currentTileId;
            OnTurnEnd.Raise();
        }

        private async Task<int> HandleBranching(List<int> movableTileIds)
        {
            int nextTileId;
            _branchSelectionTcs = new TaskCompletionSource<int>();

            foreach (var tileId in movableTileIds)
            {
                Tile tile = GetTileByTileId(tileId);
                tile.Highlight();
            }

            nextTileId = await _branchSelectionTcs.Task;

            foreach (var tileId in movableTileIds)
            {
                Tile tile = GetTileByTileId(tileId);
                tile.Unhighlight();
            }

            return nextTileId;
        }
    }
}
