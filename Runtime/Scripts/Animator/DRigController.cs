using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace David6.ShooterCore.Animation
{
    public class DRigController : MonoBehaviour
    {
        /*
            Rig 값 변경해주기
            Rig Weight 조절

        */
        [SerializeField] Rig RigLayerHand;
        [SerializeField] Rig RigLayerAim;
        [SerializeField] TwoBoneIKConstraint _leftHandIK;
        [SerializeField] TwoBoneIKConstraint _rightHandIK;
        [SerializeField] MultiAimConstraint _aimIK;


        public Transform IKHandLeft;
        public Transform IKHandRight;
        public Vector3 IKAimOffset;

        public void RigLayerInitialize()
        {
            _leftHandIK.data.target = IKHandLeft;
            _rightHandIK.data.target = IKHandRight;
            _aimIK.data.offset = IKAimOffset;
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
