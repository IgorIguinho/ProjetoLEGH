using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TradeSceneOnUnity : EditorWindow
{
   public List<SceneAsset> assets;

    [MenuItem("Tools/Update Dialogue Scriptables")]
    public static void OpenWindow()
    {
        GetWindow<TradeSceneOnUnity>("TradeScene");
    }
    private void OnGUI()
    {
        GUILayout.Label("CSV to Dialogue Scriptable", EditorStyles.boldLabel);

        assets = new List<SceneAsset>();
    }
}
