using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class DevNotesWindow : OdinMenuEditorWindow
{
    private const string NOTES_PATH = "Assets/DataObjects/Editor/DevNotesWindow";
    private string lastNoteName = "";
    private DevNotes lastSelectedNote = null;
    private bool pendingRebuild = false;

    [MenuItem("Tools/Dev Notes Window")]
    private static void OpenWindow()
    {
        GetWindow<DevNotesWindow>().Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

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
        var tree = new OdinMenuTree(supportsMultiSelect: false)
        {
            DefaultMenuStyle = OdinMenuStyle.TreeViewStyle,
            Config = { DrawSearchToolbar = true }
        };

        // Button to create new notes
        // TODO: change this to actually create a new one upon click, not show a menu with another button
        tree.Add("Create New Note", new CreateNoteButton(this), EditorIcons.Plus);

        // Load all DevNotes assets from the specified folder
        LoadNotes(tree);

        tree.Selection.SelectionChanged += (type) =>
        {
            if (type == SelectionChangedType.ItemAdded)
            {
                var selected = tree.Selection.SelectedValue;
                if (selected is DevNotes note)
                {
                    EditorGUIUtility.PingObject(note);
                    lastSelectedNote = note;
                    lastNoteName = note.key;
                }
            }
        };

        return tree;
    }

    private void LoadNotes(OdinMenuTree tree)
    {
        string[] guids = AssetDatabase.FindAssets("t:DevNotes", new[] { NOTES_PATH });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DevNotes note = AssetDatabase.LoadAssetAtPath<DevNotes>(path);

            if (note != null)
            {
                tree.Add(note.key, note, EditorIcons.File);
            }
        }
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = MenuTree.Selection.FirstOrDefault();
        if (selected == null) return;

        var selectedNote = selected.Value as DevNotes;
        if (selectedNote == null) return;

        // Make a toolbar on the top
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
        var selected = MenuTree.Selection.FirstOrDefault();
        if (selected == null) return;

        var selectedNote = selected.Value as DevNotes;
        if (selectedNote == null) return;

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
            return;
        }

        // Track the initial name when first selecting a note
        if (lastNoteName == "")
        {
            lastNoteName = selectedNote.key;
        }

        // Check if focus changed and name is different
        string currentFocus = GUI.GetNameOfFocusedControl();
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

        // If there's a pending rebuild and no text field is focused => rebuild
        if (pendingRebuild && currentFocus == "")
        {
            RenameNoteAsset(selectedNote);
            ForceMenuTreeRebuild();
            pendingRebuild = false;
        }
    }

    private void RenameNoteAsset(DevNotes note)
    {
        if (note.key == note.name) return;

        string path = AssetDatabase.GetAssetPath(note);
        AssetDatabase.RenameAsset(path, note.key);
        EditorUtility.SetDirty(note);
        AssetDatabase.SaveAssets();

        // Don't rebuild immediately, mark it as pending
        pendingRebuild = true;
    }

    // Helper class to create a new note
    private class CreateNoteButton
    {
        private readonly DevNotesWindow window;

        public CreateNoteButton(DevNotesWindow window)
        {
            this.window = window;
        }

        [Button("Create New Note", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        public void CreateNote()
        {
            DevNotes newNote = CreateInstance<DevNotes>();
            newNote.key = "New Note";

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(
                $"{NOTES_PATH}/New Note.asset");

            AssetDatabase.CreateAsset(newNote, uniquePath);
            AssetDatabase.SaveAssets();

            window.ForceMenuTreeRebuild();

            // Select the newly created note
            window.TrySelectMenuItemWithObject(newNote);
        }
    }
}