using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace David6.ShooterFramework
{
    public class InventoryPanelManager : MonoBehaviour
    {
        public static InventoryPanelManager Instance { get; private set; }

        [Tooltip("UI의 Hierarchy 상에서 아이템 버튼이 배치될 컨테이너 오브젝트")]
        [SerializeField] private GameObject ItemContainer;
        [Tooltip("컨테이너에 배치할 아이템 버튼 프리팹")]
        [SerializeField] private GameObject ItemButtonPrefab;

        [Tooltip("아이템(SO) 목록에 추가할 에셋들")]
        [SerializeField] private List<ItemDataSO> ItemList;
        /// <summary>
        /// 인벤토리 맵
        /// </summary>
        private Dictionary<GameObject, eItemType> _inventoryItemMap = new Dictionary<GameObject, eItemType>();
        /// <summary>
        /// 프리뷰에 사용될 오브젝트 맵
        /// </summary>
        private Dictionary<int, GameObject> _previewItemMap;

        private void Awake()
        {
            // 싱글톤 인스턴싱 세팅
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            // 프리뷰 리스트 세팅
            _previewItemMap = new Dictionary<int, GameObject>();
            foreach (var item in ItemList)
            {
                GameObject previewObject = GameObject.Find("Preview_" + item.ItemID);
                if (previewObject != null)
                {
                    _previewItemMap[item.ItemID] = previewObject;
                    previewObject.SetActive(false);
                }
            }

            // 임시로 아이템 데이터 등록 및 초기화
            for (int idx = 0; idx < 24; ++idx)
            {
                GameObject itemButton = Instantiate(ItemButtonPrefab, ItemContainer.transform);
                ItemDataSO itemData;
                if (idx % 3 == 1)
                {
                    itemData = ItemList[0];
                }
                else if (idx % 3 == 2)
                {
                    itemData = ItemList[1];
                }
                else
                {
                    itemData = ItemList[2];
                }
                itemButton.GetComponent<ItemButtonSetting>().Init(itemData, Random.Range(1, 10));
                _inventoryItemMap.Add(itemButton, itemData.ItemType);

                itemButton.GetComponent<Button>().onClick.AddListener(() => ShowItemPreview(itemData.ItemID));
            }
        }

        void Start()
        {
            
        }

        /// <summary>
        /// 토글에 의해 호출됨
        /// </summary>
        /// <param name="type"></param>
        public void FilterItemsByType(eItemType type)
        {
            // key value pair를 순회하여 타입에 일치하는 아이콘(버튼)만 활성화
            foreach (var kvp in _inventoryItemMap)
            {
                GameObject itemButton = kvp.Key;
                eItemType itemType = kvp.Value;

                itemButton.gameObject.SetActive(itemType == type);
            }
        }

        public void ShowItemPreview(int itemID)
        {
            // 프리뷰 아이탬 리스트를 순회하며 전부 비활성화 시킴
            foreach (var previewItemObject in _previewItemMap.Values)
            {
                previewItemObject.SetActive(false);
            }

            // ID 입력값에 해당하는 대상만 활성화
            if (_previewItemMap.TryGetValue(itemID, out GameObject itemToShow))
            {
                itemToShow.SetActive(true);
            }
        }
    }
}
