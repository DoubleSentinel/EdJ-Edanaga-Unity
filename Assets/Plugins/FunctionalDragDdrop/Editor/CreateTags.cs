using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CreateTags : Editor
{
    static CreateTags()
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // Tags to add
        string[] tagsToAdd = { "DDCanvas" };//, "DDCanvasOnDrag", "DDCanvasImages" };

        // Check if tags are not already present & add if not
        foreach (string tagToAdd in tagsToAdd)
        {
            bool tagFound = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty tag = tagsProp.GetArrayElementAtIndex(i);
                if (tag.stringValue.Equals(tagToAdd))
                {
                    tagFound = true;
                    break;
                }
            }

            // not found => add tag
            if (!tagFound)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tagToAdd;
                tagManager.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
