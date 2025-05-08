using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoMouseController : MonoBehaviour
{
    [Header("Cursor Visual")]
    public GameObject cursorVisual; // Objeto que representa visualmente o cursor na tela
    public Sprite normalCursorSprite; // Sprite para o cursor normal
    public Sprite clickCursorSprite; // Sprite para quando o cursor clica

    [Header("Cursor Actions")]
    [SerializeField] private List<CursorAction> predefinedActions = new List<CursorAction>(); // Lista de a��es definidas no Inspector

    [Header("Settings")]
    public float clickAnimationDuration = 0.2f; // Dura��o da anima��o de clique
    public float movementDuration = 1.0f; // Dura��o da movimenta��o do cursor

    [Header("Buttons e minigame")] 
    public HaterMiniGame miniGame; 
    public Button likeButton; 
    public Button reportButton;

    private bool isControllingCursor = false;
    private Vector2 originalMousePosition;
    private Queue<CursorAction> actionQueue = new Queue<CursorAction>();
    private bool isProcessingActions = false;
    private Image cursorImage;

    private void Awake()
    {
        // Verificar se o cursor visual existe, se n�o, criar um
        if (cursorVisual == null)
        {
            cursorVisual = new GameObject("AutoCursor");
            cursorImage = cursorVisual.AddComponent<Image>();
            cursorImage.sprite = normalCursorSprite;
            cursorImage.rectTransform.sizeDelta = new Vector2(32, 32); // Tamanho padr�o do cursor

            // Certifique-se que este objeto est� no Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Canvas").AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.gameObject.AddComponent<CanvasScaler>();
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            cursorVisual.transform.SetParent(canvas.transform, false);
        }
        else if (cursorImage == null)
        {
            cursorImage = cursorVisual.GetComponent<Image>();
            if (cursorImage == null)
            {
                cursorImage = cursorVisual.AddComponent<Image>();
                if (normalCursorSprite != null)
                    cursorImage.sprite = normalCursorSprite;
            }
        }

        // Inicialmente esconde o cursor personalizado
        cursorVisual.SetActive(false);
    }

    /// <summary>
    /// Fun��o gatilho principal para iniciar a sequ�ncia de a��es do cursor definidas no Inspector
    /// </summary>
    public void StartAutomatedCursorSequence()
    {
        // Limpa qualquer a��o que possa estar na fila
        actionQueue.Clear();

        // Interrompe qualquer processo em andamento
        StopAllCoroutines();

        // Reseta o estado do controle
        isProcessingActions = false;
        isControllingCursor = false;

        // Adiciona todas as a��es predefinidas � fila
        foreach (var action in predefinedActions)
        {
            actionQueue.Enqueue(action);
        }

        // Inicia o processamento das a��es
        if (actionQueue.Count > 0)
        {
            StartCoroutine(ProcessCursorActions());
            Debug.Log($"Iniciando sequ�ncia automatizada com {predefinedActions.Count} a��es");
        }
        else
        {
            Debug.LogWarning("Nenhuma a��o de cursor foi definida no Inspector");
        }
    }

    private IEnumerator ProcessCursorActions()
    {
        isProcessingActions = true;

        // Guarda posi��o original do mouse e bloqueia o cursor do jogador
        originalMousePosition = Input.mousePosition;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Ativa o cursor visual personalizado
        cursorVisual.SetActive(true);

        // Coloca o cursor visual na posi��o atual do mouse
        cursorVisual.transform.position = originalMousePosition;

        while (actionQueue.Count > 0)
        {
            CursorAction currentAction = actionQueue.Dequeue();

            // Move o cursor visual para a posi��o alvo
            yield return StartCoroutine(MoveCursorVisually(cursorVisual.transform.position, currentAction.targetPosition.position, movementDuration));

            // Espera antes de realizar a a��o
            if (currentAction.delayBeforeAction > 0)
                yield return new WaitForSeconds(currentAction.delayBeforeAction);

            // Realiza clique se necess�rio
            if (currentAction.performClick)
            {
                yield return StartCoroutine(PerformClickAnimation(currentAction.targetPosition));
                // Aqui voc� poderia adicionar c�digo para simular a intera��o real com o jogo
                // Por exemplo, realizar um raycast na posi��o do cursor para detectar objetos clic�veis
                SimulateClickAtPosition(currentAction.targetPosition.position);
            }

            // Pausa ap�s a a��o
            if (currentAction.pauseAfterAction > 0)
                yield return new WaitForSeconds(currentAction.pauseAfterAction);
        }

        // Desativa o cursor visual personalizado
        cursorVisual.SetActive(false);

        // Restaura o controle do cursor ao jogador
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isProcessingActions = false;
        Debug.Log("Sequ�ncia automatizada de cursor conclu�da");
    }

    private IEnumerator MoveCursorVisually(Vector2 startPosition, Vector2 targetPosition, float duration)
    {
        isControllingCursor = true;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calcula a posi��o interpolada com suaviza��o
            float t = Mathf.Clamp01(elapsedTime / duration);
            float smoothT = t * t * (3f - 2f * t); // Fun��o smoothstep

            // Atualiza a posi��o do cursor visual
            cursorVisual.transform.position = Vector2.Lerp(startPosition, targetPosition, smoothT);

            yield return null;
        }

        // Garante que o cursor chegue exatamente na posi��o final
        cursorVisual.transform.position = targetPosition;

        isControllingCursor = false;
    }

    private IEnumerator PerformClickAnimation(Transform actualPositionMouse)
    {
        if ( actualPositionMouse.position == likeButton.transform.position )
        {
            likeButton.onClick.Invoke();
        }
        else if (actualPositionMouse.position == reportButton.transform.position)
        {
            reportButton.onClick.Invoke();
        }

        // Muda o sprite para o sprite de clique
        if (clickCursorSprite != null && cursorImage != null)
        {
            Sprite originalSprite = cursorImage.sprite;
            cursorImage.sprite = clickCursorSprite;

            // Pequena anima��o de escala para simular o clique
            Vector3 originalScale = cursorVisual.transform.localScale;
            cursorVisual.transform.localScale = originalScale * 0.8f;

            yield return new WaitForSeconds(clickAnimationDuration);

            // Retorna ao sprite e escala originais
            cursorImage.sprite = originalSprite;
            cursorVisual.transform.localScale = originalScale;
        }
        else
        {
            // Anima��o simples de escala se n�o houver sprite de clique
            Vector3 originalScale = cursorVisual.transform.localScale;
            cursorVisual.transform.localScale = originalScale * 0.8f;

            yield return new WaitForSeconds(clickAnimationDuration);

            cursorVisual.transform.localScale = originalScale;
        }
    }

    // M�todo para simular um clique na posi��o do cursor
    private void SimulateClickAtPosition(Vector2 position)
    {
        // Cria um evento de click para o sistema de UI da Unity
        if (EventSystem.current != null)
        {
            // Converte a posi��o para coordenadas de tela
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = position;

            // Lista para armazenar resultados do raycast
            List<RaycastResult> results = new List<RaycastResult>();

            // Executa o raycast para encontrar objetos interag�veis
            EventSystem.current.RaycastAll(pointerData, results);

            // Se encontrou algum objeto, simula o clique nele
            if (results.Count > 0)
            {
                GameObject hitObject = results[0].gameObject;
                ExecuteEvents.Execute(hitObject, pointerData, ExecuteEvents.pointerClickHandler);
                Debug.Log($"Clique simulado em: {hitObject.name}");
            }
        }

        // Alternativa: Usar Physics.Raycast para objetos 3D
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Verifica se o objeto tem algum componente clic�vel
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClick();
                Debug.Log($"Clique f�sico simulado em: {hit.collider.name}");
            }
        }
    }

    public bool IsControllingCursor()
    {
        return isControllingCursor || isProcessingActions;
    }

    // Cancelar a sequ�ncia atual se necess�rio
    public void CancelCurrentSequence()
    {
        StopAllCoroutines();
        actionQueue.Clear();

        // Restaura o controle do cursor ao jogador
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desativa o cursor visual
        if (cursorVisual != null)
            cursorVisual.SetActive(false);

        isProcessingActions = false;
        isControllingCursor = false;
    }
}

// Classe para definir a��es do cursor
[System.Serializable]
public class CursorAction
{
    public Transform targetPosition;
    public float delayBeforeAction = 0.5f;
    public bool performClick = true;
    public float pauseAfterAction = 1.0f;
}

// Interface para objetos clic�veis (implementar em GameObjects que devem responder a cliques)
public interface IClickable
{
    void OnClick();
}