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
    private void Start() => trashRigidbody.AddTorque(new Vector3(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f)), ForceMode.Impulse);
    public float GetTrashMass() => trashRigidbody.mass;
}
