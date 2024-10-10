using System.Collections.Generic;
using UnityEngine;

namespace PummelPartyClone
{
    [CreateAssetMenu(fileName = "NewItemCustomGameEvent", menuName = "Game Events/Item Custom Game Event")]
    public class ItemCustomGameEvent : GameEvent<PlayerController, int>
    {
    }
}