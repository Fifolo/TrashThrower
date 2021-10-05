using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Trash : MonoBehaviour
{
    [SerializeField] private TrashType trashType = TrashType.Different;

    public TrashType Trash_Type { get { return trashType; } }

    private Rigidbody trashRigidbody;

    public enum TrashType
    {
        Plastic,
        Metal,
        Paper,
        Glass,
        Different
    }
    private void Awake() => trashRigidbody = GetComponent<Rigidbody>();
    public float GetTrashMass() => trashRigidbody.mass;
}
