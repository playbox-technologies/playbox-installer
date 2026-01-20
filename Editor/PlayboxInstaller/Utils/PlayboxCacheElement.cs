using System;

namespace PlayboxInstaller
{
    public class PlayboxCacheElement
    {
        public PlayboxCacheElement(object element, int updateTimeInMinute = 5)
        {
            Element = element;
            _updateTimeInMinute = updateTimeInMinute;
        }

        public object Element;
        public DateTime LastUpdate = DateTime.MinValue;

        private int _updateTimeInMinute = 5;

        public bool IsReadyForUpdate()
        {
            TimeSpan timeSinceLastUpdate = DateTime.UtcNow - LastUpdate;
            
            return timeSinceLastUpdate.TotalMinutes >= _updateTimeInMinute;
        }

        public void UpdateTime()
        {
            LastUpdate = DateTime.UtcNow;
        }
    }
}