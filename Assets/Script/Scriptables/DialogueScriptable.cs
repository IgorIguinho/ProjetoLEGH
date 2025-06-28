using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "dialogueScriptable", menuName = "DialogueScriptables")]
public class DialogueScriptable : ScriptableObject
{
    public List<string> text;
    public List<string>  thisIs;
    public List<Sprite> imagePlayer;
    public List<Sprite> imageNPC;
    public List<string> nameDialogue;
    public bool canSkip;
    [Header("Cutscene")]
    public bool haveCutscene;
    public List<PlayableAsset> cutScene;
    public List<int> numberCutScene;
    [Header("Battle")]
    public string battleScene;
    public bool isBattle;
    public EnemysScriptable enemy;
    [Header("NextScene")]
    public bool isNextScene;
    public string nextScene;
    [Header("LearnAction")]
    public bool learnAction;
    public AttackScriptable attackScriptable;
    [Header("DialoguePuzze")]
    public bool havePuzzle;
    public PuzzleDialogueScriptable puzzleDialogueScriptable;
    public int numberActivePuzzle;

}
