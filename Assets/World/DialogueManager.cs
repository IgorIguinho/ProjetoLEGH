using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

   
    public GameObject dialogueGroup;
    public GameObject dialogueBG;
    public Text textGeral;
    public Image imagePlayer;
    public Image imageNPC;
    public List<Color> colorText;
    public bool learnAction;
    public AttackScriptable attackScriptable;
    public bool haveCutscene;
    public List<PlayableAsset> cutScene;
    bool canRunCutscene;
    public int numberListCutscene = 0;
    PlayableDirector directorCutscene;
    public List<int> numberCutsecne;
    
    bool canNextDialogue = false;
    public char[] ctr;
    //NPC
    public List<Sprite> imageNPCList;
    public GameObject gameobject;



    //puzzle
    [Header("PuzzleInfos")]
    public bool isPuzzle;
    public PuzzleDialogueScriptable puzzleScriptables;
    public GameObject puzzleGroup;
    public Text questionText;
    public List<Button> answersButton;
    public int numberPuzzle;
    public bool havePuzzle;
    public int numberActivePuzzle;

    //Player

    [Space(1)]
    public List<Sprite> imagePlayerList;
    public bool isNextScene;
    public string nextScene;
    public string battleScene;
    public bool isBattle;

   
    public EnemysScriptable enemysScriptable;
    public List<string> dialogueList;
    public List<string> thisIsList;
    public bool isDialogue = false;
    [SerializeField]private int numberOfDialogue = 0;

    [Header("dialogue Puzzle Revolução")]
    public int numberDialogueNpc;
    public bool isBispoDialogue;
    public bool isRevulocaoDialogue;




    public bool startCoroutine = false;
    private bool isCoroutineRun = false;


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
        directorCutscene = GameObject.FindGameObjectWithTag("DirectorCutscene").GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        dialogueSistem();

    }


    public void dialogueReloadInfos(DialogueScriptable dialogue)
    {
        DialogueManager.Instance.isDialogue = false;
        DialogueManager.Instance.dialogueList = dialogue.text;
        DialogueManager.Instance.thisIsList = dialogue.thisIs;
        DialogueManager.Instance.imageNPCList = dialogue.imageNPC;
        DialogueManager.Instance.imagePlayerList = dialogue.imagePlayer;
        DialogueManager.Instance.colorText = dialogue.textColor;
        DialogueManager.Instance.isBattle = dialogue.isBattle;
        DialogueManager.Instance.battleScene = dialogue.battleScene;
        DialogueManager.Instance.enemysScriptable = dialogue.enemy;
        DialogueManager.Instance.learnAction= dialogue.learnAction;
        DialogueManager.Instance.attackScriptable = dialogue.attackScriptable;
        DialogueManager.Instance.haveCutscene = dialogue.haveCutscene;
        DialogueManager.Instance.cutScene = dialogue.cutScene;
        DialogueManager.Instance.numberCutsecne = dialogue.numberCutScene;
        DialogueManager.Instance.isNextScene = dialogue.isNextScene;
        DialogueManager.Instance.nextScene = dialogue.nextScene;
        DialogueManager.Instance.puzzleScriptables = dialogue.puzzleDialogueScriptable;
        DialogueManager.Instance.havePuzzle = dialogue.havePuzzle;
        DialogueManager.Instance.numberActivePuzzle = dialogue.numberActivePuzzle;
        DialogueManager.Instance.numberOfDialogue = 0;
        if (DialogueManager.Instance.haveCutscene == true)
        { canRunCutscene = true; }
        DialogueManager.Instance.isDialogue = true;
        DialogueManager.Instance.isPuzzle = false;
    }

    void dialogueSistem()
    {
        
        if (isDialogue == true)
        {
            //textGeral.text = dialogueList[numberOfDialogue];
             
            ctr = dialogueList[numberOfDialogue].ToCharArray();
           
            if (startCoroutine && !isCoroutineRun)
            {
                dialogueGroup.SetActive(true);
                StartCoroutine(machineText());
            
             
            }
            imageNPC.sprite = imageNPCList[numberOfDialogue];
            imagePlayer.sprite = imagePlayerList[numberOfDialogue];
            //textGeral.color = colorText[numberOfDialogue];
            dialogueBG.GetComponent<Outline>().effectColor = colorText[numberOfDialogue];
            if (thisIsList[numberOfDialogue] == "Player")
            { 
                imagePlayer.color = Color.white;
                imageNPC.color = Color.gray;
            }
            else if (thisIsList[numberOfDialogue] == "NPC") 
            { 
                imagePlayer.color = Color.gray;
                imageNPC .color = Color.white;
            }

            if (Input.GetMouseButtonDown(0) && canNextDialogue == true)
            {
                
                if (haveCutscene && numberCutsecne[numberListCutscene] == numberOfDialogue && canRunCutscene )
                {
                    directorCutscene.playableAsset = cutScene[numberListCutscene];
                    directorCutscene.Play();
                    dialogueGroup.SetActive(false);
                    canRunCutscene = false;
                    if (numberListCutscene < numberCutsecne.Count - 1)
                    {
                        numberListCutscene++;
                    }
                      
                    
                }
                else if (havePuzzle && numberActivePuzzle == numberOfDialogue)
                {
                   
                    puzzleGroup.SetActive(true);
                    isPuzzle = true;
                    isDialogue = false;
                }
                else if (directorCutscene.state != PlayState.Playing)
                {

                    numberOfDialogue++;
                    textGeral.text = "";
                   if (numberListCutscene < cutScene.Count)
                    {
                        canRunCutscene = true;
                    }
                    
                    canNextDialogue = false;
                    startCoroutine = true;
                }


            }
            else if (Input.GetMouseButtonDown(0) && canNextDialogue == false) 
            { 
                textGeral.text = dialogueList[numberOfDialogue];
          
                StopAllCoroutines();
                isCoroutineRun = false;
                canNextDialogue = true;
            }

            if (numberOfDialogue == dialogueList.Count)
            {

                if (isBattle)
                {
                    numberListCutscene = 0;
                    PassInfos.Instance.enemyToPass = enemysScriptable;

                    TransitionSceneManager.Instance.Transition(battleScene);
                }
                else if(isNextScene)
                {
                    numberListCutscene = 0;
                    isDialogue = false;
                    TransitionSceneManager.Instance.Transition(nextScene);
                }
                else
                {
                    numberListCutscene = 0;
                    isDialogue = false;
                    if (learnAction)
                    {
                        StartCoroutine(LearningActionWarning.Instance.LearnAction(attackScriptable));
                    }
                }
                
            }
            

        }
        else if (isPuzzle)
        {
            puzzleGroup.SetActive(true);
            textGeral.text = "";
            questionText.text = puzzleScriptables.question;
            for (int i = 0; i < answersButton.Count; i++)
            {
                answersButton[i].GetComponentInChildren<Text>().text = puzzleScriptables.answers[i];
                answersButton[i].onClick.AddListener(IncorretAnswerButton);
                
            }
            answersButton[puzzleScriptables.correctAnswers].onClick.RemoveListener(IncorretAnswerButton);
            answersButton[puzzleScriptables.correctAnswers].onClick.AddListener(CorrectAnswerButton);

        }
        else
        {
            if (isRevulocaoDialogue && isBispoDialogue)
            {
                numberDialogueNpc++;
                isRevulocaoDialogue = false;
                isBispoDialogue = false;

            }
            dialogueGroup.SetActive(false);
            if (gameobject != null)
            { gameobject.SetActive(true); }
            numberOfDialogue = 0;

        }
       


      


    }
    
    private void CorrectAnswerButton()
    {
        if (isPuzzle) { 
        Debug.Log("A proxmima resposta" + puzzleScriptables.answerIncorretDialogue[puzzleScriptables.correctAnswers]);
            isPuzzle = false;
            numberOfDialogue = 0;
            canNextDialogue = false;
            startCoroutine = true;

            dialogueReloadInfos(puzzleScriptables.answerIncorretDialogue[puzzleScriptables.correctAnswers]);
            puzzleGroup.SetActive(false);
        }

    }

    private void IncorretAnswerButton()
    {
        isPuzzle = false;
        numberOfDialogue = 0;
        canNextDialogue = false;
        startCoroutine = true;
        dialogueReloadInfos(puzzleScriptables.answerIncorretDialogue[0]);
        puzzleGroup.SetActive(false);
    }

    public void NextDialogueForSignal()
    {
        if (canNextDialogue == true)
        {

            
            
            if (havePuzzle && numberActivePuzzle == numberOfDialogue)
            {

                puzzleGroup.SetActive(true);
                isPuzzle = true;
                isDialogue = false;
            }
            else 
            {

                numberOfDialogue++;
                textGeral.text = "";
                if (numberListCutscene < cutScene.Count)
                {
                    canRunCutscene = true;
                }

                canNextDialogue = false;
                startCoroutine = true;
            }


        }
        else if ( canNextDialogue == false)
        {
            textGeral.text = dialogueList[numberOfDialogue];

            StopAllCoroutines();
            isCoroutineRun = false;
            canNextDialogue = true;
        }

        if (numberOfDialogue == dialogueList.Count)
        {

            if (isBattle)
            {
                numberListCutscene = 0;
                PassInfos.Instance.enemyToPass = enemysScriptable;

                TransitionSceneManager.Instance.Transition(battleScene);
            }
            else if (isNextScene)
            {
                numberListCutscene = 0;
                isDialogue = false;
                TransitionSceneManager.Instance.Transition(nextScene);
            }
            else
            {
                numberListCutscene = 0;
                isDialogue = false;
                if (learnAction)
                {
                    StartCoroutine(LearningActionWarning.Instance.LearnAction(attackScriptable));
                }
            }

        }



    }

    IEnumerator machineText()
    {
        isCoroutineRun = true;
        startCoroutine = false;
        int cout = 0;

      
        while (cout < ctr.Length)
        {
            yield return new WaitForSeconds(0.05f);
            textGeral.text += ctr[cout];
            cout++;
        }
        if (cout >= ctr.Length)
        {
           
            canNextDialogue = true;
            isCoroutineRun = false;
            yield break;
        }
    }
}
