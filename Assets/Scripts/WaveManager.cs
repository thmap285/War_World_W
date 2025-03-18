using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class EnemyGroup
    {
        public Transform enemyPrefab;
        public int count;
    }

    [System.Serializable]
    public class Wave
    {
        public EnemyGroup[] enemyGroups;
        public float rate;
        public bool hasBoss;
        public Transform bossPrefab;
    }

    public Wave[] waves;
    private int _nextWave = 0;

    public float timeBetweenWaves = 5f;
    private float _waveCountdown;

    public Transform[] spawnPoints;
    private SpawnState _state = SpawnState.COUNTING;

    // UI
    public Text waveText;
    public Text enemyCountText;
    private int _enemyAliveCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _waveCountdown = timeBetweenWaves;
        UpdateUI();
    }

    private void Update()
    {
        if (_state == SpawnState.WAITING)
        {
            if (!ZombieIsAlive())
            {
                WaveCompleted();
            }
            return;
        }

        if (_waveCountdown <= 0)
        {
            if (_state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[_nextWave]));
            }
        }
        else
        {
            _waveCountdown -= Time.deltaTime;
        }
    }

    private bool ZombieIsAlive()
    {
        return _enemyAliveCount > 0;
    }

    private void WaveCompleted()
    {
        _state = SpawnState.COUNTING;
        _waveCountdown = timeBetweenWaves;

        if (_nextWave + 1 > waves.Length - 1)
        {
            Debug.Log("Tất cả các wave đã hoàn thành!");
            return;
        }

        _nextWave++;
        UpdateUI();
    }

    private IEnumerator SpawnWave(Wave _wave)
    {
        _state = SpawnState.SPAWNING;
        _enemyAliveCount = 0;

        // Spawn zombie
        foreach (EnemyGroup group in _wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnZombie(group.enemyPrefab);
                yield return new WaitForSeconds(1f / _wave.rate);
            }
        }

        // pawn boss
        if (_wave.hasBoss && _wave.bossPrefab != null)
        {
            SpawnZombie(_wave.bossPrefab);
        }

        _state = SpawnState.WAITING;
        UpdateUI();
    }

    private void SpawnZombie(Transform zombie)
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(zombie, point.position, Quaternion.identity);

        _enemyAliveCount++;
        UpdateUI();
    }

    public void ZombieDied()
    {
        _enemyAliveCount--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        waveText.text = "Wave " + (_nextWave + 1);
        enemyCountText.text = _enemyAliveCount.ToString();
    }
}
