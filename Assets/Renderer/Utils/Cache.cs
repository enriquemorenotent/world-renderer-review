using System;
using System.Collections.Generic;
using System.Linq;

namespace Extinction.Utils
{
    public interface ICache {}

    public class Cache<TKey, TValue> : ICache
    {
        // Attributes

        Dictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();

        Func<TKey, TValue> generator;

        Func<TKey, bool> cleanUp;

        // Properties

        public int Count { get { return data.Count; } }

        // Constructor

        public Cache(Func<TKey, TValue> _generator, Func<TKey, bool> _cleanup = null)
        {
            generator = _generator;
            cleanUp = _cleanup;
        }

        // Methods

        public bool TryGetValue(TKey key, out TValue value) => data.TryGetValue(key, out value);

        public TValue At(TKey key)
        {
            if (generator == null) throw new ArgumentNullException();

            TValue value;

            lock (data) {

                if (!data.TryGetValue(key, out value))
                {
                    value = generator(key);
                    data[key] = value;
                }
            }

            return value;
        }

        public void CleanUp()
        {
            lock (data)
            {
                if (cleanUp == null) return;

                var toRemove = data.Where(pair => cleanUp(pair.Key))
                    .Select(pair => pair.Key)
                    .ToList();

                foreach (var key in toRemove)
                {
                    data.Remove(key);
                }
            }
        }
    }
}
