using System.Collections.Generic;
using System.Linq;
using David6.ShooterCore.Provider;
using David6.ShooterCore.TickSystem;

namespace David6.ShooterCore.Cooldown
{
    public class DCooldownHandler : IDCooldownProvider, IDTickable
    {
        Dictionary<string, float> _cooldowns = new Dictionary<string, float>();

        public void StartCooldown(string key, float duration)
        {
            _cooldowns[key] = duration;
        }
        public bool IsReady(string key)
        {
            if (!_cooldowns.ContainsKey(key))
            {
                return true;
            }
            if (_cooldowns[key] <= 0.0f)
            {
                _cooldowns.Remove(key);
                return true;
            }
            
            return false;
        }
        public void Tick(float deltaTime)
        {
            List<string> keys = _cooldowns.Keys.ToList();
            foreach (string key in keys)
            {
                _cooldowns[key] -= deltaTime;
                if (_cooldowns[key] <= 0.0f)
                {
                    _cooldowns.Remove(key);
                }
            }
        }
    }
}