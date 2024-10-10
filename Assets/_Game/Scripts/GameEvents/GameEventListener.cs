using UnityEngine;
using UnityEngine.Events;

namespace PummelPartyClone
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent Event;

        public UnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public virtual void OnEventRaised()
        {
            Response.Invoke();
        }
    }

    public class GameEventListener<T> : MonoBehaviour
    {
        public GameEvent<T> Event;

        public UnityEvent<T> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(T param)
        {
            Response.Invoke(param);
        }
    }
    
    public class GameEventListener<T1, T2> : MonoBehaviour
    {
        public GameEvent<T1, T2> Event;
        public UnityEvent<T1, T2> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public virtual void OnEventRaised(T1 param1, T2 param2)
        {
            Response.Invoke(param1, param2);
        }
    }
}