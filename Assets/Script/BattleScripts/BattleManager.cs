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

    public float valueBar;
    public float valueWinEnemy;
    public float valueWinPlayer;
    public float stamina;
    public float staminaMax;
    public AudioSource audioSource;

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

       
        enemyAtributes = PassInfos.Instance.enemyToPass;     
        HudBattleManager.Instance.imageEnemy.GetComponent<Animator>().Play(enemyAtributes.animationBattle.name);   
       
        enemyAction = new List<AttackScriptable>(enemyAtributes.AttackScripts);
       
        stamina = staminaMax;
        if (!isTutorial)
        {
            attacksPlayer = PassInfos.Instance.actionPlayer;
            state = BattleState.START;
            StartCoroutine(EnemyAttack(0));
        }

       
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Tab)) 
        {

            if (enemyAtributes.haveDialogueNext == true)
            {
                PassInfos.Instance.DialogueScriptable = enemyAtributes.nextDialogue;
                PassInfos.Instance.startDialogue = true;
            }

            SceneManager.LoadScene(enemyAtributes.nextScene);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            valueBar -= 100;
        }
    }

    IEnumerator SetupBattle()
    {


        HudBattleManager.Instance.textGeral.text = "Bora reagir?";


        yield return new WaitForSeconds(2f);
        if (startTimingAction == true)
        {
            if (turnsForTimingAction < maxTurnsForAction) //Somando o turno para a��o de preparo
            {
                turnsForTimingAction++;
            }
            else //A��es timing acontece
            {
                audioSource.clip = attacksPlayer[TimingActionObject].audioClip;
                audioSource.Play();
                HudBattleManager.Instance.startTextStep();
                StartCoroutine(PlayerAttack(TimingActionObject));
                state = BattleState.PLAYERTURN;
                yield break;

            }
        }

        if (startTimingActionEnemy == true)
        {
            if (turnsForTimingActionEnemy < maxTurnsForActionEnemy) //Somando o turno para a��o de preparo
            {
                turnsForTimingActionEnemy++;
            }
            else //A��es timing acontece
            {

                HudBattleManager.Instance.startTextStep();
                StartCoroutine(EnemyAttack(TimingActionObjectEnemy));
                state = BattleState.ENEMYTURN;
                yield break;

            }
        }

        state = BattleState.PLAYERTURN;
        HudBattleManager.Instance.startActionStep();
        yield break;
       
    }

    IEnumerator PlayerAttack(int i)
    {
        //codigo para a��es efetivas, super efetivas...

        if (attacksPlayer[i].typeAction == "Normal")
        {
            if (enemyAtributes.superEffective.Contains(attacksPlayer[i]))
            {
                // super efetiva
                //valueBar += (attacksPlayer[i].dmg + modificadorPlayer) * 2;
                StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer * 2));
                stamina += attacksPlayer[i].costStm;

                if (attacksPlayer[i].costStm < 0)
                {
                    //muda a cor da stamnia para vermelho
                    HudBattleManager.Instance.textStm.color = Color.red;
                }
                else if (attacksPlayer[i].costStm > 0)
                {
                    //muda para verde
                    HudBattleManager.Instance.textStm.color = Color.green;
                }

                StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
                yield return new WaitForSeconds(1.5f);
                
                HudBattleManager.Instance.textStm.color = Color.white;
                HudBattleManager.Instance.textGeral.text = "Esta a��o foi muito efetiva";

            }
            else if (enemyAtributes.noEffective.Contains(attacksPlayer[i]))
            {
                // n�o efetiva
                //valueBar += (attacksPlayer[i].dmg + modificadorPlayer) / 2;
                StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer / 2));
                stamina += attacksPlayer[i].costStm;
                if (attacksPlayer[i].costStm < 0)
                {
                    //muda a cor da stamnia para vermelho
                    HudBattleManager.Instance.textStm.color = Color.red;
                }
                else if (attacksPlayer[i].costStm > 0)
                {
                    //muda para verde
                    HudBattleManager.Instance.textStm.color = Color.green;
                }
                StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
                yield return new WaitForSeconds(1.5f);
             
                HudBattleManager.Instance.textStm.color = Color.white;

                HudBattleManager.Instance.textGeral.text = "Esta a��o n�o foi efetiva";

            }
            else if (enemyAtributes.invunerable.Contains(attacksPlayer[i]))
            {
                //invuneravel
                stamina += attacksPlayer[i].costStm;
                if (attacksPlayer[i].costStm > 0)
                {
                    //muda a cor da stamnia para vermelho
                    HudBattleManager.Instance.textStm.color = Color.red;
                }
                else if (attacksPlayer[i].costStm < 0)
                {
                    //muda para verde
                    HudBattleManager.Instance.textStm.color = Color.green;
                }
                StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
                yield return new WaitForSeconds(1.5f);
                
                HudBattleManager.Instance.textStm.color = Color.white;
                HudBattleManager.Instance.textGeral.text = "Esta a��o n�o fez nada";
            }
            else
            {
                //efetiva
                //valueBar += (attacksPlayer[i].dmg + modificadorPlayer);
                StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer));
                stamina += attacksPlayer[i].costStm;
                if (attacksPlayer[i].costStm < 0)
                {
                    //muda a cor da stamnia para vermelho
                    HudBattleManager.Instance.textStm.color = Color.red;
                }
                else if (attacksPlayer[i].costStm > 0)
                {
                    //muda para verde
                    HudBattleManager.Instance.textStm.color = Color.green;
                }
                StartCoroutine( HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
                yield return new WaitForSeconds(1.5f);
              
                HudBattleManager.Instance.textStm.color = Color.white;
                HudBattleManager.Instance.textGeral.text = "Esta a��o foi efetiva";

            }
            modificadorPlayer = 0; //Resetar o modifcador do dano
        }
        else if (attacksPlayer[i].typeAction == "BuffOrDebuffAction") //A��o que modifica o valor do proximo turno
        {
            if (attacksPlayer[i].buffPlayer)
            {
                modificadorPlayer = attacksPlayer[i].modificadorBuffOrDebuff; //Modifica o valor do turno do jogador
                stamina += attacksPlayer[i].costStm;
            }
            else //Modifica o valor do turno do inimigo
            { 
                modificadorEnemy = attacksPlayer[i].modificadorBuffOrDebuff; 
                stamina += attacksPlayer[i].costStm; 
            }

            if (attacksPlayer[i].costStm < 0)
            {
                //muda a cor da stamnia para vermelho
                HudBattleManager.Instance.textStm.color = Color.red;
            }
            else if (attacksPlayer[i].costStm > 0)
            {
                //muda para verde
                HudBattleManager.Instance.textStm.color = Color.green;
            }

            StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));

            yield return new WaitForSeconds(1.5f);
            HudBattleManager.Instance.textStm.color = Color.white;
            HudBattleManager.Instance.textGeral.text = "Esta a��o acontecera no proximo turno";
        }
        else if (attacksPlayer[i].typeAction == "WaintingTurns")
        {
            
            if (turnsForTimingAction == maxTurnsForAction && startTimingAction)
            {
              
               
                if (enemyAtributes.superEffective.Contains(attacksPlayer[i]))
                {
                    // super efetiva
                    //valueBar += (attacksPlayer[i].dmg + modificadorPlayer) * 100;
                    StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer * 100));
                    HudBattleManager.Instance.textGeral.text = "Voce " + attacksPlayer[i].fraseAction[Random.Range(0, attacksPlayer[i].fraseAction.Count)];
                    yield return new WaitForSeconds(1.5f);
                    HudBattleManager.Instance.textStm.color = Color.white;
                    HudBattleManager.Instance.textGeral.text = "Esta a��o foi muito efetiva";

                }
                else if (enemyAtributes.noEffective.Contains(attacksPlayer[i]))
                {
                    // n�o efetiva
                    //valueBar += (attacksPlayer[i].dmg + modificadorPlayer) / 2;
                    StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer / 2));

                    HudBattleManager.Instance.textGeral.text = "Voce " + attacksPlayer[i].fraseAction[Random.Range(0, attacksPlayer[i].fraseAction.Count)];
                    yield return new WaitForSeconds(1.5f);
                    HudBattleManager.Instance.textStm.color = Color.white;
                    HudBattleManager.Instance.textGeral.text = "Esta a��o n�o foi efetiva";

                }
                else if (enemyAtributes.invunerable.Contains(attacksPlayer[i]))
                {
                    //invuneravel
                                  
                    HudBattleManager.Instance.textGeral.text = "Voce " + attacksPlayer[i].fraseAction[Random.Range(0, attacksPlayer[i].fraseAction.Count)];
                    yield return new WaitForSeconds(1.5f);
                    HudBattleManager.Instance.textStm.color = Color.white;
                    HudBattleManager.Instance.textGeral.text = "Esta a��o n�o fez nada";
                }
                else
                {
                    //efetiva
                    //valueBar += (attacksPlayer[i].dmg + modificadorPlayer);
                    StartCoroutine(BarValueAnimation(attacksPlayer[i].dmg + modificadorPlayer));
                    HudBattleManager.Instance.textGeral.text = "Voce " + attacksPlayer[i].fraseAction[Random.Range(0, attacksPlayer[i].fraseAction.Count)];
                    yield return new WaitForSeconds(1.5f);
                    HudBattleManager.Instance.textStm.color = Color.white;
                    HudBattleManager.Instance.textGeral.text = "Esta a��o foi efetiva";

                }
                modificadorPlayer = 0; //Resetar o modifcador do dano
                startTimingAction = false;


            }
            else {
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
            if (attacksPlayer[i].costStm < 0)
            {
                //muda a cor da stamnia para vermelho
                HudBattleManager.Instance.textStm.color = Color.red;
            }
            else if (attacksPlayer[i].costStm > 0)
            {
                //muda para verde
                HudBattleManager.Instance.textStm.color = Color.green;
            }

            StartCoroutine(HudBattleManager.Instance.animationAttack(attacksPlayer[i], true));
            yield return new WaitForSeconds(1.5f);

            HudBattleManager.Instance.textStm.color = Color.white;
            HudBattleManager.Instance.textGeral.text = "Esta a��o foi muito efetiva";
        }
       
        yield return new WaitForSeconds(2f);


        if (valueBar <= valueWinPlayer)
            {
            
            if (enemyAtributes.haveDialogueNext == true)
            {
                PassInfos.Instance.DialogueScriptable = enemyAtributes.nextDialogue;
                PassInfos.Instance.startDialogue =true;
            }
            
            TransitionSceneManager.Instance.Transition(enemyAtributes.nextScene);

            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyAttack(Random.Range(0, enemyAction.Count)));
            }

        


    }

   

    IEnumerator  EnemyAttack(int i)
    {

       
     
        if (enemyAction[i].typeAction == "Normal" )
        { 
            //valueBar -= enemyAction[i].dmg + modificadorEnemy;

            StartCoroutine(BarValueAnimation(enemyAction[i].dmg + modificadorEnemy));
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
            audioSource.clip = enemyAction[i].audioClip;
            audioSource.Play();
        }
        else if (enemyAction[i].typeAction == "Agrupamento")
        {
            if (turnsForTimingActionEnemy == maxTurnsForActionEnemy && startTimingActionEnemy)
            {
                Debug.Log("Deu certo");
                enemyAction.Clear();
                enemyAction.Add(enemyAtributes.actionBlockEnemy);
                startTimingActionEnemy = false;
                
            }
            else if (!startTimingActionEnemy)
            {
                Debug.Log("Come�ou");
                turnsForTimingActionEnemy = 0;
                startTimingActionEnemy = true;
                maxTurnsForActionEnemy = enemyAction[i].turnsForTimingActionEnemy;
                TimingActionObjectEnemy = i;
            }
            else
            {
                //valueBar -= enemyAction[i].dmg + modificadorEnemy;
                StartCoroutine(BarValueAnimation(enemyAction[i].dmg + modificadorEnemy));
                HudBattleManager.Instance.textGeral.text = enemyAction[1].fraseAction[Random.Range(0, enemyAction[i].fraseAction.Count)];
                audioSource.clip = enemyAction[i].audioClip;
                audioSource.Play();
            }
        }
        else if (enemyAction[i].typeAction == "Roubo")
        {


            stamina -= 5 ;
            HudBattleManager.Instance.textStm.color = Color.red;
            StartCoroutine(BarValueAnimation(enemyAction[i].dmg + modificadorEnemy));
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
            audioSource.clip = enemyAction[i].audioClip;
            audioSource.Play();
        }
        else if (enemyAction[i].typeAction == "BuffOrDebuffAction")
        {
            if (enemyAction[i].buffPlayer)
            {
                modificadorPlayer = -enemyAction[i].modificadorBuffOrDebuff;             
            }
            else 
            {
                modificadorEnemy = enemyAction[i].modificadorBuffOrDebuff;
            }
            //valueBar -= enemyAction[i].dmg + modificadorEnemy;
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
            audioSource.clip = enemyAction[i].audioClip;
            audioSource.Play();
        }
        

        yield return new WaitForSeconds(2f);
        HudBattleManager.Instance.textStm.color = Color.white;
        modificadorEnemy = 0;
        if (valueBar >= valueWinEnemy)
        {
            HudBattleManager.Instance.loseScreen.SetActive(true);
            yield return new WaitForSeconds(1f);
            PassInfos.Instance.DialogueScriptable = enemyAtributes.dialogueDerrota;
            TransitionSceneManager.Instance.Transition(enemyAtributes.sceneDerrota);
        }
        else
        {
            state = BattleState.PLAYERTURN;
            StartCoroutine(SetupBattle());
        }
    }

    public void OnAttackPlayer(int i)
    {
        if (state == BattleState.PLAYERTURN)
        {
            if (stamina >= -(attacksPlayer[i].costStm))
            {
                audioSource.clip = attacksPlayer[i].audioClip;
                audioSource.Play();
                HudBattleManager.Instance.startTextStep();
                StartCoroutine(PlayerAttack(i));
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
            valueBar += 0.5f;
            cout++;
        }
            if (cout >= damage)
            {
                valueBar++;
                yield break;
            }
        }
        else if (0 > damage)
        {
            while (cout > damage)
            {
                yield return new WaitForSeconds(0.1f);
                valueBar -= 0.5f;
                cout--;
            }
            if (cout <= damage)
            {
                valueBar--;
                yield break;
            }
        }

        
    }

}