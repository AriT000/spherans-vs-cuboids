using UnityEngine;

public class ShotgunWeapon : Weapon
{

    [SerializeField] GameObject weaponObject;
    ParticleSystem particleSystem;


    public override GameObject GetWeapon()
    {

        Debug.Log("Weapon Object is Created");
        return weaponObject;
    }


    //sets the weapon object by using enabling particle system
    public override void SetWeaponObject(GameObject weapon_object)
    {
        weaponObject = weapon_object;
        particleSystem = weapon_object.GetComponent<ParticleSystem>();
        Debug.Log("Weapon is set");
        particleSystem.Stop();
    }

    public override void Fire()
    {
        particleSystem.Emit(1);
    }



}