using System.Collections.Generic;
using UnityEngine;

public class P_ProjectileManager : M_Singleton<P_ProjectileManager>
{
    [SerializeField] List<P_BaseProjectile> allProjectile = new List<P_BaseProjectile>();
    
    public void Tick(float _deltaTime)
    {
        int _count = allProjectile.Count;
        for (int i = 0; i < _count; ++i)
        {
            P_BaseProjectile _projectile = allProjectile[i];
            _projectile.Tick(_deltaTime);

            if (_projectile.IsDestroyed)
            {
                allProjectile.Remove(_projectile);
                --_count;
                --i;
            }
        }
    }

    public void AddProjectile(P_BaseProjectile _baseProjectile)
    {
        allProjectile.Add(_baseProjectile);
    }
    public void RemoveProjectile(P_BaseProjectile _baseProjectile)
    {
        allProjectile.Remove(_baseProjectile);
    }
}