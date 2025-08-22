using System;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Provider;
using UnityEngine;

namespace David6.ShooterCore.Combat
{
    public class DCombatHandler
    {
        // 무기나 장비를 알고 있어야함

        // 외부에서 호출함
        IDContextProvider _context;
        DGearData _weapon;
        GameObject _currentWeapon;


        public DCombatHandler(IDContextProvider context) => _context = context;

        public void SetWeapon(DGearData data)
        {
            _weapon = data;

            if (_currentWeapon == null)
            {
                _currentWeapon = _context.MakeObject(_weapon.GearPrefab, _context.WeaponSocket);
            }
        }

        public void Fire()
        {

        }
    }

}