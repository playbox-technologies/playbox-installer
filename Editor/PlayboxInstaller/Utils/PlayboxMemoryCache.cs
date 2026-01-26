using System.Collections.Generic;

namespace PlayboxInstaller
{
    public class PlayboxMemoryCache
    {
        private static Dictionary<string,PlayboxCacheElement> _cache = new ();

        public static void Push(KeyValuePair<string, PlayboxCacheElement> data)
        {
            if (!_cache.ContainsKey(data.Key))
            {
                data.Value.UpdateTime();
                
                _cache.Add(data.Key, data.Value);
                return;
            }

            if (data.Value.IsReadyForUpdate())
            {
                data.Value.UpdateTime();
                _cache[data.Key] = data.Value;   
            }
        }
        
        public static PlayboxCacheElement Get(string key) => _cache.ContainsKey(key) ? _cache[key] : null;
        
        public static bool Exists(string key) => _cache.ContainsKey(key);
        
        public static void Clear() => _cache.Clear();
    }
}