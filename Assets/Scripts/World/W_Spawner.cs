using System;
using System.Collections.Generic;
using UnityEngine;

public class W_Spawner : MonoBehaviour
{
    [SerializeField] W_Path path = null;
    [SerializeField] bool bIsWaitingForNextWave = false;
    [SerializeField] int iWaveIndex = 0;
    [SerializeField] int iEnemyIndex = 0;
    [SerializeField] float fCurrentWaitTime = 0f;
    [SerializeField] float fTimeToWait = 1f;
    [SerializeField] List<SSpawner_Wave> allWaves = new List<SSpawner_Wave>();

    private void Start()
    {
        if (!path)
        {
            Debug.LogError("missing path in spawner");
            return;
        }
    }

    private void Update()
    {
        TickWave(Time.deltaTime);
    }

    private void TickWave(float _deltaTime)
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
                IA_Enemy _enemy = Instantiate(_currentData.enemyPrefab, path.transform.position, Quaternion.identity, transform);
                _enemy.SetPath(path);
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
}

[Serializable]
public struct SSpawner_Wave
{
    public List<SSpawner_WaveData> waves;
    public float fSpawnRate;
    public float fWaitTimeAfterFinishSpawn;
}

[Serializable]
public struct SSpawner_WaveData
{
    public IA_Enemy enemyPrefab;
    public int iEnemyQuantity;
}