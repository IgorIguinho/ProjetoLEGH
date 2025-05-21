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

       
        enemyAtributes = PassInfos.Instance.enemyToPass;    // Puxa os atributos do inimigo  
        HudBattleManager.Instance.imageEnemy.GetComponent<Animator>().Play(enemyAtributes.animationBattle.name);   
       
        enemyAction = new List<AttackScriptable>(enemyAtributes.AttackScripts);
        soundTrack.clip = enemyAtributes.musicBattle;
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
    }

    IEnumerator SetupBattle()
    {


        HudBattleManager.Instance.textGeral.text = "Sua A��o";


        yield return new WaitForSeconds(2f);
        if (startTimingAction == true)
        {
            if (turnsForTimingAction < maxTurnsForAction) //Somando o turno para a��o de preparo
            {
                turnsForTimingAction++;
            }
            else //A��es timing acontece
            {
               
                StartCoroutine(PlayerAttack(TimingActionObject));// Chama a a��o do jogador que estava em preparo
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

                StartCoroutine(EnemyAttack(TimingActionObjectEnemy)); // Chama a a��o do inimmgo que estava em preparo
                state = BattleState.ENEMYTURN;
                yield break;

            }
        }

        state = BattleState.PLAYERTURN;
  
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
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false)); //Puxa a anima��o do ataque do inimigo
            modificadorEnemy = 0;
        }
        else if (enemyAction[i].typeAction == "Agrupamento") // Ataque que vai juntar inimigos para a batalha, demora x turnos pra funcionar
        {
            if (turnsForTimingActionEnemy == maxTurnsForActionEnemy && startTimingActionEnemy)//aviso que o roximo turno � o ataque
            {
                StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false)); //Puxa a anima��o do ataque do inimigo
                //Debug.Log("Deu certo");
                enemyAction.Clear();
                enemyAction.Add(enemyAtributes.actionBlockEnemy);
                startTimingActionEnemy = false;
             

            }
            else if (!startTimingActionEnemy) //� o come�o do ataque, assim que o inimigo usa faz isso:
            {
                StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false)); 
                //Debug.Log("Come�ou");
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
        else if (enemyAction[i].typeAction == "Roubo") //A��o de roubar estamina do jogador
        {
            stamina -= 5 ;
            HudBattleManager.Instance.textStm.color = Color.red;
            StartCoroutine(BarValueAnimation(enemyAction[i].dmg + modificadorEnemy)); modificadorEnemy = 0;
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
        }
        else if (enemyAction[i].typeAction == "BuffOrDebuffAction") //A��o de bufar a proxima a��o do inimigo ou debuff na a��o do player
        {
            if (enemyAction[i].buffPlayer)
            {
                modificadorPlayer = -enemyAction[i].modificadorBuffOrDebuff;// Modifica a a��o do player         
            }
            else 
            {
                modificadorEnemy = enemyAction[i].modificadorBuffOrDebuff;//Modifica a a��o do inimigo
            }
            
            StartCoroutine(HudBattleManager.Instance.animationAttack(enemyAction[i], false));
            
        }
        

        yield return new WaitForSeconds(2f);
        HudBattleManager.Instance.textStm.color = Color.white;
        
        if (valueBar >= valueWinEnemy) //Condi��o de perda
        {
            HudBattleManager.Instance.loseScreen.SetActive(true);
            yield return new WaitForSeconds(1f);
            PassInfos.Instance.DialogueScriptable = enemyAtributes.dialogueDerrota;
            TransitionSceneManager.Instance.Transition(enemyAtributes.sceneDerrota); // muda pra cena de derrota
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