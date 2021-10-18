using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class DamagePopUp : MonoBehaviour
{
    private const float V = 0.3f;
    private static float gettingBigger = 0.5f;
    private static float gettingSmaller = 0.5f;
    private TMP_Text dmgText;
    private Transform textTransform;
    private Transform playerCamera;
    private void Awake()
    {
        dmgText = GetComponent<TMP_Text>();
        textTransform = transform;
    }
    private void Start()
    {
        playerCamera = PlayerMovement.Instance.transform;
    }
    public void SetUp(int damage, int fontSize, Color textColor)
    {
        dmgText.color = textColor;
        dmgText.fontSize = fontSize;
        dmgText.SetText(damage.ToString());

        StartCoroutine(TextAnimation());
    }
    private void Update()
    {
        //go up
        textTransform.position += Vector3.up * Time.deltaTime;
        //rotate towards player
        textTransform.forward = playerCamera.forward;
    }
    private IEnumerator TextAnimation()
    {
        float time = 0;
        float startFontSize = dmgText.fontSize;
        while (time < gettingBigger)
        {
            dmgText.fontSize = Mathf.Lerp(startFontSize, startFontSize + startFontSize * V, time / gettingBigger);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        startFontSize = dmgText.fontSize;
        while (time < gettingSmaller)
        {
            dmgText.fontSize = Mathf.Lerp(startFontSize, startFontSize * (2*V), time / gettingSmaller);
            time += Time.deltaTime;
            yield return null;
        }
        TextPooler.Instance.ReturnObject(this);
    }
}
