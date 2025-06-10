using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassInfos : MonoBehaviour
{


    public static PassInfos Instance { get; private set; }

    public EnemysScriptable enemyToPass;
    public DialogueScriptable DialogueScriptable;
    public bool startDialogue;
    public Vector3 playerRespawn;
    public int numberMachineOpen;
    public bool learningAction;
    public AttackScriptable actionToLearning;

    public List<string> blockDialogues;
    public List<bool> puzzlesComplets;

    public List<AttackScriptable> actionPlayer;


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
  
    
  
}
