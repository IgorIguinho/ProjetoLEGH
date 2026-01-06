using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiaFollowLele : MonoBehaviour
{
    public Transform Target;
    GameObject playerObject;
    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;
    Animator animator;

    float distance;
    Vector2 directionMia;
    Vector2 lastPosition;

    float horizontal;
    float vertical;
    int lastHorizontal;
    int lastVertical;

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        Target = playerObject.transform;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        directionMia = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        distance = Vector2.Distance(transform.position, Target.position);

        Vector2 currentPosition = transform.position;

        if (distance > 1.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, Target.position, 6.5f * Time.deltaTime);
        }
        else if (distance > 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, Target.position, 4.5f * Time.deltaTime);
        }

        // Calcular direção do movimento
        Vector2 movementDirection = (Vector2)transform.position - lastPosition;

        // Normalizar para pegar horizontal e vertical
        if (movementDirection.magnitude > 0.01f) // Se está se movendo
        {
            horizontal = movementDirection.x;
            vertical = movementDirection.y;

            // Determinar direção prioritária
            if (Mathf.Abs(vertical) > Mathf.Abs(horizontal))
            {
                lastVertical = vertical > 0 ? 1 : -1;
                lastHorizontal = 0;
            }
            else if (Mathf.Abs(horizontal) > 0.01f)
            {
                lastHorizontal = horizontal > 0 ? 1 : -1;
                lastVertical = 0;
            }

            // Animações de movimento
            if (vertical > 0 && Mathf.Abs(vertical) > Mathf.Abs(horizontal))
            {
                animator.Play("Walk_CimaParecida");
                spriteRenderer.flipX = false;
            }
            else if (vertical < 0 && Mathf.Abs(vertical) > Mathf.Abs(horizontal))
            {
                animator.Play("Walk_BaixoParecida");
                spriteRenderer.flipX = false;
            }
            else if (horizontal > 0)
            {
                animator.Play("Walk_LadoParecida");
                spriteRenderer.flipX = false;
            }
            else if (horizontal < 0)
            {
                animator.Play("Walk_LadoParecida");
                spriteRenderer.flipX = true;
            }
        }
        else // Se está parado (idle)
        {
            if (lastVertical > 0)
            {
                animator.Play("IdleCima_Parecida");
                spriteRenderer.flipX = false;
            }
            else if (lastVertical < 0)
            {
                animator.Play("Idle_Parecida");
                spriteRenderer.flipX = false;
            }
            else if (lastHorizontal > 0)
            {
                animator.Play("IdleLad_Parecida");
                spriteRenderer.flipX = false;
            }
            else if (lastHorizontal < 0)
            {
                animator.Play("IdleLad_Parecida");
                spriteRenderer.flipX = true;
            }
        }

        lastPosition = currentPosition;
    }
}