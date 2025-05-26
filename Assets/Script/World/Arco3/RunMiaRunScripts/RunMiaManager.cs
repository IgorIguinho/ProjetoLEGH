using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunMiaManager : MonoBehaviour
{
    public static RunMiaManager Instance { get; private set; }
    public GameObject leleGameObjects; // Jogador
    public GameObject miaGameObject;   // Inimigo
    public GameObject finalTarget;

    public float speedLele = 5f;       // Velocidade da Lele (jogador)
    public float baseMiaSpeed = 5.2f;  // Velocidade base da Mia (ligeiramente maior que Lele)
    public float playerSpeedIncrement = 0.5f; // Quanto a velocidade do jogador aumenta a cada clique
    public float maxPlayerSpeed = 15f;
    public float miaSpeedFactor = 1.1f; // Mia ser� sempre este fator mais r�pida que o jogador

    public Image spaceImage;
    public Sprite pressedSpace;
    public Sprite nonPressedSpace;

    private Rigidbody2D leleRb;
    private Rigidbody2D miaRb;

    private float timeSinceLastPress = 0f;
    public float speedDecayRate = 0.5f; // Taxa de diminui��o da velocidade se n�o pressionar espa�o

    private void Awake()
    {
        // Se existe uma inst�ncia e n�o � esta, destrua esta
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Obt�m os componentes uma �nica vez para melhorar a performance
        leleRb = leleGameObjects.GetComponent<Rigidbody2D>();
        miaRb = miaGameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Aumenta o timer desde o �ltimo pressionar
        timeSinceLastPress += Time.deltaTime;

        // Controla a entrada do jogador
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceImage.sprite = pressedSpace;
            // Aumenta a velocidade at� o m�ximo
            if (speedLele < maxPlayerSpeed)
            {
                speedLele += playerSpeedIncrement;
            }
            timeSinceLastPress = 0f; // Reseta o timer
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            spaceImage.sprite = nonPressedSpace;
        }

        // Se o jogador n�o pressionar espa�o por um tempo, diminui a velocidade gradualmente
        if (timeSinceLastPress > 1.5f && speedLele > 5f)
        {
            speedLele -= speedDecayRate * Time.deltaTime;
        }

        // Move os personagens
        MoveCharacters();
    }

    void MoveCharacters()
    {
        // Define a velocidade diretamente (mais consistente que AddForce para este caso)
        Vector2 leleVelocity = new Vector2(speedLele, leleRb.velocity.y);
        Vector2 miaVelocity = new Vector2(baseMiaSpeed, miaRb.velocity.y);

        leleRb.velocity = -leleVelocity;
        miaRb.velocity = -miaVelocity;
    }
}