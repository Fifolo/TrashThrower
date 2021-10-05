using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DisallowMultipleComponent]
public class TrashPicker : MonoBehaviour
{
    #region Editor Exposed Fields
    [SerializeField] private float maxWeight = 100f;
    [Range(1f, 10f)]
    [SerializeField] private float pickupRadius = 10f;
    [Range(0.05f, 2f)]
    [SerializeField] private float moveSlowPerKg = 0.1f;
    #endregion

    #region Events
    public delegate void TrashPickerAction(Trash.TrashType thrashType);
    public event TrashPickerAction OnThrashPickUp;
    public event TrashPickerAction OnThrashRemove;
    #endregion

    #region Instance Variables
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
    public float CurrentBagWeight { get { return currentBagWeight; } }
    private Transform thrashPickerTransform;
    private CharacterMovement characterMovement;
    private List<Trash> thrashBag;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        thrashBag = new List<Trash>();
        thrashPickerTransform = transform;
        characterMovement = GetComponentInParent<CharacterMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //is it thrash?
        if (other.transform.TryGetComponent<Trash>(out Trash pickedUpThrash)) TryAddThrashToBag(pickedUpThrash);

        //is it thrash can?
        else if (other.transform.TryGetComponent<TrashCan>(out TrashCan thrashCan))
        {
            //do you have any thrash of that type?
            Trash.TrashType thrashCanType = thrashCan.TrashCanType;
            if (HasAnyThrashOfType(thrashCanType)) PlaceAllThrashInCan(thrashCanType, thrashCan.transform);

            //else Debug.Log($"You dont have thrash of type {thrashCanType}");
        }
    }
    #endregion

    #region Methods
    private void PlaceAllThrashInCan(Trash.TrashType thrashType, Transform canTransform)
    {
        TrashCan thrashCan = canTransform.GetComponent<TrashCan>();
        List<Trash> thrashOfType = thrashBag.Where(t => t.Trash_Type == thrashType).ToList();

        foreach (Trash thrash in thrashOfType)
        {
            RemoveThrashFromBag(thrash);
            thrashCan.AddTrashToCan(thrash);
        }
    }
    private bool HasAnyThrashOfType(Trash.TrashType thrashType) => thrashBag.Any(t => t.Trash_Type == thrashType);
    private void TryAddThrashToBag(Trash thrashToAdd)
    {
        //can you carry additional thrash?
        if (thrashToAdd.GetTrashMass() + currentBagWeight <= maxWeight)
        {
            thrashToAdd.gameObject.SetActive(false);
            thrashToAdd.transform.SetParent(thrashPickerTransform);
            thrashBag.Add(thrashToAdd);
            currentBagWeight += thrashToAdd.GetTrashMass();
            characterMovement.MovementSpeed -= moveSlowPerKg * thrashToAdd.GetTrashMass();
            OnThrashPickUp?.Invoke(thrashToAdd.Trash_Type);
        }
        //Debug.Log($"picked up {thrashToAdd.name}");
    }
    private void RemoveThrashFromBag(Trash thrashToRemove)
    {
        //remove thrash from bag if exists
        if (thrashBag.Contains(thrashToRemove))
        {
            thrashBag.Remove(thrashToRemove);
            currentBagWeight -= thrashToRemove.GetTrashMass();
            characterMovement.MovementSpeed += moveSlowPerKg * thrashToRemove.GetTrashMass();
            OnThrashRemove?.Invoke(thrashToRemove.Trash_Type);
        }
    }
    public int GetAmountFromBag(Trash.TrashType thrashType) => thrashBag.Where(t => t.Trash_Type == thrashType).Count();
    #endregion
}
