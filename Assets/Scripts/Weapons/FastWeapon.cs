using UnityEngine;

/***************************************************************
*file: SlowWeapon.cs
*author: Nathan Rinon
*class: CS 4700 ? Game Development
*assignment: program 1
*date last modified: 4/22/2026
*
*purpose: This script implements the weapon abstract class for fast weapon qualities.
*
****************************************************************/


public class FastWeapon : Weapon
{

    [SerializeField] GameObject weaponObject;
    ParticleSystem particleSystem;

    //Purpose: returns weapon that this object contains.
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

        Camera cam = Camera.main;
        if (cam != null)
        {
            var main = particleSystem.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Custom;
            main.customSimulationSpace = cam.transform;
        }

        var inheritVelocity = particleSystem.inheritVelocity;
        inheritVelocity.enabled = false;
    }

    //Purpose: Fires the particle 
    public override void Fire()
    {
        particleSystem.Emit(1);
    }



}