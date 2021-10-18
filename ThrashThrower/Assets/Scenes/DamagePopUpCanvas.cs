using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopUpCanvas : Singleton<DamagePopUpCanvas>
{
    private static Camera PLAYER_CAMERA;

    [SerializeField] private Transform dmgPopUp;
    private Transform canvasTransform;

    //based on distance from player
    [SerializeField] private AnimationCurve fontSizeCurve;
    private float minFontSize = 5f;
    private float maxFontSize = 7f;
    private float maxDistance = 50f;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color critColor;

    private float offset = 0.3f;

    protected override void Awake()
    {
        base.Awake();
        canvasTransform = transform;
    }
    private void Start()
    {
        PLAYER_CAMERA = PlayerMovement.Instance.GetComponentInChildren<Camera>();
    }

    public void SpawnText(float damageDealt, bool wasCrit, float distance, Vector3 receiverPosition)
    {
        DamagePopUp damagePopUp = TextPooler.Instance.GetLastObject();
        if (damagePopUp != null)
        {
            Transform damagePopUpTransform = damagePopUp.transform;
            damagePopUpTransform.transform.position = receiverPosition + GetRandomOffset();
            damagePopUpTransform.transform.rotation = Quaternion.identity;

            int fontSize = SetFontSize(distance);

            Color textColor = wasCrit ? critColor : normalColor;
            textColor.a = 255;

            damagePopUp.gameObject.SetActive(true);
            damagePopUp.SetUp((int)damageDealt, fontSize, textColor);
        }
    }
    private int SetFontSize(float distance)
    {
        float fontSizeMultiplier = fontSizeCurve.Evaluate(distance / maxDistance);
        int fontSize = (int)(maxFontSize * fontSizeMultiplier);
        if (fontSize < minFontSize) fontSize = (int)minFontSize;
        return fontSize;
    }
    private Vector3 GetRandomOffset()
    {
        return new Vector3(
                    Random.Range(-offset, offset),
                    Random.Range(-offset, offset),
                    0);
    }
}
