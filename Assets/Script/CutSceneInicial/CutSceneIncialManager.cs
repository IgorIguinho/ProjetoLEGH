using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutSceneIncialManager : MonoBehaviour
{
    public float cooldown;
    public int activeImage;
    public Image image;
    public List <Sprite> sprites;
    public AudioSource audioSource;
    public List<AudioClip> clip;
    public Button buttonNextImage;
    public float numberToSkip = 0;
    public float plusInCountSkip ;
    public TextMeshProUGUI textMeshProUGUI;
    public Slider sliderToSkip1;
    public Slider sliderToSkip2;
    bool canFadeInSpaceWarning = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NextImage());
        StartCoroutine(FadeSpace());


    }

    private void Update()
    {
        if (activeImage >= sprites.Count || numberToSkip > 10) { SceneManager.LoadScene("StartWorldProto"); }
        

        sliderToSkip1.value = numberToSkip;
        sliderToSkip2.value = numberToSkip;
        sliderToSkip1.GetComponentInChildren<Image>().color = new Color(170, 1, 133, numberToSkip);
        sliderToSkip2.GetComponentInChildren<Image>().color = new Color(170, 1, 133, numberToSkip);
        if (canFadeInSpaceWarning) { textMeshProUGUI.color = new Color(1, 1, 1, numberToSkip); }
        if (Input.GetKey(KeyCode.Space))
        {
            numberToSkip += plusInCountSkip;
            canFadeInSpaceWarning = true;
        }
        else if(numberToSkip > 0)
        {
            numberToSkip -= plusInCountSkip;
        }

        }

    public IEnumerator NextImage()
    {

        buttonNextImage.gameObject.SetActive(false);
        activeImage++;
        StartCoroutine(fade(true));
        yield return new WaitForSeconds(cooldown);
        image.sprite = sprites[activeImage];
        StartCoroutine(fade(false));
        if (clip[activeImage] != null)
        {
            audioSource.clip = clip[activeImage];
            audioSource.Play();
        }

        yield return new WaitForSeconds(cooldown);

        buttonNextImage.gameObject.SetActive(true);
        yield return null;
    }

    public void StartButtonCoroutine()
    {
       StartCoroutine(NextImage());
        Debug.Log("Botao aperdainho");
    }

    public void SkipCutscene()
    {
        activeImage = sprites.Count - 1;
        StartCoroutine(NextImage());
    }
 

    IEnumerator FadeSpace()
    {
        yield return new WaitForSeconds(cooldown);
        // loop over 1 second
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            // set color with i as alpha
            textMeshProUGUI.color = new Color(1, 1, 1, i);
            yield return null;
        }
        yield return new WaitForSeconds(cooldown);
        // loop over 1 second backwards
        for (float i = 1; i > 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            textMeshProUGUI.color = new Color(1, 1, 1, i);
           
        }
       

        yield return null;
    }
    IEnumerator fade(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i > 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                image.color = new Color(1, 1, 1, i);
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
                image.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}
