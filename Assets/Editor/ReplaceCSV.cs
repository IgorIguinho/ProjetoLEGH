using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UpdateDialogueScriptables : EditorWindow
{
    public List<TextAsset> csvFiles = new List<TextAsset>(); // Lista de arquivos CSV
    public List<DialogueScriptable> dialogueScriptables = new List<DialogueScriptable>(); // Lista de DialogueScriptables para atualizar
    [SerializeField] private List<Sprite> images = new List<Sprite>(); // Lista de sprites carregados
    public string spritesFolderPath = "Assets/Resources/Sprites"; // Caminho da pasta de sprites

    private Vector2 csvScrollPosition;
    private Vector2 dialogueScrollPosition;

    [MenuItem("Tools/Update Dialogue Scriptables")]
    public static void OpenWindow()
    {
        GetWindow<UpdateDialogueScriptables>("Update Dialogue Scriptables");
    }

    private void OnGUI()
    {
        GUILayout.Label("Update Dialogue Scriptables from CSV", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        // Campo para definir o caminho dos sprites
        spritesFolderPath = EditorGUILayout.TextField("Sprite Folder Path", spritesFolderPath);

        // Botão para recarregar a base de sprites
        if (GUILayout.Button("Reload Sprite Database"))
        {
            ReloadSpriteDatabase();
        }

        GUILayout.Label($"Sprites carregados: {images.Count}");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // Seção para CSV Files
        GUILayout.Label("CSV Files:", EditorStyles.boldLabel);

        csvScrollPosition = EditorGUILayout.BeginScrollView(csvScrollPosition, GUILayout.Height(150));

        // Botões para adicionar/remover elementos da lista de CSV
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add CSV Slot"))
        {
            csvFiles.Add(null);
        }
        if (GUILayout.Button("Remove Last CSV"))
        {
            if (csvFiles.Count > 0)
                csvFiles.RemoveAt(csvFiles.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        // Exibe os campos para cada CSV
        for (int i = 0; i < csvFiles.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"CSV {i}:", GUILayout.Width(50));
            csvFiles[i] = (TextAsset)EditorGUILayout.ObjectField(csvFiles[i], typeof(TextAsset), false);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                csvFiles.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Seção para DialogueScriptables
        GUILayout.Label("Dialogue Scriptables:", EditorStyles.boldLabel);

        dialogueScrollPosition = EditorGUILayout.BeginScrollView(dialogueScrollPosition, GUILayout.Height(150));

        // Botões para adicionar/remover elementos da lista de DialogueScriptables
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Dialogue Slot"))
        {
            dialogueScriptables.Add(null);
        }
        if (GUILayout.Button("Remove Last Dialogue"))
        {
            if (dialogueScriptables.Count > 0)
                dialogueScriptables.RemoveAt(dialogueScriptables.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        // Exibe os campos para cada DialogueScriptable
        for (int i = 0; i < dialogueScriptables.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Dialogue {i}:", GUILayout.Width(70));
            dialogueScriptables[i] = (DialogueScriptable)EditorGUILayout.ObjectField(dialogueScriptables[i], typeof(DialogueScriptable), false);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                dialogueScriptables.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // Informações sobre as listas
        GUILayout.Label($"Total CSV Files: {csvFiles.Count}");
        GUILayout.Label($"Total Dialogue Scriptables: {dialogueScriptables.Count}");

        EditorGUILayout.Space();


        // Botão principal para atualizar
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Update All Dialogue Scriptables", GUILayout.Height(40)))
        {
            UpdateAllDialogueScriptables();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space();
        //Botão para limpar ambas as listas
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Clean All List", GUILayout.Height(40)))
        {
            dialogueScriptables.Clear();
            csvFiles.Clear();
        }
        GUI.backgroundColor = Color.white;

    }

    private void ReloadSpriteDatabase()
    {
        if (string.IsNullOrEmpty(spritesFolderPath))
        {
            Debug.LogError("O caminho da pasta de sprites está vazio!");
            return;
        }

        Debug.Log($"Tentando carregar os sprites de: {spritesFolderPath}");

        // Busca todos os assets do tipo Sprite no caminho especificado
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesFolderPath });

        images = new List<Sprite>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                images.Add(sprite);
                Debug.Log($"Sprite carregado: {sprite.name}");
            }
            else
            {
                Debug.LogWarning($"Falha ao carregar o sprite de {assetPath}");
            }
        }

        Debug.Log($"Total de sprites carregados: {images.Count}");
    }

    private void UpdateAllDialogueScriptables()
    {
        // Validações iniciais
        if (csvFiles.Count == 0)
        {
            Debug.LogError("Nenhum arquivo CSV foi adicionado à lista!");
            return;
        }

        if (dialogueScriptables.Count == 0)
        {
            Debug.LogError("Nenhum DialogueScriptable foi adicionado à lista!");
            return;
        }

        // Reporta inconsistências de tamanho
        if (csvFiles.Count != dialogueScriptables.Count)
        {
            Debug.LogWarning($"Inconsistência detectada: {csvFiles.Count} CSV files vs {dialogueScriptables.Count} DialogueScriptables. Processando apenas os pares válidos.");
        }

        if (csvFiles.Count > dialogueScriptables.Count)
        {
            Debug.LogWarning($"Existem mais CSV files ({csvFiles.Count}) do que DialogueScriptables ({dialogueScriptables.Count}). Os CSV extras serão ignorados.");
        }

        if (dialogueScriptables.Count > csvFiles.Count)
        {
            Debug.LogWarning($"Existem mais DialogueScriptables ({dialogueScriptables.Count}) do que CSV files ({csvFiles.Count}). Os DialogueScriptables extras não serão atualizados.");
        }

        // Verifica se a lista de sprites foi carregada
        if (images == null || images.Count == 0)
        {
            Debug.LogWarning("A lista de sprites está vazia. Recarregue a base de sprites antes de continuar.");
            ReloadSpriteDatabase();
        }

        int processedCount = 0;
        int successCount = 0;
        int minCount = Mathf.Min(csvFiles.Count, dialogueScriptables.Count);

        // Processa cada par CSV/DialogueScriptable
        for (int i = 0; i < minCount; i++)
        {
            processedCount++;

            if (UpdateSingleDialogueScriptable(i))
            {
                successCount++;
            }
        }

        Debug.Log($"=== ATUALIZAÇÃO CONCLUÍDA ===");
        Debug.Log($"Total processado: {processedCount}");
        Debug.Log($"Total com sucesso: {successCount}");
        Debug.Log($"Total com erro: {processedCount - successCount}");

        // Salva as alterações
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private bool UpdateSingleDialogueScriptable(int index)
    {
        TextAsset csvFile = csvFiles[index];
        DialogueScriptable dialogue = dialogueScriptables[index];

        // Validações
        if (csvFile == null)
        {
            Debug.LogError($"CSV File no índice {index} é nulo!");
            return false;
        }

        if (dialogue == null)
        {
            Debug.LogError($"DialogueScriptable no índice {index} é nulo!");
            return false;
        }

        Debug.Log($"Iniciando atualização do par {index}: CSV '{csvFile.name}' -> DialogueScriptable '{dialogue.name}'");

        try
        {
            // Limpa completamente as listas existentes
            dialogue.thisIs?.Clear();
            dialogue.text?.Clear();
            dialogue.nameDialogue?.Clear();
            dialogue.imagePlayer?.Clear();
            dialogue.imageNPC?.Clear();

            // Inicializa as listas se forem nulas
            if (dialogue.thisIs == null) dialogue.thisIs = new List<string>();
            if (dialogue.text == null) dialogue.text = new List<string>();
            if (dialogue.nameDialogue == null) dialogue.nameDialogue = new List<string>();
            if (dialogue.imagePlayer == null) dialogue.imagePlayer = new List<Sprite>();
            if (dialogue.imageNPC == null) dialogue.imageNPC = new List<Sprite>();

            // Processa o CSV
            string[] lines = csvFile.text.Split('\n');
            int linesProcessed = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] row = ParseCSVLine(line);
                if (row.Length >= 5)
                {
                    // Adiciona dados nas listas
                    dialogue.thisIs.Add(row[0]);
                    dialogue.text.Add(row[1]);
                    dialogue.nameDialogue.Add(row[2]);

                    // Adiciona sprite do player
                    Sprite playerSprite = images.Find(image => image.name == row[3]);
                    if (playerSprite != null)
                    {
                        dialogue.imagePlayer.Add(playerSprite);
                    }
                    else
                    {
                        Debug.LogWarning($"Sprite para o Player '{row[3]}' não encontrado (linha {linesProcessed + 1}).");
                        dialogue.imagePlayer.Add(null);
                    }

                    // Adiciona sprite do NPC
                    Sprite npcSprite = images.Find(image => image.name == row[4]);
                    if (npcSprite != null)
                    {
                        dialogue.imageNPC.Add(npcSprite);
                    }
                    else
                    {
                        Debug.LogWarning($"Sprite para o NPC '{row[4]}' não encontrado (linha {linesProcessed + 1}).");
                        dialogue.imageNPC.Add(null);
                    }

                    linesProcessed++;
                }
                else
                {
                    Debug.LogWarning($"Linha ignorada no CSV '{csvFile.name}' (faltando colunas): {line}");
                }
            }

            // Marca o asset como "dirty" para salvar as mudanças
            EditorUtility.SetDirty(dialogue);

            Debug.Log($"✓ Atualização concluída com sucesso para o par {index}: {linesProcessed} linhas processadas");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao atualizar o par {index}: {e.Message}");
            return false;
        }
    }

    // Método para analisar uma linha do CSV corretamente (copiado do sistema original)
    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool insideQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"') // Alterna o estado das aspas
            {
                insideQuotes = !insideQuotes;
            }
            else if (c == ',' && !insideQuotes) // Divide a coluna fora das aspas
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c; // Adiciona o caractere atual à coluna
            }
        }

        // Adiciona a última coluna
        result.Add(current);

        return result.ToArray();
    }
}