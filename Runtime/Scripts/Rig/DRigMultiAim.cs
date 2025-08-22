using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace David6.ShooterCore.Animation
{
    public class DRigMultiAim : MonoBehaviour, IDConstraintProvider
    {
        [Header("Rig Constraint Type")]
        [SerializeField] EDConstraintType Key;
        MultiAimConstraint _constraint;
        void Awake()
        {
            var rigHandler = GetComponentInParent<DRigHandler>();
            if (rigHandler == null)
            {
                Log.ErrorAlert("캐릭터 오브젝트에 RigController가 없음.");
                return;
            }

            rigHandler.Register(this, Key);

            _constraint = GetComponent<MultiAimConstraint>();
        }

        public Component GetConstraint() => _constraint;

    }
}