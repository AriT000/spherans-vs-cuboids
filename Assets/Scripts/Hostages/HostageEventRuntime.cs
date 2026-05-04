using UnityEngine;

public class HostageEventRuntime : MonoBehaviour
{
    [SerializeField] private HostageCompanion hostage;

    private SpawnedEnemyTracker leftGuard;
    private SpawnedEnemyTracker rightGuard;
    private int guardsRemaining;
    private bool initialized;

    public void Initialize(HostageCompanion hostageCompanion, SpawnedEnemyTracker leftGuardTracker, SpawnedEnemyTracker rightGuardTracker)
    {
        hostage = hostageCompanion;
        leftGuard = leftGuardTracker;
        rightGuard = rightGuardTracker;
        guardsRemaining = 0;

        if (leftGuard != null)
        {
            guardsRemaining++;
            leftGuard.Destroyed += OnGuardDestroyed;
        }

        if (rightGuard != null)
        {
            guardsRemaining++;
            rightGuard.Destroyed += OnGuardDestroyed;
        }

        initialized = true;

        if (guardsRemaining == 0 && hostage != null)
            hostage.FreeHostage();
    }

    private void OnGuardDestroyed(SpawnedEnemyTracker _)
    {
        guardsRemaining = Mathf.Max(0, guardsRemaining - 1);

        if (guardsRemaining == 0 && hostage != null)
        {
            hostage.FreeHostage();
        }
    }

    private void OnDestroy()
    {
        if (!initialized)
            return;

        if (leftGuard != null)
            leftGuard.Destroyed -= OnGuardDestroyed;

        if (rightGuard != null)
            rightGuard.Destroyed -= OnGuardDestroyed;
    }
}
