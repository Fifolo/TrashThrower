using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
    [SerializeField] protected bool usePrefabPools = false;
    [SerializeField] protected List<PrefabPool> prefabPools;

    [Space][Space]

    [SerializeField] protected List<T> sameAmountPrefabs;
    [SerializeField] protected int amount = 10;

    protected List<T> usedList;
    protected List<T> freeList;

    protected Transform poolerTransform;

    [System.Serializable]
    protected class PrefabPool
    {
        [SerializeField] public T prefab;
        [SerializeField] public int amount = 5;
    }
    protected override void Awake()
    {
        base.Awake();
        poolerTransform = transform;
        int size = 0;

        if (usePrefabPools) size = prefabPools.Sum(p => p.amount);
        else size = sameAmountPrefabs.Count;

        if (size > 0)
        {
            usedList = new List<T>(size);
            freeList = new List<T>(size);

            if (usePrefabPools) InitializeWithPrefabPools();
            else InitializeWithList();
        }
        else Debug.Log($"{name}, no prefabs assigned");
    }
    protected virtual void InitializeWithList()
    {
        for (int j = 0; j < sameAmountPrefabs.Count; j++)
        {
            T pooledObject;
            for (int i = 0; i < amount; i++)
            {
                pooledObject = Instantiate(sameAmountPrefabs[j], poolerTransform);
                pooledObject.gameObject.SetActive(false);
                freeList.Add(pooledObject);
            }
        }
    }
    protected virtual void InitializeWithPrefabPools()
    {
        for (int j = 0; j < prefabPools.Count; j++)
        {
            T pooledObject;
            for (int i = 0; i < prefabPools[j].amount; i++)
            {
                pooledObject = Instantiate(prefabPools[j].prefab, transform);
                pooledObject.gameObject.SetActive(false);
                freeList.Add(pooledObject);
            }
        }
    }

    public virtual T GetRandomObject()
    {
        int freeCount = freeList.Count;
        if (freeCount == 0) return null;

        int randomIndex = Random.Range(0, freeCount - 1);

        T pooledObject = freeList[randomIndex];
        freeList.RemoveAt(randomIndex);
        usedList.Add(pooledObject);
        return pooledObject;
    }
    public virtual T GetLastObject()
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
