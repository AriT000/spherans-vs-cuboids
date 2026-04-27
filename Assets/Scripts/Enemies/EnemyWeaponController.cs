using Assets.Scripts.Entities;
using UnityEngine;

[RequireComponent(typeof(AttributesManager))]
public class EnemyWeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject weaponPrefab;

    [Header("Shooting")]
    [SerializeField] private float fireInterval = 1f;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float attackRange = 12f;
    [SerializeField] private float initialShotDelay = 0.25f;

    [Header("Weapon Transform")]
    [SerializeField] private bool aimAtPlayer = true;
    [SerializeField] private float weaponForwardOffset = 0f;
    [SerializeField] private Vector3 weaponLocalOffset = Vector3.zero;

    private ParticleSystem weaponParticleSystem;
    private Transform weaponTransform;
    private float nextShotTime;

    private void Start()
    {
        ResolvePlayerReference();
        InitializeWeapon();
        float intervalFloor = Mathf.Max(0.1f, fireInterval);
        nextShotTime = Time.time + initialShotDelay + Random.Range(0f, intervalFloor * 0.35f);
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            ResolvePlayerReference();
            return;
        }

        if (weaponParticleSystem == null || weaponTransform == null)
        {
            InitializeWeapon();
            if (weaponParticleSystem == null || weaponTransform == null)
            {
                return;
            }
        }

        if (aimAtPlayer)
        {
            UpdateWeaponAim();
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > attackRange)
        {
            return;
        }

        if (Time.time >= nextShotTime)
        {
            weaponParticleSystem.Emit(Mathf.Max(1, bulletsPerShot));
            nextShotTime = Time.time + Mathf.Max(0.1f, fireInterval);
        }
    }

    private void UpdateWeaponAim()
    {
        Vector2 direction = (Vector2)playerTransform.position - (Vector2)weaponTransform.position;
        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + weaponForwardOffset;
        weaponTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void InitializeWeapon()
    {
        if (weaponPrefab == null)
        {
            return;
        }

        if (weaponParticleSystem != null)
        {
            return;
        }

        GameObject weaponObject = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        weaponObject.transform.SetParent(transform);
        weaponObject.transform.localPosition = weaponLocalOffset;
        weaponObject.transform.localRotation = Quaternion.identity;

        weaponTransform = weaponObject.transform;
        weaponParticleSystem = weaponObject.GetComponent<ParticleSystem>();
        if (weaponParticleSystem != null)
        {
            weaponParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void ResolvePlayerReference()
    {
        if (playerTransform != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject == null)
        {
            playerObject = GameObject.Find("Player");
        }

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }
}
