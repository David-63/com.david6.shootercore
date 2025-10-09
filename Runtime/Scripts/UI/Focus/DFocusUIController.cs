using System;
using David6.ShooterCore.Context;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.UI.Focus
{
    public class DFocusUIController : MonoBehaviour, IDFocusUIControllerProvider
    {
        // DAmmoView _ammoView;
        [SerializeField] DFocusView _focusPanelView;
        [SerializeField] DAmmoView _ammoPanelView;
        [SerializeField] DCrossHairView _crosshairView;

        //public event Action<bool, int> OnConsumeAmmo;


        void Awake()
        {
            // 스스로 Bootstrapper에 등록
            var root = GetComponent<DPlayerBootstrapper>();
            if (root != null)
            {
                root.Register<IDFocusUIControllerProvider>(this);
            }
            else
            {
                Log.WhatHappend("초기화 안됬음");
            }

        }

        public void HandleFocusOn()
        {
            _focusPanelView?.ShowPanel();
            _ammoPanelView?.ShowPanel();
            _crosshairView?.ShowPanel();
        }
        public void HandleFocusOff()
        {
            _focusPanelView?.HidePanel();
        }
        public void CountingRounds(bool chamber, int rounds)
        {
            _ammoPanelView.ApplyCurrentRounds(chamber, rounds);
        }
        public void CountingAmmunition(int ammo)
        {
            _ammoPanelView.ApplyCurrentStorage(ammo);
        }

        public void CrosshairControl(float pixelDiameter)
        {
            _crosshairView.SetCrosshairSize(pixelDiameter);
        }
    }

    /*
        플레이어의 정보를 알고 있어야함
        데이터 변동에 따른 업데이트가 있어야함
    */
}