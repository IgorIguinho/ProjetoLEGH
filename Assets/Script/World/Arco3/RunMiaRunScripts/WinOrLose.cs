using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLose : MonoBehaviour
{
    public bool isWin;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isWin) 
            {
                SceneManager.LoadScene("WorldATO3 5.1");
            }
            else 
            {
                SceneManager.LoadScene("RunMiaRun");
            }
        }
    }
}
