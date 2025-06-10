using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AproximeDialogue : MonoBehaviour
{


    
    public GameObject storeButtonWarining;
    public GameObject storeWarning;
    public GameObject WARNINGWARNING;
    public List<DialogueScriptable> dialogue;
  
    GameObject playerObject;

    public bool revDialogue;
    public bool bispoDialogue;
   
    public bool oneDialogue;
    public int numberDialogue;
[SerializeField]    private bool block = false;
    bool canWarning = true;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (WARNINGWARNING != null )
        {
            if (numberDialogue == 1 && canWarning)
            {
                WARNINGWARNING.SetActive(true);
            }
            else
            {
                WARNINGWARNING.SetActive(false);
            }

        }
        blockDialogue();
        if (revDialogue)
        {
            numberDialogue = DialogueManager.Instance.numberDialogueNpc;
        }
    }

    public void blockDialogue()
    {
        if (PassInfos.Instance.blockDialogues.Contains(gameObject.name))
        {
            storeWarning.SetActive(false);
            storeButtonWarining.SetActive(false);
        }
        else if (oneDialogue == true )
        {
            float dist = Vector2.Distance(transform.position, playerObject.transform.position);


            if (block == false)
            {
                if (dist < 1)
                {

                    if (!DialogueManager.Instance.isDialogue || DialogueManager.Instance.isPuzzle)
                    { storeButtonWarining.SetActive(true); }

                    storeWarning.SetActive(false);
                    storeButtonWarining.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        storeWarning.SetActive(false);
                        storeButtonWarining.SetActive(false);
                        block = true;
                        PassInfos.Instance.blockDialogues.Add(gameObject.name);
                        storeButtonWarining.SetActive(false);
                        DialogueManager.Instance.dialogueReloadInfos(dialogue[numberDialogue]);

                    }
                }
                else
                {

                    storeButtonWarining.SetActive(false); storeWarning.SetActive(true);

                }
            }
        }
        else if (oneDialogue == false) 
        {
            float dist = Vector2.Distance(transform.position, playerObject.transform.position);
            if (dist < 1f)
            {
                if (!DialogueManager.Instance.isDialogue || DialogueManager.Instance.isPuzzle )
                  
                { storeButtonWarining.SetActive(true); storeWarning.SetActive(false); }
               

                if (Input.GetKeyDown(KeyCode.E))
                {
                    storeWarning.SetActive(false);
                    
                    storeButtonWarining.SetActive(false);
                    if (numberDialogue == 1)
                    { canWarning = false; }
                    DialogueManager.Instance.dialogueReloadInfos(dialogue[numberDialogue]);
                    if (bispoDialogue && DialogueManager.Instance.isRevulocaoDialogue)
                    {
                        DialogueManager.Instance.isBispoDialogue = true;
                        bispoDialogue = false;
                        storeWarning.SetActive(true);
                    }
                    else if (revDialogue) { DialogueManager.Instance.isRevulocaoDialogue = true; }
                    else { }


                }
            }
            else
            {

                storeButtonWarining.SetActive(false);
                

            }
        }
    }
}
