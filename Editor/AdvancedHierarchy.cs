using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

[InitializeOnLoad]
public static class AdvancedHierarchy
{
    static float toggleWidth = 14f;

    static AdvancedHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
    }

    static void OnHierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

        if (obj == null) return;

        Rect toggle = new Rect(selectionRect.x - (toggleWidth * 2), selectionRect.y, toggleWidth, selectionRect.height);
        bool active = EditorGUI.Toggle(toggle, obj.activeSelf);
        if (active != obj.activeSelf)
            obj.SetActive(active);

        if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) != null) return;

        Component[] components = obj.GetComponents<Component>();
        
        if (components == null || components.Length == 0) return;

        Component component = components.Length > 1 ? components[1] : components[0];

        Type type = component.GetType();
        
        GUIContent content = EditorGUIUtility.ObjectContent(component, type);
        content.text = null;
        content.tooltip = type.Name;

        if (content.image == null) return;

        Color colorBg = new Color(1, 1, 1, 0.8f);
        MonoBehaviour mb;
        if (obj.TryGetComponent(out mb))
            colorBg = new Color(.1f, .6f, .7f, 0.8f);

        var _sceneHierarchyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var _getExpandedIDs = _sceneHierarchyWindowType.GetMethod("GetExpandedIDs", BindingFlags.NonPublic | BindingFlags.Instance);
        var _lastInteractedHierarchyWindow = _sceneHierarchyWindowType.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Public | BindingFlags.Static);
        if (_lastInteractedHierarchyWindow == null) return;
        var _expandedIDs = _getExpandedIDs.Invoke(_lastInteractedHierarchyWindow.GetValue(null), null) as int[];
        var isExpanded = _expandedIDs.Contains<int>(instanceId); 
        if (isExpanded)
            colorBg = new Color(.75f, .1f, .1f, 0.8f);

        if (!obj.activeSelf)
            colorBg = Color.gray;

        Rect backgroundRectBg = selectionRect;
        backgroundRectBg.width = 20f;
        backgroundRectBg.x -= 1.75f;
        EditorGUI.DrawRect(backgroundRectBg, colorBg);

        Color colorFg = new Color(0, 0, 0, 0.8f); 
        Rect backgroundRectFg = selectionRect;
        backgroundRectFg.width = 18f;
        EditorGUI.DrawRect(backgroundRectFg, colorFg);

        EditorGUI.LabelField(selectionRect, content);
    }
}