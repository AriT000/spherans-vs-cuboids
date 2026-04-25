/***************************************************************
*file: SlowWeapon.cs
*author: Nathan Rinon
*class: CS 4700 ? Game Development
*assignment: program 1
*date last modified: 4/22/2026
*
*purpose: This script implements the weapon abstract class for slowweapon.
*
****************************************************************/


using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SlowWeapon : Weapon
{

    [SerializeField] GameObject weaponObject;
    ParticleSystem particleSystem;


    //Purpose: returns weapon that the object contains.
    public override GameObject GetWeapon()
    {

        Debug.Log("Weapon Object is Created");
        return weaponObject;
    }

    public void initialize()
    {
        
    }

    //purpose: Sets the weapon object, previously removes the current weapon and swaps the new one. Also ensures, the previous particle system was stopped.
    public override void SetWeaponObject(GameObject weapon_object)
    {
        weaponObject = weapon_object;
        particleSystem = weapon_object.GetComponent<ParticleSystem>();
        Debug.Log("Weapon is set");
        particleSystem.Stop();
    }

    //Purpose: Fires the particle 
    public override void Fire()
    {
        particleSystem.Emit(1);
    }



}