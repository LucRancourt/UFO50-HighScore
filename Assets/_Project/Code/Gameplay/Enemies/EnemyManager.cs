using _Project.Code.Core.Factory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    private List<PooledFactory<EnemyBase>> _enemyPool = new List<PooledFactory<EnemyBase>>();
    [Tooltip("Keep them in order!")]
    [SerializeField] private EnemyBase[] allEnemyPrefabs;

    private int _waveIndex;
    private int _activeEnemies = -1;

    private void Start()
    {
        if (waves.Length == 0)
        {
            Debug.Log("Forgot Spawner List!");
            return;
        }

        Reset();
        SpawnNextWave();
    }

    public void Reset()
    {
        _activeEnemies = -1;

        _enemyPool.Clear();

        foreach (EnemyBase enemy in allEnemyPrefabs)
        {
            _enemyPool.Add(new PooledFactory<EnemyBase>(enemy));
        }

        _waveIndex = 0;
    }

    public void SpawnNextWave()
    {
        if (_waveIndex >= waves.Length)
        {
            _waveIndex = 0;
            // increase difficulty
        }

        // spawn wave
        for (int i = 0; i < waves[_waveIndex].enemyPrefabs.Length; i++)
        {
            StartCoroutine(SpawnNextEnemy(waves[_waveIndex].enemyPrefabs[i], (waves[_waveIndex].delayBetweenSpawns * i)));
        }

        _activeEnemies = waves[_waveIndex].enemyPrefabs.Length;

        _waveIndex++;
    }

    private IEnumerator SpawnNextEnemy(int enemyToSpawn, float delay)
    {
        yield return new WaitForSeconds(delay);

        EnemyBase enemy;

        switch (enemyToSpawn)
        {
            case 1:
                enemy = _enemyPool[0].Create();
                enemy.OnDestroyed += Enemy1Death;
                break;

            case 2:
                enemy = _enemyPool[1].Create();
                enemy.OnDestroyed += Enemy2Death;
                break;

            case 3:
                enemy = _enemyPool[2].Create();
                enemy.OnDestroyed += Enemy3Death;
                break;
        }
    }

    private void Enemy1Death(EnemyBase enemyToReturn)
    {
        enemyToReturn.OnDestroyed -= Enemy1Death;
        _enemyPool[0].Return(enemyToReturn);
        _activeEnemies--;
    }
    private void Enemy2Death(EnemyBase enemyToReturn)
    {
        enemyToReturn.OnDestroyed -= Enemy2Death;
        _enemyPool[1].Return(enemyToReturn);
        _activeEnemies--;
    }
    private void Enemy3Death(EnemyBase enemyToReturn)
    {
        enemyToReturn.OnDestroyed -= Enemy3Death;
        _enemyPool[2].Return(enemyToReturn);
        _activeEnemies--;
    }

    private void Update()
    {
        if (_activeEnemies == 0)
            SpawnNextWave();
    }
}

[System.Serializable]
public struct Wave
{
    [Tooltip("Number is based on the enemy identifier!")]
    [SerializeField] public int[] enemyPrefabs;
    [SerializeField] public float delayBetweenSpawns;
}