using UnityEngine;
using David6.ShooterFramework.Extension;


namespace David6.ShooterFramework
{
    public partial class DavidCharacterController : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField]
        private Animator _playerAnimator;

        // 애니메이션
        private bool _hasAnimator = false;
        public Vector2 AnimDirection;
        private int _xDirectionHash;
        private int _yDirectionHash;
        private int _moveSpeedHash;
        public float _animMoveSpeed;

        private void InitializeAnimator()
        {
            _hasAnimator = this.TryGetComponentInChildren(out _playerAnimator);

            if (_hasAnimator)
            {
                _xDirectionHash = Animator.StringToHash("x_Direction");
                _yDirectionHash = Animator.StringToHash("y_Direction");
                _moveSpeedHash = Animator.StringToHash("MoveSpeed");
            }
            else
            {
                Log.AttentionPlease("애니메이터가 연결 안됬음.");
            }
        }

        private void SetAnimSpeed()
        {
            _playerAnimator.SetFloat(_moveSpeedHash, _animMoveSpeed);
        }

        private void SetAnimDirection()
        {
            _playerAnimator.SetFloat(_xDirectionHash, AnimDirection.x);
            _playerAnimator.SetFloat(_yDirectionHash, AnimDirection.y);
        }
    }
}
