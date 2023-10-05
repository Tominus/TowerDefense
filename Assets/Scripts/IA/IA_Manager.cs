using System.Collections.Generic;
using UnityEngine;

public class IA_Manager : M_Singleton<IA_Manager>
{
    [SerializeField] private List<IA_Base> allIA = new List<IA_Base>();

    public void AddIA(IA_Base _ia)
    {
        allIA.Add(_ia);
    }

    public void RemoveIA(IA_Base _ia)
    {  
        allIA.Remove(_ia);
    }

    public void Tick(float _deltaTime)
    {
        int _iaCount = allIA.Count;

        for (int i = 0; i < _iaCount; ++i)
        {
            IA_Base _iaBase = allIA[i];
            _iaBase.Tick(_deltaTime);

            if (_iaBase.IsIADestroyed)
            {
                allIA.RemoveAt(i);
                --i;
                --_iaCount;
            }
        }
    }
}