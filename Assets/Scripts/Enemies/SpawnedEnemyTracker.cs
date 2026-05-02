using UnityEngine;

public class SpawnedEnemyTracker : MonoBehaviour
{
    private WaveManager waveManager;
    private bool initialized;

    public void Initialize(WaveManager manager)
    {
        waveManager = manager;
        initialized = true;
    }

    private void OnDestroy()
    {
        if (!initialized) return;

        if (waveManager != null)
        {
            waveManager.NotifyEnemyDestroyed(this);
        }
    }
}