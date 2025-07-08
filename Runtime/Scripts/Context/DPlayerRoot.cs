using David6.shootercore.Input;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;


namespace David6.ShooterCore.Context
{
    /// <summary>
    /// The root component for the player.
    /// </summary>    
    public class DPlayerRoot : MonoBehaviour
    {
        [Header("Player Root Settings")]
        [Tooltip("Input provider behaviour that implements IDInputProvider interface.")]
        [SerializeField] private MonoBehaviour InputHanderBehaviour;
        [Tooltip("Movement provider behaviour that implements IDBaseMovementProvider interface.")]
        [SerializeField] private MonoBehaviour PlayerContextBehaviour;
        [Tooltip("Camera handler behaviour that implements IDCameraInfoProvider interface.")]
        [SerializeField] private MonoBehaviour CameraHandlerBehaviour;
        [SerializeField] private GameObject CameraHolder; // 카메라가 따라갈 GameObject
        

        // 인터페이스로 접근할 수 있는 프로퍼티
        public IDInputProvider InputProvider { get; private set; }
        public IDContextProvider ContextProvider { get; private set; }
        public IDCameraInfoProvider CameraHandler { get; private set; }


        void Awake()
        {
            InputProvider = InputHanderBehaviour as IDInputProvider;
            ContextProvider = PlayerContextBehaviour as IDContextProvider;
            CameraHandler = CameraHandlerBehaviour as IDCameraInfoProvider;
            InputBinding();
        }

        void Start()
        {            
            // 카메라 핸들러가 따라갈 GameObject 설정
            ContextProvider.SetCameraInfoProvider(CameraHandler);
            CameraHandler.CameraHolder = CameraHolder;

            Log.WhatHappend("Player root initialized successfully.");
        }

        void InputBinding()
        {
            InputProvider.OnMove += ContextProvider.HandleMoveInput;
            InputProvider.OnLook += CameraHandler.HandleLookInput;
            InputProvider.OnStartJump += ContextProvider.HandleStartJumpInput;
            InputProvider.OnStopJump += ContextProvider.HandleStopJumpInput;
            InputProvider.OnStartSprint += ContextProvider.HandleStartSprintInput;
            InputProvider.OnStopSprint += ContextProvider.HandleStopSprintInput;
            Log.WhatHappend("Input binding completed successfully.");
        }
    }
}