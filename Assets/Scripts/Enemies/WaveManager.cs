using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public static event System.Action OnAllRoundsComplete;

    [System.Serializable]
    public class EnemyPrefabEntry
    {
        public string id;
        public GameObject prefab;
    }

    [System.Serializable]
    public class HostagePrefabEntry
    {
        public string id;
        public GameObject prefab;
    }

    [System.Serializable]
    public class FormationSlot
    {
        public Vector2 localOffset;
        public List<string> allowedEnemyIds = new List<string>();
    }

    [System.Serializable]
    public class FormationDefinition
    {
        public string formationName;
        public List<FormationSlot> slots = new List<FormationSlot>();
    }

    [System.Serializable]
    public class HostageEventDefinition
    {
        public bool enabled = true;
        public int eventCount = 1;
        public string hostagePrefabId;
        public string guardEnemyId = "Guard_B";
        public float guardOffset = 2f;
        public Vector2 hostageOffsetFromPlayer = new Vector2(1.4f, 0.5f);
        public float hostageFollowSmoothTime = 0.12f;
        public LayerMask enemyLayerMask = ~0;
    }

    [System.Serializable]
    public class RoundDefinition
    {
        public string roundName = "Round";
        public int formationCount = 3;
        public float delayBetweenFormations = 2f;
        public List<FormationDefinition> allowedFormations = new List<FormationDefinition>();
        public HostageEventDefinition hostageEvent = new HostageEventDefinition();
        public AsteroidEventDefinition asteroidEvent = new AsteroidEventDefinition();
    }

    [System.Serializable]
    public class AsteroidEventDefinition
    {
        public bool enabled = false;
        public GameObject asteroidPrefab;
        public int asteroidCount = 3;
        public float delayBetweenAsteroids = 0.6f;
        public float spawnPadding = 1.5f;
        public float minDriftSpeed = 1.2f;
        public float maxDriftSpeed = 2.4f;
        public float minLifetime = 10f;
        public float maxLifetime = 16f;
    }

    [Header("Scene References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    [Header("Enemy Prefabs")]
    [SerializeField] private List<EnemyPrefabEntry> enemyPrefabs = new List<EnemyPrefabEntry>();

    [Header("Hostage Prefabs")]
    [SerializeField] private List<HostagePrefabEntry> hostagePrefabs = new List<HostagePrefabEntry>();

    [Header("Rounds")]
    [SerializeField] private List<RoundDefinition> rounds = new List<RoundDefinition>();

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-18f, -10f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(18f, 10f);
    [SerializeField] private float minDistanceFromPlayer = 6f;
    [SerializeField] private int maxSpawnPointAttempts = 30;

    [Header("Flow")]
    [SerializeField] private bool autoStart = true;

    private readonly Dictionary<string, GameObject> enemyLookup = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, GameObject> hostageLookup = new Dictionary<string, GameObject>();
    private readonly HashSet<SpawnedEnemyTracker> liveEnemies = new HashSet<SpawnedEnemyTracker>();
    private bool runningRounds;

    private void Awake()
    {
        BuildEnemyLookup();
        BuildHostageLookup();

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
        if (autoStart)
        {
            StartRounds();
        }
    }

    public void StartRounds()
    {
        if (runningRounds) return;
        runningRounds = true;
        StartCoroutine(RunRounds());
    }

    private void BuildEnemyLookup()
    {
        enemyLookup.Clear();

        foreach (EnemyPrefabEntry entry in enemyPrefabs)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.id) || entry.prefab == null)
                continue;

            if (!enemyLookup.ContainsKey(entry.id))
                enemyLookup.Add(entry.id, entry.prefab);
            else
                Debug.LogWarning($"Duplicate enemy prefab id found: {entry.id}");
        }
    }

    private void BuildHostageLookup()
    {
        hostageLookup.Clear();

        foreach (HostagePrefabEntry entry in hostagePrefabs)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.id) || entry.prefab == null)
                continue;

            if (!hostageLookup.ContainsKey(entry.id))
                hostageLookup.Add(entry.id, entry.prefab);
            else
                Debug.LogWarning($"Duplicate hostage prefab id found: {entry.id}");
        }
    }

    private IEnumerator RunRounds()
    {
        for (int i = 0; i < rounds.Count; i++)
        {
            yield return StartCoroutine(RunSingleRound(rounds[i], i + 1));
        }

        Debug.Log("All rounds complete. Boss round can go here next.");
        runningRounds = false;
        OnAllRoundsComplete?.Invoke();  // move invoke to after boss is killed for proper win condition?
    }

    private IEnumerator RunSingleRound(RoundDefinition round, int roundNumber)
    {
        Debug.Log($"Starting Round {roundNumber}: {round.roundName}");

        if (roundNumber == 6)
        {
            SpawnBossOnce();
        }

        if (round.allowedFormations == null || round.allowedFormations.Count == 0)
        {
            Debug.LogWarning($"Round '{round.roundName}' has no formations assigned.");
        }
        else
        {
            for (int i = 0; i < round.formationCount; i++)
            {
                SpawnRandomFormation(round);
                yield return new WaitForSeconds(round.delayBetweenFormations);
            }
        }
        if (round.asteroidEvent != null && round.asteroidEvent.enabled)
        {
            yield return StartCoroutine(SpawnAsteroidEvent(round.asteroidEvent));
        }
        if (round.hostageEvent != null && round.hostageEvent.enabled)
        {
            for (int i = 0; i < round.hostageEvent.eventCount; i++)
            {
                SpawnHostageEvent(round.hostageEvent);
                yield return new WaitForSeconds(1f);
            }
        }

        yield return new WaitUntil(() => liveEnemies.Count == 0);

        Debug.Log($"Finished Round {roundNumber}: {round.roundName}");
    }

    private void SpawnBossOnce()
    {
        if (!enemyLookup.TryGetValue("boss", out GameObject bossPrefab))
        {
            Debug.LogWarning("Boss prefab id 'boss' was not found in enemyPrefabs.");
            return;
        }

        Vector3 spawnPos;

        if (mainCamera != null)
        {
            float camHalfHeight = mainCamera.orthographicSize;
            float camHalfWidth = camHalfHeight * mainCamera.aspect;
            Vector3 camPos = mainCamera.transform.position;

            spawnPos = new Vector3(
                camPos.x + camHalfWidth + 2f,
                camPos.y,
                0f
            );
        }
        else
        {
            spawnPos = GetValidSpawnCenter();
        }

        SpawnEnemy(bossPrefab, spawnPos);
    }

    private void SpawnRandomFormation(RoundDefinition round)
    {
        FormationDefinition formation = round.allowedFormations[Random.Range(0, round.allowedFormations.Count)];
        Vector2 center = GetValidSpawnCenter();

        foreach (FormationSlot slot in formation.slots)
        {
            if (slot.allowedEnemyIds == null || slot.allowedEnemyIds.Count == 0)
                continue;

            string chosenEnemyId = slot.allowedEnemyIds[Random.Range(0, slot.allowedEnemyIds.Count)];

            if (!enemyLookup.TryGetValue(chosenEnemyId, out GameObject prefab))
            {
                Debug.LogWarning($"Enemy id '{chosenEnemyId}' was not found in enemyPrefabs.");
                continue;
            }

            Vector3 spawnPos = new Vector3(
                center.x + slot.localOffset.x,
                center.y + slot.localOffset.y,
                0f
            );

            SpawnEnemy(prefab, spawnPos);
        }
    }

    private SpawnedEnemyTracker SpawnEnemy(GameObject prefab, Vector3 position)
    {
        GameObject enemyObj = Instantiate(prefab, position, Quaternion.identity);

        EnemySpawnBootstrap bootstrap = enemyObj.GetComponent<EnemySpawnBootstrap>();
        if (bootstrap == null)
            bootstrap = enemyObj.AddComponent<EnemySpawnBootstrap>();

        bootstrap.Initialize(player, mainCamera);

        SpawnedEnemyTracker tracker = enemyObj.GetComponent<SpawnedEnemyTracker>();
        if (tracker == null)
            tracker = enemyObj.AddComponent<SpawnedEnemyTracker>();

        tracker.Initialize(this);
        liveEnemies.Add(tracker);
        return tracker;
    }

    public void NotifyEnemyDestroyed(SpawnedEnemyTracker tracker)
    {
        if (tracker != null)
            liveEnemies.Remove(tracker);
    }

    private Vector2 GetValidSpawnCenter()
    {
        Vector2 fallback = player != null
            ? (Vector2)player.position + Random.insideUnitCircle.normalized * minDistanceFromPlayer
            : Vector2.zero;

        for (int i = 0; i < maxSpawnPointAttempts; i++)
        {
            Vector2 point = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            if (player == null)
                return point;

            if (Vector2.Distance(point, player.position) >= minDistanceFromPlayer)
                return point;
        }

        return fallback;
    }

    private void SpawnHostageEvent(HostageEventDefinition hostageEvent)
    {
        if (player == null)
        {
            Debug.LogWarning("WaveManager cannot spawn a hostage event because the player reference is missing.");
            return;
        }

        if (!hostageLookup.TryGetValue(hostageEvent.hostagePrefabId, out GameObject hostagePrefab))
        {
            Debug.LogWarning($"Hostage prefab id '{hostageEvent.hostagePrefabId}' was not found.");
            return;
        }

        if (!enemyLookup.TryGetValue(hostageEvent.guardEnemyId, out GameObject guardPrefab))
        {
            Debug.LogWarning($"Hostage guard enemy id '{hostageEvent.guardEnemyId}' was not found.");
            return;
        }

        Vector2 center = GetValidSpawnCenter();
        GameObject hostageObj = Instantiate(hostagePrefab, new Vector3(center.x, center.y, 0f), Quaternion.identity);

        HostageCompanion hostageCompanion = hostageObj.GetComponent<HostageCompanion>();
        if (hostageCompanion == null)
            hostageCompanion = hostageObj.AddComponent<HostageCompanion>();

        hostageCompanion.Initialize(player, mainCamera, hostageEvent.hostageOffsetFromPlayer, hostageEvent.hostageFollowSmoothTime, hostageEvent.enemyLayerMask);

        Vector3 leftGuardPos = new Vector3(center.x - hostageEvent.guardOffset, center.y, 0f);
        Vector3 rightGuardPos = new Vector3(center.x + hostageEvent.guardOffset, center.y, 0f);

        SpawnedEnemyTracker leftGuard = SpawnEnemy(guardPrefab, leftGuardPos);
        SpawnedEnemyTracker rightGuard = SpawnEnemy(guardPrefab, rightGuardPos);

        HostageEventRuntime runtime = hostageObj.GetComponent<HostageEventRuntime>();
        if (runtime == null)
            runtime = hostageObj.AddComponent<HostageEventRuntime>();

        runtime.Initialize(hostageCompanion, leftGuard, rightGuard);
    }

    private IEnumerator SpawnAsteroidEvent(AsteroidEventDefinition asteroidEvent)
    {
        if (asteroidEvent.asteroidPrefab == null)
        {
            Debug.LogWarning("Asteroid event is enabled, but no asteroid prefab is assigned.");
            yield break;
        }

        for (int i = 0; i < asteroidEvent.asteroidCount; i++)
        {
            SpawnDriftingAsteroid(asteroidEvent);
            yield return new WaitForSeconds(asteroidEvent.delayBetweenAsteroids);
        }
    }

    private void SpawnDriftingAsteroid(AsteroidEventDefinition asteroidEvent)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("Cannot spawn asteroid because Main Camera is missing.");
            return;
        }

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector3 camPos = mainCamera.transform.position;

        float left = camPos.x - camWidth;
        float right = camPos.x + camWidth;
        float top = camPos.y + camHeight;
        float bottom = camPos.y - camHeight;

        float spawnX = Random.Range(camPos.x, right);
        float spawnY = top + asteroidEvent.spawnPadding;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        float targetX = Random.Range(left - asteroidEvent.spawnPadding, camPos.x);
        float targetY = bottom - asteroidEvent.spawnPadding;

        Vector2 targetPos = new Vector2(targetX, targetY);
        Vector2 driftDirection = (targetPos - (Vector2)spawnPos).normalized;

        GameObject asteroid = Instantiate(
            asteroidEvent.asteroidPrefab,
            spawnPos,
            Quaternion.identity
        );

        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();

        if (rb == null)
            rb = asteroid.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.linearVelocity = driftDirection * Random.Range(
            asteroidEvent.minDriftSpeed,
            asteroidEvent.maxDriftSpeed
        );

        rb.angularVelocity = Random.Range(-2f, 2f);

        Destroy(
            asteroid,
            Random.Range(asteroidEvent.minLifetime, asteroidEvent.maxLifetime)
        );
    }
}
