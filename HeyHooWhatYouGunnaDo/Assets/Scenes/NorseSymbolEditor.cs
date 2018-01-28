using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NorseSymbol))]
public class NorseSymbolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NorseSymbol myScript = (NorseSymbol)target;
        if (GUILayout.Button("Generate Dots"))
        {
            myScript.ReInitializeDots();
        }
    }
}