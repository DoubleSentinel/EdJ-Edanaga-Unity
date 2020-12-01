using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DDVessel)), CanEditMultipleObjects]
public class DDVesselEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DDVessel ddVessel = (DDVessel)target;

        EditorGUILayout.HelpBox("Custom editor, see the script for a wider oversight", MessageType.None);

        DrawUILine();
        EditorGUILayout.LabelField("Drop In Rules", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Drop In");
        //ddVessel.dropInTagsRule = (DDVessel.DropInTagsRuleItems)EditorGUILayout.EnumPopup("Rule", ddVessel.dropInTagsRule);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dropInTagsRule"), new GUIContent("Rule"), true);

        if (ddVessel.dropInTagsRule == DDVessel.DropInTagsRuleItems.dropInOnlyEnabledTags)
        {
            EditorGUILayout.HelpBox("No DDUnits can be dropped in, excep the ones with DDTags listed below", MessageType.Warning);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropInEnabledTags"), true);
        }
        else if (ddVessel.dropInTagsRule == DDVessel.DropInTagsRuleItems.doNotDropInDisabledTags)
        {
            EditorGUILayout.HelpBox("All DDUnits can be dropped in, excep the ones with DDTags listed below", MessageType.None);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropInDisabledTags"), true); 
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Max Number of DDUnits");
        EditorGUILayout.HelpBox("value 0 (zero) for no limit", MessageType.None);
        //ddVessel.maxNumberOfUnits = EditorGUILayout.IntField("Value", ddVessel.maxNumberOfUnits);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNumberOfUnits"), new GUIContent("Value"), true);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Default Drop In Position");
        //ddVessel.defaultDropPosition = (DDVessel.DefaultDropPositionItems)EditorGUILayout.EnumPopup("Option", ddVessel.defaultDropPosition);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultDropPosition"), new GUIContent("Option"), true);

        string defaultDropPositionMessage = "Drop In position if vessel is empty or DDUnit is not placed over other DDUnits.";
        if (ddVessel.defaultDropPosition == DDVessel.DefaultDropPositionItems.dropInAtIndex)
        {
            defaultDropPositionMessage += " Dropped DDUnit(s) are placed at Index set below";
            EditorGUILayout.HelpBox(defaultDropPositionMessage, MessageType.None);
            //ddVessel.defaultDropPositionIndex = EditorGUILayout.IntField("Index", ddVessel.defaultDropPositionIndex);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultDropPositionIndex"), new GUIContent("Index"), true);
        }
        else
        {
            EditorGUILayout.HelpBox(defaultDropPositionMessage, MessageType.None);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Drop In Over DDUnit");
        EditorGUILayout.HelpBox("Option for when DDUnit is placed over or between other DDUnit(s)", MessageType.None);
        //ddVessel.dropOverUnit = (DDVessel.DropOverUnitItems)EditorGUILayout.EnumPopup("Option", ddVessel.dropOverUnit);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dropOverUnit"), new GUIContent("Option"), true);
        if (ddVessel.dropOverUnit == DDVessel.DropOverUnitItems.dropInOnGhostPosition)
        {
            EditorGUILayout.HelpBox("If there is a Layout Group in this GameObject, a 'GhostUnit' will indicate the Drop In position", MessageType.None);
        }

        DrawUILine();
        EditorGUILayout.LabelField("Drag Out Rule", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("All DDUnits can be dragged out, excep the ones with DDTags listed below", MessageType.None);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dragOutDisabledTags"), true);

        serializedObject.ApplyModifiedProperties();
    }

    // dar vertical line on editor gui modified form code by alexanderameye
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
