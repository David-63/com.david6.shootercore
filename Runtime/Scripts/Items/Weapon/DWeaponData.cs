using UnityEngine;

namespace David6.ShooterCore.Item.Weapon
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Item/Data/Weapon Data")]
    public class DWeaponData : DBaseItemData
    {
        public float FirePower = 5.0f;
        public float FireRate = 450.0f;
        public int MagazineCapacity = 25;
        public float ProjectileSpeed = 100.0f;
        public float Range = 60.0f;
    }


}