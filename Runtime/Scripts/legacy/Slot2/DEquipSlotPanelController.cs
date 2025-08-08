using System.Collections.Generic;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Item.Gear;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipSlotPanelController : MonoBehaviour
    {
        [SerializeField] DEquipSlotPanelView2 _panelView;
        DEquipSlotPanelPresenter2 _presenter;
        public DGearData _equipTest;

        void Awake()
        {
            // 더미 모델 생성
            var dummyModel = new DEquipmentSlotModel2();

            // 프레젠트 생성
            // _presenter = new DEquipSlotPanelPresenter2(dummyModel, _panelView);
            // _presenter.Initialize();


            // 이미지 변경!!            
            dummyModel.EquipGear(EDGearType.PrimaryWeapon, _equipTest);
        }

        void OnDestroy()
        {
            _presenter.Dispose();
        }
    }
}