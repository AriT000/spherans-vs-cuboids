using System.Reflection;
using UnityEngine;

public class EnemySpawnBootstrap : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    public void Initialize(Transform targetPlayer, Camera cam)
    {
        player = targetPlayer;
        mainCamera = cam;
        ApplyReferences();
    }

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        ApplyReferences();
    }

    private void ApplyReferences()
    {
        if (player == null) return;

        AssignPlayerToEnemyLaserWeapon();
        AssignPlayerToEnemyALaser();
        AssignEnemyAimControllerRefs();
        AssignEdgeFollowerRefs();
    }

    private void AssignPlayerToEnemyLaserWeapon()
    {
        MonoBehaviour[] allBehaviours = GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            if (behaviour == null) continue;

            if (behaviour.GetType().Name == "EnemyLaserWeapon")
            {
                FieldInfo playerField = behaviour.GetType().GetField("player", BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerField != null)
                {
                    playerField.SetValue(behaviour, player);
                }
            }
        }
    }

    private void AssignPlayerToEnemyALaser()
    {
        EnemyALaser[] lasers = GetComponentsInChildren<EnemyALaser>(true);
        foreach (EnemyALaser laser in lasers)
        {
            FieldInfo playerField = typeof(EnemyALaser).GetField("player", BindingFlags.NonPublic | BindingFlags.Instance);
            if (playerField != null)
            {
                playerField.SetValue(laser, player);
            }
        }
    }

    private void AssignEnemyAimControllerRefs()
    {
        EnemyAimController[] aimControllers = GetComponentsInChildren<EnemyAimController>(true);
        foreach (EnemyAimController aim in aimControllers)
        {
            FieldInfo enemyField = typeof(EnemyAimController).GetField("enemy", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo playerField = typeof(EnemyAimController).GetField("player", BindingFlags.NonPublic | BindingFlags.Instance);

            if (enemyField != null)
                enemyField.SetValue(aim, gameObject);

            if (playerField != null)
                playerField.SetValue(aim, player.gameObject);
        }
    }

    private void AssignEdgeFollowerRefs()
    {
        EnemyEdgeFollower[] followers = GetComponentsInChildren<EnemyEdgeFollower>(true);
        foreach (EnemyEdgeFollower follower in followers)
        {
            follower.SetPlayerTransform(player);

            if (mainCamera != null)
            {
                follower.SetCamera(mainCamera);
            }
        }
    }
}