using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DDCanvas)), CanEditMultipleObjects]
public class DDCanvasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DDCanvas ddCanvas = (DDCanvas)target;

        EditorGUILayout.HelpBox("Custom editor, see the script for a wider oversight", MessageType.None);

        DrawUILine();
        EditorGUILayout.LabelField("Assist-Objects", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("See about them in the manual", MessageType.None);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canvasOnDrag"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canvasImages"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ghostUnit"), true);

        DrawUILine();
        EditorGUILayout.LabelField("User Action Options", EditorStyles.boldLabel);

        //ddCanvas.enableMultipleSelection = EditorGUILayout.Toggle("Multiple Selection", ddCanvas.enableMultipleSelection);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("enableMultipleSelection"), new GUIContent("Multiple Selection"), true);
        if (ddCanvas.enableMultipleSelection)
        {
            //ddCanvas.multipleSelectionKey = (KeyCode)EditorGUILayout.EnumPopup("MS Key", ddCanvas.multipleSelectionKey);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("multipleSelectionKey"), new GUIContent("MS Key"), true);

            EditorGUILayout.HelpBox("Multi-selection enabled using " + ddCanvas.multipleSelectionKey.ToString() +
                " + Pointer click", MessageType.None);
        }
        else
        {
            EditorGUILayout.HelpBox("Multi-selection disable. Set true to enable 'On Drag Grouping Layout'", MessageType.None);
        }

        DrawUILine();
        EditorGUILayout.LabelField("Layout/Visual Options", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Transparency of DDUnit While Being Dragged");
        //ddCanvas.alphaOnDrag = EditorGUILayout.FloatField("Alpha", ddCanvas.alphaOnDrag);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("alphaOnDrag"), new GUIContent("Alpha"), true);

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(ddCanvas.enableMultipleSelection == false);
        
        EditorGUILayout.LabelField("On Drag Grouping Layout");
        //ddCanvas.multipleDragGrouping = (DDCanvas.MultipleDragGroupingItems)EditorGUILayout.EnumPopup("Layout", ddCanvas.multipleDragGrouping);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("multipleDragGrouping"), new GUIContent("Layout"), true);

        if (ddCanvas.multipleDragGrouping == DDCanvas.MultipleDragGroupingItems.none)
        {
            EditorGUILayout.HelpBox("DDUnits are placed over each other", MessageType.None);
        }
        else if (ddCanvas.multipleDragGrouping == DDCanvas.MultipleDragGroupingItems.cardDeck)
        {
            EditorGUILayout.HelpBox("DDUnits are disposed in deck-like layout", MessageType.None);
            //ddCanvas.groupingOffset.x = EditorGUILayout.FloatField("horizontal offset", ddCanvas.groupingOffset.x);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groupingOffset.x"), new GUIContent("horizontal offset"), true);

            //ddCanvas.groupingOffset.y = EditorGUILayout.FloatField("angle offset", ddCanvas.groupingOffset.y);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groupingOffset.y"), new GUIContent("angle offset"), true);
        }
        else if (ddCanvas.multipleDragGrouping == DDCanvas.MultipleDragGroupingItems.stair)
        {
            EditorGUILayout.HelpBox("DDUnits are disposed in stair-like layout", MessageType.None);
            //ddCanvas.groupingOffset.x = EditorGUILayout.FloatField("horizontal offset", ddCanvas.groupingOffset.x);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groupingOffset.x"), new GUIContent("horizontal offset"), true);

            //ddCanvas.groupingOffset.y = EditorGUILayout.FloatField("vertical offset", ddCanvas.groupingOffset.y);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groupingOffset.y"), new GUIContent("vertical offset"), true);
        }

        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }

    public static void DrawUILine()
    {
        Color color = Color.white;
        int thickness = 1;
        int padding = 0;
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}
