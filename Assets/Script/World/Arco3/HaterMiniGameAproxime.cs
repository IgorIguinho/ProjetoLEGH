using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class HaterMiniGameAproxime : MonoBehaviour
{
    public GameObject storeButtonWarining;
    public GameObject storeWarning;
    public GameObject canvaObject;
    public GameObject miniGame;

    GameObject playerObject;
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector2.Distance(transform.position, playerObject.transform.position);


        
            if (dist < 2.5f)
            {
                storeButtonWarining.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    storeWarning.SetActive(false);
                    storeButtonWarining.SetActive(false);
                    miniGame.SetActive(true);
                    gameObject.GetComponent<HaterMiniGameAproxime>().enabled = false;
                HaterMiniGameManager.Instance.startMiniGame = true;
                }
            }
            else
            {
             storeButtonWarining.SetActive(false);
            }
        }
}
