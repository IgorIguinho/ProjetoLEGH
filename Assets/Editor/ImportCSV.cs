using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

public class CSVToDialogueEditor : EditorWindow
{
    public TextAsset csvFile; // Referência ao arquivo CSV.
    private string savePath = "Assets"; // Caminho padrão para salvar o asset.
    public string saveName;
    [SerializeField] private List<Sprite> images = new List<Sprite>(); // Lista persistente de sprites.
    public string spritesFolderPath = "Assets/Resources/Sprites";


    #if UNITY_EDITOR
    // Método para carregar os sprites no modo Editor
    [ContextMenu("Carregar Sprites no Editor")]
    public void LoadSpritesInEditor()
    {
        if (string.IsNullOrEmpty(spritesFolderPath))
        {
            Debug.LogError("O caminho da pasta de sprites está vazio!");
            return;
        }

        // Busca todos os assets do tipo Sprite no caminho especificado.
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesFolderPath });

        images = new List<Sprite>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                images.Add(sprite);
            }
        }
        saveName = csvFile.name;
        Debug.Log($"Sprites carregados no Editor: {images.Count}");
    }
#endif

    [MenuItem("Tools/CSV to Dialogue Scriptable")]
    public static void OpenWindow()
    {
        GetWindow<CSVToDialogueEditor>("CSV to Dialogue Scriptable");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV to Dialogue Scriptable", EditorStyles.boldLabel);

        // Campo para selecionar o arquivo CSV.
        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);

        // Campo para definir o local de salvamento do ScriptableObject.
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        // campo para definir o nome do objeto
        saveName = EditorGUILayout.TextField("Save Name", saveName);

        spritesFolderPath = EditorGUILayout.TextField("Sprite Folder Path", spritesFolderPath);
        

        // Botão para dar reload na base de imagens
        if (GUILayout.Button("Reload Database sprites"))
        {
          ReloadSpriteDatabase();
        }
        GUILayout.Label($"Sprites carregados: {images.Count}");

        // Botão para criar o ScriptableObject.
        if (GUILayout.Button("Generate Dialogue Scriptable"))
        {
            if (csvFile == null)
            {
                Debug.LogError("Por favor, selecione um arquivo CSV.");
                return;
            }

            GenerateDialogueScriptable();
        }
    }


    private void ReloadSpriteDatabase()
    {
        if (string.IsNullOrEmpty(spritesFolderPath))
        {
            Debug.LogError("O caminho da pasta de sprites está vazio!");
            return;
        }

        // Debug log para verificar se o caminho foi reconhecido corretamente
        Debug.Log($"Tentando carregar os sprites de: {spritesFolderPath}");

        // Busca todos os assets do tipo Sprite no caminho especificado.
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesFolderPath });

        images = new List<Sprite>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                images.Add(sprite);
                // Debug log para verificar se o sprite foi carregado
                Debug.Log($"Sprite carregado: {sprite.name}");
            }
            else
            {
                Debug.LogWarning($"Falha ao carregar o sprite de {assetPath}");
            }
        }

        Debug.Log($"Total de sprites carregados: {images.Count}");
    }
    private void GenerateDialogueScriptable()
    {
        // Verifica se o arquivo CSV está selecionado.
        if (csvFile == null)
        {
            Debug.LogError("Nenhum arquivo CSV foi selecionado.");
            return;
        }

        // Cria uma instância do DialogueScriptable.
        DialogueScriptable dialogue = ScriptableObject.CreateInstance<DialogueScriptable>();

        // Garante que as listas no DialogueScriptable não sejam nulas.
        if (dialogue.thisIs == null)
            dialogue.thisIs = new List<string>();
        if (dialogue.text == null)
            dialogue.text = new List<string>();
        if (dialogue.nameDialogue == null)
            dialogue.nameDialogue = new List<string>();
        if (dialogue.imagePlayer == null)
            dialogue.imagePlayer = new List<Sprite>();
        if (dialogue.imageNPC == null)
            dialogue.imageNPC = new List<Sprite>();

        Debug.Log("Listas inicializadas com sucesso.");

        // Lê e processa o arquivo CSV.
        string[] lines = csvFile.text.Split('\n'); // Divide o texto em linhas.
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // Ignora linhas vazias.

            string[] row = ParseCSVLine(line); // Analisa a linha do CSV.
            if (row.Length >= 5) // Garante que existam pelo menos x colunas.
            {
                // Adiciona dados nas listas
                dialogue.thisIs.Add(row[0]); // Coluna 1.
                dialogue.text.Add(row[1]);  // Coluna 2.
                dialogue.nameDialogue.Add(row[2]);  

                // Verifica a lista de imagens antes de tentar adicionar os sprites
                if (images == null || images.Count == 0)
                {
                    Debug.LogWarning("A lista de imagens está vazia ou não foi carregada.");
                    continue;
                }

                // Adiciona o sprite do player, se encontrado
                Sprite playerSprite = images.Find(image => image.name == row[3]);
                if (playerSprite != null)
                {
                    dialogue.imagePlayer.Add(playerSprite);
                }
                else
                {
                    Debug.LogWarning($"Sprite para o Player '{row[3]}' não encontrado.");
                    dialogue.imagePlayer.Add(null);
                }

                Debug.Log($"Procurando sprite para o NPC: {row[4]}");

                // Adiciona o sprite do NPC, se encontrado
                Sprite npcSprite = images.Find(image => image.name == row[4]);
                if (npcSprite != null)
                {
                    dialogue.imageNPC.Add(npcSprite);
                }
                else
                {
                    Debug.LogWarning($"Sprite para o NPC '{row[4]}' não encontrado.");
                    dialogue.imageNPC.Add(null);
                }
            }
            else
            {
                Debug.LogWarning($"Linha ignorada (faltando colunas): {line}");
            }
            //dialogue.pitchAudio.Add(float.Parse(row[5]));
        }

        // Verifica se o nome foi definido corretamente
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("Por favor, defina um nome para o ScriptableObject.");
            return;
        }

        // Define o caminho para salvar o ScriptableObject.
        string assetPath = $"{savePath}/{saveName}.asset";

        // Salva o ScriptableObject no caminho especificado.
        AssetDatabase.CreateAsset(dialogue, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"Dialogue Scriptable criado com sucesso em: {assetPath}");
    }

    // Método para analisar uma linha do CSV corretamente.
    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool insideQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"') // Alterna o estado das aspas.
            {
                insideQuotes = !insideQuotes;
            }
            else if (c == ',' && !insideQuotes) // Divide a coluna fora das aspas.
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c; // Adiciona o caractere atual à coluna.
            }
        }

        // Adiciona a última coluna.
        result.Add(current);

        return result.ToArray();
    }
    
}