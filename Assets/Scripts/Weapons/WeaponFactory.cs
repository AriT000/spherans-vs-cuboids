using UnityEngine;

public abstract class WeaponFactory: MonoBehaviour
{
    public abstract IWeapon GetWeapon();
}