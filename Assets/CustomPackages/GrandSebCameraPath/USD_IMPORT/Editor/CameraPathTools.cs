/// Gildas (c)

using Assets.USD_IMPORT.Editor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Import un fichier USDA et cree une animation Unity
/// </summary>
public class CameraPathTools : Editor
{
    [MenuItem("Camera Path Tools/Import USDA", false, -1)]
    public static void ImportUsd() {
        Debug.Log("Bouh !!!");
        


        string path = EditorUtility.OpenFilePanel("Fichier USDA a transformer en animation", "", "usda");
        if (path.Length == 0) return;

        ImportUsdManager usd = new ImportUsdManager(path);
        usd.TransformToAnimation();

        EditorUtility.DisplayDialog("Bingo !", "It works !!!", "OK");
    }
    
}
