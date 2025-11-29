using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public GameObject gameobject;
    public GameObject buttonForSkip;
    public DialogueScriptable dialoguePrefab;

    [Space(1)]
    [Header("DialogueInfos")]
    public TextMeshProUGUI textGeral;
    public TextMeshProUGUI nameHolderPlayer;
    public TextMeshProUGUI nameHolderNPC;
    public Image imagePlayer;
    public Image imageNPC;
    public List<string> dialogueName;
    public List<string> dialogueList;
    public List<Sprite> imageNPCList;
    public List<Sprite> imagePlayerList;
    public List<string> thisIsList;
    bool canNextDialogue = false;
    public char[] ctr;
    public bool isDialogue = false;
    
    [SerializeField] private int numberOfDialogue = 0;


    [Space(1)]
    [Header("PuzzleInfos")]
    public bool isPuzzle;
    public PuzzleDialogueScriptable puzzleScriptables;
    public GameObject puzzleGroup;
    public TextMeshProUGUI questionText;
    public List<Button> answersButton;
    public int numberPuzzle;
    public bool havePuzzle;
    public int numberActivePuzzle;

    //Player

    [Space(1)]
    [Header("NextInfos")]
   
    public bool isNextScene;
    public string nextScene;
    public string battleScene;
    public bool isBattle;
    public EnemysScriptable enemysScriptable;

    [Space(1)]
    [Header("Cutscene infos")]
    public bool haveCutscene;
    public List<PlayableAsset> cutScene;
    bool canRunCutscene;
    public int numberListCutscene = 0;
    [SerializeField]PlayableDirector directorCutscene;
    public List<int> numberCutsecne;

    [Space(1)]
    [Header("Learn Infos")]
    public bool learnAction;
    public AttackScriptable attackScriptable;

    [Space(1)]
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
        if (isRevulocaoDialogue && isBispoDialogue)
        {
            numberDialogueNpc++;
            isRevulocaoDialogue = false;
            isBispoDialogue = false;
            if (gameobject != null)
            { gameobject.SetActive(true); }

        }
    }


    public void dialogueReloadInfos(DialogueScriptable dialogue)
    {
        isDialogue = false;
        dialogueList = dialogue.text;
        dialogueName = dialogue.nameDialogue;
        thisIsList = dialogue.thisIs;
        imageNPCList = dialogue.imageNPC;
        imagePlayerList = dialogue.imagePlayer;
        isBattle = dialogue.isBattle;
        battleScene = dialogue.battleScene;
        enemysScriptable = dialogue.enemy;
        learnAction = dialogue.learnAction;
        attackScriptable = dialogue.attackScriptable;
        haveCutscene = dialogue.haveCutscene;
        cutScene = dialogue.cutScene;
        numberCutsecne = dialogue.numberCutScene;
        isNextScene = dialogue.isNextScene;
        nextScene = dialogue.nextScene;
        puzzleScriptables = dialogue.puzzleDialogueScriptable;
        havePuzzle = dialogue.havePuzzle;
        numberActivePuzzle = dialogue.numberActivePuzzle;
        numberOfDialogue = 0;

        if (haveCutscene == true)
        {
            canRunCutscene = true;
        }

        isDialogue = true;
        isPuzzle = false;
        dialoguePrefab = dialogue;

    }

    void dialogueSistem()
    {
        
        if (isDialogue == true)
        {
            //textGeral.text = dialogueList[numberOfDialogue];
            buttonForSkip.SetActive(dialoguePrefab.canSkip);
            ctr = dialogueList[numberOfDialogue].ToCharArray();
           
            if (startCoroutine && !isCoroutineRun)
            {
                dialogueGroup.SetActive(true);
                StartCoroutine(machineText());
            
             
            }
            imageNPC.sprite = imageNPCList[numberOfDialogue];
            imagePlayer.sprite = imagePlayerList[numberOfDialogue];
           
            if (thisIsList[numberOfDialogue] == "Player")
            {
                dialogueBG.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
                nameHolderNPC.text = "";
                nameHolderPlayer.text = dialogueName[numberOfDialogue];
                imagePlayer.color = Color.white;
                imageNPC.color = Color.gray;
            }
            else if (thisIsList[numberOfDialogue] == "NPC") 
            {
                dialogueBG.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 180, 0);
                nameHolderPlayer.text = "";
                nameHolderNPC.text = dialogueName[numberOfDialogue];
                imagePlayer.color = Color.gray;
                imageNPC .color = Color.white;
            }

            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && canNextDialogue == true)
            {
                
                if (haveCutscene && numberCutsecne[numberListCutscene] == numberOfDialogue && canRunCutscene )
                {
                    directorCutscene.playableAsset = cutScene[numberListCutscene];
                    directorCutscene.Play();
                    dialogueGroup.SetActive(false);
                    canRunCutscene = false;
                    if (numberCutsecne.Last() == dialogueList.Count)
                    {
                        dialoguePrefab.canSkip = true;
                    }
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
            else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && canNextDialogue == false) 
            { 
                textGeral.text = dialogueList[numberOfDialogue];
          
                StopAllCoroutines();
                isCoroutineRun = false;
                canNextDialogue = true;
            }

            if (numberOfDialogue == dialogueList.Count)
            {
                dialoguePrefab.canSkip = true;  

                if (isBattle)
                {
                    dialoguePrefab.canSkip = true;
                    numberListCutscene = 0;
                    PassInfos.Instance.enemyToPass = enemysScriptable;

                    TransitionSceneManager.Instance.Transition(battleScene);
                }
                else if(isNextScene)
                {
                    dialoguePrefab.canSkip = true;
                    numberListCutscene = 0;
                    isDialogue = false;
                    TransitionSceneManager.Instance.Transition(nextScene);
                }
                else
                {
                    dialoguePrefab.canSkip = true;
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
                answersButton[i].GetComponentInChildren<TextMeshProUGUI>().text = puzzleScriptables.answers[i];
                answersButton[i].onClick.AddListener(IncorretAnswerButton);
                answersButton[i].onClick.RemoveListener(CorrectAnswerButton);
              

            }
            answersButton[puzzleScriptables.correctAnswers].onClick.RemoveListener(IncorretAnswerButton);
            answersButton[puzzleScriptables.correctAnswers].onClick.AddListener(CorrectAnswerButton);
            

        }
        else
        {
            
            dialogueGroup.SetActive(false);
            
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
        if (puzzleScriptables.correctAnswers == 0)
        { dialogueReloadInfos(puzzleScriptables.answerIncorretDialogue[1]); }
        else { dialogueReloadInfos(puzzleScriptables.answerIncorretDialogue[0]); }
       
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
            dialoguePrefab.canSkip = true;
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

    public void SkipDialogue()
    {
        numberOfDialogue = dialogueList.Count;
        dialoguePrefab.canSkip = true;

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

    public void StartNewScene()
    {
        directorCutscene = GameObject.FindGameObjectWithTag("DirectorCutscene").GetComponent<PlayableDirector>();
    }
}
