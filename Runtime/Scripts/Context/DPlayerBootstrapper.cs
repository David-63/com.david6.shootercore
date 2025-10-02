using System;
using System.Collections.Generic;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;


namespace David6.ShooterCore.Context
{
    /// <summary>
    /// The root component for the player.
    /// </summary>
    public class DPlayerBootstrapper : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private GameObject CameraHolder; // 카메라가 따라갈 GameObject


        [Header("Debug Settings")]
        [SerializeField] private bool StateDebugLog = false;

        #region Providers
        readonly Dictionary<Type, IDProvider> _providers = new();
        public void Register<T>(T provider) where T : class, IDProvider => _providers[typeof(T)] = provider;
        public T Resolve<T>() where T : class, IDProvider => _providers[typeof(T)] as T;
        #endregion

        void Awake()
        {
            // dll 초기화: Xbox Controller Input 활성화
            GameInputWrapper.InitGameInput();
        }

        // 여기서 순서대로 초기화 세팅
        void Start()
        {
            IDContextProvider context = InitializeContext();


            // 카메라가 연결 되어야 할 내용
            if (!Resolve<IDCameraHandlerProvider>().SetCameraHolder(CameraHolder))
            {
                Log.ErrorAlert("Failed to setup CameraHolder in CameraHandler");
            }


            // 입력 이벤트 연결
            InputBinding();


            Log.WhatHappend("Bootstrapper work finished.");

            if (StateDebugLog)
            {
                context.ActiveStateDebugMode();
            }
        }

        private IDContextProvider InitializeContext()
        {
            var context = Resolve<IDContextProvider>();
            /*
                ================================
                == context가 연결 되어야 할 내용 ==
                ================================
                1. 카메라 (초기화 의존도 X)
                2. Rig (초기화 의존도 X)
                3. Equipment UI (초기화 의존도 X)
                4. Focus UI (압도적으로 필요!!!)

            */

            if (!context.SetCameraHandler(Resolve<IDCameraHandlerProvider>()))
            {
                Log.ErrorAlert("Failed to setup camera in context");
            }
            
            if (!context.SetRigHandler(Resolve<IDRigHandlerProvider>()))
            {
                Log.ErrorAlert("Failed to setup camera in context");
            }
            
            var equipmentUIController = Resolve<IDEquipmentUIControllerProvider>();
            if (context.SetEquipmentUIController(equipmentUIController))
            {
                equipmentUIController.RegisterOnGearChanged(context.OnGearEquipped);
            }
            else
            {
                Log.ErrorAlert("Failed to setup root panel controller in context");
            }
            
            // 4. Focus UI
            var focusUIController = Resolve<IDFocusUIControllerProvider>();
            if (context.SetFocusUIController(focusUIController))
            {
                context.CombatHandler.OnFocusActive += focusUIController.HandleFocusOn;
                context.CombatHandler.OnFocusInactive += focusUIController.HandleFocusOff;

                context.CombatHandler.OnCountingRounds += focusUIController.CountingRounds;
                context.CombatHandler.OnCountingAmmunition += focusUIController.CountingAmmunition;

                context.CombatHandler.OnSpreedChanged += focusUIController.AccuracyControl;
            }
            else
            {
                Log.ErrorAlert("Failed to setup root panel controller in context");
            }

            return context;
        }

        void OnDestroy()
        {
            var input = Resolve<IDInputProvider>();
            var context = Resolve<IDContextProvider>();
            if (input == null || context == null) return;

            input.OnLook -= Resolve<IDCameraHandlerProvider>().HandleLookInput;

            input.OnMove -= context.HandleMoveInput;
            input.OnStartJump -= context.HandleStartJumpInput;
            input.OnStopJump -= context.HandleStopJumpInput;
            input.OnStartRun -= context.HandleStartSprintInput;
            input.OnStopRun -= context.HandleStopSprintInput;
            input.OnStartAim -= context.HandleStartAimInput;
            input.OnStopAim -= context.HandleStopAimInput;
            input.OnStartFire -= context.HandleStartFireInput;
            input.OnStopFire -= context.HandleStopFireInput;
            input.OnStartReload -= context.HandleStartReloadInput;
            input.OnStopReload -= context.HandleStopReloadInput;

            input.OnPause -= context.HandlePauseInput;
            input.OnResume -= context.HandleResumeInput;
            input.OnCancelPress -= context.HandleCancelInput;

            var panelController = Resolve<IDEquipmentUIControllerProvider>();
            if (panelController == null) return;

            panelController.OnCloseUI -= input.HandleResume;
            panelController.OnCloseUI -= context.HandleCloseUI;
        }

        /// <summary>
        /// UI 제어 인풋은 Context가 받고, Context의 이벤트를 UI가 구독하는 방식과 혼용
        /// </summary>
        void InputBinding()
        {
            var input = Resolve<IDInputProvider>();
            var context = Resolve<IDContextProvider>();
            if (input == null || context == null) return;

            input.OnLook += Resolve<IDCameraHandlerProvider>().HandleLookInput;

            input.OnMove += context.HandleMoveInput;
            input.OnStartJump += context.HandleStartJumpInput;
            input.OnStopJump += context.HandleStopJumpInput;
            input.OnStartRun += context.HandleStartSprintInput;
            input.OnStopRun += context.HandleStopSprintInput;
            input.OnStartAim += context.HandleStartAimInput;
            input.OnStopAim += context.HandleStopAimInput;
            input.OnStartFire += context.HandleStartFireInput;
            input.OnStopFire += context.HandleStopFireInput;
            input.OnStartReload += context.HandleStartReloadInput;
            input.OnStopReload += context.HandleStopReloadInput;

            input.OnPause += context.HandlePauseInput;
            input.OnResume += context.HandleResumeInput;
            input.OnCancelPress += context.HandleCancelInput;

            var panelController = Resolve<IDEquipmentUIControllerProvider>();
            if (panelController == null) return;

            panelController.OnCloseUI += input.HandleResume;
            panelController.OnCloseUI += context.HandleCloseUI;

            input.OnSubmitPress += panelController.HandleSubmitPress;
            input.OnSubmitRelease += panelController.HandleSubmitRelease;
            input.OnNavigate += panelController.HandleNavigate;
        }
    }
}