using UnityEngine;

public class PROTO_RallyPoint : MonoBehaviour
{
    [SerializeField] bool bEventChangePosition = false;
    [SerializeField] IA_Ally_Guard[] debugGuards = null;
    [SerializeField] IA_Ally_Guard[] allGuards = null;
    [SerializeField] int iMaxGuards = 3;
    [SerializeField] float fRallyRadius = 0.5f;
    [SerializeField] float fRallyPointEnemyRange = 1.5f;

    private void Start()
    {
        allGuards = new IA_Ally_Guard[iMaxGuards];

        int _count = debugGuards.Length;
        for (int i = 0; i < _count; ++i)
            AddGuard(debugGuards[i]);
    }
    private void Update()
    {
        if (bEventChangePosition)
        {
            ForceAllGuardsToRallyPoint();
            bEventChangePosition = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fRallyPointEnemyRange);
    }

    // Force all guards to go around the rally point
    // Called when player move the rally point position
    private void ForceAllGuardsToRallyPoint()
    {
        float _radToAdd = Mathf.Deg2Rad * (360f / iMaxGuards);
        float _currentRad = 0f;

        for (int i = 0; i < iMaxGuards; ++i)
        {
            Vector3 _guardPosition = new Vector3(Mathf.Cos(_currentRad) * fRallyRadius, Mathf.Sin(_currentRad) * fRallyRadius);

            allGuards[i].ForceMoveToCheckPoint(_guardPosition + transform.position);
            _currentRad += _radToAdd;
        }
    }

    // Force a new spawned guards to join his position
    private void ForceGuardToRallyPoint(int _guardArrayPosition)
    {
        float _radToAdd = Mathf.Deg2Rad * (360f / iMaxGuards) * _guardArrayPosition;

        Vector3 _guardPosition = new Vector3(Mathf.Cos(_radToAdd) * fRallyRadius, Mathf.Sin(_radToAdd) * fRallyRadius);
        IA_Ally_Guard _guard = allGuards[_guardArrayPosition];
        _guard.ForceMoveToCheckPoint(_guardPosition + transform.position);
        _guard.SetRallyPointEnemyRange(fRallyPointEnemyRange, transform.position);
    }

    // Add guard spawned by the Caserne
    public void AddGuard(IA_Ally_Guard _guard)
    {
        for (int i = 0; i < iMaxGuards; ++i)
        {
            if (!allGuards[i])
            {
                _guard.OnDestroyIA += RemoveGuard;
                allGuards[i] = _guard;
                ForceGuardToRallyPoint(i);
                return;
            }
        }

        Debug.LogError("PROTO_RallyPoint::AddGuard -> No empty space for new Guard");
    }
    private void RemoveGuard(IA_Base _ally)
    {
        IA_Ally_Guard _guard = (IA_Ally_Guard)_ally;

        for (int i = 0; i < iMaxGuards; ++i)
        {
            if (allGuards[i] == _guard)
            {
                _guard.OnDestroyIA -= RemoveGuard;
                allGuards[i] = null;
                return;
            }
        }

        Debug.LogError("PROTO_RallyPoint::RemoveGuard -> Guard was not found");
    }
}