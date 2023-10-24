using System.Collections.Generic;
using UnityEngine;

public class M_WorldTickManager : M_Singleton<M_WorldTickManager>
{
    [SerializeField] private IA_Manager iaManager = null;
    [SerializeField] private P_ProjectileManager projectileManager = null;
    [SerializeField] private List<W_Spawner> allSpawners = new List<W_Spawner>();

    [SerializeField] private bool bIsGamePaused = false;


    private void Start()
    {
        if (!iaManager) Debug.LogError("IA_Manager not linked");
        if (!projectileManager) Debug.LogError("P_ProjectileManager not linked");
    }
    private void Update()
    {
        if (bIsGamePaused) return;

        float _deltaTime = Time.deltaTime;

        iaManager.Tick(_deltaTime);

        int _spawnerCount = allSpawners.Count;
        for (int i = 0; i < _spawnerCount; ++i)
        {
            allSpawners[i].TickWave(_deltaTime);
        }

        projectileManager.Tick(_deltaTime);
    }
}