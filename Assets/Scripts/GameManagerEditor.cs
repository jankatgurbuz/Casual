#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(GameManager), true)]
[CanEditMultipleObjects]
public class GameManagerEditor : Editor
{
    private ReorderableList _reorderableList;
    private bool ShowLevelsList;
    private GameManager _gameManager;
    private void OnEnable()
    {
        SerializedProperty listProperty = serializedObject.FindProperty("Levels");
        _reorderableList = new ReorderableList(serializedObject, listProperty, true, true, true, true);
        _reorderableList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var prop = listProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop);
        };

        GameObject gameM = GameObject.Find("GameManager");

        if (gameM != null)
            _gameManager = gameM.GetComponent<GameManager>();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ShowLevelsList = GUILayout.Toggle(ShowLevelsList, "ShowLevelsList");
        if (ShowLevelsList)
        {
            DrawDefaultInspector();
        }
        else
        {
            SerializedProperty randomLevel = serializedObject.FindProperty("RandomLevel");
            EditorGUILayout.PropertyField(randomLevel);
        }
        _reorderableList.DoLayoutList();

        // Control
        if (_gameManager.Levels != null && _gameManager.Levels.Length != 0)
            IsEmptyLevels();
        else
            Debug.LogError("Levels Object is Empty");

        serializedObject.ApplyModifiedProperties();
    }
    public bool IsEmptyLevels()
    {
        bool c = true;
        for (int i = 0; i < _gameManager.Levels.Length; i++)
        {
            if (_gameManager.Levels[i] == null)
            {
                Debug.LogError($"<color=magenta> Level {i} </color>" +
                    $" <color=red>In The Levels Object is Empty  </color>");
                c = false;
            }
        }
        return c;
    }
}
#endif