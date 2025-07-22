using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionFireState : DBaseState
    {
        float _fireRate = 0.05f;
        float _fireCooldown = 0.0f;

        public DActionFireState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            Context.IsFiring = true;
            FireCooldownReset();
        }

        public override void UpdateSelf()
        {
            CheckTransition();

            if (!Context.IsFireReady)
            {
                _fireCooldown -= Time.deltaTime;

                if (_fireCooldown <= 0f)
                {
                    Context.IsFireReady = true;
                }
            }
            else
            {
                Fire();
            }

        }

        public override void ExitState()
        {
            Context.IsFiring = false;

        }
        public override void CheckTransition()
        {
            if (!Context.InputFire)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionIdleState)));
            }
            else if (Context.ShouldFire())
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionFireState)));
            }
        }
        public override void InitializeSubState() { }

        // IEnumerator FireRateRoutine()
        // {
        //     Context.IsFireReady = false;
        //     Context.AnimatorProvider.SetFire();

        //     yield return new WaitForSeconds(_fireRate);
        //     Context.IsFireReady = true;
        // }

        void FireCooldownReset() => _fireCooldown = 0f;
        void Fire()
        {
            Context.AnimatorProvider.SetFire();
            Context.IsFireReady = false;
            _fireCooldown = _fireRate;
        }
    }
}