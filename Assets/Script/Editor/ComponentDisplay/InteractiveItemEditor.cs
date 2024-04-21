/*
 * File: InteractiveItemEditor.cs
 * Description: 交互项编辑器，当模式不同时隐藏不必要的字典
 * Author: tianlan
 * Last update at 24/1/31 16:27
 * 
 * Update Records:
 * tianlan  24/2/21 编写代码主体
 * tianlan  24/2/26 由于此类字符串被替换为FixedString，本脚本废弃
 */

using System;
using UnityEditor;

[Obsolete]
// [CustomEditor(typeof(InteractiveItem))]
public class InteractiveItemEditor : Editor
{
    //SerializedProperty modeProp;
    //SerializedProperty messageProp;
    //SerializedProperty messageDictProp;
    //SerializedProperty messageKeyProp;
    //SerializedProperty autoPlayProp;
    //SerializedProperty actionProp;
    //SerializedProperty playOnceProp;
    //SerializedProperty enableProp;

    //void OnEnable()
    //{
    //    modeProp = serializedObject.FindProperty("LanguageSettings");
    //    messageProp = serializedObject.FindProperty("Message");
    //    messageDictProp = serializedObject.FindProperty("MessageDictionary");
    //    messageKeyProp = serializedObject.FindProperty("MessageKey");
    //    autoPlayProp = serializedObject.FindProperty("IsAutoPlay");
    //    actionProp = serializedObject.FindProperty("InteractiveAction");
    //    playOnceProp = serializedObject.FindProperty("PlayOnce");
    //    enableProp = serializedObject.FindProperty("IsEnable");
    //}

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    EditorGUILayout.PropertyField(enableProp);
    //    EditorGUILayout.PropertyField(modeProp);

    //    InteractiveItem item = target as InteractiveItem;

    //    if (item.LanguageSettings == InteractiveItem.MessageLanguageSettings.UseNativeMessage)
    //    {
    //        EditorGUILayout.PropertyField(messageProp);
    //    }
    //    else if (item.LanguageSettings == InteractiveItem.MessageLanguageSettings.UseLanguageManager)
    //    {
    //        EditorGUILayout.PropertyField(messageDictProp);
    //        EditorGUILayout.PropertyField(messageKeyProp);
    //    }

    //    EditorGUILayout.PropertyField(autoPlayProp);
    //    EditorGUILayout.PropertyField(playOnceProp);
    //    EditorGUILayout.PropertyField(actionProp);

    //    serializedObject.ApplyModifiedProperties();
    //}
}

