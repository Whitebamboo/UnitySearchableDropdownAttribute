using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public class SearchablePopup : PopupWindowContent
{
    // Constants
    private const float WindowWidth = 200f;
    private const float WindowMaxHeight = 250f;
    private const float EntryHeight = 24f;
    private const float EntryCheckmarkWidth = 12f;
    private const float EntryCheckmarkOffset = 20f;
    private const float IconSize = 16f;
    private const float IconPadding = 4f;
    private const float ClearButtonOffsetX = 35f;
    private const float ScrollViewHeight = 225f;

    // Data
    private string[] options;
    private Action<int> onOptionSelected;
    private Vector2 scrollPosition;
    private int selectedIndex;
    private string searchQuery = string.Empty;
    private string[] filteredOptions;

    // Styles and Assets
    private static readonly GUIStyle labelStyle;
    private static readonly GUIStyle selectedLabelStyle;
    private Texture2D checkmarkTexture;
    private GUIContent searchIconContent;
    private GUIContent clearIconContent;

    // Static constructor for styles
    static SearchablePopup()
    {
        labelStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.white },
            hover = { textColor = Color.yellow, background = Texture2D.grayTexture },
            padding = new RectOffset(3, 3, 3, 3),
            margin = new RectOffset(2, 2, 2, 2)
        };

        selectedLabelStyle = new GUIStyle(labelStyle)
        {
            normal = { background = Texture2D.grayTexture }
        };
    }

    // Constructor
    public SearchablePopup(string[] options, Action<int> onOptionSelected, int initialSelection)
    {
        this.options = options;
        this.onOptionSelected = onOptionSelected;
        this.selectedIndex = initialSelection;
        LoadIcons();
        FilterOptions();
    }

    private void LoadIcons()
    {
        checkmarkTexture = EditorGUIUtility.IconContent("FilterSelectedOnly").image as Texture2D;
        searchIconContent = EditorGUIUtility.IconContent("Search Icon");
        clearIconContent = EditorGUIUtility.IconContent("winbtn_win_close");
    }

    private void FilterOptions()
    {
        filteredOptions = string.IsNullOrEmpty(searchQuery)
            ? options
            : options.Where(option => option.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(WindowWidth, Mathf.Min(WindowMaxHeight, (filteredOptions.Length + 1) * EntryHeight));
    }

    public override void OnGUI(Rect rect)
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.Space(4);

        // Clear button click event
        Rect clearButtonRect = new Rect(rect.width - ClearButtonOffsetX, IconPadding, IconSize, IconSize);
        if (Event.current.type == EventType.MouseDown && clearButtonRect.Contains(Event.current.mousePosition))
        {
            searchQuery = string.Empty;
            FilterOptions();
            Event.current.Use();
        }

        // Draw search text field
        searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
        if (EditorGUI.EndChangeCheck())
        {
            FilterOptions();
        }

        GUILayout.Space(2);

        // Draw the clear icon
        if (!string.IsNullOrEmpty(searchQuery))
        {
            GUI.Label(clearButtonRect, clearIconContent, GUIStyle.none);
        }

        // Draw the search icon
        Rect iconRect = new Rect(rect.width - IconSize - IconPadding, IconPadding - 1, IconSize, IconSize);
        GUI.Label(iconRect, searchIconContent);

        // Scrollable list of options
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(WindowWidth), GUILayout.Height(ScrollViewHeight));

        for (int i = 0; i < filteredOptions.Length; i++)
        {
            GUILayout.BeginHorizontal();

            // Draw checkmark
            bool isSelected = Array.IndexOf(options, filteredOptions[i]) == selectedIndex;
            if (isSelected)
            {
                GUILayout.Label(new GUIContent(checkmarkTexture), GUILayout.Width(EntryCheckmarkWidth));
            }
            else
            {
                GUILayout.Space(EntryCheckmarkOffset);
            }

            // Draw option label
            if (GUILayout.Button(filteredOptions[i], isSelected ? selectedLabelStyle : labelStyle))
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
