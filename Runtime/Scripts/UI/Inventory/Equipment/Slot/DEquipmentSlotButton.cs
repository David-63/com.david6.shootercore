using System;
using David6.ShooterCore.Item;
using David6.ShooterCore.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentSlotButton : MonoBehaviour
    {
        [SerializeField] EDGearSlot _gearType;
        public EDGearSlot GearType { get => _gearType; }


        [SerializeField] TMP_Text _slotName;
        [SerializeField] Image _gearImage;
        public Sprite SlotIcon
        {
            get => _gearImage.sprite;
            set
            {
                if (_gearImage.sprite != value)
                {
                    _gearImage.sprite = value;
                }
            }
        }

        public event Action<EDGearSlot> OnClicked;

        void Awake() => _slotName.text = _gearType.ToString();

        // Unity Inspector에서 이 함수 연결
        public void HandleClick() => OnClicked?.Invoke(GearType);
    }
}