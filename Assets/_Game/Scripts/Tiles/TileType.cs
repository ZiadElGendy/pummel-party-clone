using UnityEngine;

namespace PummelPartyClone
{
    [CreateAssetMenu(fileName = "TileType", menuName = "Tiles/TileType")]
    public class TileType : ScriptableObject
    {
        public Color Color;
        public string EntryLocation;
        public int CoinValue;
    }
}