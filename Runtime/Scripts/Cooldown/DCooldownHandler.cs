using System.Collections.Generic;
using System.Linq;
using David6.ShooterCore.Provider;
using David6.ShooterCore.TickSystem;

namespace David6.ShooterCore.Cooldown
{
    class CooldownData
    {
        public float RemainingTime;
        public bool IsLocked;

        public CooldownData(float duration)
        {
            RemainingTime = duration;
            IsLocked = false;
        }
    }

    public class DCooldownHandler : IDCooldownProvider, IDTickable
    {
        Dictionary<string, CooldownData> _cooldowns = new Dictionary<string, CooldownData>();

        public void StartCooldown(string key, float duration)
        {
            _cooldowns[key] = new CooldownData(duration);
        }
        public void LockCooldown(string key)
        {
            if (_cooldowns.ContainsKey(key))
            {
                _cooldowns[key].IsLocked = true;
            }
        }
        public void UnlockCooldown(string key)
        {
            if (_cooldowns.ContainsKey(key))
            {
                _cooldowns[key].IsLocked = false;
            }
        }
        public void CancelCooldown(string key)
        {
            if (_cooldowns.ContainsKey(key))
            {
                _cooldowns.Remove(key);
            }
        }
        public bool IsReady(string key)
        {
            if (!_cooldowns.ContainsKey(key)) return true;

            if (_cooldowns[key].IsLocked) return false;

            if (_cooldowns[key].RemainingTime <= 0.0f)
            {
                _cooldowns.Remove(key);
                return true;
            }

            return false;
        }
        public bool HasCooldown(string key)
        {
            return _cooldowns.ContainsKey(key);
        }

        public void Tick(float deltaTime)
        {
            List<string> keys = _cooldowns.Keys.ToList();
            foreach (string key in keys)
            {
                var target = _cooldowns[key];
                if (target.IsLocked) continue;

                target.RemainingTime -= deltaTime;
                if (target.RemainingTime <= 0.0f)
                {
                    _cooldowns.Remove(key);
                }
                else
                {
                    _cooldowns[key] = target;
                }
            }
        }
    }
}