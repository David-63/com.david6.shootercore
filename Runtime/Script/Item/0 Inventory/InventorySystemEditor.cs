using UnityEditor;
using UnityEngine;

namespace David6.ShooterFramework
{
    [CustomEditor(typeof(InventorySystem))]
    public class InventorySystemEditor : Editor
    {
        private Color _lineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        private float _rowHeight = 20f;

        public override void OnInspectorGUI()
        {
            InventorySystem inventorySystem = target as InventorySystem;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxSlot"));

            if (inventorySystem.Slots == null || inventorySystem.Slots.Count == 0)
            {
                EditorGUILayout.LabelField("Inventory is empty!");
            }
            else
            {
                GUILayout.BeginVertical();
                DrawRowHeader();
                DrawHorizontalLine();

                for (int idx = 0; idx < inventorySystem.Slots.Count; ++idx)
                {
                    DrawRow(inventorySystem.Slots[idx], idx);
                    DrawHorizontalLine();
                }

                GUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
        private void DrawRowHeader()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Item Name", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("Quantity", EditorStyles.boldLabel, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }

        private void DrawRow(InventorySlot slot, int index)
        {
            GUILayout.BeginHorizontal();
            string itemName = slot.ItemData != null ? slot.ItemData.ItemName : "Empty Slot";
            EditorGUILayout.LabelField(itemName, GUILayout.Width(200));
            int quantity = slot.ItemData != null ? slot.Quantity : 0;
            EditorGUILayout.LabelField(quantity.ToString(), GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }
        private void DrawHorizontalLine()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, _lineColor);
        }
    }
}
