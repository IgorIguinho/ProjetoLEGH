using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;


public class StartCutscene : MonoBehaviour
{
    PlayableDirector director;
    public PlayableAsset cutScene;
    public bool isPlay = false;
    

    // Start is called before the first frame update
    void Start()
    {
        director = GameObject.FindGameObjectWithTag("DirectorCutscene").GetComponent<PlayableDirector>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            director.playableAsset = cutScene;
            if (isPlay)
            {
            
                director.Play();
            }
        }
    }
}
