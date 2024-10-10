using UnityEngine;
using UnityEngine.Events;

namespace PummelPartyClone
{
    public class ItemCustomGameEventListener : AsymmetricalGameEventListener<PlayerController, int>
    {
        public override void OnEventRaised(PlayerController playerInvoking, int param)
        {
            PlayerController playerAttached = GetComponent<PlayerController>();
            if (playerInvoking != playerAttached) return;
            AsymmetricalResponse.Invoke(param);
        }
    }
}