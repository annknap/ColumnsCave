using LZWPlib;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveOrigin))]
public class CaveOriginEditor : Editor {

    public string[] arrFiles;
    public string[] arrLabels;
    public int index = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Use screen config:");

        index = EditorGUILayout.Popup(index, arrLabels);

        CaveOrigin co = target as CaveOrigin;

        if (index != co.chosenScreenConfigIdx && index < arrFiles.Length)
        {
            co.chosenScreenConfigIdx = index;
            co.LoadScreens(arrFiles[index]);
        }
    }

    void OnEnable()
    {
        CaveOrigin co = target as CaveOrigin;

        index = co.chosenScreenConfigIdx;

        RefreshScreenConfigsList();

        if (index < arrFiles.Length)
        {
            co.chosenScreenConfigIdx = index;
            co.LoadScreens(arrFiles[index]);
        }
    }

    void RefreshScreenConfigsList()
    {
        var ms = MonoScript.FromScriptableObject(this);
        var path = AssetDatabase.GetAssetPath(ms);
        path = Path.GetDirectoryName(path);
        path = path.Substring(0, path.Length - "Editor".Length) + "ScreenConfigs/";

        arrFiles = Directory.GetFiles(path, "*.txt");
        System.Array.Sort(arrFiles);

        arrLabels = new string[arrFiles.Length];

        for (int i = 0; i < arrFiles.Length; i++)
            arrLabels[i] = File.ReadAllLines(arrFiles[i])[0];
    }
}
