using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

// Helper class to create a new note

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    public class CreateNoteButton
    {
        #region Vars, Fields, Getters
        private readonly DevNotesWindow window;

        [SerializeField]
        [LabelText("Path")]
        [ValueDropdown("GetAvailablePaths", AppendNextDrawer = true, DropdownTitle = "Select an option or create one", DisableListAddButtonBehaviour = true)]
        [OnValueChanged("ValidatePath")]
        private string _pathStr;
        public string PathStr => _pathStr;

        [SerializeField]
        [LabelText("Key")]
        private string _key = "New Note";
        public string Key => _key;

        private bool pathExists = true;
        #endregion

        #region Behavior
        public CreateNoteButton(DevNotesWindow window)
        {
            this.window = window;
        }
        #endregion

        #region Utilities
        [Button("Create New Note", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        [ShowIf("pathExists")]
        public void CreateNote()
        {
            DevNote newNote = CreateInstance<DevNote>();
            newNote.key = _key;

            string targetPath = string.IsNullOrEmpty(_pathStr) ? NOTES_PATH : Path.Combine(NOTES_PATH, _pathStr);
            string fileName = _key + ".asset";
            string fullPath = Path.Combine(targetPath, fileName);
            CheckPathExists(fullPath);
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(newNote, uniquePath);
            AssetDatabase.SaveAssets();

            window.ForceMenuTreeRebuild();

            // Select the newly created note
            window.TrySelectMenuItemWithObject(newNote);
        }

        [Button("@\"Create Path: \" + _pathStr", ButtonSizes.Large), GUIColor(0.4f, 1f, 0.6f)]
        [ShowIf("@!pathExists && !string.IsNullOrEmpty(_pathStr)")]
        public void CreatePath()
        {
            string fullPath = Path.Combine(NOTES_PATH, _pathStr);

            // Create the directory structure
            string[] folders = _pathStr.Split('/');
            string currentPath = NOTES_PATH;

            foreach (string folder in folders)
            {
                if (string.IsNullOrEmpty(folder)) continue;

                string newPath = currentPath + "/" + folder;
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folder);
                }
                currentPath = newPath;
            }

            AssetDatabase.Refresh();
            pathExists = true;

            EditorUtility.DisplayDialog("Path Created",
                $"Successfully created path: {_pathStr}", "OK");
        }

        private void CheckPathExists(string path)
        {
            // Check the folder path
            if (!AssetDatabase.IsValidFolder(path))
            {
                // if not found, then create the directory procedurally
                string[] folders = path.Split('/');
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

        private IEnumerable<string> GetAvailablePaths()
        {
            var paths = new List<string>
            {
                // Add root option
                ""
            };

            // Get all subdirectories
            if (Directory.Exists(NOTES_PATH))
            {
                string[] directories = Directory.GetDirectories(NOTES_PATH, "*", SearchOption.AllDirectories);
                foreach (string dir in directories)
                {
                    // Convert to relative path from NOTES_PATH
                    string relativePath = dir.Replace(NOTES_PATH, "").Replace("\\", "/").TrimStart('/');
                    paths.Add(relativePath);
                }
            }

            return paths;
        }

        private void ValidatePath()
        {
            if (string.IsNullOrEmpty(_pathStr))
            {
                pathExists = true;
                return;
            }

            string fullPath = string.IsNullOrEmpty(_pathStr) ? NOTES_PATH : Path.Combine(NOTES_PATH, _pathStr);
            pathExists = Directory.Exists(fullPath);
        }
        #endregion
    }
}