using UnityEngine;
using UnityEngine.Serialization;

namespace PummelPartyClone
{
    public class Item : MonoBehaviour
    {
        public ItemData Data;
        //TODO: Think about how to reuse events with different values
        public ItemCustomGameEvent OnUseEvent;

        public void OnUse(PlayerController player)
        {
            int param = Data.EffectValue;
            OnUseEvent.Raise(player, param);
        }
    }
}
