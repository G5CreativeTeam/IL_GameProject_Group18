using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


public class SettingScript : MonoBehaviour
{
    public Slider audioSlider;
    public TMP_Dropdown graphicDropdown, resolutionDropdown;
    public Toggle fullscreenToggle;


    public AudioMixer audioMixer;
    private Resolution[] resolutions;

    public void Start()
    {
        Debug.Log("WAKE UP!");
        InitiateResolution();
        Load();
    }
    private void OnDisable()
    {
        Save();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    public void SetVolume(float value)
    {
        
        audioMixer.SetFloat("Volume", value);
        Debug.Log($"SetVolume successful!{value}");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void setResolution(int resolutionIndex)
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

    private void Save()
    {
        PlayerPrefs.SetFloat("Volume", audioSlider.value);
        PlayerPrefs.SetInt("quality", graphicDropdown.value);
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn?1:0);
        PlayerPrefs.SetInt("resolution",resolutionDropdown.value);
        Debug.Log("Save Succesful!");
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            float volume = PlayerPrefs.GetFloat("Volume");
            audioSlider.SetValueWithoutNotify(volume);
            SetVolume(volume);
            Debug.Log("Volume Loaded!");
        }
        else
        {

            audioSlider.SetValueWithoutNotify(audioSlider.value);
        }
        if (PlayerPrefs.HasKey("quality"))
        {
            int quality = PlayerPrefs.GetInt("quality");
            graphicDropdown.SetValueWithoutNotify(quality);
            SetQuality(quality);
            Debug.Log("Quality Loaded!");
        }
        else
        {
            graphicDropdown.SetValueWithoutNotify(graphicDropdown.value);
        }
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool fullscreen = PlayerPrefs.GetInt("fullscreen") == 1? true : false;
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
            Debug.Log("Fullscreen Loaded!");

        } else
        {
            fullscreenToggle.SetIsOnWithoutNotify(fullscreenToggle.isOn);
        }
        if (PlayerPrefs.HasKey("resolution"))
        {
            int resolutionindex = PlayerPrefs.GetInt("resolution");
            resolutionDropdown.SetValueWithoutNotify(resolutionindex);
            setResolution(resolutionindex);
            Debug.Log("Resolution Loaded!");
        } else
        {
            resolutionDropdown.SetValueWithoutNotify(resolutionDropdown.value);
        }
        Debug.Log("Load Succesful!");
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
