using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ThrashPicker))]
public class TrashPickerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI plasticText;
    [SerializeField] private TextMeshProUGUI paperText;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private TextMeshProUGUI glassText;
    [SerializeField] private TextMeshProUGUI differentText;

    private int plasticAmout = 0;
    private int paperAmout = 0;
    private int metalAmout = 0;
    private int glassAmout = 0;
    private int differentAmout = 0;

    private ThrashPicker thrashPicker;
    private void Awake()
    {
        thrashPicker = GetComponent<ThrashPicker>();
        thrashPicker.OnThrashPickUp += ThrashPicker_OnThrashPickUp;
        thrashPicker.OnThrashRemove += ThrashPicker_OnThrashRemove;
    }

    private void ThrashPicker_OnThrashRemove(Thrash.ThrashType thrashType)
    {
        switch (thrashType)
        {
            case Thrash.ThrashType.Different:
                differentAmout--;
                SetText(differentText, differentAmout);
                break;
            case Thrash.ThrashType.Plastic:
                plasticAmout--;
                SetText(plasticText, plasticAmout);
                break;
            case Thrash.ThrashType.Paper:
                paperAmout--;
                SetText(paperText, paperAmout);
                break;
            case Thrash.ThrashType.Metal:
                metalAmout--;
                SetText(metalText, metalAmout);
                break;
            case Thrash.ThrashType.Glass:
                glassAmout--;
                SetText(glassText, glassAmout);
                break;
        }
    }

    private void ThrashPicker_OnThrashPickUp(Thrash.ThrashType thrashType)
    {
        switch (thrashType)
        {
            case Thrash.ThrashType.Different:
                differentAmout++;
                SetText(differentText, differentAmout);
                break;
            case Thrash.ThrashType.Plastic:
                plasticAmout++;
                SetText(plasticText, plasticAmout);
                break;
            case Thrash.ThrashType.Paper:
                paperAmout++;
                SetText(paperText, paperAmout);
                break;
            case Thrash.ThrashType.Metal:
                metalAmout++;
                SetText(metalText, metalAmout);
                break;
            case Thrash.ThrashType.Glass:
                glassAmout++;
                SetText(glassText, glassAmout);
                break;
        }
    }
    private void OnDisable()
    {
        thrashPicker.OnThrashPickUp -= ThrashPicker_OnThrashPickUp;
        thrashPicker.OnThrashRemove -= ThrashPicker_OnThrashRemove;
    }
    private void SetText(TextMeshProUGUI tmPro, int value)
    {
        if (tmPro != null)
        {
            tmPro.text = value.ToString();
            Debug.Log($"tmpro set to {value}");
        }
    }
}
