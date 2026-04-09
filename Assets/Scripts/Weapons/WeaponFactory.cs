using UnityEngine;

public class WeaponFactory : MonoBehaviour
{
	[SerializeField] GameObject slowWeapon;
	[SerializeField] GameObject fastWeapon;


	public Weapon createWeapon(WeaponType weaponName)
	{
		Weapon weapon = null;
		if (weaponName == WeaponType.slowWeapon)
		{
			weapon = new SlowWeapon();
			weapon.SetWeaponObject(slowWeapon);
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
	fastWeapon
}
