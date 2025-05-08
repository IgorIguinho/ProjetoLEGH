using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkinMinigame : MonoBehaviour
{
    public Sprite completeSprite;
    public GameObject normalLight;
    public GameObject winLight;
    
    public void CompleteVsiual()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = completeSprite;
        normalLight.SetActive(false);
        winLight.SetActive(true);
    }
}
