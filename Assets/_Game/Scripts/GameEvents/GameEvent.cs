using System.Collections.Generic;
using UnityEngine;

namespace PummelPartyClone
{
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _eventListeners =
            new List<GameEventListener>();

        public void Raise()
        {
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised();
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }
    }

    public abstract class GameEvent<T> : ScriptableObject
    {
        private readonly List<GameEventListener<T>> _eventListeners =
            new List<GameEventListener<T>>();

        public void Raise(T param)
        {
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised(param);
        }

        public void RegisterListener(GameEventListener<T> listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener<T> listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }
    }
    
    public abstract class GameEvent<T1, T2> : ScriptableObject
    {
        private readonly List<GameEventListener<T1, T2>> _eventListeners = new List<GameEventListener<T1, T2>>();

        public void Raise(T1 param1, T2 param2)
        {
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised(param1, param2);
        }

        public void RegisterListener(GameEventListener<T1, T2> listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener<T1, T2> listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }
    }
}