using UnityEngine;

public class M_WorldTickManager : M_Singleton<M_WorldTickManager>
{
    [SerializeField] private IA_Manager iaManager = null;

    [SerializeField] private bool bIsGamePaused = false;


    private void Start()
    {
        if (!iaManager) Debug.LogError("IA_Manager not linked");
    }
    private void Update()
    {
        if (bIsGamePaused) return;

        float _deltaTime = Time.deltaTime;

        iaManager.Tick(_deltaTime);
    }
}