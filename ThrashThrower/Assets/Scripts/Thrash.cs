using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Thrash : MonoBehaviour
{
    [SerializeField] private ThrashType thrashType = ThrashType.Different;

    public ThrashType Thrash_Type { get { return thrashType; } }

    private Rigidbody thrashRigidbody;

    public enum ThrashType
    {
        Plastic,
        Metal,
        Paper,
        Glass,
        Different
    }
    private void Awake() => thrashRigidbody = GetComponent<Rigidbody>();
    public float GetThrashMass() => thrashRigidbody.mass;
}
