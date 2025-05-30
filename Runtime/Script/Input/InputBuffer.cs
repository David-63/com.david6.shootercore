using System.Collections.Generic;
using Codice.CM.Client.Differences.Graphic;
using UnityEngine;

namespace David6.ShooterFramework
{
    public class InputBuffer
    {
        struct tEntry { public string Name; public float Time; public float Expire; }
        private List<tEntry> _list = new List<tEntry>();
        private float _default;

        public InputBuffer(float defaultBufferTime) => _default = defaultBufferTime;

        public void Enqueue(string name, float buffer = -1f)
        {
            _list.Add(new tEntry { Name = name, Time = Time.time, Expire = buffer > 0 ? buffer : _default });
        }
        public List<string> DequeueAll()
        {
            float now = Time.time;
            var ready = new List<string>();
            for(int idx = _list.Count - 1; idx >= 0; idx--)
            {
                var e = _list[idx];
                if (now - e.Time <= e.Expire)
                {
                    ready.Add(e.Name);
                }
                _list.RemoveAt(idx);
            }
            return ready;
        }
    }
}