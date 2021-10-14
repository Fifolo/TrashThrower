using UnityEngine;

public class TrashPool : ObjectPool<Trash>
{
    public override Trash GetLastObject()
    {
        int freeCount = freeList.Count;
        if (freeCount == 0) return null;

        int randomIndex = Random.Range(0, freeCount);
        Trash trash = freeList[randomIndex];
        freeList.RemoveAt(randomIndex);
        usedList.Add(trash);
        return trash;
    }
    
    public Trash GetTrashOfType(Trash.TrashType trashType)
    {
        if (freeList.Count == 0) return null;

        for (int i = freeList.Count - 1; i >= 0; i--)
        {
            Trash trash = freeList[i];

            if (trash.Trash_Type == trashType)
            {
                freeList.RemoveAt(i);
                usedList.Add(trash);
                return trash;
            }
        }
        return null;
    }
}
