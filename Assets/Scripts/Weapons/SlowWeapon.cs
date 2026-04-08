using UnityEngine;

public class SlowWeapon : MonoBehaviour, IWeapon
{
    private float _bulletSpeed;

    private float _cooldown;

    [SerializeField] private string weaponName = "SlowWeapon";
    public string WeaponName { get => weaponName; set => weaponName = value ; }

    private ParticleSystem _particleSystem;

    public void fire()
    {
        throw new System.NotImplementedException();
    }

    public void Initialize()
    {
        ParticleSystem 
    }

    public void calculateDirection()
    {

    }
}