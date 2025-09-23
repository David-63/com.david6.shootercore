using System;
using David6.ShooterCore.Item;
using David6.ShooterCore.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace David6.ShooterCore.UI.Equipment
{
    public class DItemButton : MonoBehaviour, IPointerClickHandler
    {
        DGear _gearData;
        public DGear Gear { get => _gearData; set => _gearData = value; }

        [SerializeField] TMP_Text _itemName;
        public TMP_Text ItemName { get => _itemName; set => _itemName = value; }
        [SerializeField] Image _gearImage;
        public Sprite ItemIcon
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


        #region Button Event
        public event Action<DGear> OnItemSelected;
        public void HandleSelectItem() => OnItemSelected?.Invoke(Gear);
        public void OnPointerClick(PointerEventData eventdata) => OnItemSelected?.Invoke(Gear);
        #endregion

    }
}