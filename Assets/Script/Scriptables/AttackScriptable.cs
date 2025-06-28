using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum OperacaoMatematica 
{ 
    Soma,
    Subtracao,
    Multiplicacao,
    Divisao
}

[Serializable]
public struct ModificadorBuffDebuff
{
    public OperacaoMatematica operacao;
    public float valor;
}
[CreateAssetMenu(fileName = "AttackStats", menuName = "Actions")]
public class AttackScriptable : ScriptableObject
{

    public string nameTitle;
    public string useCombat;
    public string combatDescr;
    public List<string> fraseAction;
    public float dmg;
    public float costStm;
    public bool learned;
    [Space(2)]
    public AudioClip audioClip;
    
    [Header("Type action....")]
    public string typeAction;
    public string useCombatForWaitingAction;
    public bool buffPlayer;
    public ModificadorBuffDebuff modificadorBuffOrDebuff;
    //player
    public int turnsForTimingAction;

    //enemy
    [Header("Enemy type action....")]
    public int turnsForTimingActionEnemy;
}
