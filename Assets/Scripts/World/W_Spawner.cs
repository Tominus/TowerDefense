using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class W_Spawner : MonoBehaviour
{
    [SerializeField] W_Path path = null;
    [SerializeField] float fPathSize = 1.0f;
    [SerializeField] bool bIsWaitingForNextWave = false;
    [SerializeField] int iWaveIndex = 0;
    [SerializeField] int iEnemyIndex = 0;
    [SerializeField] float fCurrentWaitTime = 0f;
    [SerializeField] float fTimeToWait = 1f;
    [SerializeField] List<SSpawner_Wave> allWaves = new List<SSpawner_Wave>();
    [SerializeField] bool bIsAutoSpawner = true;

    IA_Manager iaManager = null;

    private void Start()
    {
        if (!path)
        {
            Debug.LogError("missing path in spawner");
            return;
        }
        iaManager = IA_Manager.Instance;
        if (!iaManager)
        {
            Debug.LogError("missing ia manager in spawner");
            return;
        }
    }

    public void TickWave(float _deltaTime)
    {
        if (bIsAutoSpawner)
            CalculateWaveTime(_deltaTime);
        else
            SpawnManuallyEnemy();
    }

    private void CalculateWaveTime(float _deltaTime)
    {
        fCurrentWaitTime += _deltaTime;
        if (fCurrentWaitTime > fTimeToWait)
        {
            if (bIsWaitingForNextWave)
            {
                if (++iWaveIndex >= allWaves.Count)
                {
                    iWaveIndex = 0;
                }

                bIsWaitingForNextWave = false;

                SpawnWave();
            }
            else
            {
                SpawnWave();
            }

            fCurrentWaitTime = 0f;
        }
    }

    private void SpawnWave()
    {
        SSpawner_Wave _currentWave = allWaves[iWaveIndex];
        List<SSpawner_WaveData> _waves = _currentWave.waves;

        bool _canSpawnNextWave = false;

        int _waveQty = _waves.Count;
        for (int i = 0; i < _waveQty; ++i)
        {
            SSpawner_WaveData _currentData = _waves[i];

            if (iEnemyIndex < _currentData.iEnemyQuantity)
            {
                float _perpendicularOffset = Random.Range(-fPathSize, fPathSize);

                IA_Enemy _enemy = Instantiate(_currentData.enemyPrefab, path.StartPosition + path.StartDirection * _perpendicularOffset, Quaternion.identity, transform);

                _enemy.SetPath(path);
                _enemy.SetPerpendicularOffset(_perpendicularOffset);
                iaManager.AddIA(_enemy);
            }

            //Check if he can spawn enemy on the next spawning
            if (iEnemyIndex + 1 < _currentData.iEnemyQuantity)
            {
                _canSpawnNextWave = true;
            }
        }

        if (_canSpawnNextWave)
        {
            fTimeToWait = _currentWave.fSpawnRate;
            ++iEnemyIndex;
        }
        else
        {
            bIsWaitingForNextWave = true;
            fTimeToWait = _currentWave.fWaitTimeAfterFinishSpawn;
            iEnemyIndex = 0;
        }
    }

    private void SpawnManuallyEnemy()
    {
        if (!Input.GetButtonDown("Submit")) return;

        float _perpendicularOffset = Random.Range(-fPathSize, fPathSize);

        IA_Enemy _enemy = Instantiate(allWaves[0].waves[0].enemyPrefab, path.StartPosition + path.StartDirection * _perpendicularOffset, Quaternion.identity, transform);

        _enemy.SetPath(path);
        _enemy.SetPerpendicularOffset(_perpendicularOffset);
        iaManager.AddIA(_enemy);
    }
}

[Serializable]
public struct SSpawner_Wave
{
    public List<SSpawner_WaveData> waves;
    public float fSpawnRate;
    public float fWaitTimeAfterFinishSpawn;

    public SSpawner_Wave(List<SSpawner_WaveData> _waves, float _spawnRate, float _waitTimeAfterFinishSpawn)
    {
        waves = _waves;
        fSpawnRate = _spawnRate;
        fWaitTimeAfterFinishSpawn = _waitTimeAfterFinishSpawn;
    }
}

[Serializable]
public struct SSpawner_WaveData
{
    public IA_Enemy enemyPrefab;
    public int iEnemyQuantity;

    public SSpawner_WaveData(IA_Enemy _enemyPrefab, int _enemyQuantity)
    {
        enemyPrefab = _enemyPrefab;
        iEnemyQuantity = _enemyQuantity;
    }
}