using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/***************************************************************
*file: WeaponController
*author: Nathan Rinon
*class: CS 4700 – Game Development
*assignment: program 1
*date last modified: 4/9/2026
*
*purpose: This script controls the weapon function the player uses. It listens what weapon the player has and listen if the player fires
*
****************************************************************/

public class WeaponController : MonoBehaviour
{


    private InputAction fireAction;
    [SerializeField] public WeaponFactory weaponFactory;
    private Weapon weapon;
    [SerializeField] private WeaponType weaponType;
    private GameObject _weaponGameObject;
    [SerializeField] private Transform playerTransform;

    //make a factory type weapon design pattern class for later
    void Start()
    {
        fireAction = InputSystem.actions.FindAction("Attack");
        updateWeapon(weaponType);

    }

    // Update is called once per frame
    void Update()
    {

        listenForWeaponChange();
        
        if(fireAction.WasPerformedThisFrame())
        {
            updateBulletRotation();
            weapon.Fire();   
        }
        
    }

    private void listenForWeaponChange()
    {
        
        if (Keyboard.current.digit1Key.isPressed)
        {
            updateWeapon(WeaponType.slowWeapon);
        }
        if (Keyboard.current.digit2Key.isPressed)
        {
            updateWeapon(WeaponType.fastWeapon);
        }
        else if (Keyboard.current.digit3Key.isPressed)
        {
            updateWeapon(WeaponType.ShotgunWeapon);
        }

        //ensure switching weapons doesn't destroy object
        if(transform.childCount>1)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

    }


    private void updateWeapon(WeaponType weaponType)
    {
   
        weapon = weaponFactory.createWeapon(weaponType);
        GameObject weapon_object = Instantiate(weapon.GetWeapon(), transform.position, Quaternion.identity);
        weapon_object.transform.SetParent(transform);
        weapon.SetWeaponObject(weapon_object);
        weapon.applyCrosshairColor();
        _weaponGameObject = weapon_object;
        

    }

    private void updateBulletRotation()
    {

        Vector2 crossHairPosition = transform.position;
        Vector2 playerPos = new Vector2(playerTransform.position.x, playerTransform.position.y);
        Vector2 direction = crossHairPosition - playerPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weaponGameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    //purpose: calculates the roation to transform the bullet orientation 


}
