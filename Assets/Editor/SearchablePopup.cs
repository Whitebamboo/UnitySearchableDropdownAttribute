using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public class SearchablePopup : PopupWindowContent
{
    private string[] options;
    private Action<int> onOptionSelected;
    private Vector2 scrollPosition;
    private int selectedIndex;
    private GUIStyle labelStyle;
    private GUIStyle selectedLabelStyle;
    private Texture2D checkmarkTexture;
    private string searchQuery = string.Empty;
    private string[] filteredOptions;
    private GUIContent searchIconContent;
    private GUIContent clearIconContent;

    public SearchablePopup(string[] options, Action<int> onOptionSelected, int initialSelection)
    {
        this.options = options;
        this.onOptionSelected = onOptionSelected;
        this.selectedIndex = initialSelection;
        InitializeStyles();
        LoadIcons();
        FilterOptions();
    }

    private void InitializeStyles()
    {
        // Initialize the label style
        labelStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.white },
            hover = { textColor = Color.yellow, background = Texture2D.grayTexture },
            padding = new RectOffset(3, 3, 3, 3),
            margin = new RectOffset(2, 2, 2, 2)
        };

        // Initialize the selected label style
        selectedLabelStyle = new GUIStyle(labelStyle)
        {
            normal = { background = Texture2D.grayTexture }
        };
    }

    private void LoadIcons()
    {
        checkmarkTexture = EditorGUIUtility.IconContent("FilterSelectedOnly").image as Texture2D;
        searchIconContent = EditorGUIUtility.IconContent("Search Icon");
        clearIconContent = EditorGUIUtility.IconContent("winbtn_win_close");
    }

    private void FilterOptions()
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            filteredOptions = options;
        }
        else
        {
            filteredOptions = options.Where(option => option.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
        }
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, Mathf.Min(250, (filteredOptions.Length + 1) * 24));
    }

    public override void OnGUI(Rect rect)
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.Space(4);

        //clear button click event to prevent text field taking input
        Rect clearButtonRect = new Rect(rect.width - 35, 4, 16f, 16f);
        if (Event.current.type == EventType.MouseDown && clearButtonRect.Contains(Event.current.mousePosition))
        {
            searchQuery = string.Empty;
            FilterOptions();
            // The click is intended for the button area – consume it so nothing else reacts.
            Event.current.Use();
        }

        //draw Search field
        searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
        if (EditorGUI.EndChangeCheck())
        {
            FilterOptions();
        }
        GUILayout.Space(2);

        // Draw the clear button
        clearIconContent = EditorGUIUtility.IconContent("winbtn_win_close");
        if (!string.IsNullOrEmpty(searchQuery))
        {
            GUI.Label(clearButtonRect, clearIconContent, GUIStyle.none);
        }

        // Draw the search icon
        float iconSize = 16f; 
        Rect iconRect = new Rect(rect.width - iconSize - 4, 3, iconSize, iconSize);
        GUI.Label(iconRect, searchIconContent);

        // Scrollable list of options
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200), GUILayout.Height(225));

        for (int i = 0; i < filteredOptions.Length; i++)
        {
            GUILayout.BeginHorizontal();

            // Draw the checkmark for the selected item
            if (Array.IndexOf(options, filteredOptions[i]) == selectedIndex)
            {
                GUILayout.Label(new GUIContent(checkmarkTexture), GUILayout.Width(12));
            }
            else
            {
                GUILayout.Space(20);
            }

            // Draw the option label
            if (GUILayout.Button(filteredOptions[i], Array.IndexOf(options, filteredOptions[i]) == selectedIndex ? selectedLabelStyle: labelStyle))
            {
                selectedIndex = Array.IndexOf(options, filteredOptions[i]);
                onOptionSelected?.Invoke(selectedIndex);
                editorWindow.Close();
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}