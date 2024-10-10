using UnityEngine;
using UnityEngine.Events;

namespace PummelPartyClone
{
    
    public class AsymmetricalGameEventListener<T1, T2> : GameEventListener<T1, T2>
    {
        public UnityEvent<T2> AsymmetricalResponse;
    }
}