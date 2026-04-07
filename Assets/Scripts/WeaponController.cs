using UnityEngine;

public class WeaponController : MonoBehaviour
{

    private Vector2 _WeaponCrossHairPos;
    [SerializeField] private float _bulletSpeed = 5f;


    //Weapon weapon make a factory type weapon design pattern class for later
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Weapon.spawnbullet()
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Purpose: delete bullet if far enough from camera for performance purposes
}
