using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressEForDirector : MonoBehaviour
{
    public GameObject storeButtonWarining;
    public GameObject storeWarning;
    public GameObject directorObj;

    GameObject playerObject;

  

    
    [SerializeField] private bool block = false;
    bool canWarning = true;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {     
       
        blockDialogue();
       
    }

    public void blockDialogue()
    {

        float dist = Vector2.Distance(transform.position, playerObject.transform.position);
        if (dist < 1)
        {       
            storeWarning.SetActive(false);
            storeButtonWarining.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                storeWarning.SetActive(false);
                storeButtonWarining.SetActive(false);
                directorObj.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
        else
        {

            storeButtonWarining.SetActive(false); storeWarning.SetActive(true);

        }
    }
        
     
    
}
