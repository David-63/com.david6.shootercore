using System.Collections.Generic;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.TickSystem
{
    public class DGameLoop : MonoBehaviour, IDTickSchedulerProvider
    {
        public static DGameLoop Instance { get; private set; }
        readonly List<IDTickable> _tickables = new();
        readonly List<IDFixedTickable> _fixedTickables = new();
        readonly List<IDLateTickable> _lateTickables = new();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Register(object tickableObject)
        {
            if (tickableObject is IDTickable iTickabe && !_tickables.Contains(iTickabe))
            {
                Log.WhatHappend("Tick 등록:" + tickableObject);
                _tickables.Add(iTickabe);
            }
            if (tickableObject is IDFixedTickable iFixedTickabe && !_fixedTickables.Contains(iFixedTickabe))
            {
                Log.WhatHappend("FixedTick 등록:" + tickableObject);
                _fixedTickables.Add(iFixedTickabe);
            }
            if (tickableObject is IDLateTickable iLateTickabe && !_lateTickables.Contains(iLateTickabe))
            {
                Log.WhatHappend("LateTick 등록:" + tickableObject);
                _lateTickables.Add(iLateTickabe);
            }
        }
        public void Unregister(object tickableObject)
        {
            if (tickableObject is IDTickable iTickabe)
            {
                _tickables.Remove(iTickabe);
            }
            if (tickableObject is IDFixedTickable iFixedTickabe)
            {
                _fixedTickables.Remove(iFixedTickabe);
            }
            if (tickableObject is IDLateTickable iLateTickabe)
            {
                _lateTickables.Remove(iLateTickabe);
            }
        }

        void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;

            foreach (IDFixedTickable fixedTickable in _fixedTickables)
            {
                fixedTickable.FixedTick(deltaTime);
            }
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;

            foreach (IDTickable tickable in _tickables)
            {
                tickable.Tick(deltaTime);
            }
        }

        void LateUpdate()
        {
            float deltaTime = Time.deltaTime;

            foreach (IDLateTickable lateTickable in _lateTickables)
            {
                lateTickable.LateTick(deltaTime);
            }
        }

    }
}