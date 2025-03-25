using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SearchableDropdownAttribute))]
public class SearchableDropdownDrawer : PropertyDrawer
{
    private const float PaddingLeft = 0f;
    private const float IconSpacing = 6f;
    private const float LabelPaddingLeft = 3f;
    private const float IconSizeRatio = 0.6f;

    private static readonly GUIStyle leftAlignedStyle;

    static SearchableDropdownDrawer()
    {
        leftAlignedStyle = new GUIStyle(EditorStyles.miniButtonLeft)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset((int)LabelPaddingLeft, 0, 0, 0)
        };
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        Rect buttonRect = new Rect(position.x + PaddingLeft, position.y, position.width - PaddingLeft, position.height);

        if (GUI.Button(buttonRect, GUIContent.none))
        {
            SearchableDropdownAttribute dropdownAttribute = (SearchableDropdownAttribute)attribute;
            UnityEngine.Object target = property.serializedObject.targetObject;
            string[] options = GetOptionsFromMember(target, dropdownAttribute.MemberName, dropdownAttribute.Parameters);

            if (options != null)
            {
                int currentSelection = Array.IndexOf(options, property.stringValue);
                PopupWindow.Show(buttonRect, new SearchablePopup(options, selectedIndex =>
                {
                    property.stringValue = options[selectedIndex];
                    property.serializedObject.ApplyModifiedProperties();
                }, currentSelection));
            }
            else
            {
                Debug.LogError($"No valid options found for member '{dropdownAttribute.MemberName}' on {target.GetType().Name}");
            }
        }

        GUI.Label(buttonRect, property.stringValue, leftAlignedStyle);

        Texture2D dropdownIcon = EditorGUIUtility.IconContent("d_icon dropdown@2x").image as Texture2D;
        float iconSize = EditorGUIUtility.singleLineHeight * IconSizeRatio;
        Rect iconRect = new Rect(
            buttonRect.xMax - iconSize - IconSpacing,
            buttonRect.y + (buttonRect.height - iconSize) / 2,
            iconSize,
            iconSize
        );

        if (dropdownIcon != null)
        {
            GUI.DrawTexture(iconRect, dropdownIcon);
        }

        EditorGUI.EndProperty();
    }

    private string[] GetOptionsFromMember(UnityEngine.Object target, string memberName, object[] parameters)
    {
        if (target == null || string.IsNullOrEmpty(memberName))
            return null;

        Type targetType = target.GetType();

        // Property
        PropertyInfo prop = targetType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
        {
            var result = prop.GetValue(target);
            return ConvertToStringArray(result);
        }

        // Field
        FieldInfo field = targetType.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null && typeof(IEnumerable).IsAssignableFrom(field.FieldType))
        {
            var result = field.GetValue(target);
            return ConvertToStringArray(result);
        }

        // Method
        MethodInfo method = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(m => m.Name == memberName && typeof(IEnumerable).IsAssignableFrom(m.ReturnType));

        if (method != null)
        {
            try
            {
                var result = method.Invoke(target, parameters);
                return ConvertToStringArray(result);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error invoking method '{memberName}': {e.Message}");
            }
        }

        return null;
    }

    private string[] ConvertToStringArray(object result)
    {
        if (result is IEnumerable enumerable)
        {
            List<string> list = new List<string>();
            foreach (var item in enumerable)
            {
                list.Add(item?.ToString() ?? "null");
            }
            return list.ToArray();
        }

        return null;
    }
}
