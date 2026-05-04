/***************************************************************
*file: WeaponController.cs
*author: Nathan Rinon
*class: CS 4700 – Game Development
*assignment: program 1
*date last modified: 5/2/2026
*
*purpose: This script controls the weapon function the player uses. It listens what weapon the player has and listen if the player fires
*
****************************************************************/

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class WeaponController : MonoBehaviour
{

    public TextMeshProUGUI SlowGun;
    public TextMeshProUGUI FastGun;
    public TextMeshProUGUI ShotGun;
    private InputAction fireAction;
    [SerializeField] public WeaponFactory weaponFactory;
    private Weapon weapon;
    [SerializeField] private WeaponType weaponType;
    private GameObject _weaponGameObject;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private OverheatManager overheatManager;
    [SerializeField] private AudioSource shootAudio;

    private HeatBarUI heatBarUI;


    //make a factory type weapon design pattern class for later
    void Start()
    {
        fireAction = InputSystem.actions.FindAction("Attack");
        overheatManager = GetComponent<OverheatManager>();

        heatBarUI = GameObject.FindWithTag("HudManager").GetComponent<HeatBarUI>();
        if(heatBarUI == null)
        {
            throw new NullReferenceException("No heatbar is used on scene, please put HUD prefab in scene");
        }

        heatBarUI.SetMaxHeat(overheatManager.MaxHeat);

        updateWeapon(weaponType);

    }

    // Update is called once per frame
    void Update()
    {
        overheatManager.regenerateHeat(Time.deltaTime);
        heatBarUI.updateHeat(overheatManager.Heat);
        listenForWeaponChange();
        if(fireAction.WasPerformedThisFrame() && !overheatManager.isOverHeat())
        {
            overheatManager.spendHeat();
            updateBulletRotation();
            weapon.Fire();  

            shootAudio.Play(); 
        }

        
    }

    //Purpose: Listens if user changes their weapon. 
    private void listenForWeaponChange()
    {
        
        if (Keyboard.current.digit1Key.isPressed)
        {
            ResetWeaponColors();
            updateWeapon(WeaponType.slowWeapon);
            SlowGun.color = UnityEngine.Color.red;
        }
        if (Keyboard.current.digit2Key.isPressed)
        {
            ResetWeaponColors();
            updateWeapon(WeaponType.fastWeapon);
            FastGun.color = UnityEngine.Color.red;
        }
        else if (Keyboard.current.digit3Key.isPressed)
        {
            ResetWeaponColors();
            updateWeapon(WeaponType.ShotgunWeapon);
            ShotGun.color = UnityEngine.Color.red;
        }

        //ensure switching weapons doesn't destroy object
        if(transform.childCount>1)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

    }


    //Purpose: Changes the current weapon to the parameters inserted, and createa a new game object in the editor. It ensures that the game object stays in the same place.
    private void updateWeapon(WeaponType weaponType)
    {
        weapon = weaponFactory.createWeapon(weaponType, overheatManager);
        GameObject weapon_object = Instantiate(weapon.GetWeapon(), transform.position, Quaternion.identity);
        weapon_object.transform.SetParent(transform);
        weapon.SetWeaponObject(weapon_object);
        weapon.applyCrosshairColor();
        _weaponGameObject = weapon_object;
        

    }
    
    //purpose: calculates the roation to transform the bullet orientation 
    private void updateBulletRotation()
    {

        Vector2 crossHairPosition = transform.position;
        Vector2 playerPos = new Vector2(playerTransform.position.x, playerTransform.position.y);
        Vector2 direction = crossHairPosition - playerPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weaponGameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    

    private void ResetWeaponColors()
    {
        SlowGun.color = UnityEngine.Color.white;
        FastGun.color = UnityEngine.Color.white;
        ShotGun.color = UnityEngine.Color.white;
    }

}
