using System;
using System.Collections.Generic;

using UnityEngine;

namespace David6.ShooterFramework
{
    public delegate void RotationHandler(ref Quaternion rotation, float dt);
    public delegate void VelocityHandler(ref Vector3 velocity, float dt);
    public partial class DavidCharacterController : MonoBehaviour
    {
        // 모드별 SetInputs 처리기
        private Dictionary<CharacterMode, Action<PlayerCharacterInputs>> _inputHandlers;

        // 모드별 BeforeUpdate 처리기
        private Dictionary<CharacterMode, Action<float>> _beforeUpdateHandlers;

        // 모드별 Rotation 업데이트 처리기
        private Dictionary<CharacterMode, RotationHandler> _rotationHandlers;

        // 모드별 Velocity 업데이트 처리기
        private Dictionary<CharacterMode, VelocityHandler> _velocityHandlers;

        // 모드별 AfterUpdate 처리기
        private Dictionary<CharacterMode, Action<float>> _afterUpdateHandlers;

        private void InitializeCharacterMode()
        {
            _inputHandlers = new Dictionary<CharacterMode, Action<PlayerCharacterInputs>>()
            {
                { CharacterMode.Default, HandleDefaultInputs },
                { CharacterMode.Charging, HandleChargingInputs },
                { CharacterMode.NoClip, HandleNoClipInputs },
            };

            _beforeUpdateHandlers = new Dictionary<CharacterMode, Action<float>>()
            {
                { CharacterMode.Charging, BeforeChargingUpdate },
            };

            _rotationHandlers = new Dictionary<CharacterMode, RotationHandler>()
            {
                { CharacterMode.Default, UpdateDefaultRotation },
            };

            _velocityHandlers = new Dictionary<CharacterMode, VelocityHandler>()
            {
                { CharacterMode.Default, UpdateDefaultVelocity },
                { CharacterMode.Charging, UpdateChargingVelocity },
                { CharacterMode.NoClip, UpdateNoClipVelocity },
                
            };

            _afterUpdateHandlers = new Dictionary<CharacterMode, Action<float>>()
            {
                { CharacterMode.Default, AfterDefaultUpdate },
                { CharacterMode.Charging, AfterChargingUpdate },
            };
        }

        #region 모드 제어

        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToMode(CharacterMode newState)
        {
            CharacterMode tempInitialState = CurrentCharacterMode;
            OnStateExit(tempInitialState, newState);
            CurrentCharacterMode = newState;
            OnStateEnter(newState, tempInitialState);
        }
        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(CharacterMode mode, CharacterMode fromMode)
        {
            switch (mode)
            {
                case CharacterMode.Default:
                break;
                case CharacterMode.Charging:
                    _currentChargeVelocity = Motor.CharacterForward * ChargeSpeed;
                    _isStopped = false;
                    _timeSinceStartedCharge = 0f;
                    _timeSinceStopped = 0f;
                break;
                case CharacterMode.NoClip:
                    Motor.SetCapsuleCollisionsActivation(false);
                    Motor.SetMovementCollisionsSolvingActivation(false);
                    Motor.SetGroundSolvingActivation(false);
                break;
            }
        }
        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(CharacterMode state, CharacterMode toState)
        {
            switch (state)
            {
                case CharacterMode.Default:
                break;
                case CharacterMode.NoClip:
                    Motor.SetCapsuleCollisionsActivation(true);
                    Motor.SetMovementCollisionsSolvingActivation(true);
                    Motor.SetGroundSolvingActivation(true);
                break;
            }
        }

        #endregion

        #region Handle Inputs
        private void HandleDefaultInputs(PlayerCharacterInputs inputs)
        {
            _jumpInputIsHeld = inputs.JumpHeld;
            _crouchInputIsHeld = inputs.CrouchHeld;
            _sprinting = inputs.Sprint;

            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxis.x, 0f, inputs.MoveAxis.y), 1f);

            // 캐릭터의 수평에 맞춰 카메라 방향과 회전을 계산.
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);


            // 입력으로부터 이동과 시점 벡터를 세팅.
            _moveInputVector = cameraPlanarRotation * moveInputVector;
            _lookInputVector = cameraPlanarDirection;

            // 점프 입력 설정.
            if (inputs.Jump)
            {
                _timeSinceJumpRequested = 0f;
                _jumpRequested = true;
            }

            // 웅크리기 입력 설정.
            if (inputs.Crouch)
            {
                _shouldBeCrouching = true;

                if (!_isCrouching)
                {
                    _isCrouching = true;
                    // 충돌 사이즈 변경.
                    Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                    // 이건 임시적으로 모델 크기를 줄인것(애니메이션으로 웅크리면 굳이 줄일 필요가 없지).
                    MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                }
            }
            else
            {
                _shouldBeCrouching = false;
            }

            Vector3 currentDirection = new Vector3(AnimDirection.x, 0f, AnimDirection.y);
            Vector3 slerpedDirection = Vector3.Slerp(currentDirection, moveInputVector, OrientationSharpness * Time.deltaTime);

            AnimDirection = new Vector2(slerpedDirection.x, slerpedDirection.z);

            SetAnimDirection();
        }

        private void HandleChargingInputs(PlayerCharacterInputs inputs)
        {
            if (CurrentCharacterMode != CharacterMode.Charging)
            {
                TransitionToMode(CharacterMode.Charging);
            }
        }

        private void HandleNoClipInputs(PlayerCharacterInputs inputs)
        {
            if (CurrentCharacterMode == CharacterMode.Default)
            {
                TransitionToMode(CharacterMode.NoClip);
            }
            else if (CurrentCharacterMode == CharacterMode.NoClip)
            {
                TransitionToMode(CharacterMode.Default);
            }
        }

        #endregion

        #region Before Update

        private void BeforeChargingUpdate(float dt)
        {
            // Update times
            _timeSinceStartedCharge += dt;
            if (_isStopped)
            {
                _timeSinceStopped += dt;
            }
        }

        #endregion

        #region Update Rotation

        private void UpdateDefaultRotation(ref Quaternion rot, float dt)
        {
            // 입력값이 있을때만 유효함.
            if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
            {
                // 캐릭터의 전방 방향을 카메라 입력 방향으로 보간시킴.
                Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * dt)).normalized;

                // 최종 회전을 결정함 (which will be used by the KinematicCharacterMotor).
                rot = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
            }

            if (OrientTowardsGravity)
            {
                // 위쪽 방향벡터와 중력의 반대방향벡터로 방향을 바꾸는 회전 생성하고 회전에 적용.
                rot = Quaternion.FromToRotation((rot * Vector3.up), -Gravity) * rot;
            }
        }

        #endregion

        #region Update Velocity

        private void UpdateDefaultVelocity(ref Vector3 currentVelocity, float dt)
        {
            Vector3 targetMovementVelocity = Vector3.zero;

            if (Motor.GroundingStatus.IsStableOnGround)
            {
                // 지면 경사로에서 원본 속도 재조정 (경사로 변화에 의한 속도 손실을 예방).
                currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                // 목표 속도 계산.
                Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                float applyMaxMoveSpeed = _sprinting ? MaxStableSprintSpeed : MaxStableWalkSpeed;
                targetMovementVelocity = reorientedInput * applyMaxMoveSpeed;

                // 이동속도 스무딩 처리.
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * dt));

                // 애니메이터에 이동속력 전달
                float desiredAnimSpeed = AnimDirection.sqrMagnitude <= 0.1f ? 0 : applyMaxMoveSpeed;
                _animMoveSpeed = Mathf.Lerp(_animMoveSpeed, desiredAnimSpeed, 1 - Mathf.Exp(-StableMovementSharpness * dt));
                SetAnimSpeed();
            }
            else
            {
                // 공중 이동 입력.
                if (_moveInputVector.sqrMagnitude > 0f)
                {
                    targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                    // 공중 이동중에 불안정안 경사면을 올라가는것을 방지.
                    if (Motor.GroundingStatus.FoundAnyGround)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                        targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                    }

                    // 중력을 제외한 목표속도와 현재속도 차이를 계산, 공중가속속력을 적용하여 목표속도에 점진적으로 도달하도록 계산.
                    Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                    currentVelocity += velocityDiff * AirAccelerationSpeed * dt;
                }

                // 중력 적용.
                currentVelocity += Gravity * dt;

                // 저항 적용.
                currentVelocity *= (1f / (1f + (Drag * dt)));
            }

            HandleJumpVelocity(ref currentVelocity, dt);

            // 마지막에 더하기.
            if (_internalVelocityAdd.sqrMagnitude > 0f)
            {
                currentVelocity += _internalVelocityAdd;
                _internalVelocityAdd = Vector3.zero;
            }

            if (!Motor.GroundingStatus.IsStableOnGround)
            {
                SetFreeFall(true);
            }
            else
            {
                SetFreeFall(false);
            }
        }

        private void UpdateChargingVelocity(ref Vector3 currentVelocity, float dt)
        {
            // 멈추거나 속도를 취소하려면 여기서 진행.
            if (_mustStopVelocity)
            {
                //currentVelocity = Vector3.zero;
                _mustStopVelocity = false;
            }

            if (_isStopped)
            {
                // 정지할 경우, 중력을 제외한 속도조작은 하면 안됨.
                currentVelocity += Gravity * dt;
            }
            else
            {
                // 돌진중에는, 속도는 일정하게 유지
                float previousY = currentVelocity.y;
                currentVelocity = _currentChargeVelocity;
                currentVelocity.y = previousY;
                currentVelocity += Gravity * dt;
            }
        }

        private void UpdateNoClipVelocity(ref Vector3 currentVelocity, float dt)
        {
            Vector3 targetMovementVelocity = Vector3.zero;
            float verticalInput = 0f + (_jumpInputIsHeld ? 1f : 0f) + (_crouchInputIsHeld ? -1f : 0f);

            // Smoothly interpolate to target velocity
            targetMovementVelocity = (_moveInputVector + (Motor.CharacterUp * verticalInput)).normalized * NoClipMoveSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-NoClipSharpness * dt));
        }

        #endregion

        #region AfterUpdate

        private void AfterDefaultUpdate(float dt)
        {
            // 점프 요청 플래그가 활성화 된 상태에서, 일정 시간이 경과되면 무효처리. (여유시간 보정).
            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
            {
                _jumpRequested = false;
            }
            // 지면 상태에 따른 점프 값 리셋 및 타이머 관리.
            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
            {
                // 지면 위에 있는데 점프를 하지 않는다면 상태 업데이트.
                if (!_jumpedThisFrame)
                {
                    _doubleJumpConsumed = false;
                    _jumpConsumed = false;
                }
                // 점프 여유시간 타이머 초기화.
                _timeSinceLastAbleToJump = 0f;
            }
            else
            {
                // 지면에 없는 상태에서 점프가능 시간에 점프 허용을 판단할 때 사용.
                _timeSinceLastAbleToJump += dt;
            }

            // 웅크리기 해제.
            if (_isCrouching && !_shouldBeCrouching)
            {
                // 캐릭터의 서 있는 높이와 장애물이 겹치는지 테스트.
                Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, Motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
                {
                    // 장애물이 있다면 원위치.
                    Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                }
                else
                {
                    // 장애물이 없다면 웅크리기 해제.
                    MeshRoot.localScale = Vector3.one;
                    _isCrouching = false;
                }
            }
        }

        private void AfterChargingUpdate(float dt)
        {
            // 경과시간에 따른 정지요소 감지.
            if (!_isStopped && _timeSinceStartedCharge > MaxChargeTime)
            {
                _mustStopVelocity = true;
                _isStopped = true;
            }

            // 멈추기 종료 단계를 감지하고 기본이동 상태로 전환.
            if (_timeSinceStopped > StoppedTime)
            {
                TransitionToMode(CharacterMode.Default);
            }
        }

        #endregion

    }
}
