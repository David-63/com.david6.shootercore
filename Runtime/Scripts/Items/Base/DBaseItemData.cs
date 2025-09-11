using UnityEngine;

namespace David6.ShooterCore.Item
{
    public abstract class DBaseItemData : ScriptableObject
    {
        public string ItemName; // Name of the item.
        [TextArea] public string ItemDescription; // Description of the item.
        public Sprite ItemIcon;
    }


}