using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovCamATO4 : MonoBehaviour
{
    GameObject player;
    private Vector3 targetTransform;

    [Header("Mapa geral")]
    public Transform limitY;
    public Transform limitYNegativo;
    public Transform limitX;
    public Transform limitXNegativo;

    public float smoothSpeed = 5f;
    public float defaultOrthographicSize = 5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetTransform = player.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(this.transform.position, new Vector3(targetTransform.x, targetTransform.y, -10), smoothSpeed * Time.deltaTime);
    }

    private void Update()
    {
        // Inicializa com a posição do jogador
        targetTransform = player.transform.position;
        gameObject.GetComponent<Camera>().orthographicSize = defaultOrthographicSize;

        // Verifica e aplica os limites X e Y separadamente
        // Isso permite que a câmera se mova livremente em um eixo enquanto está limitada no outro

        // Verifica limite X positivo
        if (player.transform.position.x > limitX.position.x)
        {
            targetTransform.x = limitX.position.x;
        }

        // Verifica limite X negativo
        if (player.transform.position.x < limitXNegativo.position.x)
        {
            targetTransform.x = limitXNegativo.position.x;
        }

        // Verifica limite Y positivo
        if (player.transform.position.y > limitY.position.y)
        {
            targetTransform.y = limitY.position.y;
        }

        // Verifica limite Y negativo
        if (player.transform.position.y < limitYNegativo.position.y)
        {
            targetTransform.y = limitYNegativo.position.y;
        }
    }
}