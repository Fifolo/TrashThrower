using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class TrashCan : MonoBehaviour
{
    [SerializeField] private Trash.TrashType trashCanType = Trash.TrashType.Different;
    public event Trash.TrashEvent OnTrashPutInCan;
    //public event Action OnTrashPutInCan;
    public Trash.TrashType TrashCanType { get { return trashCanType; } }
    public static int TotalThrashAmount { get; private set; }
    public int TrashInCanAmount { get; private set; }

    private Transform trashCanTransform;
    private void Awake()
    {
        trashCanTransform = transform;
    }
    public void AddTrashToCan(Trash trash)
    {
        //if thrash is same type as can => add
        if (trash.Trash_Type == trashCanType)
        {
            TotalThrashAmount += 1;
            TrashInCanAmount += 1;
            //trash.transform.SetParent(trashCanTransform);
            TrashPool.Instance.ReturnObject(trash);
            OnTrashPutInCan?.Invoke(trash.TrashContamintation);
        }
    }
}
