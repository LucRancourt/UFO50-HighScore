using _Project.Code.Core.Events;
using _Project.Code.Core.Factory;
using _Project.Code.Gameplay.GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    private List<PooledFactory<EnemyBase>> _enemyPool = new List<PooledFactory<EnemyBase>>();
    [Tooltip("Keep them in order!")]
    [SerializeField] private EnemyBase[] allEnemyPrefabs;
    [SerializeField] private float startDelay = 3.0f;
    [SerializeField] private float waveDelay = 3.0f;
    
    private int _waveIndex;
    private int _activeEnemies = -1;

    private void Start()
    {
        if (waves.Length == 0)
        {
            Debug.Log("Forgot Spawner List!");
            return;
        }

        EventBus.Instance?.Subscribe<GameStateChangedEvent>(this, Reset);
    }

    public void Reset(GameStateChangedEvent state)
    {
        _activeEnemies = -1;

        foreach (PooledFactory<EnemyBase> pool in _enemyPool)
        {
            pool.Clear();
        }

        _enemyPool.Clear();

        _waveIndex = 0;

        CancelInvoke();
        StopAllCoroutines();

        if (state.StateName != "Gameplay") return;

        foreach (EnemyBase enemy in allEnemyPrefabs)
        {
            _enemyPool.Add(new PooledFactory<EnemyBase>(enemy));
        }

        Invoke("SpawnNextWave", startDelay);
    }

    public void SpawnNextWave()
    {
        if (_waveIndex >= waves.Length)
        {
            EventBus.Instance.Publish(new GameWaveCleared());
            return;
        }

        // spawn wave
        for (int i = 0; i < waves[_waveIndex].enemyPrefabs.Length; i++)
        {
            StartCoroutine(SpawnNextEnemy(waves[_waveIndex].enemyPrefabs[i], waves[_waveIndex].splineToFollowIsOnLeft[i], waves[_waveIndex].color, (waves[_waveIndex].delayBetweenSpawns * i)));
        }

        _activeEnemies = waves[_waveIndex].enemyPrefabs.Length;

        _waveIndex++;
    }

    private IEnumerator SpawnNextEnemy(int enemyToSpawn, bool splineToFollowIsLeft, EColor color, float delay)
    {
        yield return new WaitForSeconds(delay);

        EnemyBase enemy;

        switch (enemyToSpawn)
        {
            case 1:
                enemy = _enemyPool[0].Create();
                enemy.SetSplinePath(splineToFollowIsLeft);
                enemy.ColorSwitch(color);
                enemy.OnDestroyed += Enemy1Death;
                break;

            case 2:
                enemy = _enemyPool[1].Create();
                enemy.SetSplinePath(splineToFollowIsLeft);
                enemy.ColorSwitch(color);
                enemy.OnDestroyed += Enemy2Death;
                break;

            case 3:
                enemy = _enemyPool[2].Create();
                enemy.SetSplinePath(splineToFollowIsLeft);
                enemy.ColorSwitch(color);
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
        {
            Invoke("SpawnNextWave", waveDelay);
            _activeEnemies = -1;
        }
    }
}

[System.Serializable]
public struct Wave
{
    [Tooltip("Number is based on the enemy identifier!")]
    [SerializeField, Range(1,3)] public int[] enemyPrefabs;
    [SerializeField] public bool[] splineToFollowIsOnLeft;
    [SerializeField] public EColor color;
    [SerializeField] public float delayBetweenSpawns;
}



