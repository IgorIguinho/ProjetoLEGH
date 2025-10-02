using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalDemo : MonoBehaviour
{

    bool isCoroutineRun;
    bool startCoroutine;
    public Text textGeral;
    public Text textGeral2;
    public List<string> dialogueList;
    public char[] ctr;
    public Image loadingImage;

    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(machineText());
        ctr = dialogueList[0].ToCharArray();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator machineText()
    {
        ctr = dialogueList[0].ToCharArray();
        yield return new WaitForSeconds(1f);
        int cout = 0;


        while (cout < ctr.Length)
        {
            yield return new WaitForSeconds(0.05f);
            textGeral.text += ctr[cout];
            cout++;
        }
        if (cout >= ctr.Length)
        {
            yield return new WaitForSeconds(1f);
            ctr = dialogueList[1].ToCharArray();
            StartCoroutine(machineText2());
            yield break;
        }
    }
    IEnumerator machineText2()
    {

        int cout = 0;


        while (cout < ctr.Length)
        {
            yield return new WaitForSeconds(0.05f);
            textGeral2.text += ctr[cout];
            cout++;
        }
        if (cout >= ctr.Length)
        {
            loadingImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(5f);

            SceneManager.LoadScene(sceneName);
        }
    }
}
