using UnityEngine;

namespace David6.ShooterCore.Item.Weapon
{
    [CreateAssetMenu(fileName = "MuzzleModule", menuName = "Item/Module/Weapon/Muzzle Module")]
    public class DMuzzleModule : DBaseItemModule
    {
        public GameObject MuzzleFlashFX;
        public AudioClip FireSound;
        public AudioClip EmptySound;
    }


}