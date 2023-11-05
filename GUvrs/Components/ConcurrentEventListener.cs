using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUvrs.Components
{
    public static class ConcurrentEventListener
    {
        private static readonly ConcurrentDictionary<string, List<Action<Dictionary<string, string>>>> _listeners = new();
        private static readonly object _lock = new();

        public static void Register(string name, Action<Dictionary<string, string>> callback)
        {
            lock (_lock)
            {
                if (!_listeners.ContainsKey(name))
                {
                    _listeners[name] = new();
                }

                _listeners[name].Add(callback);
            }
        }

        public static void Trigger(string name, Dictionary<string, string> data)
        {
            if (_listeners.TryGetValue(name, out var callbacks))
            {
                List<Action<Dictionary<string, string>>> callbacksSnapshot;
                lock (_lock)
                {
                    callbacksSnapshot = new(callbacks);
                }

                Parallel.ForEach(callbacksSnapshot, (callback) =>
                {
                    callback(data);
                });
            }
        }
    }
}
