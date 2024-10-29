using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SettingScript : MonoBehaviour
{

    public AudioMixer audioMixer;
    public void SetVolume(float value)
    {
        Debug.Log(value);
        audioMixer.SetFloat("volume", value);
    }

    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
