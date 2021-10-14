using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private string volumeParameter = "MasterVolume";
    [SerializeField] private Slider slider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier = 30f;
    [SerializeField] private Toggle muteToggle;

    private bool disableToggleEvent = false;

    private void Awake()
    {
        slider.onValueChanged.AddListener(SliderValueChanged);
        muteToggle.onValueChanged.AddListener(ToggleChanged);
    }

    private void ToggleChanged(bool enableSound)
    {
        if (disableToggleEvent) return;

        if (enableSound) slider.value = slider.maxValue;
        else slider.value = slider.minValue;
    }

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    private void SliderValueChanged(float value)
    {
        audioMixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        DealWithToggle();
    }

    private void DealWithToggle()
    {
        disableToggleEvent = true;
        muteToggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }
}
