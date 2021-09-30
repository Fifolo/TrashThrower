using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

[DisallowMultipleComponent]
public class ThrashPicker : MonoBehaviour
{
    public event Action<Thrash.ThrashType> OnThrashPickUp;
    public event Action<Thrash.ThrashType> OnThrashRemove;

    [SerializeField] private float maxWeight = 100f;
    [Range(1f, 10f)]
    [SerializeField] private float pickupRadius = 10f;
    public float PickUpRadius
    {
        get { return pickupRadius; }
        set
        {
            if (value > 0 && value < 40) pickupRadius = value;
            else Debug.LogWarning($"pickupRadius not set, incorrect value of {value}");
        }
    }

    private float currentBagWeight = 0f;
    private Transform thrashPickerTransform;

    private List<Thrash> thrashBag;
    private void Awake()
    {
        thrashBag = new List<Thrash>();
        thrashPickerTransform = transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        //is it thrash?
        if (other.transform.TryGetComponent<Thrash>(out Thrash pickedUpThrash))
        {
            //can you carry that much?
            if (pickedUpThrash.GetThrashMass() + currentBagWeight <= maxWeight)
            {
                pickedUpThrash.gameObject.SetActive(false);
                pickedUpThrash.transform.SetParent(thrashPickerTransform);
                AddThrashToBag(pickedUpThrash);
            }
            else
            {
                Debug.Log("Cant carry that much");
            }
        }
        //is it thrash can?
        else if (other.transform.TryGetComponent<ThrashCan>(out ThrashCan thrashCan))
        {
            //do you have any thrash of that type?
            Thrash.ThrashType thrashCanType = thrashCan.ThrashCanType;
            if (HasAnyThrashOfType(thrashCanType)) PlaceAllThrashInCan(thrashCanType, thrashCan.transform);
            else
            {
                Debug.Log($"You dont have thrash of type {thrashCanType}");
            }
        }
    }
    private void PlaceAllThrashInCan(Thrash.ThrashType thrashType, Transform canTransform)
    {
        ThrashCan thrashCan = canTransform.GetComponent<ThrashCan>();
        List<Thrash> thrashOfType = thrashBag.Where(t => t.Thrash_Type == thrashType).ToList();

        foreach(Thrash thrash in thrashOfType)
        {
            RemoveThrashFromBag(thrash);
            thrashCan.AddThrashToCan(thrash);
        }
    }
    private bool HasAnyThrashOfType(Thrash.ThrashType thrashType) => thrashBag.Any(t => t.Thrash_Type == thrashType);
    private void AddThrashToBag(Thrash thrashToAdd)
    {
        thrashBag.Add(thrashToAdd);
        OnThrashPickUp?.Invoke(thrashToAdd.Thrash_Type);
        currentBagWeight += thrashToAdd.GetThrashMass();
        Debug.Log($"picked up {thrashToAdd.name}");
    }
    private void RemoveThrashFromBag(Thrash thrashToRemove)
    {
        //remove thrash from bag if exists
        if (thrashBag.Contains(thrashToRemove))
        {
            thrashBag.Remove(thrashToRemove);
            currentBagWeight -= thrashToRemove.GetThrashMass();
            OnThrashRemove?.Invoke(thrashToRemove.Thrash_Type);
        }
    }

}
