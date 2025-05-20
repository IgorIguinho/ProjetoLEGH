using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HaterMiniGame : MonoBehaviour
{
    [Header("List de comentarios")]

    [SerializeField]
    private List<HaterMiniGameScriptable> listComments;
    [SerializeField]
    private HaterMiniGameScriptable commentsOnReader;
    public List<HaterMiniGameScriptable> listCommentsHaters;
    public List<HaterMiniGameScriptable> listCommentsFriendly;
  

    [Header(" Variaveis de derrota/vitoria")]
    [SerializeField]
    private int corrects;
    [SerializeField]
    private int incorrects;
    public GameObject parentGameObject;
    public bool isLastMinigame;
    public DialogueScriptable dialogueLose;
    public Transform respawnPlayer;
    public bool learnAction;
    public AttackScriptable actionToLearn;

    [Header("Variaveis de UI")]
    public int numberOfCommentsShow;
    public TextMeshProUGUI nickNameText;
    public TextMeshProUGUI textComentText;
    public TextMeshProUGUI publicationCount;
    public Image imageComents;
    public Button likeButton;
    public Button reportButton;
    public Button passButton;
    public AutoMouseController autoCursor;


    // Start is called before the first frame update
    void Start()
    {
        autoCursor = GameObject.FindGameObjectWithTag("Finish").GetComponent<AutoMouseController>();
        if (isLastMinigame)
        {
            listComments = listCommentsHaters;
            autoCursor.StartAutomatedCursorSequence();
        }
        else 
        {
            CompleteListComments();
        }
        
        likeButton.onClick.AddListener(LikeButtonFunction);
        reportButton.onClick.AddListener(ReportButtonFunction);
        passButton.onClick.AddListener(LikeButtonFunction);
        reloadUI(numberOfCommentsShow);
        PassInfos.Instance.playerRespawn = respawnPlayer.position;
       
    }
  
    // Update is called once per frame
    void Update()
    {

        publicationCount.text = corrects + incorrects + "/5 publicações";

    }



    public void LikeButtonFunction()
    {
        
            if (!commentsOnReader.isHater)
            {
                corrects++;
                numberOfCommentsShow++;
            if (numberOfCommentsShow < 5)
            { reloadUI(numberOfCommentsShow); }
            else { winCondicion(); }
        }
            else
            {
                incorrects++;
                numberOfCommentsShow++;
            if (numberOfCommentsShow < 5)
            { reloadUI(numberOfCommentsShow); }
            else { winCondicion(); }
        }
        
       
    }

    public void ReportButtonFunction()
    {
       
            if (commentsOnReader.isHater)
            {
                corrects++;
                numberOfCommentsShow++;
            if (numberOfCommentsShow < 5)
            { reloadUI(numberOfCommentsShow); }
            else { winCondicion(); }

        }
            else
            {
                incorrects++;
                numberOfCommentsShow++;
                if (numberOfCommentsShow < 5)
                { reloadUI(numberOfCommentsShow); }else { winCondicion(); }
            }
        
       

    }


    private void winCondicion()
    {
     
        
        if (corrects >= 3)
        {
            PassInfos.Instance.numberMachineOpen++;
            HaterMiniGameManager.Instance.startMiniGame = false;
            parentGameObject.GetComponentInChildren<HaterMiniGameAproxime>().enabled = false;
            parentGameObject.GetComponentInChildren<HaterMiniGameAproxime>().canvaObject.SetActive(false);
            parentGameObject.GetComponent<ChangeSkinMinigame>().CompleteVsiual();
           

            this.gameObject.SetActive(false);
        }
        else if (corrects < 3 )
        {
            PassInfos.Instance.DialogueScriptable = dialogueLose;
            TransitionSceneManager.Instance.Transition("ComunidadePAQ");
        }
       

    }

    public void reloadUI(int i)
    {
        HaterMiniGameScriptable Comments = listComments[i];
        commentsOnReader = Comments;
        imageComents.sprite = Comments.imageAccount;
        textComentText.text = Comments.textComents;
        nickNameText.text = Comments.nickName;
    }

    public void CompleteListComments()
    {
        int nCommentsHater;
        int nCommentsFriendly;
        nCommentsHater = Random.Range(2, 4);
        nCommentsFriendly = 5 - nCommentsHater;
        Debug.Log(nCommentsHater);
        Debug.Log(nCommentsFriendly);
        for (int i = 0; i < 5; i++) 
        {
            if (i < nCommentsHater)
            {
                listComments.Add(listCommentsHaters[Random.Range(0, listCommentsHaters.Count)]);
            }
            else 
            {
                listComments.Add(listCommentsFriendly[Random.Range(0, listCommentsFriendly.Count)]);
            }
        }

        ListRandomRange.RandomList(listComments);

    }
  
    
}

public static class ListRandomRange
{
    public static void RandomList<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }

    }
}
