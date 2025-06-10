using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Configs : MonoBehaviour
{
    public static Configs Instance { get; private set; }

    public GameObject configGroup;

    [Header("VolumeUI Varible")]
    public Slider sliderVolumeGeral;
    public Slider sliderVolumeSoundTrack;
    public Slider sliderVolumeSoundEffcts;

    [Header("Sounds Variable")]
    public float volumeGeral;
    public float volumeSoundTrack;
    public float volumeSoundEffcts;
    [SerializeField] private AudioSource soundTrackAudioSource;
    [SerializeField] private AudioSource soundEffctsAudioSource;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        TakeAudiosSource(); 
    }

    // Update is called once per frame
    void Update()
    {
        volumeGeral = sliderVolumeGeral.value;
        volumeSoundTrack = sliderVolumeSoundTrack.value * volumeGeral;
        volumeSoundEffcts = sliderVolumeSoundEffcts.value * volumeGeral;
        if (soundEffctsAudioSource != null )
        { 
            soundEffctsAudioSource.volume = volumeSoundEffcts; 
        }
        if (soundTrackAudioSource != null)
        {
            soundTrackAudioSource.volume = volumeSoundTrack;
        }
    }

    public void OpenConfigGroup(bool open)
    {
       
    }

    public void TakeAudiosSource()
    {
        soundTrackAudioSource = GameObject.FindGameObjectWithTag("SoundTrack").GetComponent<AudioSource>();
        soundEffctsAudioSource = GameObject.FindGameObjectWithTag("SoundEffects").GetComponent<AudioSource>();
    }
}
