using David6.ShooterCore.Data.Gear;
using UnityEngine;

namespace David6.ShooterCore.Combat
{
    public class DCombatHandler
    {
        // 무기나 장비를 알고 있어야함

        // 외부에서 호출함

        DGearData _weapon;

        public void SetWeapon(DGearData data) => _weapon = data;

        public void Fire()
        {

        }
    }

}