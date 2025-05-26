using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionInconScript : MonoBehaviour
{
    public AttackScriptable action;
    public TextMeshProUGUI actionNameHolder;
    public TextMeshProUGUI descriptionHolder;
    public Sprite imageSelect;
    public Sprite noSelectImage;
    public Button buttonSelect;
    [SerializeField]private bool isActive;
    // Start is called before the first frame update
    void Start()
    {
      
        StartCoroutine(checkHaveAction());
        TraderAction.Instance.gameObjectsList.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (TraderAction.Instance.numberAction == TraderAction.Instance.maxAction) { 
            if (isActive) 
            {
            
            }
            else 
            {
            buttonSelect.enabled = false;
            }
        }
        else
        { buttonSelect.enabled = true; }
    }

    public void setuUp(AttackScriptable attack)
    {
        actionNameHolder.text = attack.name;
       

        if (attack.learned == false) 
        {
            gameObject.GetComponent<Image>().color = Color.grey;
            descriptionHolder.text = "???????????????????????????????????????";
        }
        else { descriptionHolder.text = attack.combatDescr; }
    }

  
 IEnumerator checkHaveAction()
    {
        int i = 0;
        while(true){
            if ( action.name == PassInfos.Instance.actionPlayer[i].name)
            {
                gameObject.GetComponent<Image>().sprite = imageSelect;
                TraderAction.Instance.numberAction++;
                isActive = true;
                break;
            
            }
            else
            {
                i++;
                isActive = false;           
            }
           }
        yield break;
    }
    

    public void tradeAction()
    {
        if (action.learned == true)
        {
            //Parar adicionar a skill
            if (!isActive)
            {
                if (PassInfos.Instance.actionPlayer.Count < TraderAction.Instance.maxAction)
                { PassInfos.Instance.actionPlayer.Add(action);  StartCoroutine(checkHaveAction()); }
                else
                {  }
                isActive = true;
                
            }
            //Para tirar a skill
            else
            {
                gameObject.GetComponent<Image>().sprite = noSelectImage;
                PassInfos.Instance.actionPlayer.Remove(action);
                TraderAction.Instance.numberAction--;
                isActive = false;
            }
        }
    }

}
