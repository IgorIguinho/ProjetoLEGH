using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfosPersistent : MonoBehaviour
{
    
    void Awake()
    {
        // Marca este GameObject para não ser destruído ao carregar uma nova cena
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Volume.Instance.takeSoundTrack();
        CamShake.Instance.cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        DialogueManager.Instance.StartNewScene();
    }



}
