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
        [Header("Player Settings")]
        [SerializeField] private MonoBehaviour InputHanderBehaviour;
        [SerializeField] private MonoBehaviour PlayerContextBehaviour;
        [SerializeField] private MonoBehaviour CameraHandlerBehaviour;
        [SerializeField] private GameObject CameraHolder; // 카메라가 따라갈 GameObject

        IDInputProvider InputProvider { get; set; }
        IDContextProvider ContextProvider { get; set; }
        IDCameraHandlerProvider CameraHandler { get; set; }

        [Header("UI Settings")]
        [SerializeField] private MonoBehaviour RootPanelControllerBehaviour;
        [SerializeField] private MonoBehaviour RootPanelViewBehaviour;

        IDRootPanelControllerProvider RootPanelControllerProvider { get; set; }
        IDRootPanelViewProvider RootPanelViewProvider { get; set; }


        [Header("Debug Settings")]
        [SerializeField] private bool StateDebugLog = false;


        void Awake()
        {
            InputProvider = InputHanderBehaviour as IDInputProvider;
            ContextProvider = PlayerContextBehaviour as IDContextProvider;
            CameraHandler = CameraHandlerBehaviour as IDCameraHandlerProvider;
            RootPanelControllerProvider = RootPanelControllerBehaviour as IDRootPanelControllerProvider;
            RootPanelViewProvider = RootPanelViewBehaviour as IDRootPanelViewProvider;

            InputBinding();

            if (!ContextProvider.SetCameraInfoProvider(CameraHandler))
            {
                Log.WhatHappend("Failed to setup camera in context");
            }
            if (!ContextProvider.SetRootPanelController(RootPanelControllerProvider))
            {
                Log.WhatHappend("Failed to setup root panel controller in context");
            }
            
            if (!CameraHandler.SetCameraHolder(CameraHolder))
            {
                Log.WhatHappend("Failed to setup CameraHolder in CameraHandler");
            }
        }

        void OnDestroy()
        {
            if (InputProvider != null)
            {
                InputProvider.OnLook -= CameraHandler.HandleLookInput;

                InputProvider.OnMove -= ContextProvider.HandleMoveInput;
                InputProvider.OnStartJump -= ContextProvider.HandleStartJumpInput;
                InputProvider.OnStopJump -= ContextProvider.HandleStopJumpInput;
                InputProvider.OnStartRun -= ContextProvider.HandleStartSprintInput;
                InputProvider.OnStopRun -= ContextProvider.HandleStopSprintInput;
                InputProvider.OnStartAim -= ContextProvider.HandleStartAimInput;
                InputProvider.OnStopAim -= ContextProvider.HandleStopAimInput;
                InputProvider.OnStartFire -= ContextProvider.HandleStartFireInput;
                InputProvider.OnStopFire -= ContextProvider.HandleStopFireInput;
                InputProvider.OnStartReload -= ContextProvider.HandleStartReloadInput;
                InputProvider.OnStopReload -= ContextProvider.HandleStopReloadInput;

                InputProvider.OnPause -= ContextProvider.HandlePauseInput;
                InputProvider.OnResume -= ContextProvider.HandleResumeInput;
                InputProvider.OnPop -= ContextProvider.HandlePopInput;
            }

            if (RootPanelControllerProvider != null)
            {
                RootPanelControllerProvider.OnCloseUI -= InputProvider.HandleResume;
                RootPanelControllerProvider.OnCloseUI -= ContextProvider.HandleCloseUI;
            }
        }

        void Start()
        {
            if (StateDebugLog)
            {
                ContextProvider.ActiveStateDebugMode();
            }
        }

        void InputBinding()
        {
            InputProvider.OnLook += CameraHandler.HandleLookInput;

            InputProvider.OnMove += ContextProvider.HandleMoveInput;
            InputProvider.OnStartJump += ContextProvider.HandleStartJumpInput;
            InputProvider.OnStopJump += ContextProvider.HandleStopJumpInput;
            InputProvider.OnStartRun += ContextProvider.HandleStartSprintInput;
            InputProvider.OnStopRun += ContextProvider.HandleStopSprintInput;
            InputProvider.OnStartAim += ContextProvider.HandleStartAimInput;
            InputProvider.OnStopAim += ContextProvider.HandleStopAimInput;
            InputProvider.OnStartFire += ContextProvider.HandleStartFireInput;
            InputProvider.OnStopFire += ContextProvider.HandleStopFireInput;
            InputProvider.OnStartReload += ContextProvider.HandleStartReloadInput;
            InputProvider.OnStopReload += ContextProvider.HandleStopReloadInput;

            InputProvider.OnPause += ContextProvider.HandlePauseInput;
            InputProvider.OnResume += ContextProvider.HandleResumeInput;
            InputProvider.OnPop += ContextProvider.HandlePopInput;

            RootPanelControllerProvider.OnCloseUI += InputProvider.HandleResume;
            RootPanelControllerProvider.OnCloseUI += ContextProvider.HandleCloseUI;
        }
    }
}