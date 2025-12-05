using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Events
{
    /// <summary>
    /// A simple Event Bus to decouple systems.
    /// Usage: 
    /// EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
    /// EventBus.Publish(new PartSelectedEvent(partData));
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> Subscribers = new Dictionary<Type, List<Delegate>>();

        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (!Subscribers.ContainsKey(type))
            {
                Subscribers[type] = new List<Delegate>();
            }
            Subscribers[type].Add(callback);
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (Subscribers.ContainsKey(type))
            {
                Subscribers[type].Remove(callback);
            }
        }

        public static void Publish<T>(T eventToPublish)
        {
            var type = typeof(T);
            if (Subscribers.ContainsKey(type))
            {
                foreach (var subscriber in Subscribers[type])
                {
                    ((Action<T>)subscriber)?.Invoke(eventToPublish);
                }
            }
        }
    }
}
