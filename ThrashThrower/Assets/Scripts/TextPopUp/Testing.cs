using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform dmgPopUp;
    [SerializeField] private Transform target;
    [SerializeField] private Transform player;
    [SerializeField] private float maxRange = 100f;
    [SerializeField] private AnimationCurve fontSizeCurve;
    private float minFontSize = 20f;
    private float maxFontSize = 40f;
    private Camera playerCamera;
    private float range = 0.5f;
    private void Awake()
    {
        playerCamera = Camera.main;
    }
    private void Start()
    {
        DamagePopUp damagePopUp = Instantiate(dmgPopUp, DamagePopUpCanvas.Instance.transform).GetComponent<DamagePopUp>();
        damagePopUp.transform.position = playerCamera.WorldToScreenPoint(target.position + new Vector3(
            Random.Range(-range, range),
            Random.Range(-range, range), 0));

        float fontSizeMultiplier = fontSizeCurve.Evaluate(Vector3.Distance(player.position, target.position) / maxRange);
        int fontSize = (int)(maxFontSize * fontSizeMultiplier);
        if (fontSize < minFontSize) fontSize = (int)minFontSize;

        Debug.Log($"font size = {fontSize}");

        damagePopUp.SetUp(300, fontSize, Color.white);
    }
}
