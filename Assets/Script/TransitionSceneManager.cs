using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSceneManager : MonoBehaviour
{
    public static TransitionSceneManager Instance { get; private set; }

    [SerializeField] private Animator animator;

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

    public void Transition(string sceneName)
    {
        StartCoroutine(StartAnimation(sceneName));
    }

    IEnumerator StartAnimation(string sceneName)
    {
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
        animator.SetTrigger("FadeOut");
        if (PassInfos.Instance.learningAction)
        {
            StartCoroutine(LearningActionWarning.Instance.LearnAction(PassInfos.Instance.actionToLearning));
            PassInfos.Instance.learningAction = false;
        }

    }
}
