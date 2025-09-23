using System;
using David6.ShooterCore.Item;
using David6.ShooterCore.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Equipment
{
    public class DSlotButton : MonoBehaviour, IPointerClickHandler
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



        void Awake() => _slotName.text = _gearType.ToString();

        #region Button Event
        public event Action<EDGearSlot> OnSlotSelected;
        public void HandleClick() => OnSlotSelected?.Invoke(GearType);
        public void OnPointerClick(PointerEventData eventdata) => OnSlotSelected?.Invoke(GearType);
        #endregion
    }
}