using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Events
{
    /// <summary>
    /// A lightweight publish-subscribe event bus for decoupled inter-system communication.
    /// Allows systems to communicate without direct references to each other.
    /// </summary>
    /// <remarks>
    /// <para>Usage example:</para>
    /// <code>
    /// // Subscribe to an event
    /// EventBus.Subscribe&lt;PartSelectedEvent&gt;(OnPartSelected);
    /// 
    /// // Publish an event
    /// EventBus.Publish(new PartSelectedEvent(partData));
    /// 
    /// // Unsubscribe when done (important to prevent memory leaks!)
    /// EventBus.Unsubscribe&lt;PartSelectedEvent&gt;(OnPartSelected);
    /// </code>
    /// </remarks>
    public static class EventBus
    {
        #region Private Fields

        private static readonly Dictionary<Type, List<Delegate>> Subscribers = 
            new Dictionary<Type, List<Delegate>>();

        private static readonly object LockObject = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribes a callback to the specified event type.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to.</typeparam>
        /// <param name="callback">The callback to invoke when the event is published.</param>
        public static void Subscribe<T>(Action<T> callback)
        {
            if (callback == null)
            {
                Debug.LogWarning("[EventBus] Attempted to subscribe null callback");
                return;
            }

            lock (LockObject)
            {
                var type = typeof(T);
                
                if (!Subscribers.ContainsKey(type))
                {
                    Subscribers[type] = new List<Delegate>();
                }
                
                if (!Subscribers[type].Contains(callback))
                {
                    Subscribers[type].Add(callback);
                }
            }
        }

        /// <summary>
        /// Unsubscribes a callback from the specified event type.
        /// Always unsubscribe when the subscriber is destroyed to prevent memory leaks.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from.</typeparam>
        /// <param name="callback">The callback to remove.</param>
        public static void Unsubscribe<T>(Action<T> callback)
        {
            if (callback == null) return;

            lock (LockObject)
            {
                var type = typeof(T);
                
                if (Subscribers.ContainsKey(type))
                {
                    Subscribers[type].Remove(callback);
                }
            }
        }

        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <typeparam name="T">The event type to publish.</typeparam>
        /// <param name="eventToPublish">The event instance to publish.</param>
        public static void Publish<T>(T eventToPublish)
        {
            var type = typeof(T);
            List<Delegate> subscribersCopy;

            lock (LockObject)
            {
                if (!Subscribers.ContainsKey(type) || Subscribers[type].Count == 0)
                {
                    return;
                }

                // Create a copy to avoid modification during iteration
                subscribersCopy = new List<Delegate>(Subscribers[type]);
            }

            foreach (var subscriber in subscribersCopy)
            {
                try
                {
                    ((Action<T>)subscriber)?.Invoke(eventToPublish);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] Error invoking subscriber for {type.Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Clears all subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="T">The event type to clear.</typeparam>
        public static void ClearSubscribers<T>()
        {
            lock (LockObject)
            {
                var type = typeof(T);
                
                if (Subscribers.ContainsKey(type))
                {
                    Subscribers[type].Clear();
                }
            }
        }

        /// <summary>
        /// Clears all subscribers for all event types.
        /// Use with caution, typically only during application cleanup.
        /// </summary>
        public static void ClearAllSubscribers()
        {
            lock (LockObject)
            {
                Subscribers.Clear();
            }
            
            Debug.Log("[EventBus] All subscribers cleared");
        }

        /// <summary>
        /// Gets the number of subscribers for a specific event type.
        /// Useful for debugging.
        /// </summary>
        /// <typeparam name="T">The event type to check.</typeparam>
        /// <returns>The number of subscribers.</returns>
        public static int GetSubscriberCount<T>()
        {
            lock (LockObject)
            {
                var type = typeof(T);
                return Subscribers.ContainsKey(type) ? Subscribers[type].Count : 0;
            }
        }

        #endregion
    }
}
