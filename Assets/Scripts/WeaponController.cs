using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{


    private InputAction fireAction;
    [SerializeField] public WeaponFactory weaponFactory;
    private Weapon weapon;
    [SerializeField] private WeaponType weaponType;
    private GameObject _weaponGameObject;
    [SerializeField] private Transform playerTransform;

    //make a factory type weapon design pattern class for later
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireAction = InputSystem.actions.FindAction("Attack");
        weapon = weaponFactory.createWeapon(weaponType);
        GameObject weapon_object = Instantiate(weapon.GetWeapon(), transform.position, Quaternion.identity);
        weapon_object.transform.SetParent(transform);
        weapon.SetWeaponObject(weapon_object);
        _weaponGameObject = weapon_object;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(fireAction.WasPerformedThisFrame())
        {
            updateBulletRotation();
            weapon.Fire();   
        }
        
    }

    //purpose: calculates the roation to transform the bullet orientation 
    private void updateBulletRotation()
    {
        
        Vector2 crossHairPosition = transform.position;
        Vector2 playerPos = new Vector2(playerTransform.position.x, playerTransform.position.y) - crossHairPosition;
        float angle = Vector2.Angle(playerPos, Vector2.right);
        _weaponGameObject.transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

}
