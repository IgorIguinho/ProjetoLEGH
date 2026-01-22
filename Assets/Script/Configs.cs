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
    public Slider sliderVolumeSoundEffects;

    [Header("Sounds Variable")]
    public float volumeGeral;
    public float volumeSoundTrack;
    public float volumeSoundEffects;
    [SerializeField] private AudioSource soundTrackAudioSource;
    [SerializeField] private AudioSource soundEffctsAudioSource;
    [SerializeField] private GameObject SoundTrackAudioControllerAct2;

    [Header("Player Stop Walnking")]
    
    GameObject playerObject;

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
        volumeSoundEffects = sliderVolumeSoundEffects.value * volumeGeral;
        if (soundEffctsAudioSource != null )
        { 
            soundEffctsAudioSource.volume = volumeSoundEffects; 
        }
        if (soundTrackAudioSource != null)
        {
            soundTrackAudioSource.volume = volumeSoundTrack;
        }
        else if (SoundTrackAudioControllerAct2 != null)
        {
            SoundTrackAudioControllerAct2.GetComponent<SoundTrackATO2>().maxVolume = volumeSoundTrack;
        }
        
    }

    public void OpenConfigGroup(bool open)
    {
       
    }

    public void TakeAudiosSource()
    {
        soundTrackAudioSource = GameObject.FindGameObjectWithTag("SoundTrack").GetComponent<AudioSource>();
        if (soundTrackAudioSource == null) { SoundTrackAudioControllerAct2 = GameObject.FindGameObjectWithTag("SoundTrack"); }
        soundEffctsAudioSource = GameObject.FindGameObjectWithTag("SoundEffects").GetComponent<AudioSource>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public void BlockMovementPlayer(bool block)
    {
      
        playerObject.GetComponent<PlayerMov>().BlockMov(block);
    }    
    
}
