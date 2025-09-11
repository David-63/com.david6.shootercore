using UnityEngine;

namespace David6.ShooterCore.Item.Weapon
{
    [CreateAssetMenu(fileName = "MagazineModule", menuName = "Item/Module/Weapon/Magazine Module")]
    public class DMagazineModule : DBaseItemModule
    {
        public GameObject MagazineEjectFX;
        public GameObject ChamberCaseFX;
        public GameObject BulletTrailFX;

        public GameObject ImpactShardFX;
    }


}