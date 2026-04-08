using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{

    private Vector2 _WeaponCrossHairPos;
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] IWeapon _weapon;
    private InputAction fireAction;


    //make a factory type weapon design pattern class for later
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        if(fireAction.WasPerformedThisFrame())
        {
            _weapon.fire();
        }
    }
}
