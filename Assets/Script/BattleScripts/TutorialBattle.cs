
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBattle : MonoBehaviour
{

    public List<GameObject> stepsTutorialList;
    public GameObject combatManager;
    public int stepInt;
    // Start is called before the first frame update
    void Start()
    {
        disableSteps(); 
        
        
    }

    // Update is called once per frame
    void Update()
    {
     

    }


    public void nextStep()
    {
        stepInt++;
        disableSteps();
        if (stepInt >= stepsTutorialList.Count)
        {
            
            BattleManager.Instance.startCombatAfterTutorial();
        }
    }


    public void disableSteps()
    {

        for (int i = 0; i < stepsTutorialList.Count; i++)
        {
            if (i != stepInt)
            {
                stepsTutorialList[i].SetActive(false);
            }
            else { stepsTutorialList[stepInt].SetActive(true); }

        }

    }
}
