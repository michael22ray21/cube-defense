using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    #region Vars, Fields, Getters
    private const string NOTES_PATH = "Assets/DataObjects/Editor/DevNotesWindow";
    private string lastNoteName = "";
    private DevNote lastSelectedNote = null;
    private bool isEditing = false;
    private Vector2 scrollPosition;
    #endregion

    #region Behavior
    [MenuItem("Dinotonte/Dev Notes")]
    private static void OpenWindow()
    {
        GetWindow<DevNotesWindow>().Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckPathExists();
    }

    private void CheckPathExists()
    {
        // Check the folder path
        if (!AssetDatabase.IsValidFolder(NOTES_PATH))
        {
            // if not found, then create the directory procedurally
            string[] folders = NOTES_PATH.Split('/');
            string currentPath = folders[0];

            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
            AssetDatabase.Refresh();
        }
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = InitializeMenuTree();

        // Button to create new notes
        AddCreateNewNoteButton(tree);

        // Load all DevNote assets from the specified folder
        LoadNotes(tree);
        // subscribe to tree event(s)
        SubscribeToTreeEvents(tree);

        return tree;
    }

    private OdinMenuTree InitializeMenuTree()
    {
        return new OdinMenuTree(supportsMultiSelect: false)
        {
            DefaultMenuStyle = OdinMenuStyle.TreeViewStyle,
            Config = { DrawSearchToolbar = true }
        };
    }

    private void AddCreateNewNoteButton(OdinMenuTree tree)
    {
        tree.Add("Create New Note", new CreateNoteButton(this), EditorIcons.Plus);
    }

    // this function subscribes to tree events
    private void SubscribeToTreeEvents(OdinMenuTree tree)
    {
        tree.Selection.SelectionChanged += (type) =>
        {
            if (type == SelectionChangedType.ItemAdded)
            {
                var selected = tree.Selection.SelectedValue;
                if (selected is DevNote note)
                {
                    EditorGUIUtility.PingObject(note);
                    lastSelectedNote = note;
                    lastNoteName = note.key;
                }
            }
        };
    }

    private void LoadNotes(OdinMenuTree tree)
    {
        string[] guids = AssetDatabase.FindAssets("t:DevNote", new[] { NOTES_PATH });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DevNote note = AssetDatabase.LoadAssetAtPath<DevNote>(path);
            tree.Add(path[NOTES_PATH.Length..].Split('.')[0], note, EditorIcons.File);
        }

        tree.EnumerateTree().AddThumbnailIcons();
    }

    protected override void OnBeginDrawEditors()
    {
        if (GetSelectedNote() is not DevNote selectedNote) return;

        // Make a toolbar on the top
        CreateToolbar(selectedNote);
    }

    private DevNote GetSelectedNote()
    {
        var selected = MenuTree.Selection.FirstOrDefault();
        if (selected == null) return null;

        // if (isEditing)
        // {
        if (selected.Value is not DevNote selectedNote) return null;
        return selectedNote;
        // }
        // else
        // {
        //     if (selected.Value is not ViewDevNote selectedViewNote) return null;
        //     return selectedViewNote.DevNote;
        // }
    }

    private void CreateToolbar(DevNote selectedNote)
    {
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            string modeText = isEditing ? "Editing: " : "Viewing: ";
            GUILayout.Label(modeText + selectedNote.name, SirenixGUIStyles.BoldLabel);
            GUILayout.FlexibleSpace();

            if (isEditing)
            {
                // save button (refresh)
                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
                {
                    isEditing = false;
                    AssetDatabase.SaveAssets();
                    RenameNoteAsset(selectedNote);
                    ForceMenuTreeRebuild();
                }
            }
            else
            {
                // edit button (pen)
                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Pen))
                {
                    isEditing = true;
                    ForceMenuTreeRebuild();
                }
            }
            selectedNote.Show = isEditing;

            // delete button (X)
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.X))
            {
                if (EditorUtility.DisplayDialog("Delete Note",
                    $"Are you sure you want to delete '{selectedNote.name}'?",
                    "Delete", "Cancel"))
                {
                    string path = AssetDatabase.GetAssetPath(selectedNote);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                    ForceMenuTreeRebuild();
                }
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();

        if (!isEditing)
        {
            DrawViewMode(selectedNote);
        }
    }

    private void DrawViewMode(DevNote note)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (note.subNote.Count == 0) GUILayout.Label("No contents...", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16 });

        foreach (var contentItem in note.subNote)
        {
            switch (contentItem.Type)
            {
                case ContentType.Space:
                    GUILayout.Space(20);
                    break;

                case ContentType.Title:
                    GUILayout.Space(10);
                    GUILayout.Label(contentItem.TextValue, new GUIStyle(EditorStyles.boldLabel)
                    {
                        fontSize = 16,
                        wordWrap = true
                    });
                    GUILayout.Space(5);
                    break;

                case ContentType.Text:
                    GUILayout.Label(contentItem.TextValue, new GUIStyle(EditorStyles.label)
                    {
                        wordWrap = true,
                        richText = true
                    });
                    GUILayout.Space(5);
                    break;

                case ContentType.Image:
                    if (contentItem.ImageValue != null)
                    {
                        GUILayout.Space(5);
                        float maxWidth = position.width - 40;
                        float aspectRatio = (float)contentItem.ImageValue.height / contentItem.ImageValue.width;
                        float imageWidth = Mathf.Min(contentItem.ImageValue.width, maxWidth);
                        float imageHeight = imageWidth * aspectRatio;

                        Rect imageRect = GUILayoutUtility.GetRect(imageWidth, imageHeight);
                        GUI.DrawTexture(imageRect, contentItem.ImageValue, ScaleMode.ScaleToFit);
                        GUILayout.Space(5);
                    }
                    break;
            }
        }

        GUILayout.EndScrollView();
    }

    protected override void OnEndDrawEditors()
    {
        if (!isEditing) return;

        if (GetSelectedNote() is not DevNote selectedNote) return;

        // Check if user switched to a different note
        if (CheckLastNoteForRename(selectedNote)) return;

        // Track the initial name when first selecting a note
        if (lastNoteName == "")
        {
            lastNoteName = selectedNote.key;
        }

        // Check if focus changed and name is different
        RenameOnFocusChange(selectedNote);
    }

    private bool CheckLastNoteForRename(DevNote selectedNote)
    {
        // Check if user switched to a different note
        if (lastSelectedNote != selectedNote)
        {
            // Process the previous note's rename if needed
            if (lastSelectedNote != null && lastNoteName != "" && lastNoteName != lastSelectedNote.key)
            {
                RenameNoteAsset(lastSelectedNote);
            }

            lastSelectedNote = selectedNote;
            lastNoteName = selectedNote.key;
            return true;
        }
        return false;
    }

    // Check if focus changed and name is different
    private void RenameOnFocusChange(DevNote selectedNote)
    {
        Event e = Event.current;

        // If user pressed Enter or Tab, apply the rename
        if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Tab))
        {
            if (lastNoteName != selectedNote.key)
            {
                RenameNoteAsset(selectedNote);
                lastNoteName = selectedNote.key;
            }
        }
    }

    private void RenameNoteAsset(DevNote note)
    {
        if (note.key == note.name) return;

        string path = AssetDatabase.GetAssetPath(note);
        string renameLog = AssetDatabase.RenameAsset(path, note.key);
        if (renameLog.Length != 0) Debug.LogError($"[ERR] rename: \"{renameLog}\"");
        AssetDatabase.Refresh();
        ForceMenuTreeRebuild();
        TrySelectMenuItemWithObject(note);
    }
    #endregion
}