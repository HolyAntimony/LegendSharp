using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    class Weapon : Item
    {
        String weaponClass;
        double damage;
        String damageType;

        BaseWeapon baseWeapon;

        public Weapon(BaseWeapon baseItem, String itemSprite = null, String itemName = null, String itemDescription = null, String itemType = null, String weaponClass = null, double damage = Double.NaN, String damageType = null) : base(baseItem, itemSprite, itemName, itemDescription, itemType)
        {
            this.baseWeapon = baseItem;
            this.weaponClass = weaponClass;
            this.damage = damage;
            this.damageType = damageType;
        }

        public String GetWeaponClass()
        {
            return weaponClass ?? baseWeapon.weaponClass;
        }

        public double GetDamage()
        {
            return damage == Double.NaN ? baseWeapon.damage : damage;
        }

        public String GetDamageType()
        {
            return damageType ?? baseWeapon.damageType;
        }

        public bool HasWeaponClass()
        {
            return weaponClass != null;
        }

        public bool HasDamage()
        {
            return damage != Double.NaN;
        }

        public bool HasDamageType()
        {
            return damageType != null;
        }
    }
}
