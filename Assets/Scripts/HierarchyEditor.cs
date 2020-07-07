#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public class HierarchyEditor
{
    private static Color _gameManagerColor = Color.black;
    private static Color _gameManagerBackground = Color.green;
    private static GUIStyle _guiStyle;

    private static readonly FontStyle _gameManagerFont = FontStyle.Bold;
    private static readonly Vector2 _offsetPos = new Vector2(18.5f, 1.1f);
    private static readonly Vector2 _offsetSize = new Vector2(2f, 2f);

    static HierarchyEditor()
    {
        Initialize();
        AddEvent();
    }
    private static void Initialize()
    {
        CreateGuiStyle();
    }
    private static void CreateGuiStyle()
    {
        GUIStyleState gss = new GUIStyleState() { textColor = _gameManagerColor };
        _guiStyle = new GUIStyle()
        {
            normal = gss,
            fontStyle = _gameManagerFont
        };
    }
    private static void AddEvent()
    {
        EditorApplication.hierarchyWindowItemOnGUI = HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        Rect offset = new Rect(selectionRect.position + _offsetPos, selectionRect.size - _offsetSize);

        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        ManagerEditor(obj, offset, "GameManager", 0, Color.green);
        ManagerEditor(obj, offset, "TouchManager", 1, Color.cyan);
        ManagerEditor(obj, offset, "SaveSystem", 2, Color.blue);
        ManagerEditor(obj, offset, "Main Camera", 3, Color.yellow);
        ManagerEditor(obj, offset, "UIManager", 4, Color.white);
        ManagerEditor(obj, offset, "ProcessManager", 5, Color.white);

    }

    private static void ManagerEditor(GameObject obj, Rect offset, string name, int order, Color color)
    {
        if (obj != null && obj.name == name)
        {
            obj.transform.SetSiblingIndex(order);
            _gameManagerColor = obj.activeSelf ? Color.black : new Color(0.2f, 0.2f, 0.2f, 1f);
            _gameManagerBackground = obj.activeSelf ? color : Color.gray;
            CreateGuiStyle();
            EditorGUI.DrawRect(offset, _gameManagerBackground);
            EditorGUI.LabelField(offset, obj.name, _guiStyle);
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}

#endif