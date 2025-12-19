using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedFinishMinigame : MonoBehaviour
{
    public DialogueScriptable dialogue;

    public GameObject gameobject;

    private void Update()
    {
        if (PassInfos.Instance.numberMachineOpen >= 5)
        {
            gameObject.SetActive(false);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            DialogueManager.Instance.dialogueReloadInfos(dialogue);
            
            if (gameobject != null)
            { DialogueManager.Instance.gameobject = gameobject; }

        }
    }
}
