using UnityEngine;
using David6.ShooterFramework.Extension;


namespace David6.ShooterFramework
{
    public partial class DavidCharacterController : MonoBehaviour
    {
        private Animator _playerAnimator;

        // 애니메이션
        private bool _hasAnimator = false;
        private int _xDirectionHash;
        private int _yDirectionHash;
        private int _moveSpeedHash;
        private int _jumpHash;
        private int _groundedHash;
        private int _freeFallHash;
        private Vector2 AnimDirection;
        private float _animMoveSpeed;



        private void InitializeAnimator()
        {
            _hasAnimator = this.TryGetComponentInChildren(out _playerAnimator);

            if (_hasAnimator)
            {
                _xDirectionHash = Animator.StringToHash("x_Direction");
                _yDirectionHash = Animator.StringToHash("y_Direction");
                _moveSpeedHash = Animator.StringToHash("MoveSpeed");
                _jumpHash = Animator.StringToHash("Jump");
                _groundedHash = Animator.StringToHash("Grounded");
                _freeFallHash = Animator.StringToHash("FreeFall");
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
        private void SetAnimJump(bool isJump)
        {
            if (_hasAnimator)
            {
                _playerAnimator.SetBool(_jumpHash, isJump);
            }
        }

        private void SetGrounded(bool isGrounded)
        {
            if (_hasAnimator)
            {
                _playerAnimator.SetBool(_groundedHash, isGrounded);
            }
        }

        private void SetFreeFall(bool isFreeFall)
        {
            if (_hasAnimator)
            {
                _playerAnimator.SetBool(_freeFallHash, isFreeFall);
            }
        }

        
    }
}
