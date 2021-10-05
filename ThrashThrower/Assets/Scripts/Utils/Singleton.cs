using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null) Instance = (T)this;
        else
        {
            Debug.LogWarning($"{name} => Trying to instantiate second instance of singleton");
        }
    }
    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
