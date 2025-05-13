using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAtribute", menuName = "EnemyAtribute")]
public class EnemysScriptable : ScriptableObject
{
  public List<AttackScriptable> AttackScripts;
    [Header("Efetividade ações do player")]
  public List<AttackScriptable> superEffective;
  public List<AttackScriptable> noEffective;
  public List<AttackScriptable> invunerable;
    [Space(4)]
    public AudioClip musicBattle;
    public Sprite spriteBattle;
    public AttackScriptable actionBlockEnemy;
    public AnimationClip animationBattle;

    [Header("Pós batalha/Vitoria")]
    public bool haveDialogueNext;
    public DialogueScriptable nextDialogue;
    public string nextScene;
    public bool haveLearAction;
    public AttackScriptable actionToLearn;



    [Header("Pós batalha/Derrota")]
    public DialogueScriptable dialogueDerrota;
    public string sceneDerrota;
}
