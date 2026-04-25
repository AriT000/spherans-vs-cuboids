/***************************************************************
*file: ShotgunWeapon.cs
*author: Nathan Rinon
*class: CS 4700 ? Game Development
*assignment: program 1
*date last modified: 4/22/2026
*
*purpose: This script controls the abstract class Weapon and shows what every weapon has in order to use the factory design pattern.
*
****************************************************************/


using UnityEngine;

public class ShotgunWeapon : Weapon
{

    [SerializeField] private GameObject weaponObject;
    [SerializeField] private int bulletCount = 5;
    private ParticleSystem particleSystem;


    public override GameObject GetWeapon()
    {

        Debug.Log("Weapon Object is Created");
        return weaponObject;
    }


    //purpose: Sets the weapon object, previously removes the current weapon and swaps the new one. Also ensures, the previous particle system was stopped.
    public override void SetWeaponObject(GameObject weapon_object)
    {
        weaponObject = weapon_object;
        particleSystem = weapon_object.GetComponent<ParticleSystem>();
        Debug.Log("Weapon is set");
        particleSystem.Stop();
    }

    public override void Fire()
    {
        particleSystem.Emit(bulletCount);
    }



}