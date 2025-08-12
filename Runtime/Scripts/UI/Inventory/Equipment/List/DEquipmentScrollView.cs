using System;
using System.Collections.Generic;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Item.Gear;
using TMPro;
using UnityEngine;

namespace David6.ShooterCore.UI.Equipment
{
    public class DEquipmentScrollView : MonoBehaviour
    {
        [SerializeField] Transform _contentRoot;
        [SerializeField] GameObject ItemButtonPrefab;
        [SerializeField] TMP_Text _scrollViewType;

        public EDGearType CurrentType { get; set; }

        public void SetScrollViewText(EDGearType type)
        {
            CurrentType = type;
            _scrollViewType.text = type.ToString();
        }

        public void SetItems(List<DGearData> items, Action<DGearData> onClick)
        {
            foreach (Transform child in _contentRoot)
            {
                // 나중에 오브젝트 풀링으로 변경
                Destroy(child.gameObject);
            }

            foreach (DGearData item in items)
            {
                var itemObject = Instantiate(ItemButtonPrefab, _contentRoot);
                var button = itemObject.GetComponent<DItemButton>();

                button.GearData = item;
                button.ItemIcon = item.GearIcon;
                button.ItemName.text = item.GearName;
                button.OnClicked += onClick;
            }
        }
    }
}