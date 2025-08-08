using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Item.Gear;
using David6.ShooterCore.Provider;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentUIController : MonoBehaviour
    {
        [SerializeField] IDRootPanelControllerProvider _rootPanelController;

        DEquipmentSlotModel2 _equipmentModel = new DEquipmentSlotModel2();

        // [SerializeField] DEquipListPanelView _listView;
        // DEquipListPanelPresenter _listPresenter;

        public DGearData _equipTest;

        void Awake()
        {
        }
        void Start()
        {
            _equipmentModel.EquipGear(EDGearType.PrimaryWeapon, _equipTest);
        }

        void OnDestroy()
        {
            //_slotPresenter.Dispose();
            //_listPresenter.Dispose();
        }
    }
}