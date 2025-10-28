using System.IO;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    #region Vars, Fields, Getters
    private const string NOTES_PATH = "Assets/DataObjects/Editor/DevNotesWindow";
    private string lastNoteName = "";
    private string moveTargetPath = "";
    private DevNote lastSelectedNote = null;
    private Vector2 scrollPosition;
    private bool isEditing = false;
    private bool showMoveDialog = false;
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

        if (selected.Value is not DevNote selectedNote) return null;
        return selectedNote;
    }

    private void CreateToolbar(DevNote selectedNote)
    {
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            string modeText = isEditing ? "Editing: " : "Viewing: ";
            GUILayout.Label(modeText + selectedNote.name, SirenixGUIStyles.BoldLabel);
            GUILayout.FlexibleSpace();

            // Move note button (arrow)
            if (SirenixEditorGUI.ToolbarButton(new GUIContent(EditorIcons.ArrowRight.Active, "Move Note")))
            {
                showMoveDialog = !showMoveDialog;
                if (showMoveDialog)
                {
                    // Initialize with current path
                    string assetPath = AssetDatabase.GetAssetPath(selectedNote);
                    string directory = Path.GetDirectoryName(assetPath).Replace("\\", "/");
                    moveTargetPath = directory.Replace(NOTES_PATH, "").TrimStart('/');
                }
            }

            if (isEditing)
            {
                // save button (refresh)
                if (SirenixEditorGUI.ToolbarButton(new GUIContent(EditorIcons.Refresh.Active, "Save")))
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
                if (SirenixEditorGUI.ToolbarButton(new GUIContent(EditorIcons.Pen.Active, "Edit")))
                {
                    isEditing = true;
                    ForceMenuTreeRebuild();
                }
            }
            selectedNote.Show = isEditing;

            // delete button (X)
            if (SirenixEditorGUI.ToolbarButton(new GUIContent(EditorIcons.X.Active, "Delete")))
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

        // Show move dialog if active
        if (showMoveDialog)
        {
            DrawMoveDialog(selectedNote);
        }

        // If in view mode, draw custom content view
        if (!isEditing && !showMoveDialog)
        {
            DrawViewMode(selectedNote);
        }
    }

    private void DrawMoveDialog(DevNote note)
    {
        SirenixEditorGUI.BeginBox("Move Note");

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Current Location:", GetCurrentNotePath(note));

        GUILayout.Space(10);

        // Path selector
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Target Path:", GUILayout.Width(80));

        // Create a dropdown with existing paths
        if (EditorGUILayout.DropdownButton(new GUIContent(string.IsNullOrEmpty(moveTargetPath) ? "(Root)" : moveTargetPath), FocusType.Keyboard))
        {
            GenericMenu menu = new();

            // Add root option
            menu.AddItem(new GUIContent("(Root)"), moveTargetPath == "", () => { moveTargetPath = ""; });

            // Add existing paths
            if (Directory.Exists(NOTES_PATH))
            {
                string[] directories = Directory.GetDirectories(NOTES_PATH, "*", SearchOption.AllDirectories);
                foreach (string dir in directories)
                {
                    string relativePath = dir.Replace(NOTES_PATH, "").Replace("\\", "/").TrimStart('/');
                    menu.AddItem(new GUIContent(relativePath), moveTargetPath == relativePath, () => { moveTargetPath = relativePath; });
                }
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Create New Path..."), false, () => { moveTargetPath = ""; });

            menu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        // Text field for custom path
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Or type path:", GUILayout.Width(80));
        moveTargetPath = EditorGUILayout.TextField(moveTargetPath);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Check if path exists
        string fullTargetPath = string.IsNullOrEmpty(moveTargetPath) ? NOTES_PATH : Path.Combine(NOTES_PATH, moveTargetPath);
        bool pathExists = Directory.Exists(fullTargetPath);

        if (!pathExists && !string.IsNullOrEmpty(moveTargetPath))
        {
            EditorGUILayout.HelpBox($"Path '{moveTargetPath}' does not exist. Click 'Create & Move' to create it.", MessageType.Warning);
        }

        GUILayout.Space(10);

        // Action buttons
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = new Color(0.4f, 0.8f, 1f);
        if (GUILayout.Button(pathExists ? "Move" : "Create & Move", GUILayout.Height(30)))
        {
            MoveNote(note, moveTargetPath, !pathExists);
            showMoveDialog = false;
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Cancel", GUILayout.Height(30)))
        {
            showMoveDialog = false;
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        SirenixEditorGUI.EndBox();
    }

    private string GetCurrentNotePath(DevNote note)
    {
        string assetPath = AssetDatabase.GetAssetPath(note);
        string directory = Path.GetDirectoryName(assetPath).Replace("\\", "/");
        string relativePath = directory.Replace(NOTES_PATH, "").TrimStart('/');
        return string.IsNullOrEmpty(relativePath) ? "(Root)" : relativePath;
    }

    private void MoveNote(DevNote note, string targetPath, bool createPath)
    {
        string currentPath = AssetDatabase.GetAssetPath(note);
        string fullTargetPath = string.IsNullOrEmpty(targetPath) ? NOTES_PATH : Path.Combine(NOTES_PATH, targetPath);

        // Create path if needed
        if (createPath && !string.IsNullOrEmpty(targetPath))
        {
            string[] folders = targetPath.Split('/');
            string currentDir = NOTES_PATH;

            foreach (string folder in folders)
            {
                if (string.IsNullOrEmpty(folder)) continue;

                string newDir = currentDir + "/" + folder;
                if (!AssetDatabase.IsValidFolder(newDir))
                {
                    AssetDatabase.CreateFolder(currentDir, folder);
                }
                currentDir = newDir;
            }
            AssetDatabase.Refresh();
        }

        // Move the asset
        string fileName = Path.GetFileName(currentPath);
        string newPath = Path.Combine(fullTargetPath, fileName).Replace("\\", "/");

        string error = AssetDatabase.MoveAsset(currentPath, newPath);

        if (string.IsNullOrEmpty(error))
        {
            AssetDatabase.SaveAssets();
            ForceMenuTreeRebuild();
            EditorUtility.DisplayDialog("Success", $"Moved '{note.name}' to {(string.IsNullOrEmpty(targetPath) ? "root" : targetPath)}", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", $"Failed to move note: {error}", "OK");
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