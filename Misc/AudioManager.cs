using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClip;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        playAudio();
        
    }

    [ContextMenu("Play Audio")]
    public void playAudio()
    {
        int randomNumber = Random.Range(0,_audioClip.Length);
        AudioClip clip = _audioClip[randomNumber];
        _audioSource.PlayOneShot(clip);
    }

}
