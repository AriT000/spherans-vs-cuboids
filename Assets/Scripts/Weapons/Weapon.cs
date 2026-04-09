using UnityEngine;

public abstract class Weapon
{
    public abstract GameObject GetWeapon();

    public abstract void SetWeaponObject(GameObject obj);

    public abstract void Fire();
}