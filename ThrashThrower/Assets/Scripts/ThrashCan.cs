using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class ThrashCan : MonoBehaviour
{
    [SerializeField] private Thrash.ThrashType thrashCanType = Thrash.ThrashType.Different;
    [SerializeField] private TextMeshProUGUI thrashAmountText;
    public Thrash.ThrashType ThrashCanType { get { return thrashCanType; } }

    private static int totalThrashAmount = 0;
    public static int TotalThrashAmount { get { return totalThrashAmount; } }

    private int canThrashAmount = 0;
    public int ThrashAmount { get { return canThrashAmount; } }
    private Transform trashCanTransform;
    private void Awake()
    {
        trashCanTransform = transform;
        if (thrashAmountText == null) Debug.LogError($"thrashAmountText for {name} not set!");
    }
    public void AddThrashToCan(Thrash thrash)
    {
        if (thrash.Thrash_Type == thrashCanType)
        {
            canThrashAmount += 1;
            totalThrashAmount += 1;
            IncrementAmountText();
            thrash.transform.SetParent(trashCanTransform);
            //manage
        }
        else Debug.Log($"Wrong type of thrash, thrash can = {thrashCanType}, thrash = {thrash.Thrash_Type}");
    }
    private void IncrementAmountText() => thrashAmountText.text = totalThrashAmount.ToString();
}
