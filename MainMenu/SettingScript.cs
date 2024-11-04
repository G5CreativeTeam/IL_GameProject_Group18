using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


public class SettingScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Dropdown dropdown;


    public AudioMixer audioMixer;

    public void Start()
    {
        
        Load();
    }
    private void OnDisable()
    {
        Save();
    }
    public void SetVolume(float value)
    {
        audioMixer.SetFloat("volume", value);
    }

    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("volume", slider.value);
        PlayerPrefs.SetInt("quality", dropdown.value);
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            slider.SetValueWithoutNotify(volume);
            SetVolume(volume);
        }
        else
        {
            slider.SetValueWithoutNotify(slider.value);
        }
        if (PlayerPrefs.HasKey("quality"))
        {
            int quality = PlayerPrefs.GetInt("quality");
            dropdown.SetValueWithoutNotify(quality);
            SetVolume(quality);
        }
        else
        {
            dropdown.SetValueWithoutNotify(dropdown.value);
        }
    }

}
