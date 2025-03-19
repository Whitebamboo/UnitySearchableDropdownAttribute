using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SearchableDropdownAttribute))]
public class SearchableDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the property label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Draw the button
        Rect buttonRect = new Rect(position.x + 2, position.y, position.width - 3, position.height);
        if (GUI.Button(buttonRect, GUIContent.none))
        {
            SearchableDropdownAttribute dropdownAttribute = (SearchableDropdownAttribute)attribute;
            string[] options = GetOptionsFromType(dropdownAttribute.OptionsProviderType);

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
                Debug.LogError($"No valid options found in {dropdownAttribute.OptionsProviderType.Name}");
            }
        }

        // Draw the current option label text
        GUIStyle leftAlignedStyle = new GUIStyle(EditorStyles.miniButtonLeft)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(3, 0, 0, 0)
        };
        GUI.Label(buttonRect, property.stringValue, leftAlignedStyle);

        // Draw the dropdown arrow icon
        Texture2D dropdownIcon = EditorGUIUtility.IconContent("d_icon dropdown@2x").image as Texture2D;
        float iconSize = EditorGUIUtility.singleLineHeight * 0.6f;
        Rect iconRect = new Rect(buttonRect.xMax - iconSize - 6, buttonRect.y + (buttonRect.height - iconSize) / 2, iconSize, iconSize);
        if (dropdownIcon != null)
        {
            GUI.DrawTexture(iconRect, dropdownIcon);
        }

        EditorGUI.EndProperty();
    }

    private string[] GetOptionsFromType(Type providerType)
    {
        if (providerType == null)
            return null;

        // Find a static method that returns string[]
        MethodInfo method = providerType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                        .FirstOrDefault(m => m.ReturnType == typeof(string[]) && m.GetParameters().Length == 0);

        if (method == null)
            return null;

        return method.Invoke(null, null) as string[];
    }
}
