using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBehavior2 : MonoBehaviour
{

    public GameObject doorPuzzle;
    public GameObject resetButton;
    public GameObject nextPuzzleDoor;
    public AudioSource audioEffect;
    public GameObject completeLight;
    public GameObject lightNextDoor;
    public List<Transform> puzzleLock;
    GameObject player;
    bool isLocked;
    bool passed;
    public GameObject keyObj;
    public Transform playerInitialPosition;
    public Transform keyInitialPosition;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        doorPuzzle.SetActive(false);
        passed = false;
    }

    // Update is called once per frame
    void Update()
    {
        loockSistem();
    }


    void loockSistem()
    {
        if (player.transform.position.x < puzzleLock[0].position.x && player.transform.position.y < puzzleLock[1].position.y && passed == false) 
        { 
            isLocked = true;
            
        }

        //puzzle1
        if (isLocked == false) 
        {
            doorPuzzle.SetActive(false);
            resetButton.SetActive(false);
        }
        else if (isLocked == true )
        {
            doorPuzzle.SetActive(true);
            resetButton.SetActive(true);
        }
    }

    public void resetLevel()
    {
        player.transform.position = playerInitialPosition.position;
        keyObj.transform.position = keyInitialPosition.position;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "KeyPuzzle")
        {
            isLocked = false;
            passed = true;
            nextPuzzleDoor.SetActive(false);
            PassInfos.Instance.puzzlesComplets[1] = true;
            Destroy(collision.gameObject);
            completeLight.SetActive(true);
            lightNextDoor.SetActive(true);
            CamShake.Instance.start = true;
            audioEffect.Play();
        }
    }
}
