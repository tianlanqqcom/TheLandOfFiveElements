/*
 * File: FixedStringDrawer.cs
 * Description: 编辑器中修正字符串类的显示
 * Author: tianlan
 * Last update at 24/2/26   21:37
 * 
 * Update Records:
 * tianlan  24/2/26 FixedStringDrawer
 */

using GameFramework.Core;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixedString))]
public class FixedStringDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;

        Rect labelRect = new Rect(position.x, position.y, position.width, lineHeight);
        EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);

        Rect sourceRect = new Rect(position.x + 20, position.y + lineHeight, position.width - 20, lineHeight);
        EditorGUI.PropertyField(sourceRect, property.FindPropertyRelative("Source"));

        var source = (MessageLanguageSourceType)property.FindPropertyRelative("Source").enumValueIndex;

        Rect contentRect = new Rect(position.x + 20, position.y + lineHeight * 2 + spacing, position.width - 20, lineHeight);

        if (source == MessageLanguageSourceType.UseLanguageManager)
        {
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("MessageDictionary"));
            contentRect.y += lineHeight + spacing;

            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("MessageKey"));
        }
        else
        {
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("RawString"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;

        var source = (MessageLanguageSourceType)property.FindPropertyRelative("Source").enumValueIndex;

        if (source == MessageLanguageSourceType.UseLanguageManager)
        {
            return 4 * (lineHeight + spacing);
        }
        else
        {
            return 3 * (lineHeight + spacing);
        }
    }
}