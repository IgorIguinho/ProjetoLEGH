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

    // NOVOS CAMPOS ADICIONADOS
    public string csvFolderPath = "Assets/Data/CSV"; // Caminho da pasta de CSV
    public string dialogueFolderPath = "Assets/ScriptableObjects/Dialogues"; // Caminho da pasta de DialogueScriptables

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

        // NOVO: Campo e botão para auto-carregar CSVs
        csvFolderPath = EditorGUILayout.TextField("CSV Folder Path", csvFolderPath);
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Auto-Load CSV Files from Folder"))
        {
            AutoLoadCSVFiles();
        }
        GUI.backgroundColor = Color.white;

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
        for (int i = csvFiles.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"CSV {i}:", GUILayout.Width(50));

            // Garante que o índice existe antes de acessar
            if (i < csvFiles.Count)
            {
                csvFiles[i] = (TextAsset)EditorGUILayout.ObjectField(csvFiles[i], typeof(TextAsset), false);
            }

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                if (i < csvFiles.Count)
                {
                    csvFiles.RemoveAt(i);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Seção para DialogueScriptables
        GUILayout.Label("Dialogue Scriptables:", EditorStyles.boldLabel);

        // NOVO: Campo e botão para auto-carregar DialogueScriptables
        dialogueFolderPath = EditorGUILayout.TextField("Dialogue Folder Path", dialogueFolderPath);
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Auto-Load Dialogue Scriptables from Folder"))
        {
            AutoLoadDialogueScriptables();
        }
        GUI.backgroundColor = Color.white;

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
        for (int i = dialogueScriptables.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Dialogue {i}:", GUILayout.Width(70));

            // Garante que o índice existe antes de acessar
            if (i < dialogueScriptables.Count)
            {
                dialogueScriptables[i] = (DialogueScriptable)EditorGUILayout.ObjectField(dialogueScriptables[i], typeof(DialogueScriptable), false);
            }

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                if (i < dialogueScriptables.Count)
                {
                    dialogueScriptables.RemoveAt(i);
                }
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
        // Botão principal para Limpar as listas
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Clear all list", GUILayout.Height(40)))
        {
            dialogueScriptables.Clear();
            csvFiles.Clear();
        }
        GUI.backgroundColor = Color.white;
    }

    // NOVA FUNÇÃO: Auto-carregar arquivos CSV
    private void AutoLoadCSVFiles()
    {
        if (string.IsNullOrEmpty(csvFolderPath))
        {
            Debug.LogError("O caminho da pasta de CSV está vazio!");
            return;
        }

        Debug.Log($"Tentando carregar CSVs de: {csvFolderPath}");

        // Busca todos os assets do tipo TextAsset (CSV) no caminho especificado
        string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { csvFolderPath });

        // Limpa a lista atual
        csvFiles.Clear();

        int loadedCount = 0;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Verifica se o arquivo tem extensão .csv
            if (assetPath.ToLower().EndsWith(".csv"))
            {
                TextAsset csvAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (csvAsset != null)
                {
                    csvFiles.Add(csvAsset);
                    loadedCount++;
                    Debug.Log($"CSV carregado: {csvAsset.name}");
                }
                else
                {
                    Debug.LogWarning($"Falha ao carregar CSV de {assetPath}");
                }
            }
        }

        Debug.Log($"Auto-load concluído: {loadedCount} arquivos CSV carregados de {csvFolderPath}");

        if (loadedCount == 0)
        {
            Debug.LogWarning($"Nenhum arquivo CSV encontrado em {csvFolderPath}. Verifique se o caminho está correto e se existem arquivos .csv na pasta.");
        }
    }

    // NOVA FUNÇÃO: Auto-carregar DialogueScriptables
    private void AutoLoadDialogueScriptables()
    {
        if (string.IsNullOrEmpty(dialogueFolderPath))
        {
            Debug.LogError("O caminho da pasta de DialogueScriptables está vazio!");
            return;
        }

        Debug.Log($"Tentando carregar DialogueScriptables de: {dialogueFolderPath}");

        // Busca todos os assets do tipo DialogueScriptable no caminho especificado
        string[] guids = AssetDatabase.FindAssets("t:DialogueScriptable", new[] { dialogueFolderPath });

        // Limpa a lista atual
        dialogueScriptables.Clear();

        int loadedCount = 0;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            DialogueScriptable dialogueAsset = AssetDatabase.LoadAssetAtPath<DialogueScriptable>(assetPath);
            if (dialogueAsset != null)
            {
                dialogueScriptables.Add(dialogueAsset);
                loadedCount++;
                Debug.Log($"DialogueScriptable carregado: {dialogueAsset.name}");
            }
            else
            {
                Debug.LogWarning($"Falha ao carregar DialogueScriptable de {assetPath}");
            }
        }

        Debug.Log($"Auto-load concluído: {loadedCount} DialogueScriptables carregados de {dialogueFolderPath}");

        if (loadedCount == 0)
        {
            Debug.LogWarning($"Nenhum DialogueScriptable encontrado em {dialogueFolderPath}. Verifique se o caminho está correto e se existem arquivos DialogueScriptable na pasta.");
        }
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

            // CORREÇÃO PRINCIPAL: Filtra linhas vazias antes do processamento
            var validLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            Debug.Log($"Total de linhas no CSV: {lines.Length}");
            Debug.Log($"Linhas válidas (não vazias): {validLines.Length}");

            for (int lineIndex = 0; lineIndex < validLines.Length; lineIndex++)
            {
                string line = validLines[lineIndex];
                string[] row = ParseCSVLine(line);

                Debug.Log($"Processando linha {lineIndex + 1}: Colunas encontradas = {row.Length}");

                if (row.Length >= 5)
                {
                    // Log dos dados de cada linha
                    Debug.Log($"Linha {lineIndex + 1} - thisIs: '{row[0]}', text: '{row[1]}', name: '{row[2]}', playerSprite: '{row[3]}', npcSprite: '{row[4]}'");

                    // Adiciona dados nas listas
                    dialogue.thisIs.Add(row[0]);
                    dialogue.text.Add(row[1]);
                    dialogue.nameDialogue.Add(row[2]);

                    // Adiciona sprite do player
                    Sprite playerSprite = null;
                    if (!string.IsNullOrWhiteSpace(row[3]))
                    {
                        playerSprite = images.Find(image => image.name == row[3].Trim());
                        if (playerSprite == null)
                        {
                            Debug.LogWarning($"Sprite para o Player '{row[3]}' não encontrado (linha {lineIndex + 1}).");
                        }
                        else
                        {
                            Debug.Log($"Player sprite encontrado: {playerSprite.name}");
                        }
                    }
                    dialogue.imagePlayer.Add(playerSprite);

                    // Adiciona sprite do NPC - CORREÇÃO APLICADA AQUI
                    Sprite npcSprite = null;
                    if (!string.IsNullOrWhiteSpace(row[4]))
                    {
                        string npcSpriteName = row[4].Trim();
                        npcSprite = images.Find(image => image.name == npcSpriteName);
                        if (npcSprite == null)
                        {
                            Debug.LogWarning($"Sprite para o NPC '{npcSpriteName}' não encontrado (linha {lineIndex + 1}).");
                        }
                        else
                        {
                            Debug.Log($"NPC sprite encontrado: {npcSprite.name} para linha {lineIndex + 1}");
                        }
                    }
                    else
                    {
                        Debug.Log($"Nome do sprite NPC vazio na linha {lineIndex + 1}");
                    }
                    dialogue.imageNPC.Add(npcSprite);

                    linesProcessed++;
                }
                else
                {
                    Debug.LogWarning($"Linha {lineIndex + 1} ignorada no CSV '{csvFile.name}' (faltando colunas): {line}");
                }
            }

            // Marca o asset como "dirty" para salvar as mudanças
            EditorUtility.SetDirty(dialogue);

            // Debug final detalhado
            Debug.Log($"=== RESULTADO FINAL PARA {dialogue.name} ===");
            Debug.Log($"Total de linhas processadas: {linesProcessed}");
            Debug.Log($"Total de sprites Player: {dialogue.imagePlayer.Count}");
            Debug.Log($"Total de sprites NPC: {dialogue.imageNPC.Count}");

            // Lista todos os sprites NPC adicionados
            for (int j = 0; j < dialogue.imageNPC.Count; j++)
            {
                string spriteName = dialogue.imageNPC[j] != null ? dialogue.imageNPC[j].name : "NULL";
                Debug.Log($"NPC Sprite [{j}]: {spriteName}");
            }

            Debug.Log($"✓ Atualização concluída com sucesso para o par {index}: {linesProcessed} linhas processadas");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao atualizar o par {index}: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
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