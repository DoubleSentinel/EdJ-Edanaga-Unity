using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DDUnitImage)), CanEditMultipleObjects]
public class DDUnitImageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DDUnitImage ddUnitImage = (DDUnitImage)target;
        
        EditorGUILayout.HelpBox("Custom editor, see the script for a wider oversight", MessageType.None);
        
        DrawUILine();
        EditorGUILayout.LabelField("Translation", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField("Translation Function");
        //ddUnitImage.translationFunction = (DDUnitImage.TranslationFunctionItems)EditorGUILayout.EnumPopup("Option", ddUnitImage.translationFunction);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("translationFunction"), new GUIContent("Option"), true);
        if (ddUnitImage.translationFunction == DDUnitImage.TranslationFunctionItems.none)
        {
            EditorGUILayout.HelpBox("No smooth translation during drag", MessageType.None);
        }
        else if (ddUnitImage.translationFunction == DDUnitImage.TranslationFunctionItems.lerp)
        {
            EditorGUILayout.HelpBox("Smooth translation using lerp function timed by the value below", MessageType.None);
            //ddUnitImage.translationSpeed = EditorGUILayout.FloatField("Speed", ddUnitImage.translationSpeed);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("translationSpeed"), new GUIContent("Speed"), true);
        }

        DrawUILine();
        EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Rotation Function");
        //ddUnitImage.rotationFunction = (DDUnitImage.RotationFunctionItems)EditorGUILayout.EnumPopup("Option", ddUnitImage.rotationFunction);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationFunction"), new GUIContent("Option"), true);
        if (ddUnitImage.rotationFunction == DDUnitImage.RotationFunctionItems.none)
        {
            EditorGUILayout.HelpBox("No smooth rotation. Use 'lerp' to enable OnDrag Animation", MessageType.None);
        }
        else if (ddUnitImage.rotationFunction == DDUnitImage.RotationFunctionItems.lerp)
        {
            EditorGUILayout.HelpBox("Smooth rotation using lerp function timed by the value below", MessageType.None);
            //ddUnitImage.rotationSpeed = EditorGUILayout.FloatField("Speed", ddUnitImage.rotationSpeed);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"), new GUIContent("Speed"), true);
        }

        EditorGUI.BeginDisabledGroup(ddUnitImage.rotationFunction == DDUnitImage.RotationFunctionItems.none);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("OnDrag Animation");
        //ddUnitImage.onDragAnimation = (DDUnitImage.OnDragAnimationItems)EditorGUILayout.EnumPopup("Option", ddUnitImage.onDragAnimation);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onDragAnimation"), new GUIContent("Option"), true);
        if (ddUnitImage.onDragAnimation == DDUnitImage.OnDragAnimationItems.none)
        {
            EditorGUILayout.HelpBox("No animation", MessageType.None);
        }
        else if (ddUnitImage.onDragAnimation == DDUnitImage.OnDragAnimationItems.rotateToPointer)
        {
            EditorGUILayout.HelpBox("Smooth face the pointer during drag", MessageType.None);
            //ddUnitImage.rotateToPointerMaxAngle = EditorGUILayout.FloatField("Max angle", ddUnitImage.rotateToPointerMaxAngle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateToPointerMaxAngle"), new GUIContent("Max angle"), true);
        }

        EditorGUI.EndDisabledGroup();

        DrawUILine();
        EditorGUILayout.LabelField("Resize", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Resize Function");
        //ddUnitImage.resizeFunction = (DDUnitImage.ResizeFunctionItems)EditorGUILayout.EnumPopup("Option", ddUnitImage.resizeFunction);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("resizeFunction"), new GUIContent("Option"), true);
        if (ddUnitImage.resizeFunction == DDUnitImage.ResizeFunctionItems.none)
        {
            EditorGUILayout.HelpBox("No smooth resize", MessageType.None);
        }
        else if (ddUnitImage.resizeFunction == DDUnitImage.ResizeFunctionItems.lerp)
        {
            EditorGUILayout.HelpBox("Smooth resize using lerp function timed by the value below", MessageType.None);
            //ddUnitImage.resizeSpeed = EditorGUILayout.FloatField("Speed", ddUnitImage.resizeSpeed);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("resizeSpeed"), new GUIContent("Speed"), true);
        }

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
