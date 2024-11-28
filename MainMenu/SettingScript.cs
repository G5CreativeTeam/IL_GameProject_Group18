using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System;


public class SettingScript : MonoBehaviour
{
    public Slider masterSlider, musicSlider, sfxSlider;
    public TMP_Dropdown graphicDropdown, resolutionDropdown;
    public Toggle fullscreenToggle;


    public AudioMixer masterAudioMixer;

    private Resolution[] resolutions;

    public static SettingScript Instance;

    void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        InitiateResolution();
        Load();
    }
    //private void OnDisable()
    //{
    //    Save();
    //}
    //private void OnApplicationQuit()
    //{
    //    Save();
    //}
    public void SetVolume(float value)
    {
        
        masterAudioMixer.SetFloat("Volume", Mathf.Log10(value)*20);
        
    }

    public void setMusicV(float value)
    {
        masterAudioMixer.SetFloat("musicVolume", Mathf.Log10(value) * 20);
    }

    public void setSFXV(float value)
    {
        masterAudioMixer.SetFloat("sfxVolume", Mathf.Log10(value) * 20);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height,Screen.fullScreen);
    }
    public void InitiateResolution()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resolutionList = new List<string>();

        int currentResolutionIndex = 0;
        int i = 0;

        foreach (var resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            resolutionList.Add(option);

            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            } ;
            i++;
        }
        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("Volume", masterSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        PlayerPrefs.SetInt("quality", graphicDropdown.value);
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn?1:0);
        PlayerPrefs.SetInt("resolution",resolutionDropdown.value);
        
    }

    public void Load()
    {
        LoadVolume("Volume", masterSlider, SetVolume, "Master Volume");
        LoadVolume("musicVolume", musicSlider, setMusicV, "Music Volume");
        LoadVolume("sfxVolume", sfxSlider, setSFXV, "SFX Volume");

        LoadDropdown("quality", graphicDropdown, SetQuality, "Quality");
        LoadDropdown("resolution", resolutionDropdown, SetResolution, "Resolution");

        LoadToggle("fullscreen", fullscreenToggle, "Fullscreen");

        
    }

    private void LoadVolume(string key, Slider slider, Action<float> setValueAction, string debugLabel)
    {
        if (PlayerPrefs.HasKey(key))
        {
            float value = PlayerPrefs.GetFloat(key);
            slider.SetValueWithoutNotify(value);
            setValueAction(value);
            
        }
        else
        {
            slider.SetValueWithoutNotify(slider.value);
        }
    }

    private void LoadDropdown(string key, TMP_Dropdown dropdown, Action<int> setValueAction, string debugLabel)
    {
        if (PlayerPrefs.HasKey(key))
        {
            int value = PlayerPrefs.GetInt(key);
            dropdown.SetValueWithoutNotify(value);
            setValueAction(value);
            
        }
        else
        {
            dropdown.SetValueWithoutNotify(dropdown.value);
        }
    }

    private void LoadToggle(string key, Toggle toggle, string debugLabel)
    {
        if (PlayerPrefs.HasKey(key))
        {
            bool isOn = PlayerPrefs.GetInt(key) == 1;
            toggle.SetIsOnWithoutNotify(isOn);
           
        }
        else
        {
            toggle.SetIsOnWithoutNotify(toggle.isOn);
        }
    }


    public void MoveScreenInside()
    {
        gameObject.transform.position = gameObject.transform.parent.position;
    }

    public void MoveScreenOutside()
    {
        gameObject.transform.position = new Vector3(-1000, 0, 0);
    } 
}
