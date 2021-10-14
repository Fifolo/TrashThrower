using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Trash : MonoBehaviour
{
    [SerializeField] private TrashType trashType = TrashType.Different;
    [SerializeField] private float trashContamination = 1f;
    [SerializeField] private GameObject trashParticles;

    public delegate void ContaminationEvent(float contamination);
    public static event ContaminationEvent OnCurrentContaminationChange;
    public static event ContaminationEvent OnMaxContaminationReach;
    public static event ContaminationEvent OnMaxContaminationDrop;

    public static float CurrentContamination = 0f;
    public static float MaxContamination = 1000f;

    public delegate void TrashEvent(float trashPollution);
    public float TrashContamintation { get { return trashContamination; } }
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
    private void Start() => trashRigidbody.AddTorque(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)), ForceMode.Impulse);
    public float GetTrashMass() => trashRigidbody.mass;
    private bool firstEnable = true;
    private void OnEnable()
    {
        if (firstEnable)
        {
            if (trashParticles != null) Instantiate(trashParticles, transform);
            firstEnable = false;
            return;
        }
        CurrentContamination += trashContamination;
        OnCurrentContaminationChange?.Invoke(CurrentContamination);
        if (CurrentContamination >= MaxContamination) OnMaxContaminationReach?.Invoke(MaxContamination);
    }
    private bool firstDisable = true;
    private void OnDisable()
    {
        if (firstDisable)
        {
            firstDisable = false;
            return;
        }
        if (CurrentContamination >= MaxContamination)
        {
            if ((CurrentContamination - trashContamination) < MaxContamination)
            {
                OnMaxContaminationDrop?.Invoke(CurrentContamination);
            }
        }
        CurrentContamination -= trashContamination;
        OnCurrentContaminationChange?.Invoke(CurrentContamination);
    }
}
