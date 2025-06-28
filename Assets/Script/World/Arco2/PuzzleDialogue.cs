using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PuzzleDialogue : MonoBehaviour
{
    public static PuzzleDialogue Instance { get; private set; }


    public GameObject dialogueGroup;
    public GameObject dialogueBG;
    public Text textGeral;
    public Image imagePlayer;
    public Image imageNPC;
    public List<Color> colorText;


    public bool isPuzzle;
    public List<PuzzleDialogueScriptable> puzzleScriptables;
    public GameObject puzzleGroup;
    public Text quentionText;
    public List<Button> answersButton;
    public int numberPuzzle;

    
    bool canNextDialogue = false;
    public char[] ctr;
    //NPC
    public List<Sprite> imageNPCList;


    //Player


    public List<Sprite> imagePlayerList;

    public EnemysScriptable enemysScriptable;
    public List<string> dialogueList;
    public List<string> thisIsList;
    public bool isDialogue = false;
    public int numberOfDialogue = 0;
    public int numberOfStartPuzzle;



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
        puzzleGroup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        dialogueSistem();

    }

    public void dialoguePuzzleReloadInfos(DialogueScriptable dialogue)
    {
        PuzzleDialogue.Instance.isDialogue = false;
        PuzzleDialogue.Instance.dialogueList = dialogue.text;
        PuzzleDialogue.Instance.thisIsList = dialogue.thisIs;
        PuzzleDialogue.Instance.imageNPCList = dialogue.imageNPC;
        PuzzleDialogue.Instance.imagePlayerList = dialogue.imagePlayer;
        ///PuzzleDialogue.Instance.colorText = dialogue.textColor;        
        PuzzleDialogue.Instance.enemysScriptable = dialogue.enemy;
        PuzzleDialogue.Instance.startCoroutine = true;
        PuzzleDialogue.Instance.isDialogue = true;
    }


    void dialogueSistem()
    {

        if (isDialogue == true)
        {
            //textGeral.text = dialogueList[numberOfDialogue];
            dialogueGroup.SetActive(true);
            ctr = dialogueList[numberOfDialogue].ToCharArray();
            quentionText.text = "";

            for (int i = 0; i < answersButton.Count; i++)
            {
                answersButton[i].gameObject.SetActive(false);
            }

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
                imageNPC.color = Color.white;
            }




            
            if (Input.GetMouseButtonDown(0) && canNextDialogue == true)
            {
                if (numberOfDialogue == numberOfStartPuzzle )
                {
                    isPuzzle = true;
                    isDialogue = false;
                }
                textGeral.text = dialogueList[numberOfDialogue];
              
                StopAllCoroutines();
                isCoroutineRun = false;
                canNextDialogue = true;
            }



        }
        else if (isPuzzle)
        {
            textGeral.text = "";
            quentionText.text = puzzleScriptables[numberPuzzle].question;
            for (int i = 0; i < answersButton.Count; i++)
            {
                answersButton[i].gameObject.SetActive(true);
                answersButton[i].GetComponent<Text>().text = puzzleScriptables[numberPuzzle].answers[i];
                answersButton[i].onClick.AddListener(IncorretAnswerButton);
            }
            answersButton[puzzleScriptables[numberPuzzle].correctAnswers].onClick.RemoveListener(IncorretAnswerButton);
            answersButton[puzzleScriptables[numberPuzzle].correctAnswers].onClick.AddListener(CorrectAnswerButton);
           
        }
        else
        {
            dialogueGroup.SetActive(false);


            numberOfDialogue = 0;

        }





    }
   
    private void CorrectAnswerButton()
    {
      
    }

    private void IncorretAnswerButton()
    {

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
