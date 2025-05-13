using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class HaterMiniGameManager : MonoBehaviour
{
    public static HaterMiniGameManager Instance { get; private set; }

    public List<GameObject> machineHaters;
   
    [SerializeField]private int atualMachine = 0;
    public GameObject arrowToMachine;
    public int numberMachineOpen;
    public bool startMiniGame;
    public AttackScriptable actionToLearn;
    GameObject player;

    public float rotationSpeed;

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
        arrowToMachine.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = PassInfos.Instance.playerRespawn;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ArrowFollowMachine();
        enableWarningObject();
        numberMachineOpen = PassInfos.Instance.numberMachineOpen;
        atualMachine = numberMachineOpen + 1;

        if (atualMachine == machineHaters.Count +1 )
        {
            StartCoroutine(LearningActionWarning.Instance.LearnAction(actionToLearn));
            atualMachine++;
        }

    }

    public void ArrowFollowMachine()
    {
        GameObject machineForOpen = machineHaters[numberMachineOpen];
        float dist = Vector2.Distance(machineForOpen.transform.position, arrowToMachine.transform.position);
        if (dist > 1f)
        {
            arrowToMachine.SetActive(true);
            Vector3 direction = machineForOpen.transform.position - arrowToMachine.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0,0,angle - 180); 

        arrowToMachine.transform.rotation = Quaternion.RotateTowards(arrowToMachine.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 100f);
        }
        else 
        {   
            arrowToMachine.SetActive(false);
        }
    }

    public void enableWarningObject()
    {
        GameObject machineForOpen = machineHaters[numberMachineOpen];
        

        
        
            machineForOpen.GetComponentInChildren<HaterMiniGameAproxime>().enabled = true;
           machineForOpen.GetComponentInChildren<HaterMiniGameAproxime>().canvaObject.SetActive(true);

        for (int i = 0; i < numberMachineOpen; i++)
        {
            machineHaters[i].GetComponent<ChangeSkinMinigame>().CompleteVsiual();
        }

    }


    
}
