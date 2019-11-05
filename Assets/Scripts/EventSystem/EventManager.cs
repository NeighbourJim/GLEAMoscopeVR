using System;
using System.Collections.Generic;

namespace GLEAMoscopeVR.Events
{
    /// <summary>
    /// Type-safe, loosely-coupled system that handles generic events derived from <see cref="GlobalEvent"/>.
    /// Could create garbage collection penalties??? but should not be a problem for an app of this scale.
    /// http://www.willrmiller.com/?p=87
    /// </summary>
    public class EventManager
    {
        private static EventManager _instance = null;
        public static EventManager Instance => _instance ?? (_instance = new EventManager());

        /// <summary>
        /// Generic event delegate for events of type <see cref="GlobalEvent"/>
        /// </summary>
        /// <typeparam name="T">The event type (will be <see cref="GlobalEvent"/> or a derived class).</typeparam>
        /// <param name="e">The event instance (gives access to the properties for that specific type of event).</param>
        public delegate void EventDelegate<T>(T e) where T : GlobalEvent;

        /// <summary>
        /// Non-generic delegate for 
        /// </summary>
        /// <param name="e"></param>
        private delegate void EventDelegate(GlobalEvent e);

        /// <summary>
        /// Generic event-type dictionary.
        /// </summary>
        private Dictionary<Type, EventDelegate> _delegates = new Dictionary<Type, EventDelegate>();

        /// <summary>
        /// Non-generic event type dictionary.
        /// </summary>
        private Dictionary<Delegate, EventDelegate> _delegateLookup = new Dictionary<Delegate, EventDelegate>();
        
        /// <summary>
        /// Registers a 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        public void AddListener<T>(EventDelegate<T> del) where T : GlobalEvent
        {
            if (_delegateLookup.ContainsKey(del)) return;
            
            EventDelegate nonGenericDelegate = e => del((T)e);
            _delegateLookup[del] = nonGenericDelegate;

            if (_delegates.TryGetValue(typeof(T), out var tempDelegate))
            {
                _delegates[typeof(T)] = tempDelegate += nonGenericDelegate;
            }
            else
            {
                _delegates[typeof(T)] = nonGenericDelegate;
            }
        }

        public void RemoveListener<T>(EventDelegate<T> del) where T : GlobalEvent
        {
            if (_delegateLookup.TryGetValue(del, out EventDelegate internalDelegate))
            {
                if (_delegates.TryGetValue(typeof(T), out EventDelegate tempDelegate))
                {
                    tempDelegate -= internalDelegate;
                    if (tempDelegate == null)
                    {
                        _delegates.Remove(typeof(T));
                    }
                    else
                    {
                        _delegates[typeof(T)] = tempDelegate;
                    }
                }

                _delegateLookup.Remove(del);
            }
        }

        public void Raise(GlobalEvent e)
        {
            if (_delegates.TryGetValue(e.GetType(), out EventDelegate del))
            {
                del.Invoke(e);
            }
        }
    }
}