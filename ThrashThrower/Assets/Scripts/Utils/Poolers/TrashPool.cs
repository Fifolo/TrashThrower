using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPool : ObjectPool<Trash>
{
    public override Trash GetObject()
    {
        int freeCount = freeList.Count;
        if (freeCount == 0) return null;

        int randomIndex = Random.Range(0, freeCount);
        Trash pooledObject = freeList[randomIndex];
        freeList.RemoveAt(randomIndex);
        usedList.Add(pooledObject);
        return pooledObject;
    }
    public Trash GetTrashOfType(Trash.TrashType trashType)
    {
        if (freeList.Count == 0) return null;

        for (int i = freeList.Count - 1; i >= 0; i--)
        {
            Trash trash = freeList[i];

            if (trash.Trash_Type == trashType)
            {
                Trash correctTrash = trash;
                freeList.RemoveAt(i);
                usedList.Add(correctTrash);
                return correctTrash;
            }
        }
        return null;
    }
}
