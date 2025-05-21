using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HudBattleManager : MonoBehaviour
{

    public static HudBattleManager Instance { get; private set; }


    [Header("Text")]
    public TextMeshProUGUI textBalonPlayer;
    public TextMeshProUGUI textBalonEnemy;
    public TextMeshProUGUI textGeral;
    public TextMeshProUGUI textStm;
    public TextMeshProUGUI actionDescripiton;
    public TextMeshProUGUI energyDescription;

    [Header("GameOjbect")]
    public GameObject actionStep;
    public GameObject loseScreen;
    public GameObject balonPlayer;
    public GameObject balonEnemy;

    [Header("Lists")]
    public List<GameObject> buttons;

    [Header("Images")]
    public Image imageEnemy;

    [Header("Slides")]
    public Slider battleBar;
    public Image energyBar;
    public float energyValue;
    public List<Sprite> energyImages;
    

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
        textGeral.text = "Bora para a luta??";
        NameForButtons();
        balonPlayer.SetActive(false);
        balonEnemy.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        battleBar.value = BattleManager.Instance.valueBar;
        textStm.text = BattleManager.Instance.stamina.ToString() + "/" + BattleManager.Instance.staminaMax.ToString();
        AnimationEnergyBar();
        if (BattleManager.Instance.state != BattleState.PLAYERTURN) 
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].GetComponent<Button>().enabled = false;
            }
        }else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].GetComponent<Button>().enabled = true;
            }
        }

    }

    public void NameForButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = BattleManager.Instance.attacksPlayer[i].nameTitle;

            int currentIdex = i;

            // Configurar o EventTrigger
            EventTrigger trigger = buttons[i].GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = buttons[i].AddComponent<EventTrigger>();

            // Limpar entradas anteriores
            trigger.triggers.Clear();

            // Criar entrada para PointerEnter (quando o mouse passa por cima)
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { descriptionAction(currentIdex); });
            trigger.triggers.Add(entryEnter);

            // Opcional: limpar descrição ao sair
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => {
                if (actionDescripiton != null)
                    actionDescripiton.text = "";
            });

            if (BattleManager.Instance.stamina < -(BattleManager.Instance.attacksPlayer[i].costStm))
            {
                //mudar a aparancia do botao quando voce n tem estamina para usar a ação
                buttons[i].GetComponent<Image>().color = Color.grey; 
            }
            else
            {
                buttons[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void descriptionAction(int i)
    {
        // Verifique se o índice está dentro dos limites da lista
        if (i >= 0 && i < BattleManager.Instance.attacksPlayer.Count)
        {
            // Acesse o ataque pelo índice e mostre a descrição
                Debug.Log("Deu certo patrão" + BattleManager.Instance.attacksPlayer[i]);
                actionDescripiton.text = BattleManager.Instance.attacksPlayer[i].combatDescr; // Ou qualquer propriedade que você queira mostrar
            if (BattleManager.Instance.attacksPlayer[i].costStm < 0)
            {
                energyDescription.text = BattleManager.Instance.attacksPlayer[i].costStm.ToString() + " Energia";

            }
            else
            {
                energyDescription.text = "+" + BattleManager.Instance.attacksPlayer[i].costStm.ToString() + " Energia";

            }
        }
        else
        {
            // Índice inválido, limpe a descrição ou mostre uma mensagem padrão
            if (actionDescripiton != null)
                Debug.Log("errado patrinho");
            actionDescripiton.text = "";
        }
    }
   
    public IEnumerator animationAttack(AttackScriptable attack, bool isPlayer)
    {
        if (isPlayer)
        {
            balonPlayer.SetActive(true);
            textBalonPlayer.text = attack.fraseAction[Random.Range(attack.fraseAction.Count - 1, 0)];
            textGeral.text = attack.useCombat;

            yield return new WaitForSeconds(2);
            balonPlayer.SetActive(false);
            yield break;
        }
       else if (!isPlayer)
        {
            balonEnemy.gameObject.SetActive(true);
            textBalonEnemy.text = attack.fraseAction[Random.Range(attack.fraseAction.Count - 1, 0)];
            textGeral.text = attack.useCombat;

            yield return new WaitForSeconds(2);
            balonEnemy.gameObject.SetActive(false);
            yield break;
        }
     

    }

    public void AnimationEnergyBar()
    {
        energyValue = BattleManager.Instance.stamina;
        energyBar.sprite = energyImages[(int)energyValue];
    }

    public void BuffOrDebuffEffcts(bool start)
    {
        if (start)
        {

        }
        else
        {

        }
    }

    
}
