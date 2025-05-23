using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HackHack : MonoBehaviour
{
    public DialogueScriptable dialogue;
    public EnemysScriptable enemys;
    public GameObject inputFlied;
    public AttackScriptable attack;
    public string scene;
    bool eita;

    [Header("Configurações")]
    [SerializeField] private KeyCode screenshotKey = KeyCode.F12;
    [SerializeField] private int resolutionMultiplier = 1;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightShift) )
        {
            nextScene(scene);
        }
        // Pressione F12 para tirar screenshot
        if (Input.GetKeyDown(screenshotKey))
        {
            TakeScreenshot();
        }

    }
        public void nextScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }





    public void TakeScreenshot()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        // Espera o final do frame para capturar
        yield return new WaitForEndOfFrame();

        // Nome do arquivo com data e hora
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

        // Captura a tela
        ScreenCapture.CaptureScreenshot(fileName, resolutionMultiplier);

        Debug.Log("Screenshot salvo: " + fileName);
    }
}

