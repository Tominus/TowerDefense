using UnityEngine;

public class M_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance = null;

    public static T Instance => instance;

    virtual protected void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this as T;
    }
}