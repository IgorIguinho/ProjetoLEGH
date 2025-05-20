using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LearningActionWarning : MonoBehaviour
{

    public static LearningActionWarning Instance { get; private set; }


    public GameObject warningGroup;
    public TextMeshProUGUI textAction;
    public List<GameObject> warningListText;
    public Color warningImage;


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


    private void Start()
    {
    
    }

    public void Update()
    {
        Color originalColorWarning = warningGroup.GetComponent<Image>().color;
        warningGroup.GetComponent<Image>().color = new Color(originalColorWarning.r,originalColorWarning.g ,originalColorWarning.b , warningImage.a);

        Color ogCorlorText0 = warningListText[0].GetComponent<TextMeshProUGUI>().color;
        warningListText[0].GetComponent<TextMeshProUGUI>().color = new Color(ogCorlorText0.r, ogCorlorText0.g, ogCorlorText0.b, warningImage.a);

        Color ogCorlorText1 = warningListText[1].GetComponent<TextMeshProUGUI>().color;
        warningListText[1].GetComponent<TextMeshProUGUI>().color = new Color(ogCorlorText1.r, ogCorlorText1.g, ogCorlorText1.b, warningImage.a);
    }

    public IEnumerator LearnAction(AttackScriptable actionScriptable)
    {
        actionScriptable.learned = true;
        textAction.text = actionScriptable.nameTitle;
        StartCoroutine(fade(false));
        yield return new WaitForSeconds(3);
        StartCoroutine(fade(true));



    }


    IEnumerator fade(bool fadeAway )
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i > 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                warningImage = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                warningImage = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }


}
