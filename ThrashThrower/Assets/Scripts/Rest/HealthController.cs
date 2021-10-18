using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthController : MonoBehaviour, IDamagable
{
    [SerializeField] private Volume healthVolume;
    [Range(0f, 1f)]
    [SerializeField] private float maxVignetteIntensity = 0.6f;
    [Range(0f, 0.5f)]
    [SerializeField] private float minVignetteIntensity = 0.147f;
    [Range(2f, 0.1f)]
    [SerializeField] private float PULSE_SPEED = 0.7f;
    [Range(0f,0.5f)]
    [SerializeField] private float hearthPulseTreshold = 0.3f;

    public delegate void PlayerHealthEvent(float damage);
    public static event PlayerHealthEvent OnPlayerDeath;
    public static event PlayerHealthEvent OnPlayerDamageTaken;

    private static float ONE_SECOND = 1f;

    private float maxHealth = 100f;
    private float currentHealth = 0f;
    private float healthRegenPerSecond = 1f;
    private float autoRegenWaitTime = 5f;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private IEnumerator vignetteCoroutine;
    private IEnumerator autoRegenCoroutine;
    float vignetteLerpDuration = 1f;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        //health
        PlayerData playerData = transform.GetComponent<PlayerMovement>().PlayerData;
        maxHealth = playerData.baseHealth;
        healthRegenPerSecond = playerData.baseHealthRegenPerSecond;
        autoRegenWaitTime = playerData.autoRegenWaitTime;
        currentHealth = maxHealth;

        //post proccesing
        if (healthVolume == null) Debug.LogError("Health Volume for player not set!");
        else
        {
            if (healthVolume.profile.TryGet(out vignette))
            {
                vignette.smoothness.value = vignette.smoothness.max;
                vignette.intensity.value = minVignetteIntensity;
            }
            if (healthVolume.profile.TryGet(out chromaticAberration))
            {
                chromaticAberration.intensity.value = chromaticAberration.intensity.min;
            }
        }
    }
    private IEnumerator AutomaticRegeneration(float waitTime)
    {
        //wait till right amount of time passed
        float timePassed = 0;
        while (timePassed < waitTime)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        //if waitTime seconds passed, start regenerating health
        float t = 0f;
        float startHealthValue = currentHealth;
        float endValue;
        //regen health over time
        while (currentHealth < maxHealth)
        {

            //starting vignette coroutine
            endValue = VignetteValueFromHeal(healthRegenPerSecond);
            vignetteCoroutine = VignetteIntensityCoroutine(endValue < 0 ? 0 : endValue, 1f);
            StartCoroutine(vignetteCoroutine);

            //regenerating over 1 second
            t = 0f;
            startHealthValue = currentHealth;
            while (t < ONE_SECOND)
            {
                currentHealth = Mathf.Lerp(startHealthValue, startHealthValue + healthRegenPerSecond, t / ONE_SECOND);
                t += Time.deltaTime;

                if (currentHealth >= maxHealth) break;
                yield return null;
            }
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {

        if (vignetteCoroutine != null) StopCoroutine(vignetteCoroutine);
        if (autoRegenCoroutine != null) StopCoroutine(autoRegenCoroutine);

        //start vignette coroutine
        float endValue = VignetteValueFromDamage(damageAmount);
        vignetteCoroutine = VignetteIntensityCoroutine(endValue, autoRegenWaitTime);
        StartCoroutine(vignetteCoroutine);

        //start auto regen timer with standard wait time
        autoRegenCoroutine = AutomaticRegeneration(autoRegenWaitTime);
        StartCoroutine(autoRegenCoroutine);

        currentHealth -= damageAmount;

        OnPlayerDamageTaken(damageAmount);

        if(currentHealth <= 0) OnPlayerDeath?.Invoke(0);

        if (currentHealth/maxHealth <= hearthPulseTreshold && !heartPulseActive) StartCoroutine(HeartPulse());
    }
    public void Heal(float healAmount)
    {
        if (vignetteCoroutine != null) StopCoroutine(vignetteCoroutine);
        if (autoRegenCoroutine != null) StopCoroutine(autoRegenCoroutine);

        //start vignette coroutine
        float endValue = VignetteValueFromHeal(healAmount);
        vignetteCoroutine = VignetteIntensityCoroutine(endValue, vignetteLerpDuration);
        StartCoroutine(vignetteCoroutine);

        //start auto regen timer with vignette lerp duration
        //this makes is that auto regeneration runs as sun as heal vignete ends
        autoRegenCoroutine = AutomaticRegeneration(vignetteLerpDuration);
        StartCoroutine(autoRegenCoroutine);

        currentHealth += healAmount;
    }
    private float VignetteValueFromHeal(float value) => (maxHealth - (currentHealth + value)) / maxHealth;
    private float VignetteValueFromDamage(float damage) => (maxHealth - (currentHealth - damage)) / maxHealth;

    private IEnumerator VignetteIntensityCoroutine(float endValue, float duration)
    {
        if (endValue >= maxVignetteIntensity) endValue = maxVignetteIntensity;
        else if (endValue < minVignetteIntensity) endValue = minVignetteIntensity;

        float startValue = vignette.intensity.value;

        float t = 0f;
        while (t < duration)
        {
            vignette.intensity.value = Mathf.Lerp(startValue, endValue, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.value = endValue;
    }
    private bool heartPulseActive = false;
    private IEnumerator HeartPulse()
    {
        heartPulseActive = true;
        float time = 0;

        //hearth pulse
        while (currentHealth/maxHealth <= hearthPulseTreshold)
        {
            time = 0;
            while (time < (PULSE_SPEED / 2))
            {
                chromaticAberration.intensity.value = Mathf.Lerp(0f, 1f, time / (PULSE_SPEED / 2));
                time += Time.deltaTime;
                yield return null;
            }

            time = 0;
            while (time < (PULSE_SPEED / 2))
            {
                chromaticAberration.intensity.value = Mathf.Lerp(1f, 0f, time / (PULSE_SPEED / 2));
                time += Time.deltaTime;
                yield return null;
            }
        }
        //slowly reduce effect
        time = 0;
        float startValue = chromaticAberration.intensity.value;
        while (time < PULSE_SPEED)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(startValue, 0f, time / PULSE_SPEED);
            time += Time.deltaTime;
            yield return null;
        }
        chromaticAberration.intensity.value = chromaticAberration.intensity.min;
        heartPulseActive = false;
    }
}
