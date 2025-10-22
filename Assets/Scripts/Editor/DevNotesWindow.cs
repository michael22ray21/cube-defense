using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    private const string NOTES_PATH = "Assets/DataObjects/Editor/DevNotesWindow";
    private string lastNoteName = "";
    private DevNote lastSelectedNote = null;
    private bool pendingRebuild = false;

    [MenuItem("Dinotonte/Dev Notes")]
    private static void OpenWindow()
    {
        GetWindow<DevNotesWindow>().Show();
    }

    #region Behavior
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
        // TODO: change this to actually create a new one upon click, not show a menu with another button
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

            if (note != null)
            {
                tree.Add(note.key, note, EditorIcons.File);
            }
        }
    }

    protected override void OnBeginDrawEditors()
    {
        var selectedNote = GetSelectedNote();

        // Make a toolbar on the top
        CreateToolbar(selectedNote);
    }

    private DevNote GetSelectedNote()
    {
        var selected = MenuTree.Selection.FirstOrDefault();
        if (selected == null) return null;

        var selectedNote = selected.Value as DevNote;
        if (selectedNote == null) return null;

        return selectedNote;
    }

    private void CreateToolbar(DevNote selectedNote)
    {
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.Label("Editing: " + selectedNote.name, SirenixGUIStyles.BoldLabel);

            GUILayout.FlexibleSpace();

            // save button (refresh)
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                AssetDatabase.SaveAssets();
                ForceMenuTreeRebuild();
            }

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
    }

    protected override void OnEndDrawEditors()
    {
        var selectedNote = GetSelectedNote();

        // Check if user switched to a different note
        if (CheckLastNoteForRename(selectedNote)) return;

        // Track the initial name when first selecting a note
        if (lastNoteName == "")
        {
            lastNoteName = selectedNote.key;
        }

        // Check if focus changed and name is different
        RenameOnFocusChange(selectedNote);

        // If there's a pending rebuild and no text field is focused => rebuild
        DoPendingChanges(selectedNote);
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

        //TODO not working yet
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

    // If there's a pending rebuild and no text field is focused => rebuild
    private void DoPendingChanges(DevNote selectedNote)
    {
        string currentFocus = GUI.GetNameOfFocusedControl();
        if (pendingRebuild && currentFocus == "")
        {
            RenameNoteAsset(selectedNote);
            ForceMenuTreeRebuild();
            pendingRebuild = false;
        }
    }

    private void RenameNoteAsset(DevNote note)
    {
        if (note.key == note.name) return;

        string path = AssetDatabase.GetAssetPath(note);
        AssetDatabase.RenameAsset(path, note.key);
        EditorUtility.SetDirty(note);
        AssetDatabase.SaveAssets();

        // Don't rebuild immediately, mark it as pending
        pendingRebuild = true;
    }
    #endregion
}