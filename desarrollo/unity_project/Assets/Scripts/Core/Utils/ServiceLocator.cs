using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Utils
{
    /// <summary>
    /// Lightweight Service Locator for decoupling consumers from concrete Singleton references.
    /// All <see cref="StaticInstance{T}"/> subclasses auto-register on Awake.
    /// Consumers can resolve via <c>ServiceLocator.Get&lt;T&gt;()</c> instead of <c>T.Instance</c>.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, MonoBehaviour> _services = new();

        /// <summary>Register a service. Called automatically by <see cref="StaticInstance{T}"/>.</summary>
        public static void Register<T>(T service) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services[type] = service;
                return;
            }
            _services.Add(type, service);
        }

        /// <summary>Resolve a registered service. Throws if not found.</summary>
        public static T Get<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
                return service as T;

            throw new InvalidOperationException(
                $"[ServiceLocator] Service '{type.Name}' not registered. " +
                "Ensure the MonoBehaviour is in the scene and has initialized.");
        }

        /// <summary>Try to resolve a service without throwing.</summary>
        public static bool TryGet<T>(out T service) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var mb))
            {
                service = mb as T;
                return service != null;
            }
            service = null;
            return false;
        }

        /// <summary>Check if a service is registered.</summary>
        public static bool Has<T>() where T : MonoBehaviour
            => _services.ContainsKey(typeof(T));

        /// <summary>Remove a service registration. Called automatically on quit.</summary>
        public static void Unregister<T>() where T : MonoBehaviour
            => _services.Remove(typeof(T));

        /// <summary>Clear all registrations. Useful for scene reloads or testing.</summary>
        public static void Clear() => _services.Clear();
    }
}
