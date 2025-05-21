using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN,ENEMYTURN, WON, LOSE}

public class BattleManager : MonoBehaviour
{

    public static BattleManager Instance { get; private set; }

    //BattleSystem
    public BattleState state;
    public bool isTutorial;
    private bool canAttack;

    public float valueBar;
    public float valueWinEnemy;
    public float valueWinPlayer;
    public float stamina;
    public float staminaMax;
  
    public AudioSource soundTrack;

    //Types Attacks Player
    float modificadorPlayer;
    float modificadorEnemy;
    bool startTimingAction;
    [Header("Player type action....")]
    public int turnsForTimingAction;
    public int maxTurnsForAction;
    int TimingActionObject;

    [Space(4)]
    
    //Types Attacks enemy
    bool startTimingActionEnemy;
    [Header("Enemys type action....")]
    public int turnsForTimingActionEnemy;
    public int maxTurnsForActionEnemy;
    int TimingActionObjectEnemy;
    
    



    //Lists for attack
    [Header("List Action")]
    public List<AttackScriptable> attacksPlayer;
    public List<AttackScriptable> enemyAction;
    public EnemysScriptable enemyAtributes;


   


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

        canAttack = true;
        enemyAtributes = PassInfos.Instance.enemyToPass;    // Puxa os atributos do inimigo  
        HudBattleManager.Instance.imageEnemy.GetComponent<Animator>().Play(enemyAtributes.animationBattle.name);   
       
        enemyAction = new List<AttackScriptable>(enemyAtributes.AttackScripts);
        soundTrack.clip = enemyAtributes.musicBattle;
        stamina = staminaMax;
        if (!isTutorial)
        {
            attacksPlayer = PassInfos.Instance.actionPlayer;
            state = BattleState.START;
            HudBattleManager.Instance.NameForButtons();
            StartCoroutine(EnemyAttack(0));
        }

       
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Tab))  // Hack para ganhar a batalha
        {

            if (enemyAtributes.haveDialogueNext == true)
            {
                PassInfos.Instance.DialogueScriptable = enemyAtributes.nextDialogue;
                PassInfos.Instance.startDialogue = true;
            }

            SceneManager.LoadScene(enemyAtributes.nextScene);
        }
        else if (Input.GetKey(KeyCode.L))// Hack para perder a batalha
        {
            valueBar -= 100;
        }


        if (stamina > staminaMax)
        {
            stamina = staminaMax;
        }
    }

    IEnumerator SetupBattle()
    {

        HudBattleManager.Instance.textGeral.text = "Sua Ação";

        if (startTimingAction == true)
        {
            if (turnsForTimingAction < maxTurnsForAction) //Somando o turno para ação de preparo
            {
                turnsForTimingAction++;
            }
            else //Ações timing acontece
            {
               
                StartCoroutine(PlayerAttack(TimingActionObject));// Chama a ação do jogador que estava em preparo
                state = BattleState.PLAYERTURN;
                yield break;

            }
        }

        if (startTimingActionEnemy == true)
        {
            if (turnsForTimingActionEnemy < maxTurnsForActionEnemy) //Somando o turno para ação de preparo
            {
                turnsForTimingActionEnemy++;
            }
            else //Ações timing acontece
            {

                StartCoroutine(EnemyAttack(TimingActionObjectEnemy)); // Chama a ação do inimmgo que estava em preparo
                state = BattleState.ENEMYTURN;
                yield break;

            }
        }

        state = BattleState.PLAYERTURN;
        HudBattleManager.Instance.NameForButtons();
        canAttack = true;
  
        yield break;
       
    }

    IEnumerator PlayerAttack(int i)
    {
        //codigo para ações efetivas, super efetivas...

        if (attacksPlayer[i].typeAction == "Normal")
        {
            if (enemyAtributes.superEffective.Contains(attacksPlayer[i]))// super efetiva
            {
                StartCoroutine(BarValueAnimation((attacksPlayer[i].dmg + modificadorPlayer) * 2));
                modificadorPlayer = 0; //Resetar o modifcador do dano
                HudBattleManager.Instance.BuffOrDebuffEffcts(false); //Disabilita o efeito visual do buff 
                HudBattleManager.Instance.textGeral.text = "Esta ação foi super efetiva";
            }
            else if (enemyAtributes.noEffective.Contains(attacksPlayer[i]))  // não efetiva
            {             
                StartCoroutine(BarValueAnimation((attacksPlayer[i].dmg + modificadorPlayer)* 2));
                modificadorPlayer = 0;
                HudBattleManager.Instance.BuffOrDebuffEffcts(false);
                HudBattleManager.Instance.textGeral.text = "Esta ação foi não foi efetiva";
            }
            else if (enemyAtributes.invunerable.Contains(attacksPlayer[i])) //Invuneravel
            {
                HudBattleManager.Instance.textGeral.text = "O inimigo é ivuneravel a essa ação";
            }
            else if (enemyAtributes.actionIncorrect.Contains(attacksPlayer[i])) //Ação errada
            {
                HudBattleManager.Instance.textGeral.text = "Está ação deixou o inimgo com mais raiva";
                StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer));
            }
            else //efetiva
            {
                StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer));
                modificadorPlayer = 0;
                HudBattleManager.Instance.BuffOrDebuffEffcts(false);
                HudBattleManager.Instance.textGeral.text = "Esta ação foi efetiva";
            }
            stamina += attacksPlayer[i].costStm; // Muda a estamina do player conforme custo de estamina
            StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true)); 
           
        }
        else if (attacksPlayer[i].typeAction == "BuffOrDebuffAction") //Ação que modifica o valor do proximo turno
        {
            if (attacksPlayer[i].buffPlayer)//Modifica o valor da ação do jogador
            {
                modificadorPlayer = attacksPlayer[i].modificadorBuffOrDebuff; 
                stamina += attacksPlayer[i].costStm;
                HudBattleManager.Instance.textGeral.text = attacksPlayer[i].useCombat;
            }
            else //Modifica o valor da ação do inimigo
            { 
                modificadorEnemy = attacksPlayer[i].modificadorBuffOrDebuff; 
                stamina += attacksPlayer[i].costStm;
                HudBattleManager.Instance.textGeral.text = attacksPlayer[i].useCombat;
            }
            StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
    

        }
        else if (attacksPlayer[i].typeAction == "WaintingTurns")
        {
            
            if (turnsForTimingAction == maxTurnsForAction && startTimingAction) // Momento que o timing cehga no maximo e a ação acontece
            {
                if (enemyAtributes.superEffective.Contains(attacksPlayer[i]))// super efetiva
                {
                    StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer * 2));
                    modificadorPlayer = 0; //Resetar o modifcador do dano
                    HudBattleManager.Instance.BuffOrDebuffEffcts(false); //Disabilita o efeito visual do buff 
                    HudBattleManager.Instance.textGeral.text = "Esta ação foi super efetiva";
                }
                else if (enemyAtributes.noEffective.Contains(attacksPlayer[i]))  // não efetiva
                {
                    StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer / 2));
                    modificadorPlayer = 0;
                    HudBattleManager.Instance.BuffOrDebuffEffcts(false);
                    HudBattleManager.Instance.textGeral.text = "Esta ação foi não foi efetiva";
                }
                else if (enemyAtributes.invunerable.Contains(attacksPlayer[i])) //Invuneravel
                {
                    HudBattleManager.Instance.textGeral.text = "O inimigo é ivuneravel a essa ação";
                }
                else if (enemyAtributes.actionIncorrect.Contains(attacksPlayer[i])) //Ação errada
                {
                    HudBattleManager.Instance.textGeral.text = "Está ação deixou o inimgo com mais raiva";
                StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer));
                }
                else //efetiva
                {
                    StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer));
                    modificadorPlayer = 0;
                    HudBattleManager.Instance.BuffOrDebuffEffcts(false);
                    HudBattleManager.Instance.textGeral.text = "Esta ação foi efetiva";
                }
                stamina += attacksPlayer[i].costStm; // Muda a estamina do player conforme custo de estamina
                StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
                startTimingAction = false;
                yield return new WaitForSeconds(1.5f);
            }
            else // É o começo da ação, começando com a contagem 
            {
                StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
                HudBattleManager.Instance.textGeral.text =attacksPlayer[i].useCombat;
                stamina += attacksPlayer[i].costStm;
                turnsForTimingAction = 0;
                startTimingAction = true;
                maxTurnsForAction = attacksPlayer[i].turnsForTimingAction;
                TimingActionObject = i;
  
            }
        }
        else if (attacksPlayer[i].typeAction == "RestoreEnergia")
        {
            stamina += attacksPlayer[i].costStm;
            StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
            HudBattleManager.Instance.textGeral.text = attacksPlayer[i].useCombat;
            
        }
       
        yield return new WaitForSeconds(2f);

        if (valueBar <= valueWinPlayer)
        {

            if (enemyAtributes.haveDialogueNext == true)
            {
                PassInfos.Instance.DialogueScriptable = enemyAtributes.nextDialogue;
                PassInfos.Instance.startDialogue = true;
            }

            if (enemyAtributes.haveLearAction == true)
            {
                PassInfos.Instance.learningAction = true;
                PassInfos.Instance.actionToLearning = enemyAtributes.actionToLearn;
            }
            TransitionSceneManager.Instance.Transition(enemyAtributes.nextScene);
        }
        else
        {
            HudBattleManager.Instance.NameForButtons();
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyAttack(Random.Range(0, enemyAction.Count)));
        }
    }

   

    IEnumerator  EnemyAttack(int i)
    {

       
     
        if (enemyAction[i].typeAction == "Normal" )
        { 
            //valueBar -= enemyAction[i].dmg + modificadorEnemy;

            StartCoroutine(BarValueAnimation(enemyAction[i].dmg + modificadorEnemy)); //Puxa a coroutine quue faz o dano a barra
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false)); //Puxa a animação do ataque do inimigo
            modificadorEnemy = 0;
        }
        else if (enemyAction[i].typeAction == "Agrupamento") // Ataque que vai juntar inimigos para a batalha, demora x turnos pra funcionar
        {
            if (turnsForTimingActionEnemy == maxTurnsForActionEnemy && startTimingActionEnemy)//aviso que o roximo turno é o ataque
            {
                StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false)); //Puxa a animação do ataque do inimigo
                //Debug.Log("Deu certo");
                enemyAction.Clear();
                enemyAction.Add(enemyAtributes.actionBlockEnemy);
                startTimingActionEnemy = false;
             

            }
            else if (!startTimingActionEnemy) //É o começo do ataque, assim que o inimigo usa faz isso:
            {
                StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false)); 
                //Debug.Log("Começou");
                turnsForTimingActionEnemy = 0;
                startTimingActionEnemy = true;
                maxTurnsForActionEnemy = enemyAction[i].turnsForTimingActionEnemy;
                TimingActionObjectEnemy = i;
            }
            else //O inimigo finalmente ataca
            {
                StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
                StartCoroutine(BarValueAnimation(enemyAction[i].dmg + modificadorEnemy)); modificadorEnemy = 0;
                HudBattleManager.Instance.textGeral.text = enemyAction[1].fraseAction[Random.Range(0, enemyAction[i].fraseAction.Count)];
           
            }
        }
        else if (enemyAction[i].typeAction == "Roubo") //Ação de roubar estamina do jogador
        {
            stamina -= 5 ;
            HudBattleManager.Instance.textStm.color = Color.red;
            StartCoroutine(BarValueAnimation(enemyAction[i].dmg + modificadorEnemy)); modificadorEnemy = 0;
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
        }
        else if (enemyAction[i].typeAction == "BuffOrDebuffAction") //Ação de bufar a proxima ação do inimigo ou debuff na ação do player
        {
            if (enemyAction[i].buffPlayer)
            {
                modificadorPlayer = -enemyAction[i].modificadorBuffOrDebuff;// Modifica a ação do player         
            }
            else 
            {
                modificadorEnemy = enemyAction[i].modificadorBuffOrDebuff;//Modifica a ação do inimigo
            }
            
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
            
        }
        
        yield return new WaitForSeconds(2f);
        HudBattleManager.Instance.textStm.color = Color.white;
        
        if (valueBar >= valueWinEnemy) //Condição de perda
        {
            HudBattleManager.Instance.loseScreen.SetActive(true);
            yield return new WaitForSeconds(1f);
            PassInfos.Instance.DialogueScriptable = enemyAtributes.dialogueDerrota;
            TransitionSceneManager.Instance.Transition(enemyAtributes.sceneDerrota); // muda pra cena de derrota
        }
        else
        {
            state = BattleState.PLAYERTURN;
            HudBattleManager.Instance.NameForButtons();
            StartCoroutine(SetupBattle());
        }
    }

    public void OnAttackPlayer(int i)
    {
        if (state == BattleState.PLAYERTURN && canAttack)
        {
            if (stamina >= -(attacksPlayer[i].costStm))
            { 
                StartCoroutine(PlayerAttack(i));
                canAttack = false;
            }
        }

    }

    public void startCombatAfterTutorial() {
        BattleManager.Instance.state = BattleState.START;
        StartCoroutine(BattleManager.Instance.EnemyAttack(0));
    }

    IEnumerator BarValueAnimation(float damage)
    {
       
        int cout = 0;
        
        if (0 < damage)
        { 
        while (cout < damage)
        {
            yield return new WaitForSeconds(0.1f);
            valueBar += 1;
            cout++;
        }
            if (cout >= damage)
            {
                yield break;
            }
        }
        else if (0 > damage)
        {
            while (cout > damage)
            {
                yield return new WaitForSeconds(0.1f);
                valueBar -= 1;
                cout--;
            }
            if (cout <= damage)
            {
                yield break;
            }
        }

        
    }

}