using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DDUnit)), CanEditMultipleObjects]
public class DDUnitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DDUnit ddUnit = (DDUnit)target;
        
        EditorGUILayout.HelpBox("Custom editor, see the script for a wider oversight", MessageType.None);

        DrawUILine();
        EditorGUILayout.LabelField("General Options", EditorStyles.boldLabel);
        
        ddUnit.tag = EditorGUILayout.TextField("DDTag", ddUnit.tag);

        EditorGUILayout.Space();
        
        //ddUnit.canSelect = EditorGUILayout.Toggle("Can Select", ddUnit.canSelect);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canSelect"), new GUIContent("Can Select"), true);

        //ddUnit.canDrag = EditorGUILayout.Toggle("Can Drag", ddUnit.canDrag);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canDrag"), new GUIContent("Can Drag"), true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("enableSelectionOutline"), new GUIContent("Outline If Selected"), true);
        
        DrawUILine();
        EditorGUILayout.LabelField("Drag Options", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Drag Anchor");
        //ddUnit.dragAnchor = (DDUnit.DragAnchorItems)EditorGUILayout.EnumPopup("Anchor", ddUnit.dragAnchor);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dragAnchor"), new GUIContent("Anchor"), true);
        EditorGUILayout.HelpBox("Drag anchor related to the Pointer", MessageType.None);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("OnDrag Size");
        EditorGUILayout.HelpBox("set the values x, y 0 (zero) or Vector2.zero to let the value the same as before", MessageType.None);
        //ddUnit.onDragSize = EditorGUILayout.Vector2Field("Size", ddUnit.onDragSize);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onDragSize"), new GUIContent("Size"), true);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("OnDrag Rotation");
        EditorGUILayout.HelpBox("set the values x, y and z 0 (zero) or Vector3.zero to let the value the same as before", MessageType.None);
        //ddUnit.onDragRotation = EditorGUILayout.Vector3Field("Size", ddUnit.onDragRotation);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onDragRotation"), new GUIContent("Size"), true);

        DrawUILine();
        EditorGUILayout.LabelField("Drop Options", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Options If Dropped Outside Any Vessel");
        //ddUnit.dropOnVoid = (DDUnit.DropOnVoidItems)EditorGUILayout.EnumPopup("Option", ddUnit.dropOnVoid);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dropOnVoid"), new GUIContent("Option"), true);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("OnDrop Size");
        EditorGUILayout.HelpBox("set the values x, y 0 (zero) or Vector2.zero to let the value the same as before", MessageType.None);
        EditorGUILayout.HelpBox("If the Unit is placed on a Vessel, this value will be overridden by the LayoutGroup options", MessageType.Warning);
        //ddUnit.onDroppedSize = EditorGUILayout.Vector2Field("Size", ddUnit.onDroppedSize);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onDroppedSize"), new GUIContent("Size"), true);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("OnDrop Rotation");
        EditorGUILayout.HelpBox("set the values x, y and z 0 (zero) or Vector3.zero to let the value the same as before", MessageType.None);
        //ddUnit.onDropRotation = EditorGUILayout.Vector3Field("Size", ddUnit.onDropRotation);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onDropRotation"), new GUIContent("Size"), true);

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
