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
        IDCameraInfoProvider CameraHandler { get; set; }

        [Header("UI Settings")]
        [SerializeField] private MonoBehaviour InventoryControllerBehaviour;
        [SerializeField] private MonoBehaviour InventoryViewBehaviour;

        IDInventoryControllerProvider InventoryControllerProvider { get; set; }
        IDInventoryViewProvider InventoryViewProvider { get; set; }




        void Awake()
        {
            InputProvider = InputHanderBehaviour as IDInputProvider;
            ContextProvider = PlayerContextBehaviour as IDContextProvider;
            CameraHandler = CameraHandlerBehaviour as IDCameraInfoProvider;
            InventoryControllerProvider = InventoryControllerBehaviour as IDInventoryControllerProvider;
            InventoryViewProvider = InventoryViewBehaviour as IDInventoryViewProvider;
            InputBinding();
        }

        void Start()
        {
            if (!ContextProvider.SetCameraInfoProvider(CameraHandler))
            {
                Log.WhatHappend("Failed to setup camera in context");
            }
            if (!CameraHandler.SetCameraHolder(CameraHolder))
            {
                Log.WhatHappend("Failed to setup CameraHolder in CameraHandler");
            }

            if (!InventoryControllerProvider.SetViewProvider(InventoryViewProvider))
            {
                Log.WhatHappend("Failed to setup InventoryView in InventoryController");
            }
        }

        void InputBinding()
        {
            InputProvider.OnLook += CameraHandler.HandleLookInput;

            InputProvider.OnMove += ContextProvider.HandleMoveInput;
            InputProvider.OnStartJump += ContextProvider.HandleStartJumpInput;
            InputProvider.OnStopJump += ContextProvider.HandleStopJumpInput;
            InputProvider.OnStartSprint += ContextProvider.HandleStartSprintInput;
            InputProvider.OnStopSprint += ContextProvider.HandleStopSprintInput;
            InputProvider.OnStartAim += ContextProvider.HandleStartAimInput;
            InputProvider.OnStopAim += ContextProvider.HandleStopAimInput;
            InputProvider.OnStartFire += ContextProvider.HandleStartFireInput;
            InputProvider.OnStopFire += ContextProvider.HandleStopFireInput;
            InputProvider.OnStartReload += ContextProvider.HandleStartReloadInput;
            InputProvider.OnStopReload += ContextProvider.HandleStopReloadInput;

            InputProvider.OnPause += InventoryControllerProvider.HandlePause;
            InputProvider.OnResume += InventoryControllerProvider.HandleResume;
        }
    }
}