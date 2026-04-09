using UnityEngine;

/***************************************************************
*file: WeaponFactory
*author: Nathan Rinon
*class: CS 4700 – Game Development
*assignment: program 1
*date last modified: 4/9/2026
*
*purpose: This script follows the Factory pattern by checking what weapon to create for the weapon controller abstracting the need for others
*to see the implementation of theweapons
*
****************************************************************/


public class WeaponFactory : MonoBehaviour
{
	[SerializeField] private GameObject slowWeapon;
	[SerializeField] private GameObject fastWeapon;
	[SerializeField] private GameObject shotgunWeapon;
    [SerializeField] Color slowWeaponColor;
    [SerializeField] Color fastWeaponColor;
    [SerializeField] Color shotGunWeaponColor;


    public Weapon createWeapon(WeaponType weaponName)
	{
		Weapon weapon = null;
		if (weaponName == WeaponType.slowWeapon)
		{
			weapon = new SlowWeapon();
			weapon.SetWeaponObject(slowWeapon);
			weapon.setColor(slowWeaponColor);
		}
		else if (weaponName == WeaponType.fastWeapon)
		{
            weapon = new SlowWeapon();
            weapon.SetWeaponObject(fastWeapon);
			weapon.setColor(fastWeaponColor);
        }
		else if (weaponName == WeaponType.ShotgunWeapon)
		{
            
            weapon = new ShotgunWeapon();
            weapon.SetWeaponObject(shotgunWeapon);
			weapon.setColor(shotGunWeaponColor);
        }
		else
		{
			Debug.Log("Weapon does not exist");
		}

		return weapon;
		
	}


}

public enum WeaponType
{
    slowWeapon,
	fastWeapon,
	ShotgunWeapon
}
