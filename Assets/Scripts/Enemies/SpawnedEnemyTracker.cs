using System;
using UnityEngine;

public class SpawnedEnemyTracker : MonoBehaviour
{
    private WaveManager waveManager;
    private bool initialized;

    public event Action<SpawnedEnemyTracker> Destroyed;

    public void Initialize(WaveManager manager)
    {
        waveManager = manager;
        initialized = true;
    }

    private void OnDestroy()
    {
        if (!initialized) return;

        Destroyed?.Invoke(this);

        if (waveManager != null)
        {
            waveManager.NotifyEnemyDestroyed(this);
        }
    }
}
