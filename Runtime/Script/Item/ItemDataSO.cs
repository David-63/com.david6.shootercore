using UnityEngine;

namespace David6.ShooterFramework
{
    public enum eItemType { Weapon, Consumable, Equipment, Material }

    [CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Objects/ItemDataSO")]
    public class ItemDataSO : ScriptableObject
    {
        [Tooltip("아이템 식별용 ID")]
        public int ItemID;
        [Tooltip("아이템 이름")]
        public string ItemName;

        public Sprite ItemIcon;

        public eItemType ItemType;

        [Tooltip("획득시 인벤토리에 추가되는 최대 스택")]
        public int MaxStackSize = -1;

        public GameObject Prefab;
        public bool GroupedPrefab;
        public string Description;


    }
}
