using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
    [SerializeField] protected T[] prefabsToPool;
    [SerializeField] protected int poolSize = 10;
    //[SerializeField] protected bool isExpandable = false;

    protected List<T> usedList;
    protected List<T> freeList;

    protected Transform poolerTransform;
    protected override void Awake()
    {
        base.Awake();
        poolerTransform = transform;

        usedList = new List<T>(poolSize);
        freeList = new List<T>(poolSize);

        InitializeFreeList();
    }
    protected virtual void InitializeFreeList()
    {
        for (int j = 0; j < prefabsToPool.Length; j++)
        {
            T pooledObject;
            for (int i = 0; i < poolSize; i++)
            {
                pooledObject = Instantiate(prefabsToPool[j], transform);
                pooledObject.gameObject.SetActive(false);
                freeList.Add(pooledObject);
            }
        }
    }
    public virtual T GetObject()
    {
        int freeCount = freeList.Count;
        if (freeCount == 0) return null;

        T pooledObject = freeList[freeCount - 1];
        freeList.RemoveAt(freeCount - 1);
        usedList.Add(pooledObject);
        return pooledObject;
    }
    public virtual void ReturnObject(T pooledObject)
    {
        if (usedList.Contains(pooledObject))
        {
            usedList.Remove(pooledObject);

            Transform tr = pooledObject.transform;
            tr.SetParent(poolerTransform);
            tr.localPosition = Vector3.zero;
            pooledObject.gameObject.SetActive(false);

            freeList.Add(pooledObject);
        }
    }
}
