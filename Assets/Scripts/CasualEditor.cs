#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using JLibrary.JUI;

public class CasualEditor : EditorWindow
{
    private GUISkin _skin;

    private string _phaseName = string.Empty;
    private string _levelName = string.Empty;
    private string _processName = string.Empty;

    private bool _adding = false;
    private double _currentTime = 0;
    private string _tempPhaseName = null;
    private GameObject _tempPhaseObj;

    

    [MenuItem("Casual/CasualPanel")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CasualEditor));
    }
    private void OnEnable()
    {
        _skin = (GUISkin)Resources.Load("Editor/EditorSkin");
    }
    private void OnGUI()
    {
        if (CheckGameManager())
        {
            if (Button("Add GameManager", "button"))
                CreateGameManager();
        }
        else
        {
            Space(5);
            CreateLevel();
            Space(5);
            CreateProsess();
            Space(5);
            CreatePhase();
        }
    }

    private void CreateProsess()
    {
        Label("Create Process", "label");

        if (Button("Add", "button"))
        {
            GameObject process = new GameObject();
            process.AddComponent<ProcessManager>();
            process.name = !FindInActiveObjectByName("ProcessManager")
                ? "ProcessManager" : $"ProcessManager (Already Exists)";
        }

    }

    private void Space(int spaceCount)
    {
        for (int i = 0; i < spaceCount; i++)
            EditorGUILayout.Space();
    }

    private void CreateLevel()
    {
        Label("Create Level", "label");

        _levelName = TextField(_levelName, "textfield");
        _levelName = LowercaseToUpperCase(_levelName);

        string isExistPath = $"Assets/Scriptables/Levels/{_levelName}.asset";

        if (string.IsNullOrEmpty(_levelName))
            Label("The Box Cannot Be Empty", "infotext");

        else if (!IsFirstCharacterNumber(_levelName))
            Label("First Character Cannot Be Number", "infotext");

        else if (!DifferentCharacters(_levelName))
            Label("The Box Cannot Be Punctuation, Symbol or Space", "infotext");

        else if (!IsExist(_levelName, isExistPath))
            Label($"{_levelName} Already Available", "infotext");

        else
        {
            if (Button("Add", "button"))
            {
                string scriptPath = "Assets/Scriptables/Levels";

                CreateScriptFolder(scriptPath);
                CreateLevel(_levelName);

                _levelName = string.Empty;
            }
        }
    }

    #region GameManager
    private bool CheckGameManager()
    {
        return FindInActiveObjectByName("GameManager") == null ? true : false;
    }
    private void CreateGameManager()
    {
        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<GameManager>();

        GameObject touchManager = new GameObject("TouchManager");
        touchManager.AddComponent<TouchManager>();

        GameObject saveSystem = new GameObject("SaveSystem");
        touchManager.AddComponent<SaveSystem>();

        JUIEditor.JUI();
        
    }
   
    private GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
            if (objs[i].hideFlags == HideFlags.None)
                if (objs[i].name == name)
                    return objs[i].gameObject;
        return null;
    }
    #endregion

    #region PHASE
    private void CreateLevel(string levelName)
    {
        Level level = CreateInstance<Level>();

        string path = "Assets/Scriptables/Levels";
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{levelName}.asset");

        AssetDatabase.CreateAsset(level, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        FocusAsset(assetPathAndName);

    }
    private void CreatePhase()
    {
        Label("Adding Phase", "label");

        _phaseName = TextField(_phaseName, "textfield");
        _phaseName = LowercaseToUpperCase(_phaseName);

        string isExistPath = $"Assets/Scripts/Phases/{_phaseName}.cs";

        if (_adding)
            Label("Phase Is Still Being Add", "infotext");

        else if (string.IsNullOrEmpty(_phaseName))
            Label("The Box Cannot Be Empty", "infotext");

        else if (!IsFirstCharacterNumber(_phaseName))
            Label("First Character Cannot Be Number", "infotext");

        else if (!DifferentCharacters(_phaseName))
            Label("The Box Cannot Be Punctuation, Symbol or Space", "infotext");

        else if (!IsExist(_phaseName, isExistPath))
            Label($"{_phaseName} Already Available", "infotext");

        else
        {
            if (Button("Add", "button"))
            {
                string scriptPath = "Assets/Scripts/Phases";

                CreateScriptFolder(scriptPath);
                CreatePhaseScript(_phaseName);
                AddPhaseManager(_phaseName);

                _phaseName = string.Empty;
            }
        }
    }
    private void AddPhaseManager(string phaseName)
    {
        GameObject progressManager = GameObject.Find("ProcessManager");
        GameObject child = new GameObject(phaseName);
        child.transform.SetParent(progressManager.transform);
        _adding = true;
        _currentTime = EditorApplication.timeSinceStartup;
        _tempPhaseName = phaseName;
        _tempPhaseObj = child;
    }
    private void Update()
    {
        if (_adding)
        {
            if (_currentTime + 2 < EditorApplication.timeSinceStartup)
            {
                _adding = false;
                var type = Type.GetType(_tempPhaseName, true);
                _tempPhaseObj.AddComponent(type);
            }
        }
    }

    private bool IsExist(string phaseName, string path)
    {
        Type t = typeof(UnityEngine.Object);
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, t);

        if (obj != null)
            return false;

        return true;
    }
    private string LowercaseToUpperCase(string s)
    {
        if (!string.IsNullOrEmpty(s))
            return s.First().ToString().ToUpper() + s.Substring(1);

        return string.Empty;
    }
    private void CreateScriptFolder(string scriptPath)
    {
        if (!Directory.Exists(scriptPath))
            Directory.CreateDirectory(scriptPath);
    }
    private void CreatePhaseScript(string phaseName)
    {
        string tempPath = "Editor/TempPhase";

        Type t = typeof(TextAsset);
        TextAsset txt = Resources.Load(tempPath, t) as TextAsset;

        string content = txt.text;
        content = content.Replace("TempPhase", phaseName);

        string path = $"Assets/Scripts/Phases/{phaseName}.cs";

        File.WriteAllText(path, content);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        FocusAsset(path);

    }
    private void FocusAsset(string path)
    {
        EditorUtility.FocusProjectWindow();

        Type t = typeof(UnityEngine.Object);
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, t);
        Selection.activeObject = obj;

        EditorGUIUtility.PingObject(obj);
    }
    private bool DifferentCharacters(string s)
    {
        char[] c = s.ToCharArray();

        foreach (var item in c)
            if (char.IsPunctuation(item) ||
                char.IsSymbol(item) ||
                char.IsWhiteSpace(item))
                return false;

        return true;
    }
    private bool IsFirstCharacterNumber(string s)
    {
        char c = s[0];
        return char.IsNumber(c) ? false : true;
    }
    private void Label(string text, string styleText)
    {
        GUILayout.Label(text, _skin.GetStyle(styleText));
    }
    private bool Button(string text, string styleText)
    {
        return GUILayout.Button(text, _skin.GetStyle(styleText));
    }
    private string TextField(string text, string styleText)
    {
        return GUILayout.TextField(text, _skin.GetStyle(styleText));
    }
    #endregion

}

#endif