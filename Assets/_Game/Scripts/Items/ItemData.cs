using UnityEngine;
using UnityEngine.Serialization;

namespace PummelPartyClone
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string Id;
        public Sprite Icon;
        public string ItemName;
        public string ItemDescription;
        public int Price;
        public int EffectValue;
    }
}