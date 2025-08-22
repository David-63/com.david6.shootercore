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

        void Start()
        {
            var context = Resolve<IDContextProvider>();
            var panelController = Resolve<IDRootPanelControllerProvider>();

            if (!context.SetCameraHandler(Resolve<IDCameraHandlerProvider>()))
            {
                Log.ErrorAlert("Failed to setup camera in context");
            }
            if (!context.SetRigHandler(Resolve<IDRigHandlerProvider>()))
            {
                Log.ErrorAlert("Failed to setup camera in context");
            }            
            if (!context.SetRootPanelController(panelController))
            {
                Log.WhatHappend("Failed to setup root panel controller in context");
            }
            if (!Resolve<IDCameraHandlerProvider>().SetCameraHolder(CameraHolder))
            {
                Log.WhatHappend("Failed to setup CameraHolder in CameraHandler");
            }


            // 이벤트 바인딩
            InputBinding();
            panelController.RegisterOnEquip(context.OnGearEquipped);

            if (StateDebugLog)
            {
                context.ActiveStateDebugMode();
            }
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
            input.OnPop -= context.HandlePopInput;

            var panelController = Resolve<IDRootPanelControllerProvider>();
            if (panelController == null) return;

            panelController.OnCloseUI -= input.HandleResume;
            panelController.OnCloseUI -= context.HandleCloseUI;
        }


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
            input.OnPop += context.HandlePopInput;
            
            var panelController = Resolve<IDRootPanelControllerProvider>();
            if (panelController == null) return;

            panelController.OnCloseUI += input.HandleResume;
            panelController.OnCloseUI += context.HandleCloseUI;
        }
    }
}