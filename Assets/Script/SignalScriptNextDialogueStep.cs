using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalScriptNextDialogueStep : MonoBehaviour
{
    public void NextStepDialogue()
    {
        DialogueManager.Instance.NextDialogueForSignal();
    }
}
