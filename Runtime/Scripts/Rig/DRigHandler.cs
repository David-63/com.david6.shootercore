using System;
using System.Collections.Generic;
using David6.ShooterCore.Context;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace David6.ShooterCore.Animation
{
    public enum EDConstraintType
    {
        IK_LEFT,
        IK_RIGHT,
        IK_AIM,
    }
    public class DRigHandler : MonoBehaviour, IDRigHandlerProvider
    {
        /*
            Rig 값 변경해주기
            Rig Weight 조절
        */
        [SerializeField] Rig RigLayerHand;
        [SerializeField] Rig RigLayerAim;
        readonly Dictionary<(Type, EDConstraintType), IDConstraintProvider> _constraint = new();
        public void Register<T>(T constraint, EDConstraintType key) where T : class, IDConstraintProvider => _constraint[(typeof(T), key)] = constraint;
        public T Resolve<T>(EDConstraintType key) where T : class, IDConstraintProvider => _constraint[(typeof(T), key)] as T;


        Transform _IKHandLeft;
        Transform _IKHandRight;
        Vector3 _IKAimOffset;

        void Awake()
        {
            var bootstrapper = FindAnyObjectByType(typeof(DPlayerBootstrapper)) as DPlayerBootstrapper;
            bootstrapper.Register<IDRigHandlerProvider>(this);
        }

        public void SetupRigIK(DWeaponFrame weaponFrame)
        {
            _IKHandLeft = weaponFrame.GripLeft;
            _IKHandRight = weaponFrame.GripRight;
            _IKAimOffset = weaponFrame.AimRigOffset;
            RigOverride();
        }




        void RigOverride()
        {
            var leftHand = Resolve<DRIgTwoBoneIK>(EDConstraintType.IK_LEFT).GetConstraint() as TwoBoneIKConstraint;
            if (leftHand != null) leftHand.data.target = _IKHandLeft;
            else
            {
                Log.ErrorAlert("Left IK Constraint 없음");
            }

            var rightHand = Resolve<DRIgTwoBoneIK>(EDConstraintType.IK_RIGHT).GetConstraint() as TwoBoneIKConstraint;
            if (rightHand != null) rightHand.data.target = _IKHandRight;
            else
            {
                Log.ErrorAlert("Aim Constraint 없음");
            }

            var aim = Resolve<DRigMultiAim>(EDConstraintType.IK_AIM).GetConstraint() as MultiAimConstraint;
            if (aim != null) aim.data.offset = _IKAimOffset;
            else
            {
                Log.ErrorAlert("Aim Constraint 없음");
            }
        }

        public void ActiveRig()
        {
            RigLayerHand.weight = 1.0f;
            RigLayerAim.weight = 1.0f;
        }

        public void InactiveRig()
        {
            RigLayerHand.weight = 0.0f;
            RigLayerAim.weight = 0.0f;
        }

    }
}
